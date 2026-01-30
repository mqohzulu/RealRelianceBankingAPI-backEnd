using RealRelianceBanking.Application.Authentication.Common;
using RealRelianceBanking.Application.Authentication.Queries.Login;
using RealRelianceBanking.Application.Common.Errors;
using RealRelianceBanking.Application.Common.Interfaces.Authentication;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;

namespace Tests.Query
{
    public class LoginQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
        private readonly LoginQueryHandler _handler;

        public LoginQueryHandlerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _handler = new LoginQueryHandler(_mockUserRepository.Object, _mockJwtTokenGenerator.Object);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsAuthenticationResult()
        {
            // Arrange
            var query = new LoginQuery("test@example.com", "password123");
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                password = "password123",
                FirstName = "John",
                LastName = "Doe",
                Role = "Customer"
            };
            var token = "generated_jwt_token";
            var refreshTokenResult = new RefreshTokenResult("refresh_token", DateTime.UtcNow.AddDays(7));

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(query.Email))
                .ReturnsAsync(user);
            _mockJwtTokenGenerator.Setup(generator => generator.GenerateToken(user))
                .Returns(token);
            _mockJwtTokenGenerator.Setup(generator => generator.GenerateRefreshToken())
                .Returns(refreshTokenResult);
            _mockUserRepository.Setup(repo => repo.UpdateRefreshToken(user.Id, refreshTokenResult.Token, refreshTokenResult.ExpiresAt))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.email);
            Assert.Equal(user.Role, result.role);
            Assert.Equal(token, result.Token);
            Assert.Equal(refreshTokenResult.Token, result.RefreshToken);
        }

        [Fact]
        public async Task Handle_NonExistentEmail_ThrowsInvalidUserException()
        {
            // Arrange
            var query = new LoginQuery("nonexistent@example.com", "password123");

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(query.Email))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidUser>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_IncorrectPassword_ThrowsInvalidPasswordException()
        {
            // Arrange
            var query = new LoginQuery("test@example.com", "wrongpassword");
            var user = new User
            {
                Email = "test@example.com",
                password = "correctpassword",
            };

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(query.Email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidPassword>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
