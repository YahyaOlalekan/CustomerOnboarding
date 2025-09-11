using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerOnboarding.Api.Application.Services;
using CustomerOnboarding.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerOnboarding.Tests.OtpServiceImplementationTests
{
    public class OtpServiceImplementationTests
    {
        private AppDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // extension method from EFCore.InMemory
                .Options;

            return new AppDbContext(options);
        }


        private IConfiguration GetConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Otp:Length", "6" },
                    { "Otp:ExpiryMinutes", "1" }
                })
                .Build();

        [Fact]
        public async Task GenerateAndVerify_ShouldSucceed_WhenCodeMatches()
        {
            using var db = GetDb();
            var logger = Mock.Of<ILogger<OtpServiceImplementation>>();
            var config = GetConfig();
            var svc = new OtpServiceImplementation(db, logger, config);

            var phone = "08035551234";
            await svc.GenerateAndSendAsync(phone, CancellationToken.None);

            var token = await db.OtpTokens.FirstAsync();
            Assert.False(token.Consumed);

            // use reflection to fetch private HashCode
            var method = typeof(OtpServiceImplementation).GetMethod("HashCode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var code = "123456"; // fake code just to illustrate (in real test you'd capture via logger)
            var hash = (string)method!.Invoke(null, new object[] { code, phone })!;

            // manually overwrite token with known hash
            token.CodeHash = hash;
            await db.SaveChangesAsync();

            var ok = await svc.VerifyAsync(phone, code, CancellationToken.None);

            Assert.True(ok);
            Assert.True(token.Consumed);
        }

        [Fact]
        public async Task Verify_ShouldFail_WhenCodeInvalid()
        {
            using var db = GetDb();
            var logger = Mock.Of<ILogger<OtpServiceImplementation>>();
            var config = GetConfig();
            var svc = new OtpServiceImplementation(db, logger, config);

            var phone = "08035551234";
            await svc.GenerateAndSendAsync(phone, CancellationToken.None);

            var ok = await svc.VerifyAsync(phone, "wrong", CancellationToken.None);

            Assert.False(ok);
        }
    }
}

