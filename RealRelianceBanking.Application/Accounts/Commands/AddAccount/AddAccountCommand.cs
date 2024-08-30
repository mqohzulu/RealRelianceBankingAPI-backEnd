using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Commands.AddAccount
{
    public record CreateAccountCommand(
     Guid PersonId,
     string AccountNumber,
     string AccountType,
     decimal Balance,
     bool IsClosed,
     bool ActiveInd
 ) : IRequest<Guid>;

}
