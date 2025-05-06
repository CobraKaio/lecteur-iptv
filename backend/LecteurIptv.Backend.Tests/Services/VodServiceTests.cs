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
    public class VodServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<VodService>> _loggerMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly VodService _vodService;
        private readonly string _dbName;

        public VodServiceTests()
        {
            // Create a unique database name for each test run to ensure isolation
            _dbName = $"VodServiceTests_{Guid.NewGuid()}";
            _context = TestDbContextFactory.CreateDbContext(_dbName);

            // Set up mocks
            _loggerMock = new Mock<ILogger<VodService>>();
            _memoryCacheMock = new Mock<IMemoryCache>();

            // Set up memory cache mock to handle cache entries
            var cacheEntry = new Mock<ICacheEntry>();
            _memoryCacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntry.Object);

            // Create the service with mocked dependencies
            _vodService = new VodService(
                _context,
                _loggerMock.Object,
                _memoryCacheMock.Object);

            // Seed the database with test data
            TestDataGenerator.SeedVodItems(_context);
        }

        public void Dispose()
        {
            // Clean up after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetVodItemByIdAsync_WithValidId_ReturnsVodItem()
        {
            // Arrange
            var vodItemId = 1;

            // Act
            var result = await _vodService.GetVodItemByIdAsync(vodItemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vodItemId, result.Id);
            Assert.Equal("Test Movie 1", result.Title);
        }

        [Fact]
        public async Task GetVodItemByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var vodItemId = 999; // Non-existent ID

            // Act
            var result = await _vodService.GetVodItemByIdAsync(vodItemId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetActiveVodItemsAsync_ReturnsOnlyActiveVodItems()
        {
            // Arrange
            var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _vodService.GetActiveVodItemsAsync(parameters);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, item => Assert.True(item.IsActive));
        }

        [Fact]
        public async Task FilterVodItemsAsync_WithCategoryFilter_ReturnsMatchingVodItems()
        {
            // Arrange
            var filter = new VodItemFilter
            {
                Category = "Action",
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _vodService.FilterVodItemsAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Movie 1", result.Items.First().Title);
        }

        [Fact]
        public async Task FilterVodItemsAsync_WithTypeFilter_ReturnsMatchingVodItems()
        {
            // Arrange
            var filter = new VodItemFilter
            {
                Type = "series",
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _vodService.FilterVodItemsAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Series 1", result.Items.First().Title);
        }

        [Fact]
        public async Task FilterVodItemsAsync_WithYearFilter_ReturnsMatchingVodItems()
        {
            // Arrange
            var filter = new VodItemFilter
            {
                Year = 2020,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _vodService.FilterVodItemsAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Movie 1", result.Items.First().Title);
        }

        [Fact]
        public async Task FilterVodItemsAsync_WithLanguageFilter_ReturnsMatchingVodItems()
        {
            // Arrange
            var filter = new VodItemFilter
            {
                Language = "French",
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _vodService.FilterVodItemsAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Series 1", result.Items.First().Title);
        }

        [Fact]
        public async Task FilterVodItemsAsync_WithSearchTerm_ReturnsMatchingVodItems()
        {
            // Arrange
            var filter = new VodItemFilter
            {
                Query = "Series",
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _vodService.FilterVodItemsAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Series 1", result.Items.First().Title);
        }

        [Fact]
        public async Task FilterVodItemsAsync_WithMultipleFilters_ReturnsMatchingVodItems()
        {
            // Arrange
            var filter = new VodItemFilter
            {
                Type = "movie",
                Year = 2020,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _vodService.FilterVodItemsAsync(filter);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Test Movie 1", result.Items.First().Title);
        }

        [Fact]
        public async Task GetDistinctCategoriesAsync_ReturnsDistinctCategories()
        {
            // Act
            var result = await _vodService.GetDistinctCategoriesAsync();

            // Assert
            Assert.True(result.Count() > 0);
            Assert.Contains("Action", result);
            Assert.Contains("Drama", result);
        }

        [Fact]
        public async Task GetDistinctTypesAsync_ReturnsDistinctTypes()
        {
            // Act
            var result = await _vodService.GetDistinctTypesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains("movie", result);
            Assert.Contains("series", result);
        }

        [Fact]
        public async Task GetDistinctLanguagesAsync_ReturnsDistinctLanguages()
        {
            // Act
            var result = await _vodService.GetDistinctLanguagesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains("English", result);
            Assert.Contains("French", result);
        }

        [Fact]
        public async Task GetDistinctYearsAsync_ReturnsDistinctYears()
        {
            // Act
            var result = await _vodService.GetDistinctYearsAsync();

            // Assert
            Assert.True(result.Count() > 0);
            Assert.Contains(2019, result);
            Assert.Contains(2020, result);
        }
    }
}
