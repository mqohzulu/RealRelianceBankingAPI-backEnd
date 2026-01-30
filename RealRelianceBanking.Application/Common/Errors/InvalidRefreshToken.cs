namespace RealRelianceBanking.Application.Common.Errors
{
    public class InvalidRefreshToken : Exception
    {
        public InvalidRefreshToken() : base("Invalid refresh token.") { }
    }
}
