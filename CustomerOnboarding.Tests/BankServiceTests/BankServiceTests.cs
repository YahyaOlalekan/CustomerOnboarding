using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerOnboarding.Api.Application.Interfaces;
using CustomerOnboarding.Api.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerOnboarding.Tests.BankServiceTests
{
    public class BankServiceTests
    {
        [Fact]
        public async Task GetBanksRawAsync_ShouldReturnJson_WhenSuccess()
        {
            // Arrange
            var client = new Mock<IBankClient>();
            client.Setup(c => c.GetBanksRawAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync("[{\"id\":1,\"name\":\"Bank A\"}]");

            var logger = Mock.Of<ILogger<BankService>>();
            var svc = new BankService(client.Object, logger);

            // Act
            var result = await svc.GetBanksRawAsync(CancellationToken.None);

            // Assert
            Assert.Contains("Bank A", result);
        }

        [Fact]
        public async Task GetBanksRawAsync_ShouldPropagateException_WhenClientFails()
        {
            var client = new Mock<IBankClient>();
            client.Setup(c => c.GetBanksRawAsync(It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new HttpRequestException("Down"));

            var logger = Mock.Of<ILogger<BankService>>();
            var svc = new BankService(client.Object, logger);

            await Assert.ThrowsAsync<HttpRequestException>(() => svc.GetBanksRawAsync(CancellationToken.None));
        }
    }

}
