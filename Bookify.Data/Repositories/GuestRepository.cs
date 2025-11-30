using System.Collections.Generic;
using System.Threading.Tasks;
using Bookify.Data.Data;
using Bookify.Domain.Entities;
using Bookify.service.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Data.Repositories;

public class GuestRepository : IGuestRepository
{
    private readonly ApplicationDbContext _dbContext;

    public GuestRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Guest guest)
    {
        await _dbContext.Guests.AddAsync(guest);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Guests.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Guests.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<Guest?> GetByIdAsync(int id)
    {
        return await _dbContext.Guests.FirstOrDefaultAsync(g => g.Gid == id);
    }

    public async Task<IReadOnlyList<Guest>> ListAsync()
    {
        return await _dbContext.Guests.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(Guest guest)
    {
        _dbContext.Guests.Update(guest);
        await _dbContext.SaveChangesAsync();
    }
}


