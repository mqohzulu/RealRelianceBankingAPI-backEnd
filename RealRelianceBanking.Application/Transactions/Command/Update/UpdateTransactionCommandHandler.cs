using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Transactions.Command.Update
{
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, UpdateTransactionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UpdateTransactionCommandHandler(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<UpdateTransactionResult> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount == 0)
            {
                return new UpdateTransactionResult(false, "Transaction amount cannot be zero.");
            }

            if (request.TransactionDate > _dateTimeProvider.UtcNow)
            {
                return new UpdateTransactionResult(false, "Transaction date cannot be in the future.");
            }

            var existingTransaction = await _transactionRepository.GetTransactionById(request.TransactionId);
            if (existingTransaction == null)
            {
                return new UpdateTransactionResult(false, "Transaction not found.");
            }

            var account = await _accountRepository.GetAccountById(request.AccountId);
            if (account == null)
            {
                return new UpdateTransactionResult(false, "Account not found.");
            }

            if (account.Status)
            {
                return new UpdateTransactionResult(false, "Cannot update transactions for a closed account.");
            }

            try
            {
                var balanceAdjustment = request.Amount - existingTransaction.Amount;

                account.Balance += balanceAdjustment;
                await _accountRepository.UpdateAccountAsync(account);

                existingTransaction.Amount = request.Amount;
                existingTransaction.Description = request.Description;
                existingTransaction.TransactionType = request.TransactionType;
                existingTransaction.TransactionDate = request.TransactionDate;
                existingTransaction.CaptureDate = _dateTimeProvider.UtcNow;

                await _transactionRepository.UpdateTransaction(existingTransaction);

                return new UpdateTransactionResult(true, "Transaction updated successfully.");
            }
            catch (Exception)
            {
                return new UpdateTransactionResult(false, "Failed to update transaction. Please try again later.");
            }
        }
    }
}
