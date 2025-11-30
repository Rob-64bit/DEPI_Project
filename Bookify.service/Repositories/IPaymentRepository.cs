using System.Collections.Generic;
using System.Threading.Tasks;
using Bookify.Domain.Entities;

namespace Bookify.service.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IReadOnlyList<Payment>> ListAsync();
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(int id);
}


