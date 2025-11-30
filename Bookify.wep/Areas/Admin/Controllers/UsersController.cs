using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Data.Data;

namespace Bookify.wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });

            var users = await _db.Useres.ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });

            var user = await _db.Useres.FindAsync(id);
            if (user == null) return NotFound();

            user.UserRole = "admin";
            await _db.SaveChangesAsync();

            TempData["Success"] = $"{user.UserName} is now an Admin!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });

            var user = await _db.Useres.FindAsync(id);
            if (user == null) return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (user.UserId == currentUserId)
            {
                TempData["Error"] = "You cannot remove your own admin privileges!";
                return RedirectToAction("Index");
            }

            user.UserRole = "user";
            await _db.SaveChangesAsync();

            TempData["Success"] = $"{user.UserName} is now a regular user.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "Auth", new { area = "" });

            var user = await _db.Useres.FindAsync(id);
            if (user == null) return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (user.UserId == currentUserId)
            {
                TempData["Error"] = "You cannot delete yourself!";
                return RedirectToAction("Index");
            }

            var userGuests = await _db.Guests.Where(g => g.UserId == user.UserId).ToListAsync();
            foreach (var guest in userGuests)
            {
                var bookings = await _db.Bookings.Where(b => b.Gid == guest.Gid).ToListAsync();
                _db.Bookings.RemoveRange(bookings);
            }
            _db.Guests.RemoveRange(userGuests);
            _db.Useres.Remove(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"User {user.UserName} deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}