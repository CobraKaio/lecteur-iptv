using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Models.M3U;

namespace LecteurIptv.Backend.Tests.Helpers
{
    /// <summary>
    /// Helper class for generating test data
    /// </summary>
    public static class TestDataGenerator
    {
        /// <summary>
        /// Seeds the database with test channels
        /// </summary>
        /// <param name="context">The database context</param>
        public static void SeedChannels(AppDbContext context)
        {
            var channels = new List<Channel>
            {
                new Channel
                {
                    Id = 1,
                    Name = "Test Channel 1",
                    StreamUrl = "http://example.com/stream1.m3u8",
                    LogoUrl = "http://example.com/logo1.png",
                    Group = "News",
                    Category = "Information",
                    Language = "French",
                    Country = "France",
                    IsActive = true,
                    IsFeatured = true,
                    DisplayOrder = 1,
                    TvgId = "test1",
                    TvgName = "Test 1"
                },
                new Channel
                {
                    Id = 2,
                    Name = "Test Channel 2",
                    StreamUrl = "http://example.com/stream2.m3u8",
                    LogoUrl = "http://example.com/logo2.png",
                    Group = "Sports",
                    Category = "Sports",
                    Language = "English",
                    Country = "USA",
                    IsActive = true,
                    IsFeatured = false,
                    DisplayOrder = 2,
                    TvgId = "test2",
                    TvgName = "Test 2"
                },
                new Channel
                {
                    Id = 3,
                    Name = "Test Channel 3",
                    StreamUrl = "http://example.com/stream3.m3u8",
                    LogoUrl = "http://example.com/logo3.png",
                    Group = "Movies",
                    Category = "Entertainment",
                    Language = "French",
                    Country = "France",
                    IsActive = false,
                    IsFeatured = false,
                    DisplayOrder = 3,
                    TvgId = "test3",
                    TvgName = "Test 3"
                }
            };

            context.Channels.AddRange(channels);
            context.SaveChanges();
        }

        /// <summary>
        /// Seeds the database with test VOD items
        /// </summary>
        /// <param name="context">The database context</param>
        public static void SeedVodItems(AppDbContext context)
        {
            var vodItems = new List<VodItem>
            {
                new VodItem
                {
                    Id = 1,
                    Title = "Test Movie 1",
                    Description = "Description for test movie 1",
                    StreamUrl = "http://example.com/movie1.mp4",
                    ImageUrl = "http://example.com/poster1.jpg",
                    Type = "movie",
                    Category = "Action",
                    Year = 2020,
                    Duration = 120,
                    Language = "English",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new VodItem
                {
                    Id = 2,
                    Title = "Test Series 1",
                    Description = "Description for test series 1",
                    StreamUrl = "http://example.com/series1.mp4",
                    ImageUrl = "http://example.com/poster2.jpg",
                    Type = "series",
                    Category = "Drama",
                    Year = 2019,
                    Season = "1",
                    Episode = "1",
                    Language = "French",
                    IsActive = true,
                    DisplayOrder = 2
                },
                new VodItem
                {
                    Id = 3,
                    Title = "Test Movie 2",
                    Description = "Description for test movie 2",
                    StreamUrl = "http://example.com/movie2.mp4",
                    ImageUrl = "http://example.com/poster3.jpg",
                    Type = "movie",
                    Category = "Comedy",
                    Year = 2021,
                    Duration = 95,
                    Language = "English",
                    IsActive = false,
                    DisplayOrder = 3
                }
            };

            context.VodItems.AddRange(vodItems);
            context.SaveChanges();
        }

        /// <summary>
        /// Creates a sample M3U playlist for testing
        /// </summary>
        /// <returns>A sample M3U playlist</returns>
        public static LecteurIptv.Backend.Models.M3UPlaylist CreateSampleM3UPlaylist()
        {
            var playlist = new LecteurIptv.Backend.Models.M3UPlaylist
            {
                Name = "Test Playlist",
                SourceUrl = "http://example.com/playlist.m3u"
            };

            var channels = new List<M3UChannel>
            {
                new M3UChannel
                {
                    Name = "Sample Channel 1",
                    Url = "http://example.com/sample1.m3u8",
                    TvgId = "sample1",
                    Group = "Sample Group",
                    LogoUrl = "http://example.com/samplelogo1.png"
                },
                new M3UChannel
                {
                    Name = "Sample Channel 2",
                    Url = "http://example.com/sample2.m3u8",
                    TvgId = "sample2",
                    Group = "Sample Group",
                    LogoUrl = "http://example.com/samplelogo2.png"
                }
            };

            playlist.Channels.AddRange(channels);

            return playlist;
        }

        /// <summary>
        /// Seeds the database with test users
        /// </summary>
        /// <param name="context">The database context</param>
        public static void SeedUsers(AppDbContext context)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "testuser",
                    Email = "testuser@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    Role = "User",
                    IsActive = true,
                    LastLoginAt = DateTime.UtcNow.AddDays(-1)
                },
                new User
                {
                    Id = 2,
                    Username = "adminuser",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    IsActive = true,
                    LastLoginAt = DateTime.UtcNow.AddDays(-2)
                },
                new User
                {
                    Id = 3,
                    Username = "inactiveuser",
                    Email = "inactive@example.com",
                    FirstName = "Inactive",
                    LastName = "User",
                    Role = "User",
                    IsActive = false,
                    LastLoginAt = DateTime.UtcNow.AddDays(-30)
                }
            };

