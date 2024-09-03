using RealRelianceBanking.Application.Accounts.Commands.CloseAccountCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Commands.AccountTests
{
    public class CloseAccountCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly CloseAccountCommandHandler _handler;

        public CloseAccountCommandHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _handler = new CloseAccountCommandHandler(_mockAccountRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidAccount_ClosesAccountSuccessfully()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new CloseAccountCommand(accountId);
            var account = new Account
            {
                AccountID = accountId,
                Balance = 0,
                Status = false
            };

            _mockAccountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync(account);
            _mockAccountRepository.Setup(repo => repo.CloseAccount(accountId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockAccountRepository.Verify(repo => repo.CloseAccount(accountId), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentAccount_ThrowsException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new CloseAccountCommand(accountId);

            _mockAccountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync((Account)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AlreadyClosedAccount_ThrowsInvalidOperationException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new CloseAccountCommand(accountId);
            var account = new Account
            {
                AccountID = accountId,
                Balance = 0,
                Status = true  // Account is already closed
            };

            _mockAccountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AccountWithNonZeroBalance_ThrowsInvalidOperationException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new CloseAccountCommand(accountId);
            var account = new Account
            {
                AccountID = accountId,
                Balance = 100,  // Non-zero balance
                Status = false
            };

            _mockAccountRepository.Setup(repo => repo.GetAccountById(accountId))
                .ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
