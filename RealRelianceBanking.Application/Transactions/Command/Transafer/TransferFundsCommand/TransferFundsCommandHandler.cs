using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using RealRelianceBanking.Contracts.Transactions.Transafer;
using RealRelianceBanking.Contracts.Transactions.Transafer.TransferFundsCommand;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace RealRelianceBanking.Application.Transactions.Command.Transafer
{
    public class TransferFundsCommandHandler : IRequestHandler<TransferFundsCommand, TransferFundsResult>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TransferFundsCommandHandler(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IPersonRepository personRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _personRepository = personRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<TransferFundsResult> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
            {
                return new TransferFundsResult(false, "Transfer amount must be positive and non-zero.");
            }

            if (string.IsNullOrWhiteSpace(request.AccountFrom) || string.IsNullOrWhiteSpace(request.AccountTo))
            {
                return new TransferFundsResult(false, "Both source and destination account numbers are required.");
            }

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

            if (accountFrom.Status)
            {
                return new TransferFundsResult(false, "Cannot transfer funds from a closed account.");
            }

            if (accountTo.Status)
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

            var currentDate = _dateTimeProvider.UtcNow;
            var outgoingDescription = BuildTransferDescription(request.description, $"Transfer to {accountTo.AccountNumber} owned by {personTo.FirstName} {personTo.LastName}");
            var incomingDescription = BuildTransferDescription(request.description, $"Transfer from {accountFrom.AccountNumber}");

            try
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                accountFrom.Balance -= request.Amount;
                accountTo.Balance += request.Amount;
                await _accountRepository.UpdateAccountAsync(accountFrom);
                await _accountRepository.UpdateAccountAsync(accountTo);

                var transactionFrom = new TransactionsModel
                {
                    TransactionId = Guid.NewGuid(),
                    AccountId = accountFrom.AccountID,
                    Amount = -request.Amount,
                    TransactionType = "Debit",
                    TransactionDate = currentDate,
                    CaptureDate = currentDate,
                    Description = outgoingDescription
                };

                var transactionTo = new TransactionsModel
                {
                    TransactionId = Guid.NewGuid(),
                    AccountId = accountTo.AccountID,
                    Amount = request.Amount,
                    TransactionType = "Credit",
                    TransactionDate = currentDate,
                    CaptureDate = currentDate,
                    Description = incomingDescription
                };

                await _transactionRepository.AddTransaction(transactionFrom);
                await _transactionRepository.AddTransaction(transactionTo);

                scope.Complete();

                return new TransferFundsResult(true, "Transfer successful.");
            }
            catch (Exception)
            {
                return new TransferFundsResult(false, "Transfer failed due to an unexpected error. Please try again later.");
            }
        }

        private static string BuildTransferDescription(string description, string details)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return details;
            }

            return $"{description.Trim()} - {details}";
        }
    }
}
