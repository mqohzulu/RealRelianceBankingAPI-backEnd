using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Application.Person.Queries.GetPersonById;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Queries.GetPersonByEmail
{
    public class GetPersonByEmailQueryHandler : IRequestHandler<GetPersonByEmailQuery, PersonModel>
    {
        private readonly IPersonRepository _personRepository;


        public GetPersonByEmailQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }


        public async Task<PersonModel> Handle(GetPersonByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _personRepository.GetPersonByEmail(request.email);
        }
    }
}
