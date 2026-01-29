using RealRelianceBanking.Application.Accounts.Queries.GetAccount;
using RealRelianceBanking.Application.Accounts.Queries.GetAccounts;
using RealRelianceBanking.Domain.Aggregates;

namespace Tests.Query.AccountTests
{
    public class GetAccountsQueryHandlerTests
    {
        private readonly Mock<IAccountRepository> _accountRepository;
        private readonly GetAccountsQueryHandler _handler;

        public GetAccountsQueryHandlerTests()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _handler = new GetAccountsQueryHandler(_accountRepository.Object);
        }

        [Fact]
        public async Task Handle_ActiveOnlyQuery_ReturnsAccounts()
        {
            var accounts = new List<AccountAggregate>
            {
                new AccountAggregate
                {
                    AccountNumber = "100200",
                    AccountType = "Savings",
                    Balance = 25m,
                    ActiveInd = true
                }
            };

            _accountRepository.Setup(repo => repo.GetAccounts(true))
                .ReturnsAsync(accounts);

            var result = await _handler.Handle(new GetAccountsQuery(true), CancellationToken.None);

            Assert.Equal(accounts, result);
            _accountRepository.Verify(repo => repo.GetAccounts(true), Times.Once);
        }

        [Fact]
        public async Task Handle_AllAccountsQuery_ReturnsAccounts()
        {
            var accounts = new List<AccountAggregate>
            {
                new AccountAggregate
                {
                    AccountNumber = "100201",
                    AccountType = "Checking",
                    Balance = 120m,
                    ActiveInd = false
                }
            };

            _accountRepository.Setup(repo => repo.GetAccounts(false))
                .ReturnsAsync(accounts);

            var result = await _handler.Handle(new GetAccountsQuery(false), CancellationToken.None);

            Assert.Equal(accounts, result);
            _accountRepository.Verify(repo => repo.GetAccounts(false), Times.Once);
        }
    }
}
