using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Transactions.Command.Update
{
    public record UpdateTransactionCommand(
         Guid TransactionId,
         Guid AccountId,
         DateTime TransactionDate,
         decimal Amount,
         string Description,
         string TransactionType
     ) : IRequest<UpdateTransactionResult>;

    public record UpdateTransactionResult(bool Success, string Message);
}
