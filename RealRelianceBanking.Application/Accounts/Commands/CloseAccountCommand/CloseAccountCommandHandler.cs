using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Commands.CloseAccountCommand
{
    public record CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;

        public CloseAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetAccountById(request.AccountId);
            if (account == null)
            {
                throw new Exception($"Account with not found.");
            }

            if (account.Status)
            {
                throw new InvalidOperationException("This account is already closed.");
            }

            if (account.Balance != 0)
            {
                throw new InvalidOperationException("Cannot close an account with a non-zero balance.");
            }

            return await _accountRepository.CloseAccount(request.AccountId);
        }
    }
}
