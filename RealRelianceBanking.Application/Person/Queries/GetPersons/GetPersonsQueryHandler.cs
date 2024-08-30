using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Queries.GetPersons
{
    public class GetPersonsQueryHandler : IRequestHandler<GetPersonsQuery, List<PersonModel>>
    {
        private readonly IPersonRepository _personRepository;

        public GetPersonsQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<List<PersonModel>> Handle(GetPersonsQuery request, CancellationToken cancellationToken)
        {
            return await _personRepository.GetPersons(request.ActiveOnly);
        }
    }

}
