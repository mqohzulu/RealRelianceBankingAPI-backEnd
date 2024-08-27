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
    public class AccountRepository : IAccountRepository
    {
        Task IAccountRepository.Add(Account account)
        {
            throw new NotImplementedException();
        }

        Task<bool> IAccountRepository.CloseAccount(Guid accountId)
        {
            throw new NotImplementedException();
        }

        Task<Guid> IAccountRepository.CreateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        Task IAccountRepository.Delete(Guid accountId)
        {
            throw new NotImplementedException();
        }

        Task<Account> IAccountRepository.GetAccountById(Guid accountId)
        {
            throw new NotImplementedException();
        }

        Task<List<AccountAggregate>> IAccountRepository.GetAccounts(bool activeOnly)
        {
            throw new NotImplementedException();
        }

        Task<List<Account>> IAccountRepository.GetAccountsByPersonId(Guid personId)
        {
            throw new NotImplementedException();
        }

        Task<Account> IAccountRepository.GetByAccountNumber(string accountNumber)
        {
            throw new NotImplementedException();
        }

        Task IAccountRepository.UpdateAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
