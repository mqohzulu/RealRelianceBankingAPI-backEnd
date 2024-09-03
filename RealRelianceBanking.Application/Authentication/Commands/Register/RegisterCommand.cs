using MediatR;
using RealRelianceBanking.Application.Authentication.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Authentication.Commands.Register
{
    public record RegisterCommand(
      string Email,
      string FirstName,
      string LastName,
      string Password,
      string Role) : IRequest<AuthenticationResult>;
}
