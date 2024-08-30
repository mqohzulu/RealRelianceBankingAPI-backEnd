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

        public TransferFundsCommandHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<TransferFundsResult> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
        {

                var accountFrom = await _accountRepository.GetByAccountNumber(request.AccountFrom);
                var accountTo = await _accountRepository.GetByAccountNumber(request.AccountTo);

                if (accountFrom == null || accountTo == null)
                {
                    return new TransferFundsResult(false, "One or both accounts do not exist.");
                }

                if (accountFrom.Balance < request.Amount)
                {
                    return new TransferFundsResult(false, "Insufficient funds in the source account.");
                }

   
                    try
                    {
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
                            TransactionDate = DateTime.UtcNow,
                            Description = $"{request.description}\nTransfer to {accountTo.AccountNumber}"
                        };

                        var transactionTo = new TransactionsModel
                        {
                            TransactionId = Guid.NewGuid(),
                            AccountId = accountTo.AccountID,
                            Amount = request.Amount,
                            TransactionType = "Credit",
                            TransactionDate = DateTime.UtcNow,
                            Description = $"{request.description}\nTransfer to {accountTo.AccountNumber}"
                        };

                        await _transactionRepository.AddTransaction(transactionFrom);
                        await _transactionRepository.AddTransaction(transactionTo);

                        return new TransferFundsResult(true, "Transfer successful.");
                    }
                    catch (Exception ex)
                    {
                        return new TransferFundsResult(false, $"Transfer failed: {ex.Message}");
                    }
                
            }
        
    }
 }
