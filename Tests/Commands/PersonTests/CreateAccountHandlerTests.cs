using RealRelianceBanking.Application.Person.Command.CreatePerson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Commands.PersonTests
{
    public class CreatePersonCommandHandlerTests
    {
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly CreatePersonCommandHandler _handler;

        public CreatePersonCommandHandlerTests()
        {
            _personRepositoryMock = new Mock<IPersonRepository>();
            _handler = new CreatePersonCommandHandler(_personRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CreatePerson_When_IdNumberIsUnique()
        {
            // Arrange
            var command = new CreatePersonCommand(Guid.NewGuid(), 123456, "John", "Doe", "john.doe@example.com", "123-456-7890", true, new DateTime(1990, 1, 1));

            _personRepositoryMock.Setup(repo => repo.GetByIdNumberAsync(command.IdNumber))
                .ReturnsAsync((PersonModel)null);

            _personRepositoryMock.Setup(repo => repo.Add(It.IsAny<PersonModel>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _personRepositoryMock.Verify(repo => repo.GetByIdNumberAsync(command.IdNumber), Times.Once);
            _personRepositoryMock.Verify(repo => repo.Add(It.Is<PersonModel>(p =>
                p.IdNumber == command.IdNumber &&
                p.FirstName == command.FirstName &&
                p.LastName == command.LastName &&
                p.Email == command.Email &&
                p.PhoneNumber == command.PhoneNumber &&
                p.DateOfBirth == command.DateOfBirth &&
                p.ActiveInd == true)), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_IdNumberAlreadyExists()
        {
            // Arrange
            var command = new CreatePersonCommand(Guid.NewGuid(), 123456, "John", "Doe", "john.doe@example.com", "123-456-7890", true, new DateTime(1990, 1, 1));

            _personRepositoryMock.Setup(repo => repo.GetByIdNumberAsync(command.IdNumber))
                .ReturnsAsync(new PersonModel());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("A person with the same ID Number already exists.", exception.Message);

            _personRepositoryMock.Verify(repo => repo.GetByIdNumberAsync(command.IdNumber), Times.Once);
            _personRepositoryMock.Verify(repo => repo.Add(It.IsAny<PersonModel>()), Times.Never);
        }
    }
}
