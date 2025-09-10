namespace CustomerOnboarding.Api.DTOs
{
    public sealed class CreateCustomerRequest
    {
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public int StateId { get; set; }
        public int LgaId { get; set; }
    }
}
