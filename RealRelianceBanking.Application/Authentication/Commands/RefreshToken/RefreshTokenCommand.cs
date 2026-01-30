using MediatR;
using RealRelianceBanking.Application.Authentication.Common;

namespace RealRelianceBanking.Application.Authentication.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthenticationResult>;
}
