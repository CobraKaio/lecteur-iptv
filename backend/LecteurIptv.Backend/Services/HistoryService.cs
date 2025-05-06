using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Service pour la gestion de l'historique de visionnage
    /// </summary>
    public class HistoryService : IHistoryService
    {
        private readonly ILogger<HistoryService> _logger;
        private readonly AppDbContext _context;
        private readonly IChannelsService _channelsService;
        private readonly IVodService _vodService;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="channelsService">Service de gestion des chaînes</param>
        /// <param name="vodService">Service de gestion des VOD</param>
        public HistoryService(
            ILogger<HistoryService> logger,
            AppDbContext context,
            IChannelsService channelsService,
            IVodService vodService)
        {
            _logger = logger;
            _context = context;
            _channelsService = channelsService;
            _vodService = vodService;
        }

        /// <summary>
        /// Enregistre une entrée dans l'historique de visionnage
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="contentId">Identifiant du contenu</param>
        /// <param name="contentType">Type de contenu ("channel" ou "vod")</param>
        /// <param name="durationSeconds">Durée de visionnage en secondes (optionnel)</param>
        /// <param name="positionSeconds">Position de lecture en secondes (optionnel)</param>
        /// <returns>Entrée d'historique créée</returns>
        public async Task<UserHistory> LogViewAsync(int userId, int contentId, string contentType, int? durationSeconds = null, int? positionSeconds = null)
        {
            _logger.LogInformation($"Enregistrement d'une entrée dans l'historique pour l'utilisateur {userId}, contenu {contentId} de type {contentType}");

            // Vérifier si l'utilisateur existe
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"Utilisateur avec l'ID {userId} non trouvé");
                throw new ArgumentException($"Utilisateur avec l'ID {userId} non trouvé");
            }

            // Vérifier si le type de contenu est valide
            if (contentType != "channel" && contentType != "vod")
            {
                _logger.LogWarning($"Type de contenu invalide : {contentType}");
                throw new ArgumentException($"Type de contenu invalide : {contentType}. Les valeurs acceptées sont 'channel' ou 'vod'");
            }

            // Récupérer les informations sur le contenu
            string contentTitle = string.Empty;
            string contentImageUrl = string.Empty;

            if (contentType == "channel")
            {
                var channel = await _channelsService.GetChannelByIdAsync(contentId);
                if (channel == null)
                {
                    _logger.LogWarning($"Chaîne avec l'ID {contentId} non trouvée");
                    throw new ArgumentException($"Chaîne avec l'ID {contentId} non trouvée");
                }

                contentTitle = channel.Name;
                contentImageUrl = channel.LogoUrl ?? string.Empty;
            }
            else // contentType == "vod"
            {
                var vodItem = await _vodService.GetVodItemByIdAsync(contentId);
                if (vodItem == null)
                {
                    _logger.LogWarning($"Élément VOD avec l'ID {contentId} non trouvé");
                    throw new ArgumentException($"Élément VOD avec l'ID {contentId} non trouvé");
                }

                contentTitle = vodItem.Title;
                contentImageUrl = vodItem.ImageUrl ?? string.Empty;
            }

            // Vérifier s'il existe déjà une entrée récente pour ce contenu
            var existingEntry = await _context.UserHistory
                .AsTracking()
                .Where(h => h.UserId == userId && h.ContentId == contentId && h.ContentType == contentType)
                .OrderByDescending(h => h.ViewedAt)
                .FirstOrDefaultAsync();

            // Si une entrée existe et a été créée il y a moins de 30 minutes, la mettre à jour
            if (existingEntry != null && (DateTime.UtcNow - existingEntry.ViewedAt).TotalMinutes < 30)
            {
                _logger.LogInformation($"Mise à jour de l'entrée d'historique existante avec l'ID {existingEntry.Id}");

                existingEntry.ViewedAt = DateTime.UtcNow;
                
                if (durationSeconds.HasValue)
                {
                    existingEntry.DurationSeconds = durationSeconds;
                }
                
                if (positionSeconds.HasValue)
                {
                    existingEntry.PositionSeconds = positionSeconds;
                }

                await _context.SaveChangesAsync();
                return existingEntry;
            }

            // Sinon, créer une nouvelle entrée
            var historyEntry = new UserHistory
            {
                UserId = userId,
                ContentId = contentId,
                ContentType = contentType,
                ContentTitle = contentTitle,
                ContentImageUrl = contentImageUrl,
                ViewedAt = DateTime.UtcNow,
                DurationSeconds = durationSeconds,
                PositionSeconds = positionSeconds
            };

            await _context.UserHistory.AddAsync(historyEntry);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Nouvelle entrée d'historique créée avec l'ID {historyEntry.Id}");
            return historyEntry;
        }

        /// <summary>
        /// Récupère l'historique de visionnage d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="limit">Nombre maximum d'entrées à récupérer</param>
        /// <param name="offset">Nombre d'entrées à ignorer</param>
        /// <returns>Liste des entrées d'historique</returns>
        public async Task<IEnumerable<UserHistory>> GetHistoryAsync(int userId, int limit = 50, int offset = 0)
        {
            _logger.LogInformation($"Récupération de l'historique de visionnage pour l'utilisateur {userId} (limit: {limit}, offset: {offset})");

            return await _context.UserHistory
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ViewedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère l'historique de visionnage d'un utilisateur pour un type de contenu spécifique
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="contentType">Type de contenu ("channel" ou "vod")</param>
        /// <param name="limit">Nombre maximum d'entrées à récupérer</param>
        /// <param name="offset">Nombre d'entrées à ignorer</param>
        /// <returns>Liste des entrées d'historique</returns>
        public async Task<IEnumerable<UserHistory>> GetHistoryByTypeAsync(int userId, string contentType, int limit = 50, int offset = 0)
        {
            _logger.LogInformation($"Récupération de l'historique de visionnage pour l'utilisateur {userId} et le type de contenu {contentType} (limit: {limit}, offset: {offset})");

            // Vérifier si le type de contenu est valide
            if (contentType != "channel" && contentType != "vod")
            {
                _logger.LogWarning($"Type de contenu invalide : {contentType}");
                throw new ArgumentException($"Type de contenu invalide : {contentType}. Les valeurs acceptées sont 'channel' ou 'vod'");
            }

            return await _context.UserHistory
                .Where(h => h.UserId == userId && h.ContentType == contentType)
                .OrderByDescending(h => h.ViewedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Supprime une entrée de l'historique de visionnage
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="historyId">Identifiant de l'entrée d'historique</param>
        /// <returns>True si l'entrée a été supprimée, false sinon</returns>
        public async Task<bool> DeleteHistoryEntryAsync(int userId, int historyId)
        {
            _logger.LogInformation($"Suppression de l'entrée d'historique {historyId} pour l'utilisateur {userId}");

            var historyEntry = await _context.UserHistory
                .FirstOrDefaultAsync(h => h.Id == historyId && h.UserId == userId);

            if (historyEntry == null)
            {
                _logger.LogWarning($"Entrée d'historique avec l'ID {historyId} non trouvée pour l'utilisateur {userId}");
                return false;
            }

            _context.UserHistory.Remove(historyEntry);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Entrée d'historique {historyId} supprimée pour l'utilisateur {userId}");
            return true;
        }

        /// <summary>
        /// Supprime tout l'historique de visionnage d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Nombre d'entrées supprimées</returns>
        public async Task<int> ClearHistoryAsync(int userId)
        {
            _logger.LogInformation($"Suppression de tout l'historique de visionnage pour l'utilisateur {userId}");

            var historyEntries = await _context.UserHistory
                .Where(h => h.UserId == userId)
                .ToListAsync();

            if (historyEntries.Count == 0)
            {
                _logger.LogInformation($"Aucune entrée d'historique trouvée pour l'utilisateur {userId}");
                return 0;
            }

            _context.UserHistory.RemoveRange(historyEntries);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"{historyEntries.Count} entrées d'historique supprimées pour l'utilisateur {userId}");
            return historyEntries.Count;
        }

        /// <summary>
        /// Met à jour la position de lecture d'une entrée d'historique
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="historyId">Identifiant de l'entrée d'historique</param>
        /// <param name="positionSeconds">Nouvelle position de lecture en secondes</param>
        /// <returns>Entrée d'historique mise à jour</returns>
        public async Task<UserHistory> UpdatePositionAsync(int userId, int historyId, int positionSeconds)
        {
            _logger.LogInformation($"Mise à jour de la position de lecture pour l'entrée d'historique {historyId} de l'utilisateur {userId} à {positionSeconds} secondes");

            var historyEntry = await _context.UserHistory
                .AsTracking()
                .FirstOrDefaultAsync(h => h.Id == historyId && h.UserId == userId);

            if (historyEntry == null)
            {
                _logger.LogWarning($"Entrée d'historique avec l'ID {historyId} non trouvée pour l'utilisateur {userId}");
                throw new ArgumentException($"Entrée d'historique avec l'ID {historyId} non trouvée pour l'utilisateur {userId}");
            }

            historyEntry.PositionSeconds = positionSeconds;
            historyEntry.ViewedAt = DateTime.UtcNow; // Mettre à jour la date de visionnage
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Position de lecture mise à jour pour l'entrée d'historique {historyId} de l'utilisateur {userId}");
            return historyEntry;
        }
    }
}
