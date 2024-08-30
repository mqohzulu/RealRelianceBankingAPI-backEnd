using Dapper;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using RealRelianceBanking.Domain.Entities;
using RealRelianceBanking.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.Persistance
{
    public class TransactionsRepository : ITransactionRepository
    {
        private readonly DapperContext _context;
        public TransactionsRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionsModel>> GetTransactions(Guid accountId, bool isActive)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var query = "SELECT * FROM Transactions WHERE AccountId = @accountId and (@active = 0 OR active_ind = @isActive)";
                    var parameters = new DynamicParameters();
                    parameters.Add("@accountId", accountId);
                    parameters.Add("@isActive", isActive);

                    var result = await db.QueryAsync<TransactionsModel>(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {
                    return Enumerable.Empty<TransactionsModel>();
                }
            }
        }

        public async Task<TransactionsModel?> GetTransactionById(Guid transactionId)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var query = "SELECT * FROM Transactions WHERE TransactionId = @transactionId";
                    var parameters = new DynamicParameters();
                    parameters.Add("@transactionId", transactionId);

                    var result = await db.QuerySingleOrDefaultAsync<TransactionsModel>(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {
                    return new TransactionsModel();
                }
            }
        }
        public async Task<List<TransactionsModel>> GetTransactionsByAccountId(Guid accountId)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var sql = "SELECT * FROM Transactions WHERE AccountId = @AccountId ";
                    var transactions = await db.QueryAsync<TransactionsModel>(sql, new { AccountId = accountId });
                    return transactions.ToList();
                }
                catch (Exception ex)
                {
                    return new List<TransactionsModel>();
                }
            }
        }
        public async Task AddTransaction(TransactionsModel transaction)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var sql = @"
            INSERT INTO Transactions (TransactionId, AccountId, Amount, TransactionType, TransactionDate, Description, ActiveInd)
            VALUES (@TransactionId, @AccountId, @Amount, @TransactionType, @TransactionDate, @Description, 1)";

                    await db.ExecuteAsync(sql, new
                    {
                        transaction.TransactionId,
                        transaction.AccountId,
                        transaction.Amount,
                        transaction.TransactionType,
                        transaction.TransactionDate,
                        transaction.Description
                    });
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task<IEnumerable<TransactionAggregate>> GetTransactionsAsync(bool activeOnly)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT * FROM Transactions 
            WHERE (@ActiveOnly = 0 OR ActiveInd = 1)";

            var transactions = await connection.QueryAsync<TransactionAggregate>(sql, new { ActiveOnly = activeOnly });
            return transactions;
        }

        public async Task<TransactionAggregate> GetDetailsByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
            SELECT TransactionId, TransactionDate, TransactionType as [Type], Amount, Description
            FROM Transactions
            WHERE Id = @Id";

            var transaction = await connection.QuerySingleOrDefaultAsync<TransactionAggregate>(sql, new { Id = id });
            return transaction;
        }

    }
}
