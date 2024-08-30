using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Queries.GetAccountsByPersonId
{
    public class GetAccountsByPersonIdQueryHandler : IRequestHandler<GetAccountsByPersonIdQuery, List<Account>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountsByPersonIdQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<Account>> Handle(GetAccountsByPersonIdQuery request, CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAccountsByPersonId(request.PersonId);
        }
    }


}
