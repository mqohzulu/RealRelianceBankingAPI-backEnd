using MediatR;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Queries.GetAccountsByPersonId
{
    public record GetAccountsByPersonIdQuery(Guid PersonId) : IRequest<List<Account>>;


}
