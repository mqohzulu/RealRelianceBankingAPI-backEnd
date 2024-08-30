using MediatR;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Queries.GetPersonById
{
    public record GetPersonByIdQuery(Guid PersonId) : IRequest<PersonModel>;
}
