using MediatR;
using RealRelianceBanking.Application.Authentication.Common;
using RealRelianceBanking.Application.Common.Interfaces.Authentication;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using RealRelianceBanking.Application.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Authentication.Commands.Register
{
    public class RegisterCommandHandler :
    IRequestHandler<RegisterCommand, AuthenticationResult>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;

        public RegisterCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticationResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            //validate so that user doesn't exists
            if (await _userRepository.GetUserByEmail(command.Email) is not null)
            {
                throw new DuplicateEmailException();
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = command.Email,
                password = command.Password,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Role = command.Role,
            };

            await _userRepository.Add(user);

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthenticationResult(
                user.FirstName, user.LastName, user.Email, user.Role,
                token);
        }
    }
}
