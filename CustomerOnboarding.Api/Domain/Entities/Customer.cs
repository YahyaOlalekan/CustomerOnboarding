namespace CustomerOnboarding.Api.Domain.Entities
{
    public enum CustomerStatus { Pending, Active }


    public sealed class Customer
    {
        public Guid Id { get; private set; }
        public string PhoneNumber { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public int StateId { get; private set; }
        public int LgaId { get; private set; }
        public CustomerStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }


        public Customer() { }
        //private Customer() { }



        public static Customer Create(string phoneNumber, string email, string passwordHash, int stateId, int lgaId)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("phoneNumber required", nameof(phoneNumber));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email required", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("password required", nameof(passwordHash));
            if (stateId <= 0) throw new ArgumentException("stateId required", nameof(stateId));
            if (lgaId <= 0) throw new ArgumentException("lgaId required", nameof(lgaId));


            return new Customer
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                StateId = stateId,
                LgaId = lgaId,
                Status = CustomerStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }


        public void Activate()
        {
            Status = CustomerStatus.Active;
        }
    }
}
