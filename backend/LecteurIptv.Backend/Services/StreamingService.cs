using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Service pour gérer les flux vidéo
    /// </summary>
    public class StreamingService : IStreamingService
    {
        private readonly ILogger<StreamingService> _logger;
        private readonly HttpClient _httpClient;

        public StreamingService(ILogger<StreamingService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Proxy un flux vidéo
        /// </summary>
        /// <param name="url">URL du flux à proxifier</param>
        /// <param name="response">Réponse HTTP</param>
        /// <returns>Tâche asynchrone</returns>
        public async Task ProxyStreamAsync(string url, HttpResponse response)
        {
            try
            {
                _logger.LogInformation($"Proxying stream: {url}");

                // Déterminer le type de contenu en fonction de l'extension
                string contentType = GetContentTypeFromUrl(url);
                response.ContentType = contentType;

                // Pour les flux HLS et DASH, nous devons modifier les URLs dans le manifeste
                if (url.EndsWith(".m3u8") || url.EndsWith(".mpd"))
                {
                    var content = await _httpClient.GetStringAsync(url);
                    
                    // Remplacer les URLs relatives par des URLs absolues
                    var baseUrl = GetBaseUrl(url);
                    content = ReplaceRelativeUrls(content, baseUrl);
                    
                    await response.WriteAsync(content);
                }
                else
                {
                    // Pour les autres types de flux, on fait un simple proxy
                    using var stream = await _httpClient.GetStreamAsync(url);
                    await stream.CopyToAsync(response.Body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error proxying stream: {url}");
                response.StatusCode = 500;
                await response.WriteAsync($"Error proxying stream: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie si un flux est disponible
        /// </summary>
        /// <param name="url">URL du flux à vérifier</param>
        /// <returns>True si le flux est disponible, false sinon</returns>
        public async Task<bool> IsStreamAvailableAsync(string url)
        {
            try
            {
                _logger.LogInformation($"Checking stream availability: {url}");
                
                var request = new HttpRequestMessage(HttpMethod.Head, url);
                var response = await _httpClient.SendAsync(request);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking stream availability: {url}");
                return false;
            }
        }

        /// <summary>
        /// Obtient les informations sur un flux
        /// </summary>
        /// <param name="url">URL du flux</param>
        /// <returns>Informations sur le flux</returns>
        public async Task<StreamInfo> GetStreamInfoAsync(string url)
        {
            try
            {
                _logger.LogInformation($"Getting stream info: {url}");
                
                var streamInfo = new StreamInfo();
                
                // Déterminer le type de flux
                if (url.EndsWith(".m3u8"))
                {
                    streamInfo.StreamType = "HLS";
                    streamInfo.IsLive = true; // Par défaut, on considère les flux HLS comme des flux en direct
                }
                else if (url.EndsWith(".mpd"))
                {
                    streamInfo.StreamType = "DASH";
                    streamInfo.IsLive = true; // Par défaut, on considère les flux DASH comme des flux en direct
                }
                else if (url.EndsWith(".mp4") || url.EndsWith(".mkv") || url.EndsWith(".avi"))
                {
                    streamInfo.StreamType = Path.GetExtension(url).TrimStart('.');
                    streamInfo.IsLive = false;
                }
                else
                {
                    streamInfo.StreamType = "Unknown";
                }
                
                // Pour les flux HLS et DASH, on peut extraire plus d'informations du manifeste
                if (streamInfo.StreamType == "HLS" || streamInfo.StreamType == "DASH")
                {
                    var content = await _httpClient.GetStringAsync(url);
                    
                    // Extraire les informations du manifeste
                    if (streamInfo.StreamType == "HLS")
                    {
                        ExtractHlsInfo(content, streamInfo);
                    }
                    else if (streamInfo.StreamType == "DASH")
                    {
                        ExtractDashInfo(content, streamInfo);
                    }
                }
                else
                {
                    // Pour les autres types de flux, on peut utiliser ffprobe pour extraire les informations
                    await ExtractMediaInfoWithFfprobe(url, streamInfo);
                }
                
                return streamInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stream info: {url}");
                return new StreamInfo { StreamType = "Unknown" };
            }
        }

        /// <summary>
        /// Détermine le type de contenu en fonction de l'URL
        /// </summary>
        /// <param name="url">URL du flux</param>
        /// <returns>Type de contenu</returns>
        private string GetContentTypeFromUrl(string url)
        {
            if (url.EndsWith(".m3u8"))
            {
                return "application/vnd.apple.mpegurl";
            }
            else if (url.EndsWith(".mpd"))
            {
                return "application/dash+xml";
            }
            else if (url.EndsWith(".mp4"))
            {
                return "video/mp4";
            }
            else if (url.EndsWith(".ts"))
            {
                return "video/mp2t";
            }
            else if (url.EndsWith(".webm"))
            {
                return "video/webm";
            }
            else if (url.EndsWith(".mkv"))
            {
                return "video/x-matroska";
            }
            else if (url.EndsWith(".avi"))
            {
                return "video/x-msvideo";
            }
            else
            {
                return "application/octet-stream";
            }
        }

        /// <summary>
        /// Obtient l'URL de base à partir d'une URL
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>URL de base</returns>
        private string GetBaseUrl(string url)
        {
            var uri = new Uri(url);
            var baseUrl = $"{uri.Scheme}://{uri.Host}";
            
            if (!uri.IsDefaultPort)
            {
                baseUrl += $":{uri.Port}";
            }
            
            var path = uri.AbsolutePath;
            var lastSlashIndex = path.LastIndexOf('/');
            
            if (lastSlashIndex > 0)
            {
                baseUrl += path.Substring(0, lastSlashIndex + 1);
            }
            
            return baseUrl;
        }

        /// <summary>
        /// Remplace les URLs relatives par des URLs absolues dans un manifeste
        /// </summary>
        /// <param name="content">Contenu du manifeste</param>
        /// <param name="baseUrl">URL de base</param>
        /// <returns>Contenu avec les URLs absolues</returns>
        private string ReplaceRelativeUrls(string content, string baseUrl)
        {
            // Remplacer les URLs relatives par des URLs absolues
            content = Regex.Replace(content, @"(#EXT-X-STREAM-INF:[^\n]*\n)([^#][^:\n]*)", m => {
                var url = m.Groups[2].Value.Trim();
                if (!url.StartsWith("http"))
                {
                    return m.Groups[1].Value + baseUrl + url;
                }
                return m.Value;
            });
            
            // Remplacer les URLs relatives des segments
            content = Regex.Replace(content, @"(\n)([^#][^:\n]*)", m => {
                var url = m.Groups[2].Value.Trim();
                if (!url.StartsWith("http"))
                {
                    return m.Groups[1].Value + baseUrl + url;
                }
                return m.Value;
            });
            
            return content;
        }

        /// <summary>
        /// Extrait les informations d'un manifeste HLS
        /// </summary>
        /// <param name="content">Contenu du manifeste</param>
        /// <param name="streamInfo">Informations sur le flux</param>
        private void ExtractHlsInfo(string content, StreamInfo streamInfo)
        {
            // Vérifier si c'est un flux en direct ou VOD
            if (content.Contains("#EXT-X-ENDLIST"))
            {
                streamInfo.IsLive = false;
            }
            
            // Extraire la résolution et le bitrate
            var resolutionMatch = Regex.Match(content, @"RESOLUTION=(\d+x\d+)");
            if (resolutionMatch.Success)
            {
                streamInfo.Resolution = resolutionMatch.Groups[1].Value;
            }
            
            var bitrateMatch = Regex.Match(content, @"BANDWIDTH=(\d+)");
            if (bitrateMatch.Success && int.TryParse(bitrateMatch.Groups[1].Value, out int bitrate))
            {
                streamInfo.Bitrate = bitrate;
            }
            
            // Extraire les codecs
            var codecsMatch = Regex.Match(content, @"CODECS=""([^""]+)""");
            if (codecsMatch.Success)
            {
                var codecs = codecsMatch.Groups[1].Value.Split(',');
                if (codecs.Length > 0)
                {
                    streamInfo.VideoCodec = codecs[0];
                    if (codecs.Length > 1)
                    {
                        streamInfo.AudioCodec = codecs[1];
                    }
                }
            }
            
            // Extraire la durée (pour les VOD)
            if (!streamInfo.IsLive)
            {
                var durationMatches = Regex.Matches(content, @"#EXTINF:(\d+(\.\d+)?)");
                double totalDuration = 0;
                foreach (Match match in durationMatches)
                {
                    if (double.TryParse(match.Groups[1].Value, out double duration))
                    {
                        totalDuration += duration;
                    }
                }
                
                if (totalDuration > 0)
                {
                    streamInfo.Duration = totalDuration;
                }
            }
        }

        /// <summary>
        /// Extrait les informations d'un manifeste DASH
        /// </summary>
        /// <param name="content">Contenu du manifeste</param>
        /// <param name="streamInfo">Informations sur le flux</param>
        private void ExtractDashInfo(string content, StreamInfo streamInfo)
        {
            // Vérifier si c'est un flux en direct ou VOD
            if (content.Contains("type=\"dynamic\""))
            {
                streamInfo.IsLive = true;
            }
            else
            {
                streamInfo.IsLive = false;
                
                // Extraire la durée (pour les VOD)
                var durationMatch = Regex.Match(content, @"mediaPresentationDuration=""PT([^""]+)""");
                if (durationMatch.Success)
                {
                    var durationStr = durationMatch.Groups[1].Value;
                    var hours = 0.0;
                    var minutes = 0.0;
                    var seconds = 0.0;
                    
                    var hoursMatch = Regex.Match(durationStr, @"(\d+)H");
                    if (hoursMatch.Success && double.TryParse(hoursMatch.Groups[1].Value, out double h))
                    {
                        hours = h;
                    }
                    
                    var minutesMatch = Regex.Match(durationStr, @"(\d+)M");
                    if (minutesMatch.Success && double.TryParse(minutesMatch.Groups[1].Value, out double m))
                    {
                        minutes = m;
                    }
                    
                    var secondsMatch = Regex.Match(durationStr, @"(\d+(\.\d+)?)S");
                    if (secondsMatch.Success && double.TryParse(secondsMatch.Groups[1].Value, out double s))
                    {
                        seconds = s;
                    }
                    
                    streamInfo.Duration = hours * 3600 + minutes * 60 + seconds;
                }
            }
            
            // Extraire la résolution
            var resolutionMatch = Regex.Match(content, @"width=""(\d+)""\s+height=""(\d+)""");
            if (resolutionMatch.Success)
            {
                streamInfo.Resolution = $"{resolutionMatch.Groups[1].Value}x{resolutionMatch.Groups[2].Value}";
            }
            
            // Extraire le bitrate
            var bitrateMatch = Regex.Match(content, @"bandwidth=""(\d+)""");
            if (bitrateMatch.Success && int.TryParse(bitrateMatch.Groups[1].Value, out int bitrate))
            {
                streamInfo.Bitrate = bitrate;
            }
            
            // Extraire les codecs
            var videoCodecMatch = Regex.Match(content, @"codecs=""([^"",]+)""");
            if (videoCodecMatch.Success)
            {
                streamInfo.VideoCodec = videoCodecMatch.Groups[1].Value;
            }
            
            var audioCodecMatch = Regex.Match(content, @"codecs=""[^"",]+,\s*([^"",]+)""");
            if (audioCodecMatch.Success)
            {
                streamInfo.AudioCodec = audioCodecMatch.Groups[1].Value;
            }
        }

        /// <summary>
        /// Extrait les informations d'un média avec ffprobe
        /// </summary>
        /// <param name="url">URL du média</param>
        /// <param name="streamInfo">Informations sur le flux</param>
        private async Task ExtractMediaInfoWithFfprobe(string url, StreamInfo streamInfo)
        {
            try
            {
                // Vérifier si ffprobe est disponible
                var ffprobePath = "ffprobe";
                
                // Créer le processus ffprobe
                var startInfo = new ProcessStartInfo
                {
                    FileName = ffprobePath,
                    Arguments = $"-v quiet -print_format json -show_format -show_streams \"{url}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using var process = new Process { StartInfo = startInfo };
                process.Start();
                
                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
                
                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    // Analyser la sortie JSON de ffprobe
                    // Note: Dans un projet réel, vous utiliseriez System.Text.Json ou Newtonsoft.Json pour parser le JSON
                    
                    // Extraire la durée
                    var durationMatch = Regex.Match(output, @"""duration"":\s*""(\d+(\.\d+)?)""");
                    if (durationMatch.Success && double.TryParse(durationMatch.Groups[1].Value, out double duration))
                    {
                        streamInfo.Duration = duration;
                    }
                    
                    // Extraire la résolution
                    var widthMatch = Regex.Match(output, @"""width"":\s*(\d+)");
                    var heightMatch = Regex.Match(output, @"""height"":\s*(\d+)");
                    if (widthMatch.Success && heightMatch.Success)
                    {
                        streamInfo.Resolution = $"{widthMatch.Groups[1].Value}x{heightMatch.Groups[1].Value}";
                    }
                    
                    // Extraire le bitrate
                    var bitrateMatch = Regex.Match(output, @"""bit_rate"":\s*""(\d+)""");
                    if (bitrateMatch.Success && int.TryParse(bitrateMatch.Groups[1].Value, out int bitrate))
                    {
                        streamInfo.Bitrate = bitrate;
                    }
                    
                    // Extraire les codecs
                    var videoCodecMatch = Regex.Match(output, @"""codec_name"":\s*""([^""]+)"",\s*""codec_long_name"":\s*""[^""]+""[^}]*""codec_type"":\s*""video""");
                    if (videoCodecMatch.Success)
                    {
                        streamInfo.VideoCodec = videoCodecMatch.Groups[1].Value;
                    }
                    
                    var audioCodecMatch = Regex.Match(output, @"""codec_name"":\s*""([^""]+)"",\s*""codec_long_name"":\s*""[^""]+""[^}]*""codec_type"":\s*""audio""");
                    if (audioCodecMatch.Success)
                    {
                        streamInfo.AudioCodec = audioCodecMatch.Groups[1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error extracting media info with ffprobe: {url}");
            }
        }
    }
}
