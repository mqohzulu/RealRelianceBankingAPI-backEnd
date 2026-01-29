using FluentValidation;
using RealRelianceBanking.Contracts.Transactions.Transafer.TransferFundsCommand;

namespace RealRelianceBanking.Application.Transactions.Command.Transafer
{
    public class TransferFundsCommandValidator : AbstractValidator<TransferFundsCommand>
    {
        public TransferFundsCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.AccountFrom)
                .NotEmpty();

            RuleFor(x => x.AccountTo)
                .NotEmpty();
        }
    }
}
