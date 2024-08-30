using MediatR;
using RealRelianceBanking.Application.Authentication.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Authentication.Queries.Login
{
    public record LoginQuery(
     string Email,
     string Password) : IRequest<AuthenticationResult>;
}
