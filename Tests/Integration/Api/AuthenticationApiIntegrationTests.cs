using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.Integration.Database;
using Xunit;

namespace Tests.Integration.Api
{
    [Collection("Database collection")]
    public class AuthenticationApiIntegrationTests
    {
        private readonly TestDatabaseFixture _database;

        public AuthenticationApiIntegrationTests(TestDatabaseFixture database)
        {
            _database = database;
        }

        [Fact]
        public async Task RefreshToken_Returns_New_Tokens_For_Valid_RefreshToken()
        {
            await _database.ResetAsync();

            var refreshToken = "refresh-token-valid";
            var expiresAt = DateTime.UtcNow.AddDays(1);

            await _database.InsertUserAsync(
                email: "refresh.user@example.com",
                password: "pass123",
                firstName: "Refresh",
                lastName: "User",
                role: "Customer",
                refreshToken: refreshToken,
                refreshTokenExpires: expiresAt);

            using var factory = new TestApiFactory(_database.ConnectionString);
            using var client = factory.CreateClient();

            var payload = JsonSerializer.Serialize(new { refreshToken });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Authentication/refresh-token", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(responseBody);

            Assert.Equal(JsonValueKind.Object, json.RootElement.ValueKind);
            Assert.True(HasProperty(json.RootElement, "token"));
            Assert.True(HasProperty(json.RootElement, "refreshToken"));
            Assert.True(HasProperty(json.RootElement, "refreshTokenExpires"));
        }

        [Fact]
        public async Task RefreshToken_Returns_Unauthorized_For_Expired_RefreshToken()
        {
            await _database.ResetAsync();

            var refreshToken = "refresh-token-expired";
            var expiresAt = DateTime.UtcNow.AddMinutes(-5);

            await _database.InsertUserAsync(
                email: "expired.user@example.com",
                password: "pass123",
                firstName: "Expired",
                lastName: "User",
                role: "Customer",
                refreshToken: refreshToken,
                refreshTokenExpires: expiresAt);

            using var factory = new TestApiFactory(_database.ConnectionString);
            using var client = factory.CreateClient();

            var payload = JsonSerializer.Serialize(new { refreshToken });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Authentication/refresh-token", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private static bool HasProperty(JsonElement element, string name)
        {
            return element.EnumerateObject()
                .Any(property => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
