using RealRelianceBanking.Application.Person.Command.CreatePerson;

namespace Tests.Commands.PersonTests
{
    public class CreatePersonCommandHandlerTests
    {
        private readonly Mock<IPersonRepository> _personRepository;
        private readonly CreatePersonCommandHandler _handler;

        public CreatePersonCommandHandlerTests()
        {
            _personRepository = new Mock<IPersonRepository>();
            _handler = new CreatePersonCommandHandler(_personRepository.Object);
        }

        [Fact]
        public async Task Handle_NewPerson_ReturnsSuccessResponse()
        {
            var command = new CreatePersonCommand(
                Guid.Empty,
                123456,
                "Jamie",
                "Reed",
                "jamie@example.com",
                "555-0101",
                true,
                new DateTime(1990, 1, 1));

            _personRepository.Setup(repo => repo.GetByIdNumberAsync(command.IdNumber))
                .ReturnsAsync((PersonModel)null);
            _personRepository.Setup(repo => repo.Add(It.IsAny<PersonModel>()))
                .ReturnsAsync(Guid.NewGuid());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.NotEqual(Guid.Empty, result.PersonId);
            Assert.Null(result.ErrorMessage);
            _personRepository.Verify(repo => repo.Add(It.Is<PersonModel>(p =>
                p.IdNumber == command.IdNumber &&
                p.FirstName == command.FirstName &&
                p.LastName == command.LastName &&
                p.Email == command.Email &&
                p.PhoneNumber == command.PhoneNumber &&
                p.DateOfBirth == command.DateOfBirth &&
                p.ActiveInd)), Times.Once);
        }

        [Fact]
        public async Task Handle_DuplicateIdNumber_ReturnsFailureResponse()
        {
            var command = new CreatePersonCommand(
                Guid.Empty,
                999999,
                "Alex",
                "Stone",
                "alex@example.com",
                "555-0199",
                true,
                new DateTime(1985, 12, 10));

            _personRepository.Setup(repo => repo.GetByIdNumberAsync(command.IdNumber))
                .ReturnsAsync(new PersonModel { IdNumber = command.IdNumber });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal($"A person with ID Number {command.IdNumber} already exists.", result.ErrorMessage);
            _personRepository.Verify(repo => repo.Add(It.IsAny<PersonModel>()), Times.Never);
        }
    }
}
