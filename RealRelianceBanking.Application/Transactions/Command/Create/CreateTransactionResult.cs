using System;

namespace RealRelianceBanking.Application.Transactions.Command.Create
{
    public record CreateTransactionResult(bool Success, string Message, Guid? TransactionId = null);
}
