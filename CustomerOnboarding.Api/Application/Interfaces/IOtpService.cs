namespace CustomerOnboarding.Api.Application.Interfaces
{
    public interface IOtpService
    {
        Task GenerateAndSendAsync(string phoneNumber, CancellationToken ct);
        Task<bool> VerifyAsync(string phoneNumber, string code, CancellationToken ct);
    }
}
