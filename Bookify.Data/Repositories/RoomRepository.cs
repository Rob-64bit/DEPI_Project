using System.Collections.Generic;
using System.Threading.Tasks;
using Bookify.Data.Data;
using Bookify.Domain.Entities;
using Bookify.service.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Data.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoomRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    //ده عشان المشكلة اللي الخاصة اني معملتش في الداتا بيز اوتو انكريمنت لل id // خلي بالك ***
    public async Task AddAsync(Room room)
    {
        var maxId = await _dbContext.Rooms.MaxAsync(r => (int?)r.RoomId) ?? 0;
        room.RoomId = maxId + 1;
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {

        var entity = await _dbContext.Rooms.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Rooms.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _dbContext.Rooms.FirstOrDefaultAsync(r => r.RoomId == id);
    }

    public async Task<IReadOnlyList<Room>> ListAsync()
    {
        return await _dbContext.Rooms.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(Room room)
    {
        _dbContext.Rooms.Update(room);
        await _dbContext.SaveChangesAsync();
    }
}


