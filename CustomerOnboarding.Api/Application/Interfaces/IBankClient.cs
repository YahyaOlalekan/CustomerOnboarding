namespace CustomerOnboarding.Api.Application.Interfaces
{
    public interface IBankClient
    {
        Task<string> GetBanksRawAsync(CancellationToken ct);
    }
}
