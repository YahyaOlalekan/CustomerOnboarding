using CustomerOnboarding.Api.Application.Interfaces;

namespace CustomerOnboarding.Api.Domain.External
{
    public sealed class BankClient : IBankClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<BankClient> _logger;


        public BankClient(HttpClient http, IConfiguration config, ILogger<BankClient> logger)
        {
            _http = http;
            _config = config;
            _logger = logger;
        }


        public async Task<string> GetBanksRawAsync(CancellationToken ct)
        {
            // Optionally add subscription header
            var header = _config["AlatApi:SubscriptionHeader"];
            var key = _config["AlatApi:SubscriptionKey"];
            if (!string.IsNullOrWhiteSpace(header) && !string.IsNullOrWhiteSpace(key))
            {
                if (!_http.DefaultRequestHeaders.Contains(header))
                    _http.DefaultRequestHeaders.Add(header, key);
            }


            using var resp = await _http.GetAsync(string.Empty, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(ct);
        }
    }
}
