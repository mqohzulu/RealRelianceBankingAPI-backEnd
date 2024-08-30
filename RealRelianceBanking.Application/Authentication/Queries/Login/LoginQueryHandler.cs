using MediatR;
using RealRelianceBanking.Application.Authentication.Common;
using RealRelianceBanking.Application.Common.Interfaces.Authentication;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using RealRelianceBanking.Application.Common.Errors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Authentication.Queries.Login
{
    public class LoginQueryHandler :
        IRequestHandler<LoginQuery, AuthenticationResult>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;

        public LoginQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticationResult> Handle(LoginQuery query, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetUserByEmail(query.Email) is not User user)
            {
                throw new InvalidUser();
            }


            if (user.password != query.Password)
            {
                throw new InvalidPassword();
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthenticationResult(
              user.FirstName, user.LastName, user.Email, user.Role,
                token);
        }
    }
}
