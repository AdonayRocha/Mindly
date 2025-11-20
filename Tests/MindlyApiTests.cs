using Mindly;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Mindly.Tests
{
    public class MindlyApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public MindlyApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

        [Fact]
        public async Task HealthCheck_ReturnsHealthy()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }
    }
}
