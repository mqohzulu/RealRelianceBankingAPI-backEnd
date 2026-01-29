using FluentValidation;

namespace RealRelianceBanking.Application.Accounts.Commands.AddAccount
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.PersonId)
                .NotEmpty();

            RuleFor(x => x.AccountNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.AccountType)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0);
        }
    }
}
