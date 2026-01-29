using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Transactions.Command.Create
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateTransactionCommandHandler(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CreateTransactionResult> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount == 0)
            {
                return new CreateTransactionResult(false, "Transaction amount cannot be zero.");
            }

            if (request.TransactionDate > _dateTimeProvider.UtcNow)
            {
                return new CreateTransactionResult(false, "Transaction date cannot be in the future.");
            }

            if (!IsValidTransactionType(request.TransactionType))
            {
                return new CreateTransactionResult(false, "Transaction type must be Debit or Credit.");
            }

            var account = await _accountRepository.GetAccountById(request.AccountId);
            if (account == null)
            {
                return new CreateTransactionResult(false, "Account not found.");
            }

            if (account.Status)
            {
                return new CreateTransactionResult(false, "Cannot add transactions to a closed account.");
            }

            var signedAmount = request.TransactionType.Equals("Debit", StringComparison.OrdinalIgnoreCase)
                ? -Math.Abs(request.Amount)
                : Math.Abs(request.Amount);

            account.Balance += signedAmount;
            await _accountRepository.UpdateAccountAsync(account);

            var transaction = new TransactionsModel
            {
                TransactionId = Guid.NewGuid(),
                AccountId = request.AccountId,
                TransactionDate = request.TransactionDate,
                CaptureDate = _dateTimeProvider.UtcNow,
                Amount = signedAmount,
                TransactionType = NormalizeTransactionType(request.TransactionType),
                Description = request.Description
            };

            await _transactionRepository.AddTransaction(transaction);

            return new CreateTransactionResult(true, "Transaction created successfully.", transaction.TransactionId);
        }

        private static bool IsValidTransactionType(string transactionType)
        {
            return transactionType.Equals("Debit", StringComparison.OrdinalIgnoreCase)
                || transactionType.Equals("Credit", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeTransactionType(string transactionType)
        {
            return transactionType.Equals("Debit", StringComparison.OrdinalIgnoreCase) ? "Debit" : "Credit";
        }
    }
}
