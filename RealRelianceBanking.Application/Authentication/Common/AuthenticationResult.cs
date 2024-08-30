using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Authentication.Common
{
    public record AuthenticationResult(string FirstName, string LastName, string email, string role, string Token);
}
