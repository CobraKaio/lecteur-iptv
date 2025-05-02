using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Service pour parser les fichiers M3U
    /// </summary>
    public class M3UParser : IM3UParser
    {
        private readonly ILogger<M3UParser> _logger;
        private readonly HttpClient _httpClient;

        public M3UParser(ILogger<M3UParser> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Parse un fichier M3U à partir d'une URL
        /// </summary>
        /// <param name="url">URL du fichier M3U</param>
        /// <returns>Playlist M3U parsée</returns>
        public async Task<M3UPlaylist> ParseFromUrlAsync(string url)
        {
            try
            {
                _logger.LogInformation($"Parsing M3U from URL: {url}");

                var content = await _httpClient.GetStringAsync(url);
                var playlist = ParseContent(content);
                playlist.SourceUrl = url;

                return playlist;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing M3U from URL: {url}");
                throw;
            }
        }

        /// <summary>
        /// Parse un fichier M3U à partir d'un chemin local
        /// </summary>
        /// <param name="filePath">Chemin du fichier M3U</param>
        /// <returns>Playlist M3U parsée</returns>
        public async Task<M3UPlaylist> ParseFromFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation($"Parsing M3U from file: {filePath}");

                var content = await File.ReadAllTextAsync(filePath);
                var playlist = ParseContent(content);
                playlist.SourceUrl = filePath;

                return playlist;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing M3U from file: {filePath}");
                throw;
            }
        }

        /// <summary>
        /// Parse le contenu d'un fichier M3U
        /// </summary>
        /// <param name="content">Contenu du fichier M3U</param>
        /// <returns>Playlist M3U parsée</returns>
        private M3UPlaylist ParseContent(string content)
        {
            var playlist = new M3UPlaylist();

            // Vérifier que le fichier commence par #EXTM3U
            if (!content.TrimStart().StartsWith("#EXTM3U"))
            {
                throw new FormatException("Invalid M3U format: file does not start with #EXTM3U");
            }

            // Extraire les attributs de la playlist
            var headerMatch = Regex.Match(content, @"#EXTM3U(.*?)(?=\r|\n)");
            if (headerMatch.Success && headerMatch.Groups.Count > 1)
            {
                var headerAttributes = headerMatch.Groups[1].Value;
                ExtractAttributes(headerAttributes, playlist.Attributes);
            }

            // Extraire le nom de la playlist si présent
            if (playlist.Attributes.ContainsKey("x-tvg-url"))
            {
                playlist.Name = Path.GetFileNameWithoutExtension(playlist.Attributes["x-tvg-url"]);
            }
            else
            {
                playlist.Name = "IPTV Playlist";
            }

            // Parser les chaînes
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            M3UChannel? currentChannel = null;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                // Ignorer les lignes vides ou les commentaires
                if (string.IsNullOrWhiteSpace(line) || (line.StartsWith("#") && !line.StartsWith("#EXTINF")))
                {
                    continue;
                }

                // Traiter les informations de chaîne
                if (line.StartsWith("#EXTINF"))
                {
                    currentChannel = new M3UChannel();

                    // Extraire la durée et le nom
                    var match = Regex.Match(line, @"#EXTINF:([-\d\.]+)\s*(.*?),\s*(.*?)$");
                    if (match.Success && match.Groups.Count > 3)
                    {
                        var attributes = match.Groups[2].Value;
                        currentChannel.Name = match.Groups[3].Value.Trim();

                        // Extraire les attributs
                        ExtractAttributes(attributes, currentChannel.Attributes);

                        // Traiter les attributs spécifiques
                        if (currentChannel.Attributes.ContainsKey("tvg-id"))
                        {
                            currentChannel.TvgId = currentChannel.Attributes["tvg-id"];
                        }

                        if (currentChannel.Attributes.ContainsKey("tvg-name"))
                        {
                            currentChannel.TvgName = currentChannel.Attributes["tvg-name"];
                        }

                        if (currentChannel.Attributes.ContainsKey("tvg-logo"))
                        {
                            currentChannel.LogoUrl = currentChannel.Attributes["tvg-logo"];
                        }

                        if (currentChannel.Attributes.ContainsKey("group-title"))
                        {
                            currentChannel.Group = currentChannel.Attributes["group-title"];
                        }

                        if (currentChannel.Attributes.ContainsKey("tvg-language"))
                        {
                            currentChannel.Language = currentChannel.Attributes["tvg-language"];
                        }
                    }
                }
                // Traiter l'URL de la chaîne
                else if (currentChannel != null)
                {
                    currentChannel.Url = line;

                    // Déterminer si c'est un contenu VOD ou Live
                    if (line.EndsWith(".mp4") || line.EndsWith(".mkv") || line.EndsWith(".avi") ||
                        line.Contains("/movie/") || currentChannel.Group.Contains("VOD") ||
                        currentChannel.Group.Contains("Movie"))
                    {
                        currentChannel.IsVod = true;
                        currentChannel.IsLive = false;
                    }

                    playlist.Channels.Add(currentChannel);
                    currentChannel = null;
                }
            }

            _logger.LogInformation($"Parsed {playlist.Channels.Count} channels from M3U content");
            return playlist;
        }

        /// <summary>
        /// Extrait les attributs d'une chaîne ou d'une playlist
        /// </summary>
        /// <param name="attributesString">Chaîne contenant les attributs</param>
        /// <param name="attributes">Dictionnaire où stocker les attributs</param>
        private void ExtractAttributes(string attributesString, Dictionary<string, string> attributes)
        {
            var matches = Regex.Matches(attributesString, @"([a-zA-Z0-9\-_]+)=""([^""]*)""|([a-zA-Z0-9\-_]+)=([^ ""]+)");

            foreach (Match match in matches)
            {
                string key, value;

                if (match.Groups[1].Success)
                {
                    key = match.Groups[1].Value;
                    value = match.Groups[2].Value;
                }
                else
                {
                    key = match.Groups[3].Value;
                    value = match.Groups[4].Value;
                }

                attributes[key] = value;
            }
        }
    }
}
