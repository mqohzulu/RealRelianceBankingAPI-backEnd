using RealRelianceBanking.Domain.Aggregates;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Common.Interfaces.Persistance
{
    public interface IAccountRepository
    {
        Task Add(Account account);
        Task Delete(Guid accountId);
        Task<Guid> CreateAccount(Account account);
        Task<Account> GetAccountById(Guid accountId);

        Task<List<Account>> GetAccountsByPersonId(Guid personId);

        Task UpdateAccountAsync(Account account);
        Task<List<AccountAggregate>> GetAccounts(bool activeOnly);

        Task<Account> GetByAccountNumber(string accountNumber);
        Task<bool> CloseAccount(Guid accountId);
    }
}
