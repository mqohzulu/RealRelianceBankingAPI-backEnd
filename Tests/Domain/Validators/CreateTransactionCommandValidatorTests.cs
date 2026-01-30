using Tests.TestHelpers;
using RealRelianceBanking.Application.Transactions.Command.Create;
using System;
using Xunit;

namespace Tests.Domain.Validators
{
    public class CreateTransactionCommandValidatorTests
    {
        private readonly CreateTransactionCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_AccountId_Is_Empty()
        {
            var model = new CreateTransactionCommand(Guid.Empty, DateTime.UtcNow, 100m, "Credit", "Deposit");
            _validator.ShouldHaveErrorFor(model, nameof(CreateTransactionCommand.AccountId));
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Zero()
        {
            var model = new CreateTransactionCommand(Guid.NewGuid(), DateTime.UtcNow, 0m, "Credit", "Deposit");
            _validator.ShouldHaveErrorFor(model, nameof(CreateTransactionCommand.Amount));
        }

        [Fact]
        public void Should_Have_Error_When_TransactionDate_In_Future()
        {
            var model = new CreateTransactionCommand(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(5), 10m, "Debit", "Test");
            _validator.ShouldHaveErrorFor(model, nameof(CreateTransactionCommand.TransactionDate));
        }

        [Fact]
        public void Should_Have_Error_When_TransactionType_Is_Invalid()
        {
            var model = new CreateTransactionCommand(Guid.NewGuid(), DateTime.UtcNow, 10m, "Transfer", "Test");
            _validator.ShouldHaveErrorFor(model, nameof(CreateTransactionCommand.TransactionType));
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Model()
        {
            var model = new CreateTransactionCommand(Guid.NewGuid(), DateTime.UtcNow, 10m, "Debit", "Test");
            _validator.ShouldNotHaveAnyErrors(model);
        }
    }
}
