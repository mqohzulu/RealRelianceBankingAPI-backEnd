using Tests.TestHelpers;
using RealRelianceBanking.Application.Accounts.Commands.AddAccount;
using System;
using Xunit;

namespace Tests.Domain.Validators
{
    public class CreateAccountCommandValidatorTests
    {
        private readonly CreateAccountCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_PersonId_Is_Empty()
        {
            var model = new CreateAccountCommand(Guid.Empty, "ACC123", "Checking", 100m, false, true);
            _validator.ShouldHaveErrorFor(model, nameof(CreateAccountCommand.PersonId));
        }

        [Fact]
        public void Should_Have_Error_When_AccountNumber_Is_Empty()
        {
            var model = new CreateAccountCommand(Guid.NewGuid(), "", "Checking", 100m, false, true);
            _validator.ShouldHaveErrorFor(model, nameof(CreateAccountCommand.AccountNumber));
        }

        [Fact]
        public void Should_Have_Error_When_AccountNumber_Is_Too_Long()
        {
            var model = new CreateAccountCommand(Guid.NewGuid(), new string('1', 21), "Checking", 100m, false, true);
            _validator.ShouldHaveErrorFor(model, nameof(CreateAccountCommand.AccountNumber));
        }

        [Fact]
        public void Should_Have_Error_When_AccountType_Is_Empty()
        {
            var model = new CreateAccountCommand(Guid.NewGuid(), "ACC123", "", 100m, false, true);
            _validator.ShouldHaveErrorFor(model, nameof(CreateAccountCommand.AccountType));
        }

        [Fact]
        public void Should_Have_Error_When_Balance_Is_Negative()
        {
            var model = new CreateAccountCommand(Guid.NewGuid(), "ACC123", "Checking", -1m, false, true);
            _validator.ShouldHaveErrorFor(model, nameof(CreateAccountCommand.Balance));
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Model()
        {
            var model = new CreateAccountCommand(Guid.NewGuid(), "ACC123", "Checking", 100m, false, true);
            _validator.ShouldNotHaveAnyErrors(model);
        }
    }
}
