using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Queries.GetAccountById
{
    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Account>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIdQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAccountById(request.AccountId);
        }
    }

}
