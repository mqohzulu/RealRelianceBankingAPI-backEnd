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
            var existingPerson = await _personRepository.GetPersonById(request.Person.PersonID);
            if (existingPerson == null)
            {
                throw new Exception($"Person was not found.");
            }

            if (existingPerson.IdNumber != request.Person.IdNumber)
            {
                var personWithSameIdNumber = await _personRepository.GetByIdNumber(request.Person.IdNumber);
                if (personWithSameIdNumber != null && personWithSameIdNumber.PersonID != request.Person.PersonID)
                {
                    throw new InvalidOperationException("A person with this ID Number already exists.");
                }
            }

            return await _personRepository.Update(request.Person);
        }
    }
}
