namespace RealRelianceBanking.Application.Authentication.Common
{
    public record RefreshTokenResult(string Token, DateTime ExpiresAt);
}
