using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.Integration.Database;
using Xunit;

namespace Tests.Integration.Api
{
    [Collection("Database collection")]
    public class PersonsApiIntegrationTests
    {
        private readonly TestDatabaseFixture _database;

        public PersonsApiIntegrationTests(TestDatabaseFixture database)
        {
            _database = database;
        }

        [Fact]
        public async Task GetPersons_Returns_Seeded_Person()
        {
            await _database.ResetAsync();

            await _database.InsertPersonAsync(7001, "Api", "Person", "api.person@example.com");

            using var factory = new TestApiFactory(_database.ConnectionString);
            using var client = factory.CreateClient();

            var response = await client.GetAsync("/api/Persons/GetPersons?activeOnly=true");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var payload = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(payload);

            Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
            Assert.Contains(json.RootElement.EnumerateArray(), element =>
                element.TryGetProperty("email", out var email) &&
                email.GetString() == "api.person@example.com");
        }
    }
}
