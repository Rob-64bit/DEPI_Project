using Microsoft.AspNetCore.Mvc;
using Bookify.service.Repositories;
using Bookify.wep.Models.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookify.wep.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IGuestRepository _guestRepo;

        public PaymentsController(IPaymentRepository paymentRepo, IBookingRepository bookingRepo, IGuestRepository guestRepo)
        {
            _paymentRepo = paymentRepo;
            _bookingRepo = bookingRepo;
            _guestRepo = guestRepo;
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _paymentRepo.ListAsync();
            var vm = new List<PaymentAdminDto>();

            foreach (var p in payments)
            {
                string? guestName = null;

                var booking = await _bookingRepo.GetByIdAsync(p.BookingId);
                if (booking != null)
                {
                    var guest = await _guestRepo.GetByIdAsync(booking.Gid);
                    guestName = guest?.Fullname;
                }

                vm.Add(new PaymentAdminDto(
                    p.Id,
                    p.BookingId,
                    guestName,
                    p.StripePaymentIntentId,
                    p.Status,
                    p.Amount,
                    p.PaymentDate
                ));
            }

            return View(vm);
        }
    }
}
