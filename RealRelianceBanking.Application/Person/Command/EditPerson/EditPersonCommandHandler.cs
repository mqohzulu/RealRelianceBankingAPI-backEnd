using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Command.EditPerson
{
    public class EditPersonCommandHandler : IRequestHandler<EditPersonCommand, bool>
    {
        private readonly IPersonRepository _personRepository;

        public EditPersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<bool> Handle(EditPersonCommand request, CancellationToken cancellationToken)
        {
            var existingPerson = await _personRepository.GetPersonById(request.personId);
            if (existingPerson == null)
            {
                throw new Exception($"Person was not found.");
            }

            if (existingPerson.IdNumber != request.IdNumber)
            {
                var personWithSameIdNumber = await _personRepository.GetByIdNumber(request.IdNumber);
                if (personWithSameIdNumber != null && personWithSameIdNumber.IdNumber != request.IdNumber)
                {
                    throw new InvalidOperationException("A person with this ID Number already exists.");
                }
            }

            return await _personRepository.Update(request);
        }
    }
}
