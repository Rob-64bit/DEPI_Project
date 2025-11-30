using Microsoft.AspNetCore.Mvc;
using Bookify.service.Repositories;
using Bookify.wep.Models.Guests;

namespace Bookify.wep.Controllers
{
    public class GuestsController : Controller
    {
        private readonly IGuestRepository _guests;

        public GuestsController(IGuestRepository guests)
        {
            _guests = guests;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _guests.ListAsync();
            return View(items.Select(g => g.ToDto()).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var guest = await _guests.GetByIdAsync(id);
            if (guest == null) return NotFound();

            return View(guest.ToDto());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGuestDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _guests.AddAsync(dto.ToEntity());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var guest = await _guests.GetByIdAsync(id);
            if (guest == null) return NotFound();

            var dto = new UpdateGuestDto(guest.Phone, guest.Fullname, guest.UserId);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateGuestDto dto)
        {
            var guest = await _guests.GetByIdAsync(id);
            if (guest == null) return NotFound();

            if (!ModelState.IsValid)
                return View(dto);

            dto.Apply(guest);
            await _guests.UpdateAsync(guest);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var guest = await _guests.GetByIdAsync(id);
            if (guest == null) return NotFound();

            return View(guest.ToDto());
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _guests.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
