namespace CustomerOnboarding.Api.Domain.Entities
{
    public sealed class State
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<Lga> Lgas { get; set; } = new List<Lga>();
    }
}
