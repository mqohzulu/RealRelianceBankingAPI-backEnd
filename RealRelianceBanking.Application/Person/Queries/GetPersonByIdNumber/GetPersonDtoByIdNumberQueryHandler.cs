using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Queries.GetPersonByIdNumber
{

    public class GetPersonDtoByIdNumberQueryHandler : IRequestHandler<GetPersonDtoByIdNumberQuery, PersonDto>
    {
        private readonly IPersonRepository _personRepository;

        public GetPersonDtoByIdNumberQueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<PersonDto> Handle(GetPersonDtoByIdNumberQuery request, CancellationToken cancellationToken)
        {
            return await _personRepository.GetPersonDtoByIdNumber(request.IdNumber);
        }
    }

}
