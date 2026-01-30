using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.Integration.Database;
using Xunit;

namespace Tests.Integration.Api
{
    [Collection("Database collection")]
    public class AccountsApiIntegrationTests
    {
        private readonly TestDatabaseFixture _database;

        public AccountsApiIntegrationTests(TestDatabaseFixture database)
        {
            _database = database;
        }

        [Fact]
        public async Task GetAccounts_Returns_Seeded_Account()
        {
            await _database.ResetAsync();

            var personId = await _database.InsertPersonAsync(9001, "Api", "User", "api.user@example.com");
            await _database.InsertAccountAsync(personId, "API-ACC-1", "Checking", 500m);

            using var factory = new TestApiFactory(_database.ConnectionString);
            using var client = factory.CreateClient();

            var response = await client.GetAsync("/api/Accounts/GetAccounts?activeOnly=true");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var payload = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(payload);

            Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
            Assert.Contains(json.RootElement.EnumerateArray(), element =>
                element.TryGetProperty("accountNumber", out var number) &&
                number.GetString() == "API-ACC-1");
        }
    }
}
