using AutoMapper;
using CustomerOnboarding.Api.Application.Interfaces;
using CustomerOnboarding.Api.Application.Services;
using CustomerOnboarding.Api.Domain.Entities;
using CustomerOnboarding.Api.Mapping;
using Moq;
using Xunit;

namespace CustomerOnboarding.Tests
{
    public class CustomerServiceTests
    {
        [Fact]
        public async Task StartOnboarding_ShouldThrow_When_EmailExists()
        {
            // Arrange
            var repo = new Mock<ICustomerRepository>();
            repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Customer.Create("08031234567", "existing@test.com", "hashedpass", 1, 1));

            var otp = new Mock<IOtpService>();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            var logger = Mock.Of<Microsoft.Extensions.Logging.ILogger<CustomerService>>();

            var svc = new CustomerService(repo.Object, otp.Object, mapper, logger);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.StartOnboardingAsync(
                    new CustomerOnboarding.Api.DTOs.CreateCustomerRequest
                    {
                        PhoneNumber = "08031234567",
                        Email = "a@b.com",
                        Password = "pass",
                        StateId = 1,
                        LgaId = 1
                    },
                    CancellationToken.None));
        }
    }
}
