using RealRelianceBanking.Application.Accounts.Commands.AddAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Commands.AccountTests
{
    public class CreateAccountCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly CreateAccountCommandHandler _handler;

        public CreateAccountCommandHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockPersonRepository = new Mock<IPersonRepository>();
            _handler = new CreateAccountCommandHandler(_mockAccountRepository.Object, _mockPersonRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesAccountSuccessfully()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var command = new CreateAccountCommand(
                personId,
                "123456",
                "Savings",
                100m,
                false,
                true
            );

            var person = new PersonModel { PersonID = personId };

            _mockPersonRepository.Setup(repo => repo.GetPersonById(personId))
                .ReturnsAsync(person);
            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("123456"))
                .ReturnsAsync((Account)null);
            _mockAccountRepository.Setup(repo => repo.CreateAccount(It.IsAny<Account>()))
                .ReturnsAsync(accountId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(accountId, result);
            _mockAccountRepository.Verify(repo => repo.CreateAccount(It.Is<Account>(a =>
                a.PersonID == personId &&
                a.AccountNumber == "123456" &&
                a.AccountType == "Savings" &&
                a.Balance == 100m &&
                a.Status == false &&
                a.ActiveInd == true
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentPerson_ThrowsException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var command = new CreateAccountCommand(
                personId,
                "123456",
                "Savings",
                100m,
                false,
                true
            );

            _mockPersonRepository.Setup(repo => repo.GetPersonById(personId))
                .ReturnsAsync((PersonModel)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DuplicateAccountNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var command = new CreateAccountCommand(
                personId,
                "123456",
                "Savings",
                100m,
                false,
                true
            );

            var person = new PersonModel { PersonID = personId };
            var existingAccount = new Account { AccountNumber = "123456" };

            _mockPersonRepository.Setup(repo => repo.GetPersonById(personId))
                .ReturnsAsync(person);
            _mockAccountRepository.Setup(repo => repo.GetByAccountNumber("123456"))
                .ReturnsAsync(existingAccount);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
