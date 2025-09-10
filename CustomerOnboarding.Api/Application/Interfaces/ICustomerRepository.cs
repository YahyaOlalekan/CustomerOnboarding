using CustomerOnboarding.Api.Domain.Entities;

namespace CustomerOnboarding.Api.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer, CancellationToken ct);
        Task<Customer?> GetByPhoneAsync(string phoneNumber, CancellationToken ct);
        Task<Customer?> GetByEmailAsync(string email, CancellationToken ct);
        Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct);
        Task UpdateAsync(Customer customer, CancellationToken ct);
    }
}
