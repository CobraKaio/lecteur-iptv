using System.Collections.Generic;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de gestion des favoris
    /// </summary>
    public interface IFavoritesService
    {
        /// <summary>
        /// Récupère les chaînes favorites d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des chaînes favorites</returns>
        Task<IEnumerable<Channel>> GetFavoriteChannelsAsync(int userId);

        /// <summary>
        /// Récupère les éléments VOD favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des éléments VOD favoris</returns>
        Task<IEnumerable<VodItem>> GetFavoriteVodsAsync(int userId);

        /// <summary>
        /// Ajoute une chaîne aux favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne a été ajoutée aux favoris, false sinon</returns>
        Task<bool> AddFavoriteChannelAsync(int userId, int channelId);

        /// <summary>
        /// Ajoute un élément VOD aux favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>True si l'élément VOD a été ajouté aux favoris, false sinon</returns>
        Task<bool> AddFavoriteVodAsync(int userId, int vodItemId);

        /// <summary>
        /// Supprime une chaîne des favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne a été supprimée des favoris, false sinon</returns>
        Task<bool> RemoveFavoriteChannelAsync(int userId, int channelId);

        /// <summary>
        /// Supprime un élément VOD des favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>True si l'élément VOD a été supprimé des favoris, false sinon</returns>
        Task<bool> RemoveFavoriteVodAsync(int userId, int vodItemId);

        /// <summary>
        /// Vérifie si une chaîne est dans les favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne est dans les favoris, false sinon</returns>
        Task<bool> IsChannelFavoriteAsync(int userId, int channelId);

        /// <summary>
        /// Vérifie si un élément VOD est dans les favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>True si l'élément VOD est dans les favoris, false sinon</returns>
        Task<bool> IsVodFavoriteAsync(int userId, int vodItemId);
    }
}
