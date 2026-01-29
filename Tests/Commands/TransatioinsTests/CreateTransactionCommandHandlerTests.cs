using Moq;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using RealRelianceBanking.Application.Transactions.Command.Create;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Commands.TransatioinsTests
{
    public class CreateTransactionCommandHandlerTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepository;
        private readonly Mock<IAccountRepository> _accountRepository;
        private readonly Mock<IDateTimeProvider> _dateTimeProvider;
        private readonly CreateTransactionCommandHandler _handler;

        public CreateTransactionCommandHandlerTests()
        {
            _transactionRepository = new Mock<ITransactionRepository>();
            _accountRepository = new Mock<IAccountRepository>();
            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _dateTimeProvider.Setup(provider => provider.UtcNow).Returns(DateTime.UtcNow);

            _handler = new CreateTransactionCommandHandler(
                _transactionRepository.Object,
                _accountRepository.Object,
                _dateTimeProvider.Object);
        }

        [Fact]
        public async Task Handle_ValidCreditTransaction_UpdatesBalance()
        {
            var accountId = Guid.NewGuid();
            var account = new Account
            {
                AccountID = accountId,
                Balance = 100m,
                Status = false
            };

            _accountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync(account);

            var command = new CreateTransactionCommand(
                accountId,
                DateTime.UtcNow.Date,
                50m,
                "Credit",
                "Deposit");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal(150m, account.Balance);
            _transactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Once);
            _accountRepository.Verify(repo => repo.UpdateAccountAsync(account), Times.Once);
        }
    }
}
