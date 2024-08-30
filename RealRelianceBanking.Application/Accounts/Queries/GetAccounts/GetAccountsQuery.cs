using MediatR;
using RealRelianceBanking.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Queries.GetAccounts
{
    public record GetAccountsQuery(bool ActiveOnly) : IRequest<List<AccountAggregate>>;
}
