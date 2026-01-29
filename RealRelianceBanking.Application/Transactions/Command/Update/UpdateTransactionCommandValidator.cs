using FluentValidation;
using System;

namespace RealRelianceBanking.Application.Transactions.Command.Update
{
    public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
    {
        public UpdateTransactionCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty();

            RuleFor(x => x.AccountId)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .NotEqual(0);

            RuleFor(x => x.TransactionDate)
                .LessThanOrEqualTo(DateTime.UtcNow);

            RuleFor(x => x.TransactionType)
                .Must(type => type.Equals("Debit", StringComparison.OrdinalIgnoreCase)
                    || type.Equals("Credit", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Transaction type must be Debit or Credit.");
        }
    }
}
