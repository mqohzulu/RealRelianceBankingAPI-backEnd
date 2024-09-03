using RealRelianceBanking.Application.Authentication.Commands.Register;
using RealRelianceBanking.Application.Common.Errors;
using RealRelianceBanking.Application.Common.Interfaces.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Commands.AuthenticationTests
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
        private readonly RegisterCommandHandler _handler;

        public RegisterCommandHandlerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _handler = new RegisterCommandHandler(_mockUserRepository.Object, _mockJwtTokenGenerator.Object);
        }
        [Fact]
        public async Task Handle_ValidRequest_RegistersUserSuccessfully()
        {
            // Arrange
            var command = new RegisterCommand(
                "john@example.com",
                "john",
                "Doe",
                "password123",
                "Customer"
            );

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(command.Email))
                .ReturnsAsync((User)null);

            var newUser = new User
            {
                Email = command.Email,
                password = command.Password,
                Role = command.Role,
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                .Callback<User>(user =>
                {
                    user.FirstName = "John";
                    user.LastName = "Doe";
                });

            var token = "generated_jwt_token";
            _mockJwtTokenGenerator.Setup(generator => generator.GenerateToken(It.IsAny<User>()))
                .Returns(token);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal(command.Email, result.email);
            Assert.Equal(command.Role, result.role);
            Assert.Equal(token, result.Token);

            _mockUserRepository.Verify(repo => repo.Add(It.Is<User>(u =>
                u.Email == command.Email &&
                u.password == command.Password &&
                u.Role == command.Role
            )), Times.Once);

            _mockJwtTokenGenerator.Verify(generator => generator.GenerateToken(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ExistingEmail_ThrowsDuplicateEmailException()
        {
            // Arrange
            var command = new RegisterCommand(
                "existing@example.com",
                "existing",
                "fisher",
                "password123",
                "Customer"
            );

            var existingUser = new User { Email = command.Email };
            _mockUserRepository.Setup(repo => repo.GetUserByEmail(command.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateEmailException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _mockUserRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
            _mockJwtTokenGenerator.Verify(generator => generator.GenerateToken(It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task Handle_DatabaseError_ThrowsDuplicateEmailException()
        {
            // Arrange
            var command = new RegisterCommand(
                "error@example.com",
                "error",
                "random",
                "password123",
                "Customer"
            );

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(command.Email))
                .ReturnsAsync(new User());
            // Act & Assert
            await Assert.ThrowsAsync<DuplicateEmailException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _mockUserRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
            _mockJwtTokenGenerator.Verify(generator => generator.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}
