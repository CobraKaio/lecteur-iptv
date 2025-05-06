using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LecteurIptv.Backend.Data.Extensions
{
    /// <summary>
    /// Extensions pour le contexte de base de données
    /// </summary>
    public static class AppDbContextExtensions
    {
        /// <summary>
        /// Récupère les chaînes actives
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <returns>Liste des chaînes actives</returns>
        public static async Task<List<Channel>> GetActiveChannelsAsync(this AppDbContext context)
        {
            return await context.Channels
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les chaînes par groupe
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="group">Groupe de chaînes</param>
        /// <returns>Liste des chaînes du groupe spécifié</returns>
        public static async Task<List<Channel>> GetChannelsByGroupAsync(this AppDbContext context, string group)
        {
            return await context.Channels
                .Where(c => c.IsActive && c.Group == group)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les programmes TV à venir pour une chaîne
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <param name="startDate">Date de début</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>Liste des programmes TV à venir pour la chaîne spécifiée</returns>
        public static async Task<List<TvProgram>> GetUpcomingProgramsAsync(this AppDbContext context, int channelId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var now = DateTime.UtcNow;
            var start = startDate ?? now;
            var end = endDate ?? now.AddDays(7);

            return await context.Programs
                .Where(p => p.ChannelId == channelId && p.StartTime >= start && p.StartTime <= end)
                .OrderBy(p => p.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère le programme TV en cours pour une chaîne
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Programme TV en cours pour la chaîne spécifiée</returns>
        public static async Task<TvProgram?> GetCurrentProgramAsync(this AppDbContext context, int channelId)
        {
            var now = DateTime.UtcNow;

            return await context.Programs
                .Where(p => p.ChannelId == channelId && p.StartTime <= now && p.EndTime >= now)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Récupère les chaînes favorites d'un utilisateur
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des chaînes favorites de l'utilisateur</returns>
        public static async Task<List<Channel>> GetUserFavoriteChannelsAsync(this AppDbContext context, int userId)
        {
            return await context.UserFavoriteChannels
                .Where(ufc => ufc.UserId == userId)
                .Select(ufc => ufc.Channel)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les éléments VOD favoris d'un utilisateur
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des éléments VOD favoris de l'utilisateur</returns>
        public static async Task<List<VodItem>> GetUserFavoriteVodsAsync(this AppDbContext context, int userId)
        {
            return await context.UserFavoriteVods
                .Where(ufv => ufv.UserId == userId)
                .Select(ufv => ufv.VodItem)
                .OrderBy(v => v.Title)
                .ToListAsync();
        }

        /// <summary>
        /// Recherche des chaînes par nom
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <returns>Liste des chaînes correspondant au terme de recherche</returns>
        public static async Task<List<Channel>> SearchChannelsAsync(this AppDbContext context, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await context.GetActiveChannelsAsync();

            var normalizedSearchTerm = searchTerm.ToLower();

            return await context.Channels
                .Where(c => c.IsActive && 
                    (c.Name.ToLower().Contains(normalizedSearchTerm) || 
                     c.TvgName.ToLower().Contains(normalizedSearchTerm) ||
                     c.Group.ToLower().Contains(normalizedSearchTerm) ||
                     c.Category.ToLower().Contains(normalizedSearchTerm)))
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Recherche des programmes TV par titre
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <returns>Liste des programmes TV correspondant au terme de recherche</returns>
        public static async Task<List<TvProgram>> SearchProgramsAsync(this AppDbContext context, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<TvProgram>();

            var normalizedSearchTerm = searchTerm.ToLower();

            return await context.Programs
                .Where(p => p.Title.ToLower().Contains(normalizedSearchTerm) || 
                           p.Description.ToLower().Contains(normalizedSearchTerm) ||
                           p.Category.ToLower().Contains(normalizedSearchTerm))
                .OrderBy(p => p.StartTime)
                .ToListAsync();
        }
    }
}