            // Set passwords for the users
            foreach (var user in users)
            {
                CreatePasswordHash("password", out string passwordHash, out string passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        /// <summary>
        /// Seeds the database with test favorites
        /// </summary>
        /// <param name="context">The database context</param>
        public static void SeedFavorites(AppDbContext context)
        {
            // Make sure users and content exist
            if (!context.Users.Any())
                SeedUsers(context);

            if (!context.Channels.Any())
                SeedChannels(context);

            if (!context.VodItems.Any())
                SeedVodItems(context);

            // Add favorite channels
            var favoriteChannels = new List<UserFavoriteChannel>
            {
                new UserFavoriteChannel
                {
                    Id = 1,
                    UserId = 1,
                    ChannelId = 1,
                    AddedAt = DateTime.UtcNow.AddDays(-5)
                },
                new UserFavoriteChannel
                {
                    Id = 2,
                    UserId = 1,
                    ChannelId = 2,
                    AddedAt = DateTime.UtcNow.AddDays(-3)
                },
                new UserFavoriteChannel
                {
                    Id = 3,
                    UserId = 2,
                    ChannelId = 1,
                    AddedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            context.UserFavoriteChannels.AddRange(favoriteChannels);

            // Add favorite VOD items
            var favoriteVods = new List<UserFavoriteVod>
            {
                new UserFavoriteVod
                {
                    Id = 1,
                    UserId = 1,
                    VodItemId = 1,
                    AddedAt = DateTime.UtcNow.AddDays(-4)
                },
                new UserFavoriteVod
                {
                    Id = 2,
                    UserId = 1,
                    VodItemId = 2,
                    AddedAt = DateTime.UtcNow.AddDays(-2)
                },
                new UserFavoriteVod
                {
                    Id = 3,
                    UserId = 2,
                    VodItemId = 2,
                    AddedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            context.UserFavoriteVods.AddRange(favoriteVods);
            context.SaveChanges();
        }

        /// <summary>
        /// Seeds the database with test viewing history
        /// </summary>
        /// <param name="context">The database context</param>
        public static void SeedHistory(AppDbContext context)
        {
            // Make sure users and content exist
            if (!context.Users.Any())
                SeedUsers(context);

            if (!context.Channels.Any())
                SeedChannels(context);

            if (!context.VodItems.Any())
                SeedVodItems(context);

            var history = new List<UserHistory>
            {
                new UserHistory
                {
                    Id = 1,
                    UserId = 1,
                    ContentId = 1,
                    ContentType = "channel",
                    ContentTitle = "Test Channel 1",
                    ContentImageUrl = "http://example.com/logo1.png",
                    ViewedAt = DateTime.UtcNow.AddDays(-1),
                    DurationSeconds = 300
                },
                new UserHistory
                {
                    Id = 2,
                    UserId = 1,
                    ContentId = 1,
                    ContentType = "vod",
                    ContentTitle = "Test Movie 1",
                    ContentImageUrl = "http://example.com/poster1.jpg",
                    ViewedAt = DateTime.UtcNow.AddDays(-2),
                    DurationSeconds = 1800,
                    PositionSeconds = 900
                },
                new UserHistory
                {
                    Id = 3,
                    UserId = 2,
                    ContentId = 2,
                    ContentType = "channel",
                    ContentTitle = "Test Channel 2",
                    ContentImageUrl = "http://example.com/logo2.png",
                    ViewedAt = DateTime.UtcNow.AddHours(-12),
                    DurationSeconds = 600
                }
            };

            context.UserHistory.AddRange(history);
            context.SaveChanges();
        }

        /// <summary>
        /// Creates a password hash and salt
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="passwordHash">The resulting password hash</param>
        /// <param name="passwordSalt">The resulting password salt</param>
        private static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = Convert.ToBase64String(hmac.Key);
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
