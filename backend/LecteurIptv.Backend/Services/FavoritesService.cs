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
    /// Service de gestion des favoris
    /// </summary>
    public class FavoritesService : IFavoritesService
    {
        private readonly ILogger<FavoritesService> _logger;
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Contexte de base de données</param>
        public FavoritesService(
            ILogger<FavoritesService> logger,
            AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Récupère les chaînes favorites d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des chaînes favorites</returns>
        public async Task<IEnumerable<Channel>> GetFavoriteChannelsAsync(int userId)
        {
            _logger.LogInformation($"Récupération des chaînes favorites de l'utilisateur avec l'ID {userId}");
            
            return await _context.UserFavoriteChannels
                .Where(ufc => ufc.UserId == userId)
                .Select(ufc => ufc.Channel)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les éléments VOD favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des éléments VOD favoris</returns>
        public async Task<IEnumerable<VodItem>> GetFavoriteVodsAsync(int userId)
        {
            _logger.LogInformation($"Récupération des éléments VOD favoris de l'utilisateur avec l'ID {userId}");
            
            return await _context.UserFavoriteVods
                .Where(ufv => ufv.UserId == userId)
                .Select(ufv => ufv.VodItem)
                .OrderBy(v => v.DisplayOrder)
                .ThenBy(v => v.Title)
                .ToListAsync();
        }

        /// <summary>
        /// Ajoute une chaîne aux favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne a été ajoutée aux favoris, false sinon</returns>
        public async Task<bool> AddFavoriteChannelAsync(int userId, int channelId)
        {
            _logger.LogInformation($"Ajout de la chaîne avec l'ID {channelId} aux favoris de l'utilisateur avec l'ID {userId}");
            
            // Vérifier si l'utilisateur existe
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"Utilisateur avec l'ID {userId} non trouvé");
                return false;
            }

            // Vérifier si la chaîne existe
            var channel = await _context.Channels.FindAsync(channelId);
            if (channel == null)
            {
                _logger.LogWarning($"Chaîne avec l'ID {channelId} non trouvée");
                return false;
            }

            // Vérifier si la chaîne est déjà dans les favoris
            var existingFavorite = await _context.UserFavoriteChannels
                .FirstOrDefaultAsync(ufc => ufc.UserId == userId && ufc.ChannelId == channelId);
                
            if (existingFavorite != null)
            {
                _logger.LogInformation($"La chaîne avec l'ID {channelId} est déjà dans les favoris de l'utilisateur avec l'ID {userId}");
                return true;
            }

            // Ajouter la chaîne aux favoris
            var userFavoriteChannel = new UserFavoriteChannel
            {
                UserId = userId,
                ChannelId = channelId
            };

            await _context.UserFavoriteChannels.AddAsync(userFavoriteChannel);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Ajoute un élément VOD aux favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>True si l'élément VOD a été ajouté aux favoris, false sinon</returns>
        public async Task<bool> AddFavoriteVodAsync(int userId, int vodItemId)
        {
            _logger.LogInformation($"Ajout de l'élément VOD avec l'ID {vodItemId} aux favoris de l'utilisateur avec l'ID {userId}");
            
            // Vérifier si l'utilisateur existe
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"Utilisateur avec l'ID {userId} non trouvé");
                return false;
            }

            // Vérifier si l'élément VOD existe
            var vodItem = await _context.VodItems.FindAsync(vodItemId);
            if (vodItem == null)
            {
                _logger.LogWarning($"Élément VOD avec l'ID {vodItemId} non trouvé");
                return false;
            }

            // Vérifier si l'élément VOD est déjà dans les favoris
            var existingFavorite = await _context.UserFavoriteVods
                .FirstOrDefaultAsync(ufv => ufv.UserId == userId && ufv.VodItemId == vodItemId);
                
            if (existingFavorite != null)
            {
                _logger.LogInformation($"L'élément VOD avec l'ID {vodItemId} est déjà dans les favoris de l'utilisateur avec l'ID {userId}");
                return true;
            }

            // Ajouter l'élément VOD aux favoris
            var userFavoriteVod = new UserFavoriteVod
            {
                UserId = userId,
                VodItemId = vodItemId
            };

            await _context.UserFavoriteVods.AddAsync(userFavoriteVod);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Supprime une chaîne des favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne a été supprimée des favoris, false sinon</returns>
        public async Task<bool> RemoveFavoriteChannelAsync(int userId, int channelId)
        {
            _logger.LogInformation($"Suppression de la chaîne avec l'ID {channelId} des favoris de l'utilisateur avec l'ID {userId}");
            
            var userFavoriteChannel = await _context.UserFavoriteChannels
                .AsTracking()
                .FirstOrDefaultAsync(ufc => ufc.UserId == userId && ufc.ChannelId == channelId);
                
            if (userFavoriteChannel == null)
            {
                _logger.LogWarning($"La chaîne avec l'ID {channelId} n'est pas dans les favoris de l'utilisateur avec l'ID {userId}");
                return false;
            }

            _context.UserFavoriteChannels.Remove(userFavoriteChannel);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Supprime un élément VOD des favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>True si l'élément VOD a été supprimé des favoris, false sinon</returns>
        public async Task<bool> RemoveFavoriteVodAsync(int userId, int vodItemId)
        {
            _logger.LogInformation($"Suppression de l'élément VOD avec l'ID {vodItemId} des favoris de l'utilisateur avec l'ID {userId}");
            
            var userFavoriteVod = await _context.UserFavoriteVods
                .AsTracking()
                .FirstOrDefaultAsync(ufv => ufv.UserId == userId && ufv.VodItemId == vodItemId);
                
            if (userFavoriteVod == null)
            {
                _logger.LogWarning($"L'élément VOD avec l'ID {vodItemId} n'est pas dans les favoris de l'utilisateur avec l'ID {userId}");
                return false;
            }

            _context.UserFavoriteVods.Remove(userFavoriteVod);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Vérifie si une chaîne est dans les favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne est dans les favoris, false sinon</returns>
        public async Task<bool> IsChannelFavoriteAsync(int userId, int channelId)
        {
            _logger.LogInformation($"Vérification si la chaîne avec l'ID {channelId} est dans les favoris de l'utilisateur avec l'ID {userId}");
            
            return await _context.UserFavoriteChannels
                .AnyAsync(ufc => ufc.UserId == userId && ufc.ChannelId == channelId);
        }

        /// <summary>
        /// Vérifie si un élément VOD est dans les favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>True si l'élément VOD est dans les favoris, false sinon</returns>
        public async Task<bool> IsVodFavoriteAsync(int userId, int vodItemId)
        {
            _logger.LogInformation($"Vérification si l'élément VOD avec l'ID {vodItemId} est dans les favoris de l'utilisateur avec l'ID {userId}");
            
            return await _context.UserFavoriteVods
                .AnyAsync(ufv => ufv.UserId == userId && ufv.VodItemId == vodItemId);
        }
    }
}
