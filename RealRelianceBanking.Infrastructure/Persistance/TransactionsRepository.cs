using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.Persistance
{
    internal class TransactionsRepository : ITransactionRepository
    {
        Task ITransactionRepository.AddTransaction(TransactionsModel transaction)
        {
            throw new NotImplementedException();
        }

        Task<TransactionAggregate> ITransactionRepository.GetDetailsByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<TransactionsModel?> ITransactionRepository.GetTransactionById(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TransactionsModel>> ITransactionRepository.GetTransactions(Guid accountId, bool isActive)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TransactionAggregate>> ITransactionRepository.GetTransactionsAsync(bool activeOnly)
        {
            throw new NotImplementedException();
        }

        Task<List<TransactionsModel>> ITransactionRepository.GetTransactionsByAccountId(Guid accountId)
        {
            throw new NotImplementedException();
        }
    }
}
