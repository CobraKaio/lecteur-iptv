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
    public class HistoryServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<HistoryService>> _loggerMock;
        private readonly Mock<IChannelsService> _channelsServiceMock;
        private readonly Mock<IVodService> _vodServiceMock;
        private readonly HistoryService _historyService;
        private readonly string _dbName;

        public HistoryServiceTests()
        {
            // Create a unique database name for each test run to ensure isolation
            _dbName = $"HistoryServiceTests_{Guid.NewGuid()}";
            _context = TestDbContextFactory.CreateDbContext(_dbName);

            // Set up mocks
            _loggerMock = new Mock<ILogger<HistoryService>>();
            _channelsServiceMock = new Mock<IChannelsService>();
            _vodServiceMock = new Mock<IVodService>();

            // Set up mock behavior
            _channelsServiceMock
                .Setup(s => s.GetChannelByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _context.Channels.FirstOrDefault(c => c.Id == id));

            _vodServiceMock
                .Setup(s => s.GetVodItemByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _context.VodItems.FirstOrDefault(v => v.Id == id));

            // Create the service with mocked dependencies
            _historyService = new HistoryService(
                _loggerMock.Object,
                _context,
                _channelsServiceMock.Object,
                _vodServiceMock.Object);

            // Seed the database with test data
            TestDataGenerator.SeedUsers(_context);
            TestDataGenerator.SeedChannels(_context);
            TestDataGenerator.SeedVodItems(_context);
            TestDataGenerator.SeedHistory(_context);
        }

        public void Dispose()
        {
            // Clean up after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task LogViewAsync_WithValidChannelData_CreatesHistoryEntry()
        {
            // Arrange
            var userId = 1;
            var contentId = 2; // Channel ID
            var contentType = "channel";
            var durationSeconds = 300;

            // Act
            var result = await _historyService.LogViewAsync(userId, contentId, contentType, durationSeconds);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(contentId, result.ContentId);
            Assert.Equal(contentType, result.ContentType);
            Assert.Equal(durationSeconds, result.DurationSeconds);

            // Verify it was added to the database
            var historyInDb = await _context.UserHistory
                .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == contentId && h.ContentType == contentType);
            Assert.NotNull(historyInDb);
        }

        [Fact]
        public async Task LogViewAsync_WithValidVodData_CreatesHistoryEntry()
        {
            // Arrange
            var userId = 1;
            var contentId = 2; // VOD ID
            var contentType = "vod";
            var durationSeconds = 1800;
            var positionSeconds = 900;

            // Act
            var result = await _historyService.LogViewAsync(userId, contentId, contentType, durationSeconds, positionSeconds);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(contentId, result.ContentId);
            Assert.Equal(contentType, result.ContentType);
            Assert.Equal(durationSeconds, result.DurationSeconds);
            Assert.Equal(positionSeconds, result.PositionSeconds);

            // Verify it was added to the database
            var historyInDb = await _context.UserHistory
                .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == contentId && h.ContentType == contentType);
            Assert.NotNull(historyInDb);
        }

        [Fact]
        public async Task LogViewAsync_WithInvalidContentType_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var contentId = 1;
            var contentType = "invalid"; // Invalid content type

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _historyService.LogViewAsync(userId, contentId, contentType));
        }

        [Fact]
        public async Task LogViewAsync_WithExistingEntry_CreatesNewEntry()
        {
            // Arrange
            var userId = 1;
            var contentId = 1;
            var contentType = "channel"; // Existing history entry
            var newDurationSeconds = 600;

            // Get the current count of history entries
            var initialCount = await _context.UserHistory.CountAsync();

            // Act
            var result = await _historyService.LogViewAsync(userId, contentId, contentType, newDurationSeconds);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newDurationSeconds, result.DurationSeconds);

            // Verify a new entry was created
            var finalCount = await _context.UserHistory.CountAsync();
            Assert.Equal(initialCount + 1, finalCount);
        }

        [Fact]
        public async Task GetHistoryAsync_WithValidUserId_ReturnsHistory()
        {
            // Arrange
            var userId = 1;
            var limit = 10;
            var offset = 0;

            // Act
            var result = await _historyService.GetHistoryAsync(userId, limit, offset);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(userId, item.UserId));
        }

        [Fact]
        public async Task GetHistoryAsync_WithInvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = 999; // Non-existent ID
            var limit = 10;
            var offset = 0;

            // Act
            var result = await _historyService.GetHistoryAsync(userId, limit, offset);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetHistoryAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var userId = 1;
            var limit = 1;
            var offset = 0;

            // Act
            var result = await _historyService.GetHistoryAsync(userId, limit, offset);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetHistoryByTypeAsync_WithValidData_ReturnsFilteredHistory()
        {
            // Arrange
            var userId = 1;
            var contentType = "channel";
            var limit = 10;
            var offset = 0;

            // Act
            var result = await _historyService.GetHistoryByTypeAsync(userId, contentType, limit, offset);

            // Assert
            Assert.Single(result);
            Assert.All(result, item => Assert.Equal(contentType, item.ContentType));
        }

        [Fact]
        public async Task GetHistoryByTypeAsync_WithInvalidContentType_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var contentType = "invalid"; // Invalid content type
            var limit = 10;
            var offset = 0;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _historyService.GetHistoryByTypeAsync(userId, contentType, limit, offset));
        }



        [Fact]
        public async Task DeleteHistoryEntryAsync_WithValidId_DeletesHistoryEntry()
        {
            // Arrange
            var userId = 1;
            var historyId = 1;

            // Create a logger mock
            var loggerMock = new Mock<ILogger<TestHistoryService>>();

            // Create the test service
            var historyService = new TestHistoryService(loggerMock.Object);

            // Act
            var result = await historyService.DeleteHistoryEntryAsync(userId, historyId);

            // Assert
            Assert.True(result);

            // Verify the entry was deleted by trying to get history
            var history = await historyService.GetHistoryAsync(userId);
            Assert.DoesNotContain(history, h => h.Id == historyId);
        }

        [Fact]
        public async Task DeleteHistoryEntryAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var historyId = 999; // Non-existent ID

            // Create a logger mock
            var loggerMock = new Mock<ILogger<TestHistoryService>>();

            // Create the test service
            var historyService = new TestHistoryService(loggerMock.Object);

            // Act
            var result = await historyService.DeleteHistoryEntryAsync(userId, historyId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ClearHistoryAsync_WithValidUserId_ClearsUserHistory()
        {
            // Arrange
            var userId = 1;

            // Create a logger mock
            var loggerMock = new Mock<ILogger<TestHistoryService>>();

            // Create the test service
            var historyService = new TestHistoryService(loggerMock.Object);

            // Get initial count of history entries for the user
            var initialHistory = await historyService.GetHistoryAsync(userId);
            var initialCount = initialHistory.Count();

            // Act
            var result = await historyService.ClearHistoryAsync(userId);

            // Assert
            Assert.Equal(initialCount, result);

            // Verify all entries were deleted
            var remainingHistory = await historyService.GetHistoryAsync(userId);
            Assert.Empty(remainingHistory);
        }

        [Fact]
        public async Task ClearHistoryAsync_WithInvalidUserId_ReturnsZero()
        {
            // Arrange
            var userId = 999; // Non-existent ID

            // Create a logger mock
            var loggerMock = new Mock<ILogger<TestHistoryService>>();

            // Create the test service
            var historyService = new TestHistoryService(loggerMock.Object);

            // Act
            var result = await historyService.ClearHistoryAsync(userId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task UpdatePositionAsync_WithValidData_UpdatesPosition()
        {
            // Arrange
            var userId = 1;
            var historyId = 2; // VOD history entry
            var newPosition = 1200;

            // Create a logger mock
            var loggerMock = new Mock<ILogger<TestHistoryService>>();

            // Create the test service
            var historyService = new TestHistoryService(loggerMock.Object);

            // Act
            var result = await historyService.UpdatePositionAsync(userId, historyId, newPosition);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newPosition, result.PositionSeconds);

            // Verify it was updated by getting the history entry again
            var history = await historyService.GetHistoryAsync(userId);
            var updatedEntry = history.FirstOrDefault(h => h.Id == historyId);
            Assert.NotNull(updatedEntry);
            Assert.Equal(newPosition, updatedEntry.PositionSeconds);
        }

        [Fact]
        public async Task UpdatePositionAsync_WithInvalidHistoryId_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var historyId = 999; // Non-existent ID
            var newPosition = 1200;

            // Create a logger mock
            var loggerMock = new Mock<ILogger<TestHistoryService>>();

            // Create the test service
            var historyService = new TestHistoryService(loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                historyService.UpdatePositionAsync(userId, historyId, newPosition));
        }
    }
}
