using Microsoft.AspNetCore.Mvc;
using Bookify.service.Repositories;
using Bookify.wep.Models.Rooms;

namespace Bookify.wep.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _rooms;
        private readonly IBookingRepository _bookingRepo;

        public RoomsController(IRoomRepository rooms, IBookingRepository bookingRepo)
        {
            _rooms = rooms;
            _bookingRepo = bookingRepo;
        }

        // GET: Rooms with Search & Filter
        public async Task<IActionResult> Index(
            DateTime? checkin,
            DateTime? checkout,
            int? guests,
            string filter,
            string sort)
        {
            var items = await _rooms.ListAsync();
            var rooms = items.Select(r => r.ToDto()).ToList();

            // 1️⃣ Filter بالتواريخ (استبعاد الغرف المحجوزة)
            if (checkin.HasValue && checkout.HasValue)
            {
                var ciDate = DateOnly.FromDateTime(checkin.Value);
                var coDate = DateOnly.FromDateTime(checkout.Value);

                // جيب الحجوزات المتعارضة
                var allBookings = await _bookingRepo.ListAsync();
                var bookedRoomIds = allBookings
                    .Where(b => !(coDate <= b.CheckIn || ciDate >= b.CheckOut))
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToList();

                // استبعد الغرف المحجوزة
                rooms = rooms.Where(r => !bookedRoomIds.Contains(r.RoomId)).ToList();
            }

            // 2️⃣ Filter بالـ Amenities
            if (!string.IsNullOrEmpty(filter))
            {
                rooms = filter.ToLower() switch
                {
                    "ocean" => rooms.Where(r => r.RoomType.Contains("Ocean", StringComparison.OrdinalIgnoreCase)).ToList(),
                    "king" => rooms.Where(r => r.RoomType.Contains("King", StringComparison.OrdinalIgnoreCase)).ToList(),
                    "balcony" => rooms.Where(r => r.RoomType.Contains("Balcony", StringComparison.OrdinalIgnoreCase)).ToList(),
                    _ => rooms
                };
            }

            // 3️⃣ Sort بالسعر
            if (!string.IsNullOrEmpty(sort))
            {
                rooms = sort switch
                {
                    "low-to-high" => rooms.OrderBy(r => r.Price).ToList(),
                    "high-to-low" => rooms.OrderByDescending(r => r.Price).ToList(),
                    _ => rooms
                };
            }
            else
            {
                // Default: Sort by Price Low to High
                rooms = rooms.OrderBy(r => r.Price).ToList();
            }

            // تمرير الـ parameters للـ View عشان نحتفظ بيها
            ViewBag.CheckIn = checkin?.ToString("yyyy-MM-dd");
            ViewBag.CheckOut = checkout?.ToString("yyyy-MM-dd");
            ViewBag.Guests = guests;
            ViewBag.Filter = filter;
            ViewBag.Sort = sort;

            return View(rooms);
        }

        public async Task<IActionResult> Details(int id)
        {
            var room = await _rooms.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room.ToDto());
        }

        // CREATE
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoomDto dto)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });
            if (!ModelState.IsValid)
                return View(dto);
            await _rooms.AddAsync(dto.ToEntity());
            TempData["Success"] = "Room added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // EDIT
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });
            var room = await _rooms.GetByIdAsync(id);
            if (room == null) return NotFound();
            var dto = new UpdateRoomDto(room.RoomType, room.Rstatus, room.Price);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRoomDto dto)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });
            var room = await _rooms.GetByIdAsync(id);
            if (room == null) return NotFound();
            if (!ModelState.IsValid)
                return View(dto);
            dto.Apply(room);
            await _rooms.UpdateAsync(room);
            TempData["Success"] = "Room updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // DELETE 
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });
            var room = await _rooms.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room.ToDto());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });
            await _rooms.DeleteAsync(id);
            TempData["Success"] = "Room deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}