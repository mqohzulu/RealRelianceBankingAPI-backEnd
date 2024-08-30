using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Queries.GetPersonById
{
    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonModel>
    {
        private readonly IPersonRepository _personRepository;


        public GetPersonByIdQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }


        public async Task<PersonModel> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            return await _personRepository.GetPersonById(request.PersonId);
        }

    }
}
