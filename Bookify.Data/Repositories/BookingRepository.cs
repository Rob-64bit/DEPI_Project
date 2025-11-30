using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookify.Data.Data;
using Bookify.Domain.Entities;
using Bookify.service.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Data.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BookingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking)
    {
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Bookings.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Bookings.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _dbContext.Bookings
            .Include(b => b.Room)
            .Include(b => b.GidNavigation)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == id);
    }

    public async Task<IReadOnlyList<Booking>> ListAsync()
    {
        return await _dbContext.Bookings.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _dbContext.Bookings.Update(booking);
        await _dbContext.SaveChangesAsync();
    }
}


