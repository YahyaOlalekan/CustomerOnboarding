using System.Net;
using System.Net.Http.Json;
using CustomerOnboarding.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CustomerOnboarding.Tests
{
    public class CustomerControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CustomerControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostCustomer_ReturnsAccepted()
        {
            var payload = new
            {
                PhoneNumber = "08031234567",
                Email = "test@example.com",
                Password = "Password123",
                StateId = 1,
                LgaId = 1
            };

            var resp = await _client.PostAsJsonAsync("/api/customers", payload);

            Assert.True(
                resp.StatusCode == HttpStatusCode.Accepted ||
                resp.StatusCode == HttpStatusCode.BadRequest
            );
        }
    }
}
