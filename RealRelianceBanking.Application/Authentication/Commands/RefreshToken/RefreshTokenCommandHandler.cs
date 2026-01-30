using MediatR;
using RealRelianceBanking.Application.Authentication.Common;
using RealRelianceBanking.Application.Common.Errors;
using RealRelianceBanking.Application.Common.Interfaces.Authentication;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Application.Common.Interfaces.Services;

namespace RealRelianceBanking.Application.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IDateTimeProvider dateTimeProvider)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<AuthenticationResult> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByRefreshToken(command.RefreshToken);

            if (user is null || string.IsNullOrWhiteSpace(user.RefreshToken))
            {
                throw new InvalidRefreshToken();
            }

            if (!string.Equals(user.RefreshToken, command.RefreshToken, StringComparison.Ordinal))
            {
                throw new InvalidRefreshToken();
            }

            if (!user.RefreshTokenExpires.HasValue || user.RefreshTokenExpires <= _dateTimeProvider.UtcNow)
            {
                throw new RefreshTokenExpired();
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            var refreshTokenResult = _jwtTokenGenerator.GenerateRefreshToken();
            await _userRepository.UpdateRefreshToken(user.Id, refreshTokenResult.Token, refreshTokenResult.ExpiresAt);

            return new AuthenticationResult(
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                token,
                refreshTokenResult.Token,
                refreshTokenResult.ExpiresAt);
        }
    }
}
