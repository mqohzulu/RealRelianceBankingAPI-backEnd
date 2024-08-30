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
    public class AccountsRepository : IAccountRepository
    {
        private readonly DapperContext _context;

        public AccountsRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAccount(Account account)
        {
            using (var db = _context.CreateConnection())
            {
                var sql = @"
                INSERT INTO Account (
                    AccountId, PersonId, AccountNumber, AccountType, Balance, IsClosed, ActiveInd
                ) VALUES (
                    @AccountId, @PersonId, @AccountNumber, @AccountType, @Balance, @IsClosed, @ActiveInd
                )";

                var parameters = new
                {
                    account.AccountID,
                    account.PersonID,
                    account.AccountNumber,
                    account.AccountType,
                    account.Balance,
                    IsClosed = account.Status,
                    account.ActiveInd
                };

                await db.ExecuteAsync(sql, parameters);
                return account.AccountID;
            }
        }
        public async Task<List<Account>> GetAccountsByPersonId(Guid personId)
        {
            using (var _dbConnection = _context.CreateConnection())
            {
                try
                {
                    var sql = "SELECT * FROM Account WHERE PersonId = @PersonId";
                    var accounts = await _dbConnection.QueryAsync<Account>(sql, new { PersonId = personId });
                    return accounts.ToList();
                }
                catch (Exception)
                {
                    return new List<Account>();
                }
            }

        }

        public async Task Add(Account account)
        {

            using (var _dbConnection = _context.CreateConnection())
            {
                try
                {
                    var sql = "INSERT INTO Account (AccountId, PersonId, AccountNumber, AccountType, Balance, IsClosed, ActiveInd, CreatedBy, CreatedDate) VALUES (@AccountId, @PersonId, @AccountNumber, @AccountType, @Balance, @IsClosed, @ActiveInd, @CreatedBy, @CreatedDate)";
                    await _dbConnection.ExecuteAsync(sql, account);
                }
                catch (Exception ex)
                {
                    new Exception();
                }
            }

        }
        public async Task Delete(Guid accountId)
        {

            using (var _dbConnection = _context.CreateConnection())
            {
                try
                {
                    var sql = "UPDATE Account SET activeInd = 0 WHERE AccountId = @AccountId";
                    await _dbConnection.ExecuteAsync(sql, new { AccountId = accountId });
                }
                catch (Exception ex)
                {
                    new Exception();
                }
            }

        }

        public async Task<Account> GetAccountById(Guid accountId)
        {

            using (var _dbConnection = _context.CreateConnection())
            {
                try
                {
                    var sql = "SELECT * FROM Account WHERE AccountId = @AccountId";
                    return await _dbConnection.QuerySingleOrDefaultAsync<Account>(sql, new { AccountId = accountId });
                }
                catch (Exception ex)
                {
                    return new Account();
                }
            }
        }
        public async Task UpdateAccountAsync(Account account)
        {

            using (var _dbConnection = _context.CreateConnection())
            {
                try
                {
                    var sql = "UPDATE Account SET Balance = @Balance  WHERE AccountId = @AccountId";
                    await _dbConnection.QuerySingleOrDefaultAsync<Account>(sql, new { account.Balance, account.AccountID });
                }
                catch (Exception ex)
                {
                }
            }
        }

        public async Task<List<AccountAggregate>> GetAccounts(bool activeOnly)
        {
            using (var _dbConnection = _context.CreateConnection())
            {
                try
                {
                    var sql = @"SELECT * FROM Account WHERE 1=1";
                    if (activeOnly)
                    {
                        sql += " AND ActiveInd = 1 AND IsClosed = 0";
                    }

                    var accounts = await _dbConnection.QueryAsync<AccountAggregate>(sql);
                    return accounts.ToList();
                }
                catch (Exception ex)
                {
                    return new List<AccountAggregate>();
                }
            }
        }

        public async Task<Account> GetByAccountNumber(string accountNumber)
        {
            using (var db = _context.CreateConnection())
            {
                var sql = "SELECT * FROM Account WHERE AccountNumber = @AccountNumber";
                var parameters = new { AccountNumber = accountNumber };

                return await db.QuerySingleOrDefaultAsync<Account>(sql, parameters);
            }
        }

        public async Task<bool> CloseAccount(Guid accountId)
        {
            using (var db = _context.CreateConnection())
            {
                var sql = @"
                UPDATE Account 
                SET IsClosed = 1, ActiveInd = 0 
                WHERE AccountId = @AccountId";

                var parameters = new { AccountId = accountId };

                var affectedRows = await db.ExecuteAsync(sql, parameters);
                return affectedRows > 0;
            }
        }
    }
}
