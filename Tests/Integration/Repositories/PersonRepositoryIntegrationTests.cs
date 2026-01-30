using Microsoft.Extensions.Options;
using RealRelianceBanking.Infrastructure.DBContext;
using RealRelianceBanking.Infrastructure.Persistance;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tests.Integration.Database;
using Xunit;

namespace Tests.Integration.Repositories
{
    [Collection("Database collection")]
    public class PersonRepositoryIntegrationTests
    {
        private readonly TestDatabaseFixture _database;

        public PersonRepositoryIntegrationTests(TestDatabaseFixture database)
        {
            _database = database;
        }

        [Fact]
        public async Task GetPersonByEmail_Returns_Inserted_Person()
        {
            await _database.ResetAsync();

            await _database.InsertPersonAsync(223344, "Alice", "Brown", "alice.brown@example.com");

            var repository = CreateRepository();
            var result = await repository.GetPersonByEmail("alice.brown@example.com");

            Assert.NotNull(result);
            Assert.Equal("alice.brown@example.com", result.Email);
        }

        [Fact]
        public async Task GetPersons_Returns_Only_Active_When_Flag_Set()
        {
            await _database.ResetAsync();

            await _database.InsertPersonAsync(1001, "Active", "User", "active.user@example.com", activeInd: true);
            await _database.InsertPersonAsync(1002, "Inactive", "User", "inactive.user@example.com", activeInd: false);

            var repository = CreateRepository();
            var result = await repository.GetPersons(activeOnly: true);

            Assert.Single(result);
            Assert.Equal("active.user@example.com", result.Single().Email);
        }

        private PersonRepository CreateRepository()
        {
            var settings = Options.Create(new DapperSettings { SqlServer = _database.ConnectionString });
            var context = new DapperContext(settings);
            return new PersonRepository(context);
        }
    }
}
