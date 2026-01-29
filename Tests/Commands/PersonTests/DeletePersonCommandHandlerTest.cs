using RealRelianceBanking.Application.Person.Command.DeletePerson;
using System;
using System.Threading.Tasks;

namespace Tests.Commands.PersonTests
{
    public class DeletePersonCommandHandlerTests
    {
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly DeletePersonCommandHandler _handler;

        public DeletePersonCommandHandlerTests()
        {
            _mockPersonRepository = new Mock<IPersonRepository>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _handler = new DeletePersonCommandHandler(
                _mockPersonRepository.Object,
                _mockAccountRepository.Object);
        }

        [Fact]
        public async Task Handle_PersonWithNoActiveAccounts_ShouldDeactivatePerson()
        {
            // Arrange
            const int idNumber = 123456;
            var command = new DeletePersonCommand(idNumber);
            var person = new PersonModel
            {
                PersonID = Guid.NewGuid(),
                IdNumber = idNumber
            };

            _mockPersonRepository.Setup(repo => repo.GetByIdNumberAsync(idNumber))
                .ReturnsAsync(person);
            _mockAccountRepository.Setup(repo => repo.HasActiveAccounts(person.PersonID))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockPersonRepository.Verify(repo => repo.Deactivate(person.PersonID), Times.Once);
            Assert.True(result.Success);
            Assert.Equal("Person deleted successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_PersonWithActiveAccounts_ShouldReturnFailureResult()
        {
            // Arrange
            const int idNumber = 654321;
            var command = new DeletePersonCommand(idNumber);
            var person = new PersonModel
            {
                PersonID = Guid.NewGuid(),
                IdNumber = idNumber
            };

            _mockPersonRepository.Setup(repo => repo.GetByIdNumberAsync(idNumber))
                .ReturnsAsync(person);
            _mockAccountRepository.Setup(repo => repo.HasActiveAccounts(person.PersonID))
                .ReturnsAsync(true);

            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Cannot delete person with active accounts. Close all accounts first.", result.Message);
            _mockPersonRepository.Verify(repo => repo.Deactivate(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PersonNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            const int idNumber = 999999;
            var command = new DeletePersonCommand(idNumber);

            _mockPersonRepository.Setup(repo => repo.GetByIdNumberAsync(idNumber))
                .ReturnsAsync((PersonModel)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Person not found.", result.Message);
            _mockAccountRepository.Verify(repo => repo.HasActiveAccounts(It.IsAny<Guid>()), Times.Never);
            _mockPersonRepository.Verify(repo => repo.Deactivate(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AccountRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            const int idNumber = 123987;
            var command = new DeletePersonCommand(idNumber);
            var person = new PersonModel
            {
                PersonID = Guid.NewGuid(),
                IdNumber = idNumber
            };

            _mockPersonRepository.Setup(repo => repo.GetByIdNumberAsync(idNumber))
                .ReturnsAsync(person);
            _mockAccountRepository.Setup(repo => repo.HasActiveAccounts(person.PersonID))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
