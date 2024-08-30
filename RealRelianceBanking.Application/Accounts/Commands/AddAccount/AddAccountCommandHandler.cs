using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Commands.AddAccount
{
    public record CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPersonRepository _personRepository;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IPersonRepository personRepository)
        {
            _accountRepository = accountRepository;
            _personRepository = personRepository;
        }

        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // Check if the person exists
            var person = await _personRepository.GetPersonById(request.PersonId);
            if (person == null)
            {
                throw new Exception($"Person was not found.");
            }

            // Check if the account number is unique
            var existingAccount = await _accountRepository.GetByAccountNumber(request.AccountNumber);
            if (existingAccount != null)
            {
                throw new InvalidOperationException("An account with this account number already exists.");
            }

            var account = new Account
            {
                AccountID = Guid.NewGuid(),
                PersonID = request.PersonId,
                AccountNumber = request.AccountNumber,
                AccountType = request.AccountType,
                Balance = request.Balance,
                Status = request.IsClosed,
                ActiveInd = request.ActiveInd
            };

            return await _accountRepository.CreateAccount(account);
        }
    }

}
