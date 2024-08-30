using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace RealRelianceBanking.Contracts.Transactions.Transafer.TransferFundsCommand
{
    public record TransferFundsCommand(string AccountFrom, string AccountTo, decimal Amount, string description) : IRequest<TransferFundsResult>;
}
