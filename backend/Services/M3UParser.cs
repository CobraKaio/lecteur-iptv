using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StreamingAPI.Services
{
    public class M3UParser
    {
        private readonly HttpClient _httpClient;
        private readonly string _samplesDirectory;

        public M3UParser(HttpClient httpClient, string samplesDirectory)
        {
            _httpClient = httpClient;
            _samplesDirectory = samplesDirectory;
        }

        public class Channel
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string Name { get; set; } = "";
            public string? Logo { get; set; }
            public string? Group { get; set; }
            public string Url { get; set; } = "";
        }

        public async Task<List<Channel>> ParseFromUrlAsync(string url)
        {
            var content = await _httpClient.GetStringAsync(url);
            return ParseM3UContent(content);
        }

        public List<Channel> ParseM3UContent(string content)
        {
            var channels = new List<Channel>();
            var lines = content.Split('\n');

            if (!lines[0].Contains("#EXTM3U"))
            {
                throw new FormatException("Format M3U invalide");
            }

            Channel? currentChannel = null;

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("#EXTINF:"))
                {
                    // Nouvelle chaîne
                    currentChannel = new Channel();

                    // Extraction du nom et des attributs
                    var infoMatch = Regex.Match(line, @"#EXTINF:-1\s+(.*),(.*)$");
                    if (infoMatch.Success && infoMatch.Groups.Count >= 3)
                    {
                        var attributes = infoMatch.Groups[1].Value;
                        var name = infoMatch.Groups[2].Value.Trim();

                        currentChannel.Name = name;

                        // Extraction du logo
                        var logoMatch = Regex.Match(attributes, @"tvg-logo=""([^""]*)""");
                        if (logoMatch.Success)
                        {
                            currentChannel.Logo = logoMatch.Groups[1].Value;
                        }

                        // Extraction du groupe
                        var groupMatch = Regex.Match(attributes, @"group-title=""([^""]*)""");
                        if (groupMatch.Success)
                        {
                            currentChannel.Group = groupMatch.Groups[1].Value;
                        }
                    }
                }
                else if (line.Length > 0 && !line.StartsWith("#") && currentChannel != null)
                {
                    // URL de la chaîne
                    currentChannel.Url = line;

                    if (!string.IsNullOrEmpty(currentChannel.Name) && !string.IsNullOrEmpty(currentChannel.Url))
                    {
                        channels.Add(currentChannel);
                    }

                    currentChannel = null;
                }
            }

            return channels;
        }

        public List<string> GetSamplePlaylists()
        {
            var samples = new List<string>();
            
            if (Directory.Exists(_samplesDirectory))
            {
                var files = Directory.GetFiles(_samplesDirectory, "*.m3u");
                samples.AddRange(files.Select(Path.GetFileName));
            }
            
            return samples;
        }
    }
}
