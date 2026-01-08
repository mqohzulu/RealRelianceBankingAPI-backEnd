using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Contact.Commands
{
    public record SendContactCommand(string Name,
        string Email,
        string Subject,
        string Message
    ) : IRequest<SendContactResult>;

    public record SendContactResult(
        bool Success,
        string Message
    );
}
