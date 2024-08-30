using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Command.CreatePerson
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
    {
        private readonly IPersonRepository _personRepository;

        public CreatePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var existingPerson = await _personRepository.GetByIdNumberAsync(request.IdNumber);

            if (existingPerson != null)
            {
                throw new ApplicationException("A person with the same ID Number already exists.");
            }

            var person = new PersonModel
            {
                PersonID = Guid.NewGuid(),
                IdNumber = request.IdNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                ActiveInd = true
            };

            return await _personRepository.Add(person);
        }
    }
}
