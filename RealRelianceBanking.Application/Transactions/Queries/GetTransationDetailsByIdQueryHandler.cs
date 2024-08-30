using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RealRelianceBanking.Application.Transactions.Queries
{
    public class GetTransationDetailsByIdQueryHandler : IRequestHandler<GetTransationDetailsById, TransactionAggregate>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetTransationDetailsByIdQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<TransactionAggregate> Handle(GetTransationDetailsById request, CancellationToken cancellationToken)
        {
            var transaction = await _transactionRepository.GetDetailsByIdAsync(request.transactionId);

            return new TransactionAggregate
            {
                TransactionId = transaction.TransactionId,
                TransactionDate = transaction.TransactionDate,
                Type = transaction.Type,
                Amount = transaction.Amount,
                Description = transaction.Description
            };
        }
    }
}
