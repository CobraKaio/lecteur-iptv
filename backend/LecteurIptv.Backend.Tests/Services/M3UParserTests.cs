using System;
using System.IO;
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
    public class M3UParserTests
    {
        private readonly Mock<ILogger<M3UParser>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly M3UParser _m3uParser;

        public M3UParserTests()
        {
            _loggerMock = new Mock<ILogger<M3UParser>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _m3uParser = new M3UParser(_loggerMock.Object, _httpClient);
        }

        [Fact]
        public async Task ParseFromUrlAsync_WithValidUrl_ReturnsPlaylist()
        {
            // Arrange
            var url = "http://example.com/playlist.m3u";
            var m3uContent = @"#EXTM3U
#EXTINF:-1 tvg-id=""france2.fr"" tvg-name=""France 2"" tvg-logo=""https://example.com/logos/france2.png"" group-title=""France"",France 2
http://example.com/stream/france2.m3u8
#EXTINF:-1 tvg-id=""france3.fr"" tvg-name=""France 3"" tvg-logo=""https://example.com/logos/france3.png"" group-title=""France"",France 3
http://example.com/stream/france3.m3u8";
            
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(m3uContent)
                });
            
            // Act
            var result = await _m3uParser.ParseFromUrlAsync(url);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(url, result.SourceUrl);
            Assert.Equal(2, result.Channels.Count);
            
            var channel1 = result.Channels[0];
            Assert.Equal("France 2", channel1.Name);
            Assert.Equal("http://example.com/stream/france2.m3u8", channel1.Url);
            Assert.Equal("france2.fr", channel1.TvgId);
            Assert.Equal("France", channel1.Group);
            Assert.Equal("https://example.com/logos/france2.png", channel1.LogoUrl);
            
            var channel2 = result.Channels[1];
            Assert.Equal("France 3", channel2.Name);
            Assert.Equal("http://example.com/stream/france3.m3u8", channel2.Url);
        }

        [Fact]
        public async Task ParseFromUrlAsync_WithInvalidUrl_ThrowsException()
        {
            // Arrange
            var url = "http://example.com/playlist.m3u";
            
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
            
            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _m3uParser.ParseFromUrlAsync(url));
        }

        [Fact]
        public async Task ParseFromFileAsync_WithValidFile_ReturnsPlaylist()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var m3uContent = @"#EXTM3U
#EXTINF:-1 tvg-id=""france2.fr"" tvg-name=""France 2"" tvg-logo=""https://example.com/logos/france2.png"" group-title=""France"",France 2
http://example.com/stream/france2.m3u8
#EXTINF:-1 tvg-id=""france3.fr"" tvg-name=""France 3"" tvg-logo=""https://example.com/logos/france3.png"" group-title=""France"",France 3
http://example.com/stream/france3.m3u8";
            
            File.WriteAllText(tempFile, m3uContent);
            
            try
            {
                // Act
                var result = await _m3uParser.ParseFromFileAsync(tempFile);
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(tempFile, result.SourceUrl);
                Assert.Equal(2, result.Channels.Count);
                
                var channel1 = result.Channels[0];
                Assert.Equal("France 2", channel1.Name);
                Assert.Equal("http://example.com/stream/france2.m3u8", channel1.Url);
                Assert.Equal("france2.fr", channel1.TvgId);
                Assert.Equal("France", channel1.Group);
                Assert.Equal("https://example.com/logos/france2.png", channel1.LogoUrl);
                
                var channel2 = result.Channels[1];
                Assert.Equal("France 3", channel2.Name);
                Assert.Equal("http://example.com/stream/france3.m3u8", channel2.Url);
            }
            finally
            {
                // Clean up
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public async Task ParseFromFileAsync_WithInvalidFile_ThrowsException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _m3uParser.ParseFromFileAsync(nonExistentFile));
        }
    }
}
