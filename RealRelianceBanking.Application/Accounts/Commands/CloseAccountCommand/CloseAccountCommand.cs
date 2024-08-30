using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Accounts.Commands.CloseAccountCommand
{
    public record CloseAccountCommand(Guid AccountId) : IRequest<bool>;
}
