using CustomerOnboarding.Api.Application.Interfaces;
using CustomerOnboarding.Api.Domain.Entities;
using CustomerOnboarding.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CustomerOnboarding.Api.Application.Services
{
    public sealed class OtpServiceImplementation : IOtpService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<OtpServiceImplementation> _logger;
        private readonly IConfiguration _config;


        public OtpServiceImplementation(AppDbContext db, ILogger<OtpServiceImplementation> logger, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _config = config;
        }



        public async Task GenerateAndSendAsync(string phoneNumber, CancellationToken ct)
        {
            var length = int.Parse(_config["Otp:Length"] ?? "6");
            var expiry = int.Parse(_config["Otp:ExpiryMinutes"] ?? "5");
            var code = GenerateCode(length);
            var hash = HashCode(code, phoneNumber);


            // invalidate existing tokens for phone
            var existing = await _db.OtpTokens.Where(o => o.PhoneNumber == phoneNumber && !o.Consumed).ToListAsync(ct);
            foreach (var e in existing) e.Consumed = true;


            var token = new OtpToken
            {
                PhoneNumber = phoneNumber,
                CodeHash = hash,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiry),
                Consumed = false
            };
            await _db.OtpTokens.AddAsync(token, ct);
            await _db.SaveChangesAsync(ct);


            // mock send
            _logger.LogInformation("[MOCK OTP] Phone: {Phone} Code: {Code}", phoneNumber, code);
        }


        public async Task<bool> VerifyAsync(string phoneNumber, string code, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var tokens = await _db.OtpTokens.Where(o => o.PhoneNumber == phoneNumber && !o.Consumed && o.ExpiresAtUtc >= now).ToListAsync(ct);
            var hash = HashCode(code, phoneNumber);
            var match = tokens.FirstOrDefault(t => t.CodeHash == hash);
            if (match is null) return false;
            match.Consumed = true;
            await _db.SaveChangesAsync(ct);
            return true;
        }


        private static string GenerateCode(int length)
        {
            const string digits = "0123456789";
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var chars = bytes.Select(b => digits[b % digits.Length]).ToArray();
            return new string(chars);
        }


        private static string HashCode(string code, string phone)
        {
            using var sha = SHA256.Create();
            var input = Encoding.UTF8.GetBytes(code + "|" + phone);
            var hashed = sha.ComputeHash(input);
            return Convert.ToHexString(hashed);
        }
    }
}

