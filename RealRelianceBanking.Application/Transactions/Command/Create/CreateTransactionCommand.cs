using MediatR;
using System;

namespace RealRelianceBanking.Application.Transactions.Command.Create
{
    public record CreateTransactionCommand(
        Guid AccountId,
        DateTime TransactionDate,
        decimal Amount,
        string TransactionType,
        string Description) : IRequest<CreateTransactionResult>;
}
