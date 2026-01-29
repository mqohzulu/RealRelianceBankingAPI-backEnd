using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Queries.SearchPersons
{
    public class SearchPersonsQueryHandler : IRequestHandler<SearchPersonsQuery, List<PersonModel>>
    {
        private readonly IPersonRepository _personRepository;

        public SearchPersonsQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public Task<List<PersonModel>> Handle(SearchPersonsQuery request, CancellationToken cancellationToken)
        {
            return _personRepository.SearchPersons(request.IdNumber, request.LastName, request.AccountNumber);
        }
    }
}
