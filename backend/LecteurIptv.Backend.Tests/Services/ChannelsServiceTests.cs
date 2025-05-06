using System;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using LecteurIptv.Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LecteurIptv.Backend.Tests.Services
{
    public class ChannelsServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<ChannelsService>> _loggerMock;
        private readonly Mock<IStreamingService> _streamingServiceMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly ChannelsService _channelsService;
        private readonly string _dbName;

        public ChannelsServiceTests()
        {
            // Create a unique database name for each test run to ensure isolation
            _dbName = $"ChannelsServiceTests_{Guid.NewGuid()}";
            _context = TestDbContextFactory.CreateDbContext(_dbName);

            // Set up mocks
            _loggerMock = new Mock<ILogger<ChannelsService>>();
            _streamingServiceMock = new Mock<IStreamingService>();
            _memoryCacheMock = new Mock<IMemoryCache>();

            // Set up memory cache mock to handle cache entries
            var cacheEntry = new Mock<ICacheEntry>();
            _memoryCacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntry.Object);

            // Create the service with mocked dependencies
            _channelsService = new ChannelsService(
                _loggerMock.Object,
                _context,
                _streamingServiceMock.Object,
                _memoryCacheMock.Object);

            // Seed the database with test data
            TestDataGenerator.SeedChannels(_context);
        }

        public void Dispose()
        {
            // Clean up after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllChannelsAsync_ReturnsAllChannels()
        {
            // Arrange
            var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _channelsService.GetAllChannelsAsync(parameters);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(1, result.TotalPages);
            Assert.False(result.HasNextPage);
            Assert.False(result.HasPreviousPage);
        }

        [Fact]
        public async Task GetActiveChannelsAsync_ReturnsOnlyActiveChannels()
        {
            // Arrange
            var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _channelsService.GetActiveChannelsAsync(parameters);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, item => Assert.True(item.IsActive));
        }

        [Fact]
        public async Task GetChannelByIdAsync_WithValidId_ReturnsChannel()
        {
            // Arrange
            var channelId = 1;

            // Act
            var result = await _channelsService.GetChannelByIdAsync(channelId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(channelId, result.Id);
            Assert.Equal("Test Channel 1", result.Name);
        }

        [Fact]
        public async Task GetChannelByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var channelId = 999; // Non-existent ID

            // Act
            var result = await _channelsService.GetChannelByIdAsync(channelId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetChannelsByGroupAsync_ReturnsChannelsInGroup()
        {
            // Arrange
            var group = "News";
            var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _channelsService.GetChannelsByGroupAsync(group, parameters);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Channel 1", result.Items.First().Name);
        }

        [Fact]
        public async Task GetChannelsByCategoryAsync_ReturnsChannelsInCategory()
        {
            // Arrange
            var category = "Sports";
            var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _channelsService.GetChannelsByCategoryAsync(category, parameters);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Channel 2", result.Items.First().Name);
        }

        [Fact]
        public async Task SearchChannelsAsync_WithValidTerm_ReturnsMatchingChannels()
        {
            // Arrange
            var searchTerm = "Channel 1";
            var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _channelsService.SearchChannelsAsync(searchTerm, parameters);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Channel 1", result.Items.First().Name);
        }

        [Fact]
        public async Task SearchChannelsAsync_WithEmptyTerm_ReturnsActiveChannels()
        {
            // Arrange
            var searchTerm = "";
            var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _channelsService.SearchChannelsAsync(searchTerm, parameters);

            // Assert
            Assert.Equal(2, result.TotalCount); // Only active channels
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public async Task AddChannelAsync_AddsNewChannel()
        {
            // Arrange
            var newChannel = new Channel
            {
                Name = "New Test Channel",
                StreamUrl = "http://example.com/new-stream.m3u8",
                LogoUrl = "http://example.com/new-logo.png",
                Group = "Test",
                Category = "Test",
                IsActive = true
            };

            _streamingServiceMock
                .Setup(s => s.IsStreamAvailableAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _channelsService.AddChannelAsync(newChannel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Test Channel", result.Name);

            // Verify it was added to the database
            var channelInDb = await _context.Channels.FirstOrDefaultAsync(c => c.Name == "New Test Channel");
            Assert.NotNull(channelInDb);
        }

        [Fact]
        public async Task UpdateChannelAsync_WithValidId_UpdatesChannel()
        {
            // Arrange
            var channelId = 1;
            var updatedChannel = new Channel
            {
                Id = channelId,
                Name = "Updated Channel",
                StreamUrl = "http://example.com/updated-stream.m3u8",
                LogoUrl = "http://example.com/updated-logo.png",
                Group = "Updated",
                Category = "Updated",
                IsActive = true
            };

            _streamingServiceMock
                .Setup(s => s.IsStreamAvailableAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _channelsService.UpdateChannelAsync(channelId, updatedChannel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Channel", result.Name);

            // Verify it was updated in the database
            var channelInDb = await _context.Channels.FindAsync(channelId);
            Assert.Equal("Updated Channel", channelInDb.Name);
        }

        [Fact]
        public async Task UpdateChannelAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var channelId = 999; // Non-existent ID
            var updatedChannel = new Channel
            {
                Id = channelId,
                Name = "Updated Channel",
                StreamUrl = "http://example.com/updated-stream.m3u8"
            };

            // Act
            var result = await _channelsService.UpdateChannelAsync(channelId, updatedChannel);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteChannelAsync_WithValidId_DeletesChannel()
        {
            // Arrange
            var channelId = 1;

            // Act
            var result = await _channelsService.DeleteChannelAsync(channelId);

            // Assert
            Assert.True(result);

            // Verify it was deleted from the database
            var channelInDb = await _context.Channels.FindAsync(channelId);
            Assert.Null(channelInDb);
        }

        [Fact]
        public async Task DeleteChannelAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var channelId = 999; // Non-existent ID

            // Act
            var result = await _channelsService.DeleteChannelAsync(channelId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ImportChannelsFromM3UAsync_ImportsChannels()
        {
            // Arrange
            var playlist = TestDataGenerator.CreateSampleM3UPlaylist();
            var initialCount = await _context.Channels.CountAsync();

            _streamingServiceMock
                .Setup(s => s.IsStreamAvailableAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var importedCount = await _channelsService.ImportChannelsFromM3UAsync(playlist);

            // Assert
            Assert.Equal(2, importedCount);

            // Verify channels were added to the database
            var finalCount = await _context.Channels.CountAsync();
            Assert.Equal(initialCount + 2, finalCount);

            // Verify the imported channels exist
            var importedChannel = await _context.Channels.FirstOrDefaultAsync(c => c.Name == "Sample Channel 1");
            Assert.NotNull(importedChannel);
            Assert.Equal("http://example.com/sample1.m3u8", importedChannel.StreamUrl);
        }

        [Fact]
        public async Task IsChannelAvailableAsync_WithValidIdAndAvailableStream_ReturnsTrue()
        {
            // Arrange
            var channelId = 1;

            _streamingServiceMock
                .Setup(s => s.IsStreamAvailableAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _channelsService.IsChannelAvailableAsync(channelId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsChannelAvailableAsync_WithValidIdAndUnavailableStream_ReturnsFalse()
        {
            // Arrange
            var channelId = 1;

            _streamingServiceMock
                .Setup(s => s.IsStreamAvailableAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _channelsService.IsChannelAvailableAsync(channelId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsChannelAvailableAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var channelId = 999; // Non-existent ID

            // Act
            var result = await _channelsService.IsChannelAvailableAsync(channelId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetDistinctGroupsAsync_ReturnsDistinctGroups()
        {
            // Act
            var result = await _channelsService.GetDistinctGroupsAsync();

            // Assert
            Assert.True(result.Count() > 0);
            Assert.Contains("News", result);
            Assert.Contains("Sports", result);
        }

        [Fact]
        public async Task GetDistinctCategoriesAsync_ReturnsDistinctCategories()
        {
            // Act
            var result = await _channelsService.GetDistinctCategoriesAsync();

            // Assert
            Assert.True(result.Count() > 0);
            Assert.Contains("Information", result);
            Assert.Contains("Sports", result);
        }
    }
}
