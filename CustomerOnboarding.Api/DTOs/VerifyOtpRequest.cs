namespace CustomerOnboarding.Api.DTOs
{
    public sealed class VerifyOtpRequest
    {
        public string PhoneNumber { get; set; } = default!;
        public string Code { get; set; } = default!;
    }
}
