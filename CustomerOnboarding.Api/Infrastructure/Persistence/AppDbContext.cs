using CustomerOnboarding.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CustomerOnboarding.Api.Infrastructure.Persistence;


public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<State> States => Set<State>();
    public DbSet<Lga> Lgas => Set<Lga>();
    public DbSet<OtpToken> OtpTokens => Set<OtpToken>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Customer>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
            b.Property(c => c.Email).IsRequired().HasMaxLength(256);
            b.HasIndex(c => c.PhoneNumber).IsUnique();
            b.HasIndex(c => c.Email).IsUnique();
        });


        modelBuilder.Entity<State>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Name).IsRequired().HasMaxLength(100);
        });


        modelBuilder.Entity<Lga>(b =>
        {
            b.HasKey(l => l.Id);
            b.Property(l => l.Name).IsRequired().HasMaxLength(100);
            b.HasOne(l => l.State).WithMany(s => s.Lgas).HasForeignKey(l => l.StateId);
        });


        modelBuilder.Entity<OtpToken>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.PhoneNumber).IsRequired().HasMaxLength(20);
            b.Property(o => o.CodeHash).IsRequired().HasMaxLength(128);
        });


        // Seeding of basic states and lgas
        modelBuilder.Entity<State>().HasData(
        new State { Id = 1, Name = "Lagos" },
        new State { Id = 2, Name = "Oyo" }
        );


        modelBuilder.Entity<Lga>().HasData(
        new Lga { Id = 1, Name = "Ikeja", StateId = 1 },
        new Lga { Id = 2, Name = "Epe", StateId = 1 },
        new Lga { Id = 3, Name = "Ibadan North", StateId = 2 },
        new Lga { Id = 4, Name = "Ibadan South-West", StateId = 2 }
        );
    }
}