using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Data.Extensions;
using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Service de gestion des programmes TV
    /// </summary>
    public class ProgramsService : IProgramsService
    {
        private readonly ILogger<ProgramsService> _logger;
        private readonly AppDbContext _context;
        private readonly IXmltvParser _xmltvParser;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="xmltvParser">Parser XMLTV</param>
        public ProgramsService(
            ILogger<ProgramsService> logger,
            AppDbContext context,
            IXmltvParser xmltvParser)
        {
            _logger = logger;
            _context = context;
            _xmltvParser = xmltvParser;
        }

        /// <summary>
        /// Récupère tous les programmes TV
        /// </summary>
        /// <returns>Liste des programmes TV</returns>
        public async Task<IEnumerable<TvProgram>> GetAllProgramsAsync()
        {
            _logger.LogInformation("Récupération de tous les programmes TV");
            return await _context.Programs
                .OrderBy(p => p.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère un programme TV par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <returns>Programme TV correspondant ou null si non trouvé</returns>
        public async Task<TvProgram> GetProgramByIdAsync(int id)
        {
            _logger.LogInformation($"Récupération du programme TV avec l'ID {id}");
            return await _context.Programs
                .Include(p => p.Channel)
                .AsTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Récupère les programmes TV pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Liste des programmes TV pour la chaîne spécifiée</returns>
        public async Task<IEnumerable<TvProgram>> GetProgramsByChannelIdAsync(int channelId)
        {
            _logger.LogInformation($"Récupération des programmes TV pour la chaîne avec l'ID {channelId}");
            return await _context.Programs
                .Where(p => p.ChannelId == channelId)
                .OrderBy(p => p.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les programmes TV pour une période spécifique
        /// </summary>
        /// <param name="startTime">Date et heure de début</param>
        /// <param name="endTime">Date et heure de fin</param>
        /// <returns>Liste des programmes TV pour la période spécifiée</returns>
        public async Task<IEnumerable<TvProgram>> GetProgramsByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            _logger.LogInformation($"Récupération des programmes TV entre {startTime} et {endTime}");
            return await _context.Programs
                .Where(p => (p.StartTime >= startTime && p.StartTime <= endTime) ||
                           (p.EndTime >= startTime && p.EndTime <= endTime) ||
                           (p.StartTime <= startTime && p.EndTime >= endTime))
                .OrderBy(p => p.StartTime)
                .ThenBy(p => p.ChannelId)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les programmes TV en cours pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Programme TV en cours pour la chaîne spécifiée ou null si aucun</returns>
        public async Task<TvProgram> GetCurrentProgramForChannelAsync(int channelId)
        {
            _logger.LogInformation($"Récupération du programme TV en cours pour la chaîne avec l'ID {channelId}");
            var now = DateTime.UtcNow;
            return await _context.Programs
                .Where(p => p.ChannelId == channelId && p.StartTime <= now && p.EndTime >= now)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Récupère les programmes TV à venir pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <param name="count">Nombre de programmes à récupérer</param>
        /// <returns>Liste des programmes TV à venir pour la chaîne spécifiée</returns>
        public async Task<IEnumerable<TvProgram>> GetUpcomingProgramsForChannelAsync(int channelId, int count = 5)
        {
            _logger.LogInformation($"Récupération des {count} prochains programmes TV pour la chaîne avec l'ID {channelId}");
            var now = DateTime.UtcNow;
            return await _context.Programs
                .Where(p => p.ChannelId == channelId && p.StartTime > now)
                .OrderBy(p => p.StartTime)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Recherche des programmes TV par titre ou description
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <returns>Liste des programmes TV correspondant au terme de recherche</returns>
        public async Task<IEnumerable<TvProgram>> SearchProgramsAsync(string searchTerm)
        {
            _logger.LogInformation($"Recherche de programmes TV avec le terme '{searchTerm}'");

            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<TvProgram>();

            var normalizedSearchTerm = searchTerm.ToLower();

            return await _context.Programs
                .Where(p => p.Title.ToLower().Contains(normalizedSearchTerm) ||
                           p.Description.ToLower().Contains(normalizedSearchTerm) ||
                           p.Category.ToLower().Contains(normalizedSearchTerm))
                .OrderBy(p => p.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Ajoute un nouveau programme TV
        /// </summary>
        /// <param name="program">Programme TV à ajouter</param>
        /// <returns>Programme TV ajouté</returns>
        public async Task<TvProgram> AddProgramAsync(TvProgram program)
        {
            _logger.LogInformation($"Ajout d'un nouveau programme TV : {program.Title}");

            // Vérifier que la chaîne existe
            var channel = await _context.Channels.FindAsync(program.ChannelId);
            if (channel == null)
            {
                _logger.LogWarning($"La chaîne avec l'ID {program.ChannelId} n'existe pas");
                throw new ArgumentException($"La chaîne avec l'ID {program.ChannelId} n'existe pas");
            }

            // Vérifier que les dates sont valides
            if (program.StartTime >= program.EndTime)
            {
                _logger.LogWarning("La date de début doit être antérieure à la date de fin");
                throw new ArgumentException("La date de début doit être antérieure à la date de fin");
            }

            await _context.Programs.AddAsync(program);
            await _context.SaveChangesAsync();

            return program;
        }

        /// <summary>
        /// Met à jour un programme TV existant
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <param name="program">Nouvelles données du programme</param>
        /// <returns>Programme TV mis à jour ou null si non trouvé</returns>
        public async Task<TvProgram> UpdateProgramAsync(int id, TvProgram program)
        {
            _logger.LogInformation($"Mise à jour du programme TV avec l'ID {id}");

            var existingProgram = await _context.Programs
                .AsTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProgram == null)
            {
                _logger.LogWarning($"Programme TV avec l'ID {id} non trouvé");
                return null;
            }

            // Vérifier que la chaîne existe
            if (program.ChannelId != existingProgram.ChannelId)
            {
                var channel = await _context.Channels.FindAsync(program.ChannelId);
                if (channel == null)
                {
                    _logger.LogWarning($"La chaîne avec l'ID {program.ChannelId} n'existe pas");
                    throw new ArgumentException($"La chaîne avec l'ID {program.ChannelId} n'existe pas");
                }
            }

            // Vérifier que les dates sont valides
            if (program.StartTime >= program.EndTime)
            {
                _logger.LogWarning("La date de début doit être antérieure à la date de fin");
                throw new ArgumentException("La date de début doit être antérieure à la date de fin");
            }

            // Mettre à jour les propriétés
            existingProgram.ChannelId = program.ChannelId;
            existingProgram.Title = program.Title;
            existingProgram.Description = program.Description;
            existingProgram.StartTime = program.StartTime;
            existingProgram.EndTime = program.EndTime;
            existingProgram.Category = program.Category;
            existingProgram.ImageUrl = program.ImageUrl;
            existingProgram.Year = program.Year;
            existingProgram.Country = program.Country;
            existingProgram.Language = program.Language;
            existingProgram.Actors = program.Actors;
            existingProgram.Director = program.Director;
            existingProgram.Episode = program.Episode;
            existingProgram.Season = program.Season;

            await _context.SaveChangesAsync();

            return existingProgram;
        }

        /// <summary>
        /// Supprime un programme TV
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <returns>True si le programme a été supprimé, false sinon</returns>
        public async Task<bool> DeleteProgramAsync(int id)
        {
            _logger.LogInformation($"Suppression du programme TV avec l'ID {id}");

            var program = await _context.Programs
                .AsTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
            {
                _logger.LogWarning($"Programme TV avec l'ID {id} non trouvé");
                return false;
            }

            _context.Programs.Remove(program);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Importe des programmes TV à partir d'un fichier EPG (Electronic Program Guide)
        /// </summary>
        /// <param name="epgUrl">URL du fichier EPG</param>
        /// <returns>Nombre de programmes importés</returns>
        public async Task<int> ImportProgramsFromEpgAsync(string epgUrl)
        {
            _logger.LogInformation($"Importation de programmes TV à partir du fichier EPG : {epgUrl}");

            try
            {
                // Utiliser le parser XMLTV pour récupérer les données
                var parseResult = await _xmltvParser.ParseFromUrlAsync(epgUrl);

                int importedCount = 0;
                int updatedCount = 0;

                // Traiter les programmes
                foreach (var xmltvProgramme in parseResult.Programmes)
                {
                    // Trouver la chaîne correspondante par son ID XMLTV
                    var channel = await _context.Channels
                        .FirstOrDefaultAsync(c => c.TvgId == xmltvProgramme.ChannelId);

                    if (channel == null)
                    {
                        _logger.LogDebug($"Aucune chaîne trouvée avec l'ID XMLTV '{xmltvProgramme.ChannelId}'");
                        continue;
                    }

                    // Vérifier si le programme existe déjà
                    var existingProgram = await _context.Programs
                        .AsTracking()
                        .FirstOrDefaultAsync(p => p.ChannelId == channel.Id &&
                                               p.StartTime == xmltvProgramme.StartTime &&
                                               p.EndTime == xmltvProgramme.EndTime);

                    if (existingProgram != null)
                    {
                        // Mettre à jour le programme existant
                        existingProgram.Title = xmltvProgramme.Title;
                        existingProgram.Description = xmltvProgramme.Description;
                        existingProgram.Category = xmltvProgramme.Category;
                        existingProgram.Language = xmltvProgramme.Language;
                        existingProgram.Actors = xmltvProgramme.Actors;
                        existingProgram.Director = xmltvProgramme.Directors;
                        existingProgram.ImageUrl = xmltvProgramme.ImageUrl;
                        existingProgram.Year = xmltvProgramme.Year;

                        // Extraire les informations d'épisode si disponibles
                        if (!string.IsNullOrEmpty(xmltvProgramme.Episode))
                        {
                            // Format XMLTV pour les épisodes: saison.épisode.partie
                            var parts = xmltvProgramme.Episode.Split('.');
                            if (parts.Length >= 2)
                            {
                                if (int.TryParse(parts[0], out var season))
                                    existingProgram.Season = season.ToString();

                                if (int.TryParse(parts[1], out var episode))
                                    existingProgram.Episode = episode.ToString();
                            }
                        }

                        updatedCount++;
                    }
                    else
                    {
                        // Créer un nouveau programme
                        var newProgram = new TvProgram
                        {
                            ChannelId = channel.Id,
                            Title = xmltvProgramme.Title,
                            Description = xmltvProgramme.Description,
                            StartTime = xmltvProgramme.StartTime,
                            EndTime = xmltvProgramme.EndTime,
                            Category = xmltvProgramme.Category,
                            Language = xmltvProgramme.Language,
                            Actors = xmltvProgramme.Actors,
                            Director = xmltvProgramme.Directors,
                            ImageUrl = xmltvProgramme.ImageUrl,
                            Year = xmltvProgramme.Year
                        };

                        // Extraire les informations d'épisode si disponibles
                        if (!string.IsNullOrEmpty(xmltvProgramme.Episode))
                        {
                            // Format XMLTV pour les épisodes: saison.épisode.partie
                            var parts = xmltvProgramme.Episode.Split('.');
                            if (parts.Length >= 2)
                            {
                                if (int.TryParse(parts[0], out var season))
                                    newProgram.Season = season.ToString();

                                if (int.TryParse(parts[1], out var episode))
                                    newProgram.Episode = episode.ToString();
                            }
                        }

                        await _context.Programs.AddAsync(newProgram);
                        importedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Importation terminée : {importedCount} programmes ajoutés, {updatedCount} programmes mis à jour");
                return importedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'importation des programmes TV à partir du fichier EPG : {epgUrl}");
                throw;
            }
        }

        /// <summary>
        /// Récupère les catégories de programmes TV distinctes
        /// </summary>
        /// <returns>Liste des catégories de programmes TV</returns>
        public async Task<IEnumerable<string>> GetDistinctCategoriesAsync()
        {
            _logger.LogInformation("Récupération des catégories de programmes TV distinctes");

            return await _context.Programs
                .Where(p => !string.IsNullOrEmpty(p.Category))
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }


    }
}
