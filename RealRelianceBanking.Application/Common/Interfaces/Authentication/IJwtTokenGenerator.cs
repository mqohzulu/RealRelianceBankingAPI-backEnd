using RealRelianceBanking.Application.Authentication.Common;
using RealRelianceBanking.Domain.Entities;

namespace RealRelianceBanking.Application.Common.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
        RefreshTokenResult GenerateRefreshToken();
    }
}
