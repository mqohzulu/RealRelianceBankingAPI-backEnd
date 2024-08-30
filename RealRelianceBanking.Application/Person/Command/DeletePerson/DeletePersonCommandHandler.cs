using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Command.DeletePerson
{
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IPersonRepository _personRepository;

        public DeletePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<Unit> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var hasActiveAccounts = await _personRepository.HasActiveAccounts(request.PersonId);

            if (hasActiveAccounts)
            {
                throw new ApplicationException("Cannot delete a person with active accounts.");
            }

            await _personRepository.Deactivate(request.PersonId);
            return Unit.Value;
        }
    }

}
