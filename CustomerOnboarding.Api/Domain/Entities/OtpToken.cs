namespace CustomerOnboarding.Api.Domain.Entities
{
    public sealed class OtpToken
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string CodeHash { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public bool Consumed { get; set; }
    }
}
