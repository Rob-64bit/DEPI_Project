using System.Collections.Generic;
using System.Threading.Tasks;
using Bookify.Domain.Entities;

namespace Bookify.service.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<IReadOnlyList<Booking>> ListAsync();
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(int id);
}


