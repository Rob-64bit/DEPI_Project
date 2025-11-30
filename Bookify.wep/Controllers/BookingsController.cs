using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.service.Repositories;
using Bookify.wep.Models.Bookings;
using Bookify.Domain.Entities;
using Bookify.Data.Data;
using Stripe.Checkout;
using Newtonsoft.Json;

namespace Bookify.wep.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly ApplicationDbContext _db;

        public BookingsController(IBookingRepository bookingRepo, IRoomRepository roomRepo, ApplicationDbContext db)
        {
            _bookingRepo = bookingRepo;
            _roomRepo = roomRepo;
            _db = db;
        }

        // ============================
        // LIST & DETAILS
        // ============================
        public async Task<IActionResult> Index()
        {
            var items = await _bookingRepo.ListAsync();
            return View(items.Select(b => b.ToDto()).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View(booking.ToDto());
        }

        // GET: CREATE PAGE
        [HttpGet]
        public async Task<IActionResult> Create(int roomId, DateTime? checkin, DateTime? checkout, int guests = 1)
        {
            var room = await _roomRepo.GetByIdAsync(roomId);
            if (room == null) return NotFound();

            var vm = new BookingCreateViewModel
            {
                RoomId = room.RoomId,
                CheckIn = checkin ?? DateTime.Today.AddDays(1),
                CheckOut = checkout ?? DateTime.Today.AddDays(2),
                Guests = guests
            };

            ViewBag.RoomType = room.RoomType;

            vm.RoomPrice = room.Price;
            var nights = (vm.CheckOut - vm.CheckIn).Days;
            if (nights < 1) nights = 1;

            vm.TotalPrice = vm.RoomPrice * nights;

            return View(vm);
        }

        // POST: CHECKOUT (Stripe Payment)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(BookingCreateViewModel vm)
        {
            if (vm.CheckOut <= vm.CheckIn)
            {
                ModelState.AddModelError("", "Check-out must be after check-in.");
            }

            var room = await _roomRepo.GetByIdAsync(vm.RoomId);
            if (room == null) return NotFound();

            vm.RoomPrice = room.Price;
            var nights = (vm.CheckOut - vm.CheckIn).Days;
            if (nights < 1) nights = 1;
            vm.TotalPrice = vm.RoomPrice * nights;

            if (!ModelState.IsValid)
            {
                ViewBag.RoomType = room.RoomType;
                return View("Create", vm);
            }

            // availability
            var ci = DateOnly.FromDateTime(vm.CheckIn);
            var co = DateOnly.FromDateTime(vm.CheckOut);

            var allBookings = await _bookingRepo.ListAsync();
            var overlap = allBookings.Any(b => b.RoomId == vm.RoomId && !(co <= b.CheckIn || ci >= b.CheckOut));

            if (overlap)
            {
                ModelState.AddModelError("", "Room is already booked for these dates.");
                ViewBag.RoomType = room.RoomType;
                return View("Create", vm);
            }

            HttpContext.Session.SetString("PendingBooking", JsonConvert.SerializeObject(vm));

            // Stripe
            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(vm.TotalPrice * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Room Booking"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = domain + "/Bookings/PaymentSuccess?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/Bookings/Create?roomId=" + vm.RoomId
            };

            var service = new SessionService();
            var session = service.Create(options);

            HttpContext.Session.SetString("StripeSessionId", session.Id);

            return Redirect(session.Url);
        }

//مهم ده عشان اول مرة استخدمها فركز فيها         // STRIPE SUCCESS — CREATE BOOKING + PAYMENT
        public async Task<IActionResult> PaymentSuccess(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                session_id = HttpContext.Session.GetString("StripeSessionId");
                if (string.IsNullOrEmpty(session_id))
                    return RedirectToAction("Index");
            }

            var sessionService = new SessionService();
            var stripeSession = sessionService.Get(session_id);

            if (stripeSession.PaymentStatus != "paid")
            {
                TempData["Error"] = "Payment not completed.";
                return RedirectToAction("Index");
            }

            var json = HttpContext.Session.GetString("PendingBooking");
            if (json == null)
                return RedirectToAction("Index");

            var vm = JsonConvert.DeserializeObject<BookingCreateViewModel>(json)!;

            // create/find guest
            var guest = await _db.Guests.FirstOrDefaultAsync(g => g.Phone == vm.Phone);
            if (guest == null)
            {
                guest = new Guest
                {
                    Fullname = vm.FullName,
                    Phone = vm.Phone
                };

                guest.Gid = (await _db.Guests.MaxAsync(g => (int?)g.Gid) ?? 0) + 1;
                _db.Guests.Add(guest);
                await _db.SaveChangesAsync();
            }

            // create booking
            var booking = new Booking
            {
                RoomId = vm.RoomId,
                Gid = guest.Gid,
                CheckIn = DateOnly.FromDateTime(vm.CheckIn),
                CheckOut = DateOnly.FromDateTime(vm.CheckOut),
                BookingStatus = "confirmed"
            };
            booking.BookingId = (await _db.Bookings.MaxAsync(b => (int?)b.BookingId) ?? 0) + 1;

            await _bookingRepo.AddAsync(booking);

            // create payment
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                StripePaymentIntentId = stripeSession.PaymentIntentId,
                Status = "succeeded",
                Amount = (decimal)((stripeSession.AmountTotal ?? 0) / 100m),
                PaymentDate = DateTime.Now
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            HttpContext.Session.Remove("PendingBooking");
            HttpContext.Session.Remove("StripeSessionId");

            return RedirectToAction(nameof(Confirmation), new { id = booking.BookingId });
        }

        // CONFIRMATION صفح 
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        // MY BOOKINGS صفحة
        [HttpGet]
        public async Task<IActionResult> MyBookings(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return View(new List<Booking>());

            var guest = await _db.Guests.FirstOrDefaultAsync(g => g.Phone == phone);

            if (guest == null)
                return View(new List<Booking>());

            var all = await _bookingRepo.ListAsync();
            var myBookings = all
                .Where(b => b.Gid == guest.Gid)
                .ToList();

            foreach (var b in myBookings)
            {
                b.Payments = await _db.Payments
                    .Where(p => p.BookingId == b.BookingId)
                    .ToListAsync();
            }

            return View(myBookings);
        }

        // CUSTOMER DETAILS
        
        [HttpGet("Bookings/CustomerDetails/{id}")]
        public async Task<IActionResult> CustomerDetails(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return NotFound();

            var guest = await _db.Guests.FindAsync(booking.Gid);
            var room = await _roomRepo.GetByIdAsync(booking.RoomId);

            ViewBag.GuestName = guest?.Fullname ?? "Guest";
            ViewBag.GuestPhone = guest?.Phone ?? "N/A";
            ViewBag.RoomType = room?.RoomType ?? "Unknown";
            ViewBag.RoomPrice = room?.Price ?? 0;

            return View(booking);
        }

        // ============================
        // CANCEL BOOKING (simple)
        // ============================
        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.BookingStatus = "canceled";
            await _bookingRepo.UpdateAsync(booking);

            TempData["Success"] = "Booking canceled successfully.";
            return RedirectToAction("CustomerDetails", new { id });
        }

        // ============================
        // EDIT / DELETE
        // ============================
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return NotFound();

            var dto = new UpdateBookingDto(booking.RoomId, booking.Gid, booking.CheckIn, booking.CheckOut, booking.BookingStatus);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateBookingDto dto)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return NotFound();

            if (!ModelState.IsValid)
                return View(dto);

            dto.Apply(booking);
            await _bookingRepo.UpdateAsync(booking);

            return RedirectToAction(nameof(Details), new { id = booking.BookingId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View(booking.ToDto());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookingRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
