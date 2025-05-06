using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LecteurIptv.Backend.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IConfigurationSection> _jwtSectionMock;
        private readonly JwtService _jwtService;
        private readonly User _testUser;

        public JwtServiceTests()
        {
            // Set up configuration mock
            _configurationMock = new Mock<IConfiguration>();
            _jwtSectionMock = new Mock<IConfigurationSection>();

            // Set up JWT configuration values
            _jwtSectionMock.Setup(s => s.Value).Returns("TestSecretKeyWithAtLeast32Characters_ForHMACSHA256Signing");
            _configurationMock.Setup(c => c["Jwt:SecretKey"]).Returns("TestSecretKeyWithAtLeast32Characters_ForHMACSHA256Signing");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _configurationMock.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("60");

            // Create the service with mocked dependencies
            _jwtService = new JwtService(_configurationMock.Object);

            // Create a test user
            _testUser = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "testuser@example.com",
                Role = "User"
            };
        }

        [Fact]
        public void GenerateToken_WithValidUser_ReturnsValidToken()
        {
            // Act
            var token = _jwtService.GenerateToken(_testUser);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Decode the token to verify its contents
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Verify token claims
            Assert.Equal(_testUser.Id.ToString(), jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal(_testUser.Email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(_testUser.Username, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal(_testUser.Role, jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);

            // Verify token properties
            Assert.Equal("TestIssuer", jwtToken.Issuer);
            Assert.Equal("TestAudience", jwtToken.Audiences.First());
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var token = _jwtService.GenerateToken(_testUser);

            // Act
            var result = _jwtService.ValidateToken(token);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ReturnsFalse()
        {
            // Arrange
            var invalidToken = "invalid.token.string";

            // Act
            var result = _jwtService.ValidateToken(invalidToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateToken_WithExpiredToken_ReturnsFalse()
        {
            // This test would ideally create a token with a very short expiry
            // and then wait for it to expire, but that's not practical in a unit test.
            // Instead, we can mock the configuration to return a negative expiry time
            // to simulate an expired token.

            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Jwt:SecretKey"]).Returns("TestSecretKeyWithAtLeast32Characters_ForHMACSHA256Signing");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            configMock.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("-60"); // Negative expiry time

            var expiredTokenService = new JwtService(configMock.Object);
            var expiredToken = expiredTokenService.GenerateToken(_testUser);

            // Act
            var result = _jwtService.ValidateToken(expiredToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateToken_WithNullToken_ReturnsFalse()
        {
            // Act
            var result = _jwtService.ValidateToken(null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateToken_WithEmptyToken_ReturnsFalse()
        {
            // Act
            var result = _jwtService.ValidateToken(string.Empty);

            // Assert
            Assert.False(result);
        }
    }
}
