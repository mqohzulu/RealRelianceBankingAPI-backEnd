using MediatR;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Transactions.Command.Update
{
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, UpdateTransactionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public UpdateTransactionCommandHandler(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public async Task<UpdateTransactionResult> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount == 0)
            {
                return new UpdateTransactionResult(false, "Transaction amount cannot be zero.");
            }

            if (request.TransactionDate > DateTime.UtcNow)
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

            if (account.Status == true)
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

                await _transactionRepository.UpdateTransaction(existingTransaction);

                return new UpdateTransactionResult(true, "Transaction updated successfully.");
            }
            catch (Exception ex)
            {
                return new UpdateTransactionResult(false, "Failed to update transaction. Please try again later.");
            }
        }
    }
}
