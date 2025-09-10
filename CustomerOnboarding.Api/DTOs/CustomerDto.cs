namespace CustomerOnboarding.Api.DTOs
{
    public sealed class CustomerDto
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int StateId { get; set; }
        public int LgaId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
