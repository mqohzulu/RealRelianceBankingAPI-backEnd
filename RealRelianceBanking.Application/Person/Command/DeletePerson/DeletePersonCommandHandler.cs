using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;

namespace RealRelianceBanking.Application.Person.Command.DeletePerson
{
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, DeletePersonResult>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IAccountRepository _accountRepository;

        public DeletePersonCommandHandler(
            IPersonRepository personRepository,
            IAccountRepository accountRepository)
        {
            _personRepository = personRepository;
            _accountRepository = accountRepository;
        }

        public async Task<DeletePersonResult> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByIdNumberAsync(request.IdNumber);

            if (person == null)
                return new DeletePersonResult(false, "Person not found.");

            bool hasActiveAccounts = await _accountRepository.HasActiveAccounts(person.PersonID);

            if (hasActiveAccounts)
                return new DeletePersonResult(false, "Cannot delete person with active accounts. Close all accounts first.");

            await _personRepository.Deactivate(person.PersonID);
            return new DeletePersonResult(true, "Person deleted successfully.");
        }
    }
}