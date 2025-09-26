using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Domain.Common.Settings;
using BaseCleanWithJwt.Domain.DTO.AuthDTO;
using BaseCleanWithJwt.Domain.DTO.UserDTO;
using BaseCleanWithJwt.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace BaseCleanWithJwt.Application.Service.Tests;

public class AuthServiceTest
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
    private readonly Mock<IJwtProvider> _jwtProvider;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IUserService> _userService;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _jwtProvider = new Mock<IJwtProvider>();
        _userRepository = new Mock<IUserRepository>();
        _userService = new Mock<IUserService>();

        _jwtSettings = Options.Create(new JwtSettings
        {
            Expiration = TimeSpan.FromMinutes(30),
            RefreshTokenExpiration = TimeSpan.FromDays(7),
            IssuerSigningKey = "super-secret-key-for-testing-purposes-that-is-long-enough"
        });

        _authService = new AuthService(
            _refreshTokenRepository.Object,
            _jwtSettings,
            _userRepository.Object,
            _jwtProvider.Object,
            _userService.Object
        );
    }

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        const string password = "SecurePassword123!";
        var userId = Guid.NewGuid();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new UserModel
        {
            Id = userId,
            Email = "user@test.com",
            PasswordHash = hashedPassword,
            FullName = "Test User"
        };

        var request = new LoginRequestDTO
        {
            Email = user.Email,
            Password = password
        };

        const string expectedAccessToken = "access-token-12345";
        const string expectedRefreshToken = "refresh-token-67890";
        var expectedAccessTokenExpiry = DateTime.UtcNow.Add(_jwtSettings.Value.Expiration);
        var expectedRefreshTokenExpiry = DateTime.UtcNow.Add(_jwtSettings.Value.RefreshTokenExpiration);

        _userRepository.Setup(r => r.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _jwtProvider.Setup(j => j.GenerateToken(user))
            .Returns(expectedAccessToken);

        _jwtProvider.Setup(j => j.GenerateRefreshToken())
            .Returns(expectedRefreshToken);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAccessToken, result.AccessToken);
        Assert.Equal(expectedRefreshToken, result.RefreshToken);
        Assert.True(Math.Abs((result.AccessTokenExpires - expectedAccessTokenExpiry).TotalSeconds) < 5);
        Assert.True(Math.Abs((result.RefreshTokenExpires - expectedRefreshTokenExpiry).TotalSeconds) < 5);

        // Verify interactions
        _userRepository.Verify(r => r.GetByEmailAsync(user.Email), Times.Once);
        _jwtProvider.Verify(j => j.GenerateToken(user), Times.Once);
        _jwtProvider.Verify(j => j.GenerateRefreshToken(), Times.Once);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(userId, expectedRefreshToken, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var request = new LoginRequestDTO
        {
            Email = "nonexistent@test.com",
            Password = "any-password"
        };

        _userRepository.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((UserModel?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Null(result);

        // Verify that no tokens were generated
        _jwtProvider.Verify(j => j.GenerateToken(It.IsAny<UserModel>()), Times.Never);
        _jwtProvider.Verify(j => j.GenerateRefreshToken(), Times.Never);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password")
        };

        var request = new LoginRequestDTO
        {
            Email = user.Email,
            Password = "wrong-password"
        };

        _userRepository.Setup(r => r.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Null(result);

        // Verify that tokens were not generated
        _jwtProvider.Verify(j => j.GenerateToken(It.IsAny<UserModel>()), Times.Never);
        _jwtProvider.Verify(j => j.GenerateRefreshToken(), Times.Never);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    #endregion

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_WithNewUser_ShouldCreateUserAndReturnTokens()
    {
        // Arrange
        var request = new UserRequestDTO
        {
            Email = "newuser@test.com",
            Password = "SecurePassword123!",
            FullName = "New User"
        };

        var userId = Guid.NewGuid();
        var createdUserResponse = new UserResponseDTO
        {
            Id = userId,
            Email = request.Email,
            FullName = request.FullName
        };

        var userModel = new UserModel
        {
            Id = userId,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName
        };

        const string expectedAccessToken = "access-token-new-user";
        const string expectedRefreshToken = "refresh-token-new-user";

        _userRepository.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((UserModel?)null);

        _userService.Setup(s => s.InsertOneAsync(request))
            .ReturnsAsync(createdUserResponse);

        _userRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(userModel);

        _jwtProvider.Setup(j => j.GenerateToken(userModel))
            .Returns(expectedAccessToken);

        _jwtProvider.Setup(j => j.GenerateRefreshToken())
            .Returns(expectedRefreshToken);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAccessToken, result.AccessToken);
        Assert.Equal(expectedRefreshToken, result.RefreshToken);
        Assert.True(result.AccessTokenExpires > DateTime.UtcNow);
        Assert.True(result.RefreshTokenExpires > DateTime.UtcNow);

        // Verify all interactions
        _userRepository.Verify(r => r.GetByEmailAsync(request.Email), Times.Once);
        _userService.Verify(s => s.InsertOneAsync(request), Times.Once);
        _userRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _jwtProvider.Verify(j => j.GenerateToken(userModel), Times.Once);
        _jwtProvider.Verify(j => j.GenerateRefreshToken(), Times.Once);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(userId, expectedRefreshToken, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new UserRequestDTO
        {
            Email = "existing@test.com",
            Password = "SecurePassword123!",
            FullName = "Existing User"
        };

        var existingUser = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = "existing-hash"
        };

        _userRepository.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(request));

        Assert.Equal("Email already registered.", exception.Message);

        // Verify that user creation was not attempted
        _userService.Verify(s => s.InsertOneAsync(It.IsAny<UserRequestDTO>()), Times.Never);
        _jwtProvider.Verify(j => j.GenerateToken(It.IsAny<UserModel>()), Times.Never);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserNotFoundAfterRegistration_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new UserRequestDTO
        {
            Email = "newuser@test.com",
            Password = "SecurePassword123!",
            FullName = "New User"
        };

        var userId = Guid.NewGuid();
        var createdUserResponse = new UserResponseDTO
        {
            Id = userId,
            Email = request.Email,
            FullName = request.FullName
        };

        _userRepository.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((UserModel?)null);

        _userService.Setup(s => s.InsertOneAsync(request))
            .ReturnsAsync(createdUserResponse);

        _userRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((UserModel?)null); // User not found after registration

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(request));

        Assert.Equal("User not found after registration.", exception.Message);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        const string refreshToken = "valid-refresh-token";
        var userId = Guid.NewGuid();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(1);

        var refreshTokenData = new RefreshTokenModel
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = refreshTokenExpiry
        };

        var user = new UserModel
        {
            Id = userId,
            Email = "user@test.com",
            PasswordHash = "hash",
            FullName = "Test User"
        };

        const string expectedAccessToken = "new-access-token";
        const string expectedNewRefreshToken = "new-refresh-token";

        _refreshTokenRepository.Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(refreshTokenData);

        _userRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _jwtProvider.Setup(j => j.GenerateToken(user))
            .Returns(expectedAccessToken);

        _jwtProvider.Setup(j => j.GenerateRefreshToken())
            .Returns(expectedNewRefreshToken);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAccessToken, result.AccessToken);
        Assert.Equal(expectedNewRefreshToken, result.RefreshToken);
        Assert.True(result.AccessTokenExpires > DateTime.UtcNow);
        Assert.True(result.RefreshTokenExpires > DateTime.UtcNow);

        // Verify interactions
        _refreshTokenRepository.Verify(r => r.GetByTokenAsync(refreshToken), Times.Once);
        _userRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _jwtProvider.Verify(j => j.GenerateToken(user), Times.Once);
        _jwtProvider.Verify(j => j.GenerateRefreshToken(), Times.Once);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(userId, expectedNewRefreshToken, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        const string refreshToken = "invalid-refresh-token";

        _refreshTokenRepository.Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync((RefreshTokenModel?)null);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.Null(result);

        // Verify no other operations were performed
        _userRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _jwtProvider.Verify(j => j.GenerateToken(It.IsAny<UserModel>()), Times.Never);
        _jwtProvider.Verify(j => j.GenerateRefreshToken(), Times.Never);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredToken_ShouldReturnNull()
    {
        // Arrange
        const string refreshToken = "expired-refresh-token";
        var userId = Guid.NewGuid();
        var expiredTokenData = new RefreshTokenModel
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(-1) // Expired
        };

        _refreshTokenRepository.Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(expiredTokenData);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.Null(result);

        // Verify no tokens were generated
        _userRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _jwtProvider.Verify(j => j.GenerateToken(It.IsAny<UserModel>()), Times.Never);
        _refreshTokenRepository.Verify(r =>
            r.UpdateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenUserNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const string refreshToken = "valid-refresh-token";
        var userId = Guid.NewGuid();
        var refreshTokenData = new RefreshTokenModel
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _refreshTokenRepository.Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(refreshTokenData);

        _userRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RefreshTokenAsync(refreshToken));

        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithNullTokenValue_ShouldReturnNull()
    {
        // Arrange
        const string refreshToken = "token-with-null-value";
        var userId = Guid.NewGuid();
        var refreshTokenData = new RefreshTokenModel
        {
            UserId = userId,
            Token = null, // Null token value
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _refreshTokenRepository.Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(refreshTokenData);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.Null(result);
    }
    #endregion

}