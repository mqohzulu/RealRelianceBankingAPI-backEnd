using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealRelianceBanking.Application.Authentication.Common;
using RealRelianceBanking.Application.Common.Interfaces.Authentication;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using RealRelianceBanking.Domain.Entities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RealRelianceBanking.Infrastructure.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtOptions)
        {
            _dateTimeProvider = dateTimeProvider;
            _jwtSettings = jwtOptions.Value;
        }

        public string GenerateToken(User user)
        {
            var signingCredentials = new SigningCredentials(
                 new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                 SecurityAlgorithms.HmacSha256);



            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.FirstName.ToString()),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };
            claims.Add(new Claim(ClaimTypes.Role, user.Role));

            var expiryMinutes = _jwtSettings.ExpiryMinutes > 0
                ? _jwtSettings.ExpiryMinutes
                : _jwtSettings.ExpiryInMinutes;

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: _dateTimeProvider.UtcNow.AddMinutes(expiryMinutes),
                claims: claims,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public RefreshTokenResult GenerateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);
            var expiresAt = _dateTimeProvider.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
            return new RefreshTokenResult(refreshToken, expiresAt);
        }

    }
}
