using RealRelianceBanking.Domain.Aggregates;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Common.Interfaces.Persistance
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<TransactionsModel>> GetTransactions(Guid accountId, bool isActive);
        Task<TransactionsModel?> GetTransactionById(Guid transactionId);
        Task<List<TransactionsModel>> GetTransactionsByAccountId(Guid accountId);
        Task AddTransaction(TransactionsModel transaction);
        Task<IEnumerable<TransactionAggregate>> GetTransactionsAsync(bool activeOnly);
        Task<TransactionAggregate> GetDetailsByIdAsync(Guid id);
    }
}
