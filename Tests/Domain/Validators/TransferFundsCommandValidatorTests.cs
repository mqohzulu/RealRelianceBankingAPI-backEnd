using Tests.TestHelpers;
using RealRelianceBanking.Application.Transactions.Command.Transafer;
using RealRelianceBanking.Contracts.Transactions.Transafer.TransferFundsCommand;
using Xunit;

namespace Tests.Domain.Validators
{
    public class TransferFundsCommandValidatorTests
    {
        private readonly TransferFundsCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Not_Positive()
        {
            var model = new TransferFundsCommand("ACC1", "ACC2", 0m, "Test");
            _validator.ShouldHaveErrorFor(model, nameof(TransferFundsCommand.Amount));
        }

        [Fact]
        public void Should_Have_Error_When_AccountFrom_Is_Empty()
        {
            var model = new TransferFundsCommand("", "ACC2", 10m, "Test");
            _validator.ShouldHaveErrorFor(model, nameof(TransferFundsCommand.AccountFrom));
        }

        [Fact]
        public void Should_Have_Error_When_AccountTo_Is_Empty()
        {
            var model = new TransferFundsCommand("ACC1", "", 10m, "Test");
            _validator.ShouldHaveErrorFor(model, nameof(TransferFundsCommand.AccountTo));
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Model()
        {
            var model = new TransferFundsCommand("ACC1", "ACC2", 10m, "Test");
            _validator.ShouldNotHaveAnyErrors(model);
        }
    }
}
