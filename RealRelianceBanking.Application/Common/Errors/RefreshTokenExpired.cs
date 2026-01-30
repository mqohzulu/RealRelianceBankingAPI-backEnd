namespace RealRelianceBanking.Application.Common.Errors
{
    public class RefreshTokenExpired : Exception
    {
        public RefreshTokenExpired() : base("Refresh token expired.") { }
    }
}
