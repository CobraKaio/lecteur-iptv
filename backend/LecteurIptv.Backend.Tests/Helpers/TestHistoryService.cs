using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Tests.Helpers
{
    /// <summary>
    /// Test implementation of IHistoryService for unit testing
    /// </summary>
    public class TestHistoryService : IHistoryService
    {
        private readonly ILogger<TestHistoryService> _logger;
        private readonly List<UserHistory> _historyItems;

        public TestHistoryService(ILogger<TestHistoryService> logger)
        {
            _logger = logger;
            _historyItems = new List<UserHistory>
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
        }

        public async Task<UserHistory> LogViewAsync(int userId, int contentId, string contentType, int? durationSeconds = null, int? positionSeconds = null)
        {
            if (contentType != "channel" && contentType != "vod")
            {
                throw new ArgumentException($"Invalid content type: {contentType}. Must be 'channel' or 'vod'.");
            }

            var newId = _historyItems.Count > 0 ? _historyItems.Max(h => h.Id) + 1 : 1;

            var historyEntry = new UserHistory
            {
                Id = newId,
                UserId = userId,
                ContentId = contentId,
                ContentType = contentType,
                ContentTitle = $"Test {contentType} {contentId}",
                ContentImageUrl = contentType == "channel"
                    ? $"http://example.com/logo{contentId}.png"
                    : $"http://example.com/poster{contentId}.jpg",
                ViewedAt = DateTime.UtcNow,
                DurationSeconds = durationSeconds,
                PositionSeconds = positionSeconds
            };

            _historyItems.Add(historyEntry);

            return await Task.FromResult(historyEntry);
        }

        public async Task<IEnumerable<UserHistory>> GetHistoryAsync(int userId, int limit = 10, int offset = 0)
        {
            return await Task.FromResult(
                _historyItems
                    .Where(h => h.UserId == userId)
                    .OrderByDescending(h => h.ViewedAt)
                    .Skip(offset)
                    .Take(limit)
                    .ToList()
            );
        }

        public async Task<IEnumerable<UserHistory>> GetHistoryByTypeAsync(int userId, string contentType, int limit = 10, int offset = 0)
        {
            if (contentType != "channel" && contentType != "vod")
            {
                throw new ArgumentException($"Invalid content type: {contentType}. Must be 'channel' or 'vod'.");
            }

            return await Task.FromResult(
                _historyItems
                    .Where(h => h.UserId == userId && h.ContentType == contentType)
                    .OrderByDescending(h => h.ViewedAt)
                    .Skip(offset)
                    .Take(limit)
                    .ToList()
            );
        }

        public async Task<bool> DeleteHistoryEntryAsync(int userId, int historyId)
        {
            var historyEntry = _historyItems.FirstOrDefault(h => h.Id == historyId && h.UserId == userId);

            if (historyEntry == null)
            {
                return await Task.FromResult(false);
            }

            _historyItems.Remove(historyEntry);
            return await Task.FromResult(true);
        }

        public async Task<int> ClearHistoryAsync(int userId)
        {
            var historyEntries = _historyItems.Where(h => h.UserId == userId).ToList();

            if (historyEntries.Count == 0)
            {
                return await Task.FromResult(0);
            }

            foreach (var entry in historyEntries)
            {
                _historyItems.Remove(entry);
            }

            return await Task.FromResult(historyEntries.Count);
        }

        public async Task<UserHistory> UpdatePositionAsync(int userId, int historyId, int positionSeconds)
        {
            var historyEntry = _historyItems.FirstOrDefault(h => h.Id == historyId && h.UserId == userId);

            if (historyEntry == null)
            {
                throw new ArgumentException($"History entry with ID {historyId} not found for user {userId}");
            }

            historyEntry.PositionSeconds = positionSeconds;
            historyEntry.ViewedAt = DateTime.UtcNow;

            return await Task.FromResult(historyEntry);
        }
    }
}
