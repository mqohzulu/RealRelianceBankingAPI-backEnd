using Microsoft.Extensions.Options;
using RealRelianceBanking.Domain.Entities;
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
    public class TransactionsRepositoryIntegrationTests
    {
        private readonly TestDatabaseFixture _database;

        public TransactionsRepositoryIntegrationTests(TestDatabaseFixture database)
        {
            _database = database;
        }

        [Fact]
        public async Task AddTransaction_Then_GetTransactionsByAccountId_Returns_Row()
        {
            await _database.ResetAsync();

            var personId = await _database.InsertPersonAsync(554433, "Sam", "Lee", "sam.lee@example.com");
            var accountId = await _database.InsertAccountAsync(personId, "ACC0099", "Checking", 10m);

            var repository = CreateRepository();
            var transaction = new TransactionsModel
            {
                TransactionId = Guid.NewGuid(),
                AccountId = accountId,
                Amount = 50m,
                TransactionType = "Credit",
                TransactionDate = DateTime.UtcNow,
                CaptureDate = DateTime.UtcNow,
                Description = "Test credit"
            };

            await repository.AddTransaction(transaction);

            var result = await repository.GetTransactionsByAccountId(accountId);

            Assert.Single(result);
            Assert.Equal("Credit", result.First().TransactionType);
        }

        private TransactionsRepository CreateRepository()
        {
            var settings = Options.Create(new DapperSettings { SqlServer = _database.ConnectionString });
            var context = new DapperContext(settings);
            return new TransactionsRepository(context);
        }
    }
}
