using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.service.Repositories;
using Bookify.wep.Data; // <- تأكد هذا الـ namespace مطابق لمكان ApplicationDbContext عندك

namespace Bookify.wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IRoomRepository _rooms;
        private readonly IBookingRepository _bookings;
        private readonly ApplicationDbContext _db;   // <-- الحقل الجديد

        public DashboardController(
            IRoomRepository rooms,
            IBookingRepository bookings,
            ApplicationDbContext db)                 // <-- الحقن هنا
        {
            _rooms = rooms;
            _bookings = bookings;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _rooms.ListAsync();
            var bookings = await _bookings.ListAsync();

            // Count users via Identity DbContext
            var usersCount = await _db.Users.CountAsync();

            var vm = new AdminDashboardVm
            {
                RoomsCount = rooms.Count,
                BookingsCount = bookings.Count,
                PendingCount = bookings.Count(b => b.BookingStatus == "Pending"),
                UsersCount = usersCount,
            };

            return View(vm);
        }
    }

    // Simple VM placed here for convenience (or move to Models/Admin)
    public class AdminDashboardVm
    {
        public int RoomsCount { get; set; }
        public int BookingsCount { get; set; }
        public int PendingCount { get; set; }
        public int UsersCount { get; set; }
    }
}
