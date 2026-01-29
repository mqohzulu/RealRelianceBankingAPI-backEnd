using MediatR;
using RealRelianceBanking.Domain.Entities;
using System.Collections.Generic;

namespace RealRelianceBanking.Application.Person.Queries.SearchPersons
{
    public record SearchPersonsQuery(int? IdNumber, string? LastName, string? AccountNumber)
        : IRequest<List<PersonModel>>;
}
