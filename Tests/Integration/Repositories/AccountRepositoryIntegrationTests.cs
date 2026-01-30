using Microsoft.Extensions.Options;
using RealRelianceBanking.Domain.Entities;
using RealRelianceBanking.Infrastructure.DBContext;
using RealRelianceBanking.Infrastructure.Persistance;
using System;
using System.Threading.Tasks;
using Tests.Integration.Database;
using Xunit;

namespace Tests.Integration.Repositories
{
    [Collection("Database collection")]
    public class AccountRepositoryIntegrationTests
    {
        private readonly TestDatabaseFixture _database;

        public AccountRepositoryIntegrationTests(TestDatabaseFixture database)
        {
            _database = database;
        }

        [Fact]
        public async Task GetAccountById_Returns_Inserted_Account()
        {
            await _database.ResetAsync();

            var personId = await _database.InsertPersonAsync(12345, "Jane", "Doe", "jane.doe@example.com");
            var accountId = await _database.InsertAccountAsync(personId, "ACC0001", "Checking", 250m);

            var repository = CreateRepository();
            var result = await repository.GetAccountById(accountId);

            Assert.NotNull(result);
            Assert.Equal(accountId, result.AccountID);
            Assert.Equal("ACC0001", result.AccountNumber);
        }

        [Fact]
        public async Task GetAccounts_Returns_Only_Active_When_Flag_Set()
        {
            await _database.ResetAsync();

            var personId = await _database.InsertPersonAsync(12346, "John", "Smith", "john.smith@example.com");
            await _database.InsertAccountAsync(personId, "ACC0002", "Savings", 100m, isClosed: false, activeInd: true);
            await _database.InsertAccountAsync(personId, "ACC0003", "Savings", 100m, isClosed: true, activeInd: false);

            var repository = CreateRepository();
            var result = await repository.GetAccounts(activeOnly: true);

            Assert.Single(result);
            Assert.Equal("ACC0002", result[0].AccountNumber);
        }

        private AccountsRepository CreateRepository()
        {
            var settings = Options.Create(new DapperSettings { SqlServer = _database.ConnectionString });
            var context = new DapperContext(settings);
            return new AccountsRepository(context);
        }
    }
}
