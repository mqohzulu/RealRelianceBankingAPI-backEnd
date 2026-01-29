using FluentValidation;

namespace RealRelianceBanking.Application.Person.Command.CreatePerson
{
    public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
    {
        public CreatePersonCommandValidator()
        {
            RuleFor(x => x.IdNumber)
                .GreaterThan(0);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
