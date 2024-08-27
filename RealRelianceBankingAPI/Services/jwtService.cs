using System.Security.Claims;
using System.IdentityModel.Tokens;


using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace RealRelianceBankingAPI.Services
{
    public class JwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid ExtractJwt()
        {
            string authorizationHandler = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string jwt = authorizationHandler.Split(' ')[1];

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.ReadToken(jwt);
            JwtSecurityToken JwtToken = token as JwtSecurityToken;
            IEnumerable<Claim> claim = JwtToken.Claims;

            string subject = claim.First(c => c.Type == "sub").Value;

            return Guid.Parse(subject);
        }
    }
}
