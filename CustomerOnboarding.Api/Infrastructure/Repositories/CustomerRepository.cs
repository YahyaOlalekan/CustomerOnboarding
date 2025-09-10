using CustomerOnboarding.Api.Application.Interfaces;
using CustomerOnboarding.Api.Domain.Entities;
using CustomerOnboarding.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerOnboarding.Api.Infrastructure.Repositories
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _db;
        public CustomerRepository(AppDbContext db) => _db = db;


        public async Task AddAsync(Customer customer, CancellationToken ct)
        {
            await _db.Customers.AddAsync(customer, ct);
            await _db.SaveChangesAsync(ct);
        }


        public async Task<Customer?> GetByPhoneAsync(string phoneNumber, CancellationToken ct)
        {
            return await _db.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber, ct);
        }


        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _db.Customers.FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), ct);
        }


        public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Customers.AsNoTracking().OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
        }


        public async Task UpdateAsync(Customer customer, CancellationToken ct)
        {
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync(ct);
        }
    }
}
