using RealRelianceBanking.Application.Accounts.Queries.GetAccountById;

namespace Tests.Query.AccountTests
{
    public class GetAccountByIdQueryHandlerTests
    {
        private readonly Mock<IAccountRepository> _accountRepository;
        private readonly GetAccountByIdQueryHandler _handler;

        public GetAccountByIdQueryHandlerTests()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _handler = new GetAccountByIdQueryHandler(_accountRepository.Object);
        }

        [Fact]
        public async Task Handle_ExistingAccount_ReturnsAccount()
        {
            var accountId = Guid.NewGuid();
            var account = new Account
            {
                AccountID = accountId,
                AccountNumber = "555000",
                Balance = 250m
            };

            _accountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync(account);

            var result = await _handler.Handle(new GetAccountByIdQuery(accountId), CancellationToken.None);

            Assert.Equal(account, result);
            _accountRepository.Verify(repo => repo.GetAccountById(accountId), Times.Once);
        }

        [Fact]
        public async Task Handle_MissingAccount_ReturnsNull()
        {
            var accountId = Guid.NewGuid();

            _accountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync((Account)null);

            var result = await _handler.Handle(new GetAccountByIdQuery(accountId), CancellationToken.None);

            Assert.Null(result);
            _accountRepository.Verify(repo => repo.GetAccountById(accountId), Times.Once);
        }
    }
}
