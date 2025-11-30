using System.Collections.Generic;
using System.Threading.Tasks;
using Bookify.Domain.Entities;

namespace Bookify.service.Repositories;

public interface IGuestRepository
{
    Task<Guest?> GetByIdAsync(int id);
    Task<IReadOnlyList<Guest>> ListAsync();
    Task AddAsync(Guest guest);
    Task UpdateAsync(Guest guest);
    Task DeleteAsync(int id);
}


