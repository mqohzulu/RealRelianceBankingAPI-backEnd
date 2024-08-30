using MediatR;
using RealRelianceBanking.Application.Accounts.Queries.GetAccounts;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Queries.GetAccount
{
    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountAggregate>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountsQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<AccountAggregate>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAccounts(request.ActiveOnly);
        }
    }

}
