using MediatR;
using RealRelianceBanking.Application.Person.Command.DeletePerson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Commands.PersonTests
{
    public class DeletePersonCommandHandlerTests
    {
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly DeletePersonCommandHandler _handler;

        public DeletePersonCommandHandlerTests()
        {
            _mockPersonRepository = new Mock<IPersonRepository>();
            _handler = new DeletePersonCommandHandler(_mockPersonRepository.Object);
        }

        [Fact]
        public async Task Handle_PersonWithNoActiveAccounts_ShouldDeactivatePerson()
        {
            // Arrange
            var command = new DeletePersonCommand(Guid.NewGuid());
            _mockPersonRepository.Setup(repo => repo.HasActiveAccounts(command.PersonId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockPersonRepository.Verify(repo => repo.Deactivate(command.PersonId), Times.Once);
            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_PersonWithActiveAccounts_ShouldThrowApplicationException()
        {
            // Arrange
            var command = new DeletePersonCommand(Guid.NewGuid());
            _mockPersonRepository.Setup(repo => repo.HasActiveAccounts(command.PersonId))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(command, CancellationToken.None));
            _mockPersonRepository.Verify(repo => repo.Deactivate(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new DeletePersonCommand(Guid.NewGuid());
            _mockPersonRepository.Setup(repo => repo.HasActiveAccounts(command.PersonId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeactivateThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new DeletePersonCommand(Guid.NewGuid());
            _mockPersonRepository.Setup(repo => repo.HasActiveAccounts(command.PersonId))
                .ReturnsAsync(false);
            _mockPersonRepository.Setup(repo => repo.Deactivate(command.PersonId))
                .ThrowsAsync(new Exception("Deactivation failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
