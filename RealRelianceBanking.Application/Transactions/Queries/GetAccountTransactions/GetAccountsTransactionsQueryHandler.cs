using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using RealRelianceBanking.Application.Transactions.Queries.GetAccountTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RealRelianceBanking.Application.Transactions.Queries.GetAccountTransactions
{
    public class GetAccountTransactionsQueryHandler : IRequestHandler<GetAccountTransactionsQuery, List<TransactionsModel>>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetAccountTransactionsQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<TransactionsModel>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
        {
            return await _transactionRepository.GetTransactionsByAccountId(request.AccountId);
        }
    }


}
