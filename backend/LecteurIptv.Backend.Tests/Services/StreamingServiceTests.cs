using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LecteurIptv.Backend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace LecteurIptv.Backend.Tests.Services
{
    public class StreamingServiceTests
    {
        private readonly Mock<ILogger<StreamingService>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly StreamingService _streamingService;

        public StreamingServiceTests()
        {
            _loggerMock = new Mock<ILogger<StreamingService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _streamingService = new StreamingService(_loggerMock.Object, _httpClient);
        }

        [Fact]
        public async Task IsStreamAvailableAsync_WithAvailableStream_ReturnsTrue()
        {
            // Arrange
            var streamUrl = "http://example.com/stream.m3u8";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("#EXTM3U\n#EXT-X-VERSION:3\n#EXT-X-STREAM-INF:BANDWIDTH=1280000\nstream.m3u8")
                });

            // Act
            var result = await _streamingService.IsStreamAvailableAsync(streamUrl);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsStreamAvailableAsync_WithUnavailableStream_ReturnsFalse()
        {
            // Arrange
            var streamUrl = "http://example.com/stream.m3u8";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            var result = await _streamingService.IsStreamAvailableAsync(streamUrl);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsStreamAvailableAsync_WithException_ReturnsFalse()
        {
            // Arrange
            var streamUrl = "http://example.com/stream.m3u8";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Connection failed"));

            // Act
            var result = await _streamingService.IsStreamAvailableAsync(streamUrl);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetStreamInfoAsync_WithValidStream_ReturnsStreamInfo()
        {
            // Arrange
            var streamUrl = "http://example.com/stream.m3u8";
            var hlsManifest = @"#EXTM3U
#EXT-X-VERSION:3
#EXT-X-STREAM-INF:BANDWIDTH=1280000,RESOLUTION=1280x720,CODECS=""avc1.4d401f,mp4a.40.2""
stream_720p.m3u8
#EXT-X-STREAM-INF:BANDWIDTH=2560000,RESOLUTION=1920x1080,CODECS=""avc1.4d401f,mp4a.40.2""
stream_1080p.m3u8";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(hlsManifest)
                });

            // Act
            var result = await _streamingService.GetStreamInfoAsync(streamUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("HLS", result.StreamType);
            Assert.Equal("1280x720", result.Resolution);
            Assert.Equal(1280000, result.Bitrate);
        }

        [Fact]
        public async Task GetStreamInfoAsync_WithInvalidStream_ReturnsUnknownType()
        {
            // Arrange
            var streamUrl = "http://example.com/stream.m3u8";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            var result = await _streamingService.GetStreamInfoAsync(streamUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Unknown", result.StreamType);
        }


    }
}
