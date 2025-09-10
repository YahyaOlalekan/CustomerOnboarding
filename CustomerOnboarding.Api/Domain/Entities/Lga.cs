namespace CustomerOnboarding.Api.Domain.Entities
{
    public sealed class Lga
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int StateId { get; set; }
        public State? State { get; set; }
    }
}
