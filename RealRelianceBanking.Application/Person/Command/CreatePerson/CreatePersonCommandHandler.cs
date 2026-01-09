using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;

namespace RealRelianceBanking.Application.Person.Command.CreatePerson
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, CreatePersonResponse>
    {
        private readonly IPersonRepository _personRepository;

        public CreatePersonCommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<CreatePersonResponse> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var existingPerson = await _personRepository.GetByIdNumberAsync(request.IdNumber);

            if (existingPerson != null)
            {
                return new CreatePersonResponse(
                    Success: false,
                    ErrorMessage: $"A person with ID Number {request.IdNumber} already exists."
                );
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

            var personId = await _personRepository.Add(person);
            return new CreatePersonResponse(
                Success: true,
                PersonId: personId
            );
        }
    }
}