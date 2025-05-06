using System;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using LecteurIptv.Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LecteurIptv.Backend.Tests.Services
{
    public class FavoritesServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<FavoritesService>> _loggerMock;
        private readonly FavoritesService _favoritesService;
        private readonly string _dbName;

        public FavoritesServiceTests()
        {
            // Create a unique database name for each test run to ensure isolation
            _dbName = $"FavoritesServiceTests_{Guid.NewGuid()}";
            _context = TestDbContextFactory.CreateDbContext(_dbName);

            // Set up mocks
            _loggerMock = new Mock<ILogger<FavoritesService>>();

            // Create the service with mocked dependencies
            _favoritesService = new FavoritesService(_loggerMock.Object, _context);

            // Seed the database with test data
            TestDataGenerator.SeedUsers(_context);
            TestDataGenerator.SeedChannels(_context);
            TestDataGenerator.SeedVodItems(_context);
            TestDataGenerator.SeedFavorites(_context);
        }

        public void Dispose()
        {
            // Clean up after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetFavoriteChannelsAsync_WithValidUserId_ReturnsFavoriteChannels()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _favoritesService.GetFavoriteChannelsAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == 1);
            Assert.Contains(result, c => c.Id == 2);
        }

        [Fact]
        public async Task GetFavoriteChannelsAsync_WithInvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = 999; // Non-existent ID

            // Act
            var result = await _favoritesService.GetFavoriteChannelsAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFavoriteVodsAsync_WithValidUserId_ReturnsFavoriteVods()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _favoritesService.GetFavoriteVodsAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, v => v.Id == 1);
            Assert.Contains(result, v => v.Id == 2);
        }

        [Fact]
        public async Task GetFavoriteVodsAsync_WithInvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = 999; // Non-existent ID

            // Act
            var result = await _favoritesService.GetFavoriteVodsAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddFavoriteChannelAsync_WithValidIds_AddsChannelToFavorites()
        {
            // Arrange
            var userId = 1;
            var channelId = 3; // Channel not yet in favorites

            // Act
            var result = await _favoritesService.AddFavoriteChannelAsync(userId, channelId);

            // Assert
            Assert.True(result);

            // Verify it was added to the database
            var favoriteInDb = await _context.UserFavoriteChannels
                .FirstOrDefaultAsync(ufc => ufc.UserId == userId && ufc.ChannelId == channelId);
            Assert.NotNull(favoriteInDb);
        }

        [Fact]
        public async Task AddFavoriteChannelAsync_WithInvalidUserId_ReturnsFalse()
        {
            // Arrange
            var userId = 999; // Non-existent ID
            var channelId = 1;

            // Act
            var result = await _favoritesService.AddFavoriteChannelAsync(userId, channelId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddFavoriteChannelAsync_WithInvalidChannelId_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var channelId = 999; // Non-existent ID

            // Act
            var result = await _favoritesService.AddFavoriteChannelAsync(userId, channelId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddFavoriteChannelAsync_WithExistingFavorite_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var channelId = 1; // Already in favorites

            // Act
            var result = await _favoritesService.AddFavoriteChannelAsync(userId, channelId);

            // Assert
            Assert.True(result);

            // Verify no duplicate was added
            var favoritesCount = await _context.UserFavoriteChannels
                .CountAsync(ufc => ufc.UserId == userId && ufc.ChannelId == channelId);
            Assert.Equal(1, favoritesCount);
        }

        [Fact]
        public async Task AddFavoriteVodAsync_WithValidIds_AddsVodToFavorites()
        {
            // Arrange
            var userId = 1;
            var vodItemId = 3; // VOD not yet in favorites

            // Act
            var result = await _favoritesService.AddFavoriteVodAsync(userId, vodItemId);

            // Assert
            Assert.True(result);

            // Verify it was added to the database
            var favoriteInDb = await _context.UserFavoriteVods
                .FirstOrDefaultAsync(ufv => ufv.UserId == userId && ufv.VodItemId == vodItemId);
            Assert.NotNull(favoriteInDb);
        }

        [Fact]
        public async Task AddFavoriteVodAsync_WithInvalidUserId_ReturnsFalse()
        {
            // Arrange
            var userId = 999; // Non-existent ID
            var vodItemId = 1;

            // Act
            var result = await _favoritesService.AddFavoriteVodAsync(userId, vodItemId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddFavoriteVodAsync_WithInvalidVodItemId_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var vodItemId = 999; // Non-existent ID

            // Act
            var result = await _favoritesService.AddFavoriteVodAsync(userId, vodItemId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddFavoriteVodAsync_WithExistingFavorite_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var vodItemId = 1; // Already in favorites

            // Act
            var result = await _favoritesService.AddFavoriteVodAsync(userId, vodItemId);

            // Assert
            Assert.True(result);

            // Verify no duplicate was added
            var favoritesCount = await _context.UserFavoriteVods
                .CountAsync(ufv => ufv.UserId == userId && ufv.VodItemId == vodItemId);
            Assert.Equal(1, favoritesCount);
        }

        [Fact]
        public async Task RemoveFavoriteChannelAsync_WithValidIds_RemovesChannelFromFavorites()
        {
            // Arrange
            var userId = 1;
            var channelId = 1; // Existing favorite

            // Act
            var result = await _favoritesService.RemoveFavoriteChannelAsync(userId, channelId);

            // Assert
            Assert.True(result);

            // Verify it was removed from the database
            var favoriteInDb = await _context.UserFavoriteChannels
                .FirstOrDefaultAsync(ufc => ufc.UserId == userId && ufc.ChannelId == channelId);
            Assert.Null(favoriteInDb);
        }

        [Fact]
        public async Task RemoveFavoriteChannelAsync_WithNonExistentFavorite_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var channelId = 999; // Non-existent favorite

            // Act
            var result = await _favoritesService.RemoveFavoriteChannelAsync(userId, channelId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveFavoriteVodAsync_WithValidIds_RemovesVodFromFavorites()
        {
            // Arrange
            var userId = 1;
            var vodItemId = 1; // Existing favorite

            // Act
            var result = await _favoritesService.RemoveFavoriteVodAsync(userId, vodItemId);

            // Assert
            Assert.True(result);

            // Verify it was removed from the database
            var favoriteInDb = await _context.UserFavoriteVods
                .FirstOrDefaultAsync(ufv => ufv.UserId == userId && ufv.VodItemId == vodItemId);
            Assert.Null(favoriteInDb);
        }

        [Fact]
        public async Task RemoveFavoriteVodAsync_WithNonExistentFavorite_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var vodItemId = 999; // Non-existent favorite

            // Act
            var result = await _favoritesService.RemoveFavoriteVodAsync(userId, vodItemId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsChannelFavoriteAsync_WithFavoriteChannel_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var channelId = 1; // Existing favorite

            // Act
            var result = await _favoritesService.IsChannelFavoriteAsync(userId, channelId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsChannelFavoriteAsync_WithNonFavoriteChannel_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var channelId = 3; // Non-favorite channel

            // Act
            var result = await _favoritesService.IsChannelFavoriteAsync(userId, channelId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsVodFavoriteAsync_WithFavoriteVod_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var vodItemId = 1; // Existing favorite

            // Act
            var result = await _favoritesService.IsVodFavoriteAsync(userId, vodItemId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsVodFavoriteAsync_WithNonFavoriteVod_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var vodItemId = 3; // Non-favorite VOD

            // Act
            var result = await _favoritesService.IsVodFavoriteAsync(userId, vodItemId);

            // Assert
            Assert.False(result);
        }
    }
}
