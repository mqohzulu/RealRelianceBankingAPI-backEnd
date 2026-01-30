using Tests.TestHelpers;
using RealRelianceBanking.Application.Person.Command.CreatePerson;
using System;
using Xunit;

namespace Tests.Domain.Validators
{
    public class CreatePersonCommandValidatorTests
    {
        private readonly CreatePersonCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_IdNumber_Is_Not_Positive()
        {
            var model = new CreatePersonCommand(Guid.NewGuid(), 0, "John", "Doe", "john@example.com", "123", true, DateTime.UtcNow);
            _validator.ShouldHaveErrorFor(model, nameof(CreatePersonCommand.IdNumber));
        }

        [Fact]
        public void Should_Have_Error_When_FirstName_Is_Empty()
        {
            var model = new CreatePersonCommand(Guid.NewGuid(), 100, "", "Doe", "john@example.com", "123", true, DateTime.UtcNow);
            _validator.ShouldHaveErrorFor(model, nameof(CreatePersonCommand.FirstName));
        }

        [Fact]
        public void Should_Have_Error_When_LastName_Is_Empty()
        {
            var model = new CreatePersonCommand(Guid.NewGuid(), 100, "John", "", "john@example.com", "123", true, DateTime.UtcNow);
            _validator.ShouldHaveErrorFor(model, nameof(CreatePersonCommand.LastName));
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new CreatePersonCommand(Guid.NewGuid(), 100, "John", "Doe", "not-an-email", "123", true, DateTime.UtcNow);
            _validator.ShouldHaveErrorFor(model, nameof(CreatePersonCommand.Email));
        }

        [Fact]
        public void Should_Have_Error_When_PhoneNumber_Too_Long()
        {
            var model = new CreatePersonCommand(Guid.NewGuid(), 100, "John", "Doe", "john@example.com", new string('1', 16), true, DateTime.UtcNow);
            _validator.ShouldHaveErrorFor(model, nameof(CreatePersonCommand.PhoneNumber));
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Model()
        {
            var model = new CreatePersonCommand(Guid.NewGuid(), 100, "John", "Doe", "john@example.com", "123456", true, DateTime.UtcNow);
            _validator.ShouldNotHaveAnyErrors(model);
        }
    }
}
