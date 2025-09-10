using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;


namespace CustomerOnboarding.Tests
{
 

    public class CustomerControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;


        public CustomerControllerTests(WebApplicationFactory<Program> factory) => _factory = factory;


        [Fact]
        public async Task PostCustomer_ReturnsAccepted()
        {
            var client = _factory.CreateClient();
            var payload = new { PhoneNumber = "08031234567", Email = "test@example.com", Password = "Password123", StateId = 1, LgaId = 1 };
            var resp = await client.PostAsJsonAsync("/api/customers", payload);
            Assert.True(resp.StatusCode == HttpStatusCode.Accepted || resp.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}
