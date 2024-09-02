using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Contracts.Transactions.Transafer;
using RealRelianceBanking.Contracts.Transactions.Transafer.TransferFundsCommand;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RealRelianceBanking.Application.Transactions.Command.Transafer
{
    public class TransferFundsCommandHandler : IRequestHandler<TransferFundsCommand, TransferFundsResult>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPersonRepository _personRepository;

        public TransferFundsCommandHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IPersonRepository personRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _personRepository = personRepository;
        }

        public async Task<TransferFundsResult> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            if (request.Amount <= 0)
            {
                return new TransferFundsResult(false, "Transfer amount must be positive.");
            }

            // Fetch accounts and person information
            var accountFrom = await _accountRepository.GetByAccountNumber(request.AccountFrom);
            var accountTo = await _accountRepository.GetByAccountNumber(request.AccountTo);

            if (accountFrom == null || accountTo == null)
            {
                return new TransferFundsResult(false, "One or both accounts do not exist.");
            }

            if (accountFrom.AccountID == accountTo.AccountID)
            {
                return new TransferFundsResult(false, "Cannot transfer funds to the same account.");
            }
            if (accountTo.Status ==true )
            {
                return new TransferFundsResult(false, "Cannot transfer funds to a closed account.");
            }

            var personTo = await _personRepository.GetPersonById(accountTo.PersonID);
            if (personTo == null)
            {
                return new TransferFundsResult(false, "Recipient person not found.");
            }

            if (accountFrom.Balance < request.Amount)
            {
                return new TransferFundsResult(false, "Insufficient funds in the source account.");
            }

            try
            {
                // Update account balances
                accountFrom.Balance -= request.Amount;
                accountTo.Balance += request.Amount;
                await _accountRepository.UpdateAccountAsync(accountFrom);
                await _accountRepository.UpdateAccountAsync(accountTo);

                // Create transaction records
                var transactionFrom = new TransactionsModel
                {
                    TransactionId = Guid.NewGuid(),
                    AccountId = accountFrom.AccountID,
                    Amount = -request.Amount,
                    TransactionType = "Debit",
                    TransactionDate = DateTime.UtcNow,
                    Description = $"{request.description}\nTransfer to {accountTo.AccountNumber} owned by {personTo.FirstName} {personTo.LastName}"
                };

                var transactionTo = new TransactionsModel
                {
                    TransactionId = Guid.NewGuid(),
                    AccountId = accountTo.AccountID,
                    Amount = request.Amount,
                    TransactionType = "Credit",
                    TransactionDate = DateTime.UtcNow,
                    Description = $"{request.description}\nTransfer from {accountFrom.AccountNumber}"
                };

                await _transactionRepository.AddTransaction(transactionFrom);
                await _transactionRepository.AddTransaction(transactionTo);


                return new TransferFundsResult(true, "Transfer successful.");
            }
            catch (Exception ex)
            {
                return new TransferFundsResult(false, "Transfer failed due to an unexpected error. Please try again later.");
            }
        }

    }
 }
