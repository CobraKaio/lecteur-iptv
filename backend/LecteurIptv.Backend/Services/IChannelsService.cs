using System.Collections.Generic;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de gestion des chaînes
    /// </summary>
    public interface IChannelsService
    {
        /// <summary>
        /// Récupère toutes les chaînes avec pagination
        /// </summary>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes</returns>
        Task<PaginatedResult<Channel>> GetAllChannelsAsync(PaginationParameters parameters);

        /// <summary>
        /// Récupère les chaînes actives avec pagination
        /// </summary>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes actives</returns>
        Task<PaginatedResult<Channel>> GetActiveChannelsAsync(PaginationParameters parameters);

        /// <summary>
        /// Récupère une chaîne par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>Chaîne correspondante ou null si non trouvée</returns>
        Task<Channel> GetChannelByIdAsync(int id);

        /// <summary>
        /// Récupère les chaînes par groupe avec pagination
        /// </summary>
        /// <param name="group">Groupe de chaînes</param>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes du groupe spécifié</returns>
        Task<PaginatedResult<Channel>> GetChannelsByGroupAsync(string group, PaginationParameters parameters);

        /// <summary>
        /// Récupère les chaînes par catégorie avec pagination
        /// </summary>
        /// <param name="category">Catégorie de chaînes</param>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes de la catégorie spécifiée</returns>
        Task<PaginatedResult<Channel>> GetChannelsByCategoryAsync(string category, PaginationParameters parameters);

        /// <summary>
        /// Recherche des chaînes par nom avec pagination
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes correspondant au terme de recherche</returns>
        Task<PaginatedResult<Channel>> SearchChannelsAsync(string searchTerm, PaginationParameters parameters);

        /// <summary>
        /// Ajoute une nouvelle chaîne
        /// </summary>
        /// <param name="channel">Chaîne à ajouter</param>
        /// <returns>Chaîne ajoutée</returns>
        Task<Channel> AddChannelAsync(Channel channel);

        /// <summary>
        /// Met à jour une chaîne existante
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <param name="channel">Nouvelles données de la chaîne</param>
        /// <returns>Chaîne mise à jour ou null si non trouvée</returns>
        Task<Channel> UpdateChannelAsync(int id, Channel channel);

        /// <summary>
        /// Supprime une chaîne
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne a été supprimée, false sinon</returns>
        Task<bool> DeleteChannelAsync(int id);

        /// <summary>
        /// Importe des chaînes à partir d'une playlist M3U
        /// </summary>
        /// <param name="playlist">Playlist M3U</param>
        /// <returns>Nombre de chaînes importées</returns>
        Task<int> ImportChannelsFromM3UAsync(M3UPlaylist playlist);

        /// <summary>
        /// Vérifie si une chaîne est disponible
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne est disponible, false sinon</returns>
        Task<bool> IsChannelAvailableAsync(int id);

        /// <summary>
        /// Récupère les groupes de chaînes distincts
        /// </summary>
        /// <returns>Liste des groupes de chaînes</returns>
        Task<IEnumerable<string>> GetDistinctGroupsAsync();

        /// <summary>
        /// Récupère les catégories de chaînes distinctes
        /// </summary>
        /// <returns>Liste des catégories de chaînes</returns>
        Task<IEnumerable<string>> GetDistinctCategoriesAsync();
    }
}
