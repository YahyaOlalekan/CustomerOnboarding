using CustomerOnboarding.Api.Application.Interfaces;

namespace CustomerOnboarding.Api.Application.Services
{
    public sealed class BankService
    {
        private readonly IBankClient _client;
        private readonly ILogger<BankService> _logger;


        public BankService(IBankClient client, ILogger<BankService> logger)
        {
            _client = client;
            _logger = logger;
        }


        public async Task<string> GetBanksRawAsync(CancellationToken ct)
        {
            var json = await _client.GetBanksRawAsync(ct);
            _logger.LogInformation("Fetched banks");
            return json;
        }
    }
}
