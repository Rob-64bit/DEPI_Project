using System.Collections.Generic;
using System.Threading.Tasks;
using Bookify.Domain.Entities;

namespace Bookify.service.Repositories;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(int id);
    Task<IReadOnlyList<Room>> ListAsync();
    Task AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(int id);
}


