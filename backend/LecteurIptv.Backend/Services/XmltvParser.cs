using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using LecteurIptv.Backend.Models;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Parser pour les fichiers XMLTV (Electronic Program Guide)
    /// </summary>
    public class XmltvParser : IXmltvParser
    {
        private readonly ILogger<XmltvParser> _logger;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="httpClient">Client HTTP</param>
        public XmltvParser(ILogger<XmltvParser> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Parse un fichier XMLTV à partir d'une URL
        /// </summary>
        /// <param name="url">URL du fichier XMLTV</param>
        /// <returns>Résultat du parsing contenant les chaînes et les programmes</returns>
        public async Task<XmltvParseResult> ParseFromUrlAsync(string url)
        {
            try
            {
                _logger.LogInformation($"Parsing XMLTV from URL: {url}");

                var content = await _httpClient.GetStringAsync(url);
                return ParseContent(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing XMLTV from URL: {url}");
                throw;
            }
        }

        /// <summary>
        /// Parse un fichier XMLTV à partir d'un chemin local
        /// </summary>
        /// <param name="filePath">Chemin du fichier XMLTV</param>
        /// <returns>Résultat du parsing contenant les chaînes et les programmes</returns>
        public async Task<XmltvParseResult> ParseFromFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation($"Parsing XMLTV from file: {filePath}");

                var content = await File.ReadAllTextAsync(filePath);
                return ParseContent(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing XMLTV from file: {filePath}");
                throw;
            }
        }

        /// <summary>
        /// Parse un fichier XMLTV à partir d'un flux
        /// </summary>
        /// <param name="stream">Flux contenant le fichier XMLTV</param>
        /// <returns>Résultat du parsing contenant les chaînes et les programmes</returns>
        public async Task<XmltvParseResult> ParseFromStreamAsync(Stream stream)
        {
            try
            {
                _logger.LogInformation("Parsing XMLTV from stream");

                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();
                return ParseContent(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing XMLTV from stream");
                throw;
            }
        }

        /// <summary>
        /// Parse le contenu XMLTV
        /// </summary>
        /// <param name="content">Contenu XMLTV</param>
        /// <returns>Résultat du parsing contenant les chaînes et les programmes</returns>
        private XmltvParseResult ParseContent(string content)
        {
            try
            {
                var result = new XmltvParseResult();
                var doc = XDocument.Parse(content);

                // Récupérer les chaînes
                var channelElements = doc.Descendants("channel");
                foreach (var channelElement in channelElements)
                {
                    var channel = ParseChannel(channelElement);
                    result.Channels.Add(channel);
                }

                // Récupérer les programmes
                var programmeElements = doc.Descendants("programme");
                foreach (var programmeElement in programmeElements)
                {
                    var programme = ParseProgramme(programmeElement);
                    if (programme != null)
                    {
                        result.Programmes.Add(programme);
                    }
                }

                _logger.LogInformation($"Parsed {result.Channels.Count} channels and {result.Programmes.Count} programmes");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing XMLTV content");
                throw;
            }
        }

        /// <summary>
        /// Parse un élément channel
        /// </summary>
        /// <param name="channelElement">Élément XML channel</param>
        /// <returns>Chaîne XMLTV</returns>
        private XmltvChannel ParseChannel(XElement channelElement)
        {
            var id = channelElement.Attribute("id")?.Value ?? string.Empty;
            var displayName = channelElement.Element("display-name")?.Value ?? id;
            var icon = channelElement.Element("icon")?.Attribute("src")?.Value ?? string.Empty;

            return new XmltvChannel
            {
                Id = id,
                DisplayName = displayName,
                IconUrl = icon
            };
        }

        /// <summary>
        /// Parse un élément programme
        /// </summary>
        /// <param name="programmeElement">Élément XML programme</param>
        /// <returns>Programme XMLTV ou null si le parsing a échoué</returns>
        private XmltvProgramme ParseProgramme(XElement programmeElement)
        {
            try
            {
                var channelId = programmeElement.Attribute("channel")?.Value ?? string.Empty;
                var startStr = programmeElement.Attribute("start")?.Value ?? string.Empty;
                var stopStr = programmeElement.Attribute("stop")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(channelId) || string.IsNullOrEmpty(startStr) || string.IsNullOrEmpty(stopStr))
                {
                    _logger.LogWarning("Programme element missing required attributes");
                    return null;
                }

                // Parser les dates
                if (!TryParseXmltvDateTime(startStr, out var start) || !TryParseXmltvDateTime(stopStr, out var stop))
                {
                    _logger.LogWarning($"Failed to parse programme dates: start={startStr}, stop={stopStr}");
                    return null;
                }

                var title = programmeElement.Elements("title").FirstOrDefault()?.Value ?? string.Empty;
                var description = programmeElement.Elements("desc").FirstOrDefault()?.Value ?? string.Empty;
                var category = programmeElement.Elements("category").FirstOrDefault()?.Value ?? string.Empty;
                var language = programmeElement.Elements("language").FirstOrDefault()?.Value ?? string.Empty;

                // Récupérer les crédits (acteurs, réalisateurs, etc.)
                var credits = programmeElement.Element("credits");
                var actors = credits?.Elements("actor").Select(e => e.Value).ToList() ?? new List<string>();
                var directors = credits?.Elements("director").Select(e => e.Value).ToList() ?? new List<string>();

                // Récupérer les informations sur l'épisode
                var episode = programmeElement.Element("episode-num")?.Value ?? string.Empty;

                // Récupérer l'année de production
                int? year = null;
                var yearElement = programmeElement.Element("date");
                if (yearElement != null && int.TryParse(yearElement.Value, out var yearValue))
                {
                    year = yearValue;
                }

                // Récupérer l'URL de l'image
                var icon = programmeElement.Element("icon")?.Attribute("src")?.Value ?? string.Empty;

                return new XmltvProgramme
                {
                    ChannelId = channelId,
                    StartTime = start,
                    EndTime = stop,
                    Title = title,
                    Description = description,
                    Category = category,
                    Language = language,
                    Actors = string.Join(", ", actors),
                    Directors = string.Join(", ", directors),
                    Episode = episode,
                    Year = year,
                    ImageUrl = icon
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing programme element");
                return null;
            }
        }

        /// <summary>
        /// Essaie de parser une date au format XMLTV
        /// </summary>
        /// <param name="dateStr">Chaîne de date au format XMLTV</param>
        /// <param name="result">Date parsée</param>
        /// <returns>True si le parsing a réussi, false sinon</returns>
        private bool TryParseXmltvDateTime(string dateStr, out DateTime result)
        {
            result = DateTime.MinValue;

            if (string.IsNullOrEmpty(dateStr))
            {
                return false;
            }

            // Format XMLTV standard: YYYYMMDDHHMMSS +0000
            // Exemple: 20230101120000 +0100
            try
            {
                // Séparer la date et le fuseau horaire
                var parts = dateStr.Split(' ');
                var datePart = parts[0].Trim();

                // Parser la date
                if (datePart.Length >= 14)
                {
                    var year = int.Parse(datePart.Substring(0, 4));
                    var month = int.Parse(datePart.Substring(4, 2));
                    var day = int.Parse(datePart.Substring(6, 2));
                    var hour = int.Parse(datePart.Substring(8, 2));
                    var minute = int.Parse(datePart.Substring(10, 2));
                    var second = int.Parse(datePart.Substring(12, 2));

                    result = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

                    // Appliquer le fuseau horaire si présent
                    if (parts.Length > 1)
                    {
                        var timeZonePart = parts[1].Trim();
                        if (timeZonePart.StartsWith("+") || timeZonePart.StartsWith("-"))
                        {
                            var sign = timeZonePart[0] == '+' ? 1 : -1;
                            var hours = int.Parse(timeZonePart.Substring(1, 2));
                            var minutes = timeZonePart.Length >= 5 ? int.Parse(timeZonePart.Substring(3, 2)) : 0;
                            var offset = new TimeSpan(hours, minutes, 0);
                            result = result.AddHours(-sign * offset.TotalHours);
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing XMLTV date: {dateStr}");
            }

            return false;
        }
    }

    /// <summary>
    /// Résultat du parsing d'un fichier XMLTV
    /// </summary>
    public class XmltvParseResult
    {
        /// <summary>
        /// Liste des chaînes
        /// </summary>
        public List<XmltvChannel> Channels { get; set; } = new List<XmltvChannel>();

        /// <summary>
        /// Liste des programmes
        /// </summary>
        public List<XmltvProgramme> Programmes { get; set; } = new List<XmltvProgramme>();
    }

    /// <summary>
    /// Chaîne XMLTV
    /// </summary>
    public class XmltvChannel
    {
        /// <summary>
        /// Identifiant de la chaîne
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nom d'affichage de la chaîne
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// URL de l'icône de la chaîne
        /// </summary>
        public string IconUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Programme XMLTV
    /// </summary>
    public class XmltvProgramme
    {
        /// <summary>
        /// Identifiant de la chaîne
        /// </summary>
        public string ChannelId { get; set; } = string.Empty;

        /// <summary>
        /// Date et heure de début du programme
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Date et heure de fin du programme
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Titre du programme
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description du programme
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Catégorie du programme
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Langue du programme
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Acteurs du programme
        /// </summary>
        public string Actors { get; set; } = string.Empty;

        /// <summary>
        /// Réalisateurs du programme
        /// </summary>
        public string Directors { get; set; } = string.Empty;

        /// <summary>
        /// Épisode du programme
        /// </summary>
        public string Episode { get; set; } = string.Empty;

        /// <summary>
        /// Année de production du programme
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// URL de l'image du programme
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
    }
}
