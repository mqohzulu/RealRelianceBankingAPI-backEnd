using MediatR;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;

namespace RealRelianceBanking.Application.Transactions.Queries.GetTransactionById
{
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionsModel>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<TransactionsModel> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _transactionRepository.GetTransactionById(request.TransactionId);
        }
    }

}
