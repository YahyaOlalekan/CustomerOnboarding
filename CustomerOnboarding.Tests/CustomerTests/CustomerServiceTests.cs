using AutoMapper;
using CustomerOnboarding.Api.Application.Interfaces;
using CustomerOnboarding.Api.Application.Services;
using CustomerOnboarding.Api.Domain.Entities;
using CustomerOnboarding.Api.DTOs;
using CustomerOnboarding.Api.Infrastructure.Persistence;
using CustomerOnboarding.Api.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerOnboarding.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repo = new();
        private readonly Mock<IOtpService> _otp = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger = Mock.Of<ILogger<CustomerService>>();

        public CustomerServiceTests()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile(new MappingProfile()));
            _mapper = new Mapper(cfg);
        }

        [Fact]
        public async Task StartOnboarding_ShouldCreateCustomer_WhenValid()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                PhoneNumber = "08030000001",
                Email = "new@test.com",
                Password = "Password123",
                StateId = 1,
                LgaId = 1
            };

            _repo.Setup(r => r.GetByPhoneAsync(request.PhoneNumber, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Customer?)null);
            _repo.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Customer?)null);

            _hasher.Setup(h => h.HashPassword(request.Password))
                   .Returns("hashed-pass");

            var svc = new CustomerService(_repo.Object, _otp.Object, _mapper, _logger, _hasher.Object);

            // Act
            await svc.StartOnboardingAsync(request, CancellationToken.None);

            // Assert
            _repo.Verify(r => r.AddAsync(It.Is<Customer>(c => c.Email == "new@test.com" && c.Status == CustomerStatus.Pending), It.IsAny<CancellationToken>()), Times.Once);
            _otp.Verify(o => o.GenerateAndSendAsync(request.PhoneNumber, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartOnboarding_ShouldThrow_When_EmailExists()
        {
            // Arrange
            _repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Customer.Create("0803", "exists@test.com", "hash", 1, 1));

            var svc = new CustomerService(_repo.Object, _otp.Object, _mapper, _logger, _hasher.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.StartOnboardingAsync(new CreateCustomerRequest
                {
                    PhoneNumber = "08035555555",
                    Email = "exists@test.com",
                    Password = "pass",
                    StateId = 1,
                    LgaId = 1
                }, CancellationToken.None));
        }

        [Fact]
        public async Task StartOnboarding_ShouldThrow_When_PhoneExists()
        {
            // Arrange
            _repo.Setup(r => r.GetByPhoneAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Customer.Create("0803", "dup@test.com", "hash", 1, 1));

            var svc = new CustomerService(_repo.Object, _otp.Object, _mapper, _logger, _hasher.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.StartOnboardingAsync(new CreateCustomerRequest
                {
                    PhoneNumber = "0803",
                    Email = "new@test.com",
                    Password = "pass",
                    StateId = 1,
                    LgaId = 1
                }, CancellationToken.None));
        }

        [Fact]
        public async Task VerifyOtp_ShouldActivateCustomer_WhenValidOtp()
        {
            // Arrange
            var customer = Customer.Create("0803", "otp@test.com", "hash", 1, 1);

            _repo.Setup(r => r.GetByPhoneAsync(customer.PhoneNumber, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(customer);

            _otp.Setup(o => o.VerifyAsync(customer.PhoneNumber, "123456", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var svc = new CustomerService(_repo.Object, _otp.Object, _mapper, _logger, _hasher.Object);

            // Act
            var dto = await svc.VerifyOtpAndActivateAsync(new VerifyOtpRequest { PhoneNumber = customer.PhoneNumber, Code = "123456" }, CancellationToken.None);

            // Assert
            Assert.Equal(CustomerStatus.Active, customer.Status);
            Assert.Equal(customer.Email, dto.Email);
            _repo.Verify(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task VerifyOtp_ShouldThrow_WhenInvalidOtp()
        {
            var customer = Customer.Create("0803", "otp@test.com", "hash", 1, 1);

            _repo.Setup(r => r.GetByPhoneAsync(customer.PhoneNumber, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(customer);

            _otp.Setup(o => o.VerifyAsync(customer.PhoneNumber, "bad", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var svc = new CustomerService(_repo.Object, _otp.Object, _mapper, _logger, _hasher.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.VerifyOtpAndActivateAsync(new VerifyOtpRequest { PhoneNumber = customer.PhoneNumber, Code = "bad" }, CancellationToken.None));
        }
    }
}