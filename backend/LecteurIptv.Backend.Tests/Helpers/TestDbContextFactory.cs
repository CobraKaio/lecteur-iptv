using LecteurIptv.Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LecteurIptv.Backend.Tests.Helpers
{
    /// <summary>
    /// Factory class for creating test database contexts
    /// </summary>
    public static class TestDbContextFactory
    {
        /// <summary>
        /// Creates a new in-memory database context for testing
        /// </summary>
        /// <param name="databaseName">Name of the in-memory database</param>
        /// <returns>A new AppDbContext instance using an in-memory database</returns>
        public static AppDbContext CreateDbContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            var loggerFactory = new Mock<ILoggerFactory>();
            var context = new AppDbContext(options);
            
            // Clear the database
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            return context;
        }
    }
}
