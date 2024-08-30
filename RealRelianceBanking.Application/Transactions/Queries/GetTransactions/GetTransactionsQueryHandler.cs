using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Transactions.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<TransactionAggregate>>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<TransactionAggregate>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _transactionRepository.GetTransactionsAsync(request.ActiveOnly);
            return transactions.Select(t => new TransactionAggregate
            {
                TransactionId = t.TransactionId,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
            });
        }
    }
}
