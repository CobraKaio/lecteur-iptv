using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de gestion de l'historique de visionnage
    /// </summary>
    public interface IHistoryService
    {
        /// <summary>
        /// Enregistre une entrée dans l'historique de visionnage
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="contentId">Identifiant du contenu</param>
        /// <param name="contentType">Type de contenu ("channel" ou "vod")</param>
        /// <param name="durationSeconds">Durée de visionnage en secondes (optionnel)</param>
        /// <param name="positionSeconds">Position de lecture en secondes (optionnel)</param>
        /// <returns>Entrée d'historique créée</returns>
        Task<UserHistory> LogViewAsync(int userId, int contentId, string contentType, int? durationSeconds = null, int? positionSeconds = null);

        /// <summary>
        /// Récupère l'historique de visionnage d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="limit">Nombre maximum d'entrées à récupérer</param>
        /// <param name="offset">Nombre d'entrées à ignorer</param>
        /// <returns>Liste des entrées d'historique</returns>
        Task<IEnumerable<UserHistory>> GetHistoryAsync(int userId, int limit = 50, int offset = 0);

        /// <summary>
        /// Récupère l'historique de visionnage d'un utilisateur pour un type de contenu spécifique
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="contentType">Type de contenu ("channel" ou "vod")</param>
        /// <param name="limit">Nombre maximum d'entrées à récupérer</param>
        /// <param name="offset">Nombre d'entrées à ignorer</param>
        /// <returns>Liste des entrées d'historique</returns>
        Task<IEnumerable<UserHistory>> GetHistoryByTypeAsync(int userId, string contentType, int limit = 50, int offset = 0);

        /// <summary>
        /// Supprime une entrée de l'historique de visionnage
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="historyId">Identifiant de l'entrée d'historique</param>
        /// <returns>True si l'entrée a été supprimée, false sinon</returns>
        Task<bool> DeleteHistoryEntryAsync(int userId, int historyId);

        /// <summary>
        /// Supprime tout l'historique de visionnage d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Nombre d'entrées supprimées</returns>
        Task<int> ClearHistoryAsync(int userId);

        /// <summary>
        /// Met à jour la position de lecture d'une entrée d'historique
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="historyId">Identifiant de l'entrée d'historique</param>
        /// <param name="positionSeconds">Nouvelle position de lecture en secondes</param>
        /// <returns>Entrée d'historique mise à jour</returns>
        Task<UserHistory> UpdatePositionAsync(int userId, int historyId, int positionSeconds);
    }
}
