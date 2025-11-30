using System.Collections.Generic;
using System.Threading.Tasks;
using Bookify.Data.Data;
using Bookify.Domain.Entities;
using Bookify.service.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Data.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PaymentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Payments.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Payments.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyList<Payment>> ListAsync()
    {
        return await _dbContext.Payments.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _dbContext.Payments.Update(payment);
        await _dbContext.SaveChangesAsync();
    }
}


