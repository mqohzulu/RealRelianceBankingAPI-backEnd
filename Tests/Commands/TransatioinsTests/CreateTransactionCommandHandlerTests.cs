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

        [Fact]
        public async Task Handle_ValidDebitTransaction_DeductsBalance()
        {
            var accountId = Guid.NewGuid();
            var account = new Account
            {
                AccountID = accountId,
                Balance = 200m,
                Status = false
            };

            _accountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync(account);

            var command = new CreateTransactionCommand(
                accountId,
                DateTime.UtcNow.Date,
                75m,
                "Debit",
                "Withdrawal");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal(125m, account.Balance);
            _transactionRepository.Verify(repo => repo.AddTransaction(It.Is<TransactionsModel>(t => t.Amount == -75m && t.TransactionType == "Debit")), Times.Once);
            _accountRepository.Verify(repo => repo.UpdateAccountAsync(account), Times.Once);
        }

        [Fact]
        public async Task Handle_ZeroAmount_ReturnsFailureResult()
        {
            var command = new CreateTransactionCommand(
                Guid.NewGuid(),
                DateTime.UtcNow.Date,
                0m,
                "Credit",
                "Zero amount");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Transaction amount cannot be zero.", result.Message);
            _accountRepository.Verify(repo => repo.GetAccountById(It.IsAny<Guid>()), Times.Never);
            _transactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }

        [Fact]
        public async Task Handle_FutureTransactionDate_ReturnsFailureResult()
        {
            var command = new CreateTransactionCommand(
                Guid.NewGuid(),
                DateTime.UtcNow.AddDays(1),
                25m,
                "Credit",
                "Future date");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Transaction date cannot be in the future.", result.Message);
            _accountRepository.Verify(repo => repo.GetAccountById(It.IsAny<Guid>()), Times.Never);
            _transactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidTransactionType_ReturnsFailureResult()
        {
            var command = new CreateTransactionCommand(
                Guid.NewGuid(),
                DateTime.UtcNow.Date,
                25m,
                "Refund",
                "Invalid type");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Transaction type must be Debit or Credit.", result.Message);
            _accountRepository.Verify(repo => repo.GetAccountById(It.IsAny<Guid>()), Times.Never);
            _transactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AccountNotFound_ReturnsFailureResult()
        {
            var accountId = Guid.NewGuid();
            var command = new CreateTransactionCommand(
                accountId,
                DateTime.UtcNow.Date,
                40m,
                "Credit",
                "Missing account");

            _accountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync((Account)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Account not found.", result.Message);
            _accountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
            _transactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ClosedAccount_ReturnsFailureResult()
        {
            var accountId = Guid.NewGuid();
            var account = new Account
            {
                AccountID = accountId,
                Balance = 100m,
                Status = true
            };

            _accountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync(account);

            var command = new CreateTransactionCommand(
                accountId,
                DateTime.UtcNow.Date,
                20m,
                "Credit",
                "Closed account");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Cannot add transactions to a closed account.", result.Message);
            _accountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
            _transactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }
    }
}
