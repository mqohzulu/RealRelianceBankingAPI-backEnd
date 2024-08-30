using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Contracts.Transactions.Transafer
{
    public record TransferFundsResult(bool Success, string Message);
}
