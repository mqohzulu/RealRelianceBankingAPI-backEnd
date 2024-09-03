using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using RealRelianceBanking.Contracts.Transactions.Transafer.TransferFundsCommand;
using RealRelianceBanking.Application.Transactions.Command.Transafer;
using RealRelianceBanking.Domain.Aggregates;
namespace Tests.Commands.TransatioinsTests
{
    public class TransferCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly TransferFundsCommandHandler _handler;

        public TransferCommandHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockPersonRepository = new Mock<IPersonRepository>();
            _handler = new TransferFundsCommandHandler(_mockAccountRepository.Object, _mockTransactionRepository.Object, _mockPersonRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidTransfer_ReturnsSuccessResult()
        {
            // Arrange
            var command = new TransferFundsCommand("123456", "789012", 100, "Test transfer");

            var accountFrom = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "123456",
                Balance = 500,
                Status = false
            };

            var accountTo = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "789012",
                Balance = 200,
                Status = false
            };

            var personTo = new PersonModel
            {
                PersonID = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe"
            };

            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("123456"))
                .ReturnsAsync(accountFrom);
            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("789012"))
                .ReturnsAsync(accountTo);
            _mockAccountRepository.Setup(repo => repo.AccountExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _mockPersonRepository.Setup(repo => repo.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(personTo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Transfer successful.", result.Message);

            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.Is<Account>(a => a.AccountNumber == "123456" && a.Balance == 400)), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.Is<Account>(a => a.AccountNumber == "789012" && a.Balance == 300)), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.AddTransaction(It.Is<TransactionsModel>(t => t.Amount == -100 && t.TransactionType == "Debit")), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.AddTransaction(It.Is<TransactionsModel>(t => t.Amount == 100 && t.TransactionType == "Credit")), Times.Once);
        }

        [Fact]
        public async Task Handle_InsufficientFunds_ReturnsFailureResult()
        {
            // Arrange
            var command = new TransferFundsCommand("123456", "789012", 1000, "Test transfer 2");

            var accountFrom = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "123456",
                Balance = 500,
                Status = false
            };

            var accountTo = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "789012",
                Balance = 200,
                Status = false
            };
            var personTo = new PersonModel
            {
                PersonID = accountTo.PersonID,
                FirstName = "Peter",
                LastName = "Pen"
            };

            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("123456"))
     .ReturnsAsync(accountFrom);
            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("789012"))
                .ReturnsAsync(accountTo);
            _mockAccountRepository.Setup(repo => repo.AccountExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _mockPersonRepository.Setup(repo => repo.GetPersonById(accountTo.PersonID))
                .ReturnsAsync(personTo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Insufficient funds in the source account.", result.Message);

            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
            _mockTransactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ZeroAmount_ReturnsFailureResult()
        {
            // Arrange
            var command = new TransferFundsCommand("123456", "789012", 0, "Test transfer");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Transfer amount must be positive and non-zero.", result.Message);

            _mockAccountRepository.Verify(repo => repo.GetByAccountNumber(It.IsAny<string>()), Times.Never);
            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
            _mockTransactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }
        [Fact]
        public async Task Handle_TransferFromClosedAccount_ReturnsFailureResult()
        {
            // Arrange
            var command = new TransferFundsCommand("123456", "789012", 100, "Test transfer from closed account");

            var accountFrom = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "123456",
                Balance = 500,
                Status = true,  // Source account is closed
                PersonID = Guid.NewGuid()
            };

            var accountTo = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "789012",
                Balance = 200,
                Status = false,  // Destination account is open
                PersonID = Guid.NewGuid()
            };

            var personTo = new PersonModel
            {
                PersonID = accountTo.PersonID,
                FirstName = "Jane",
                LastName = "Doe"
            };

            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("123456"))
                .ReturnsAsync(accountFrom);
            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("789012"))
                .ReturnsAsync(accountTo);
            _mockAccountRepository.Setup(repo => repo.AccountExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _mockPersonRepository.Setup(repo => repo.GetPersonById(accountTo.PersonID))
                .ReturnsAsync(personTo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot transfer funds from a closed account.", result.Message);
            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
            _mockTransactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }
        [Fact]
        public async Task Handle_TransferToClosedAccount_ReturnsFailureResult()
        {
            // Arrange
            var command = new TransferFundsCommand("123456", "789012", 100, "Test transfer to closed account");

            var accountFrom = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "123456",
                Balance = 500,
                Status = false,  // Source account is open
                PersonID = Guid.NewGuid()
            };

            var accountTo = new Account
            {
                AccountID = Guid.NewGuid(),
                AccountNumber = "789012",
                Balance = 200,
                Status = true,  // Destination account is closed
                PersonID = Guid.NewGuid()
            };

            var personTo = new PersonModel
            {
                PersonID = accountTo.PersonID,
                FirstName = "Jane",
                LastName = "Doe"
            };

            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("123456"))
                .ReturnsAsync(accountFrom);
            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("789012"))
                .ReturnsAsync(accountTo);
            _mockAccountRepository.Setup(repo => repo.AccountExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _mockPersonRepository.Setup(repo => repo.GetPersonById(accountTo.PersonID))
                .ReturnsAsync(personTo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot transfer funds to a closed account.", result.Message);
            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
            _mockTransactionRepository.Verify(repo => repo.AddTransaction(It.IsAny<TransactionsModel>()), Times.Never);
        }

    }
}