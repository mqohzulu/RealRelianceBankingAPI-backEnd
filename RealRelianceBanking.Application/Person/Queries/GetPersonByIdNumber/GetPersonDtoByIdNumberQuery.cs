using MediatR;
using RealRelianceBanking.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Queries.GetPersonByIdNumber
{
    public record GetPersonDtoByIdNumberQuery(int IdNumber) : IRequest<PersonDto>;
}
