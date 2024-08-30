using MediatR;
using RealRelianceBanking.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Transactions.Queries.GetTransactions
{
    public record GetTransactionsQuery(bool ActiveOnly) : IRequest<IEnumerable<TransactionAggregate>>;
}
