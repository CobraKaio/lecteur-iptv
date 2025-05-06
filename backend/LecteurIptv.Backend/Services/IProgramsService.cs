using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de gestion des programmes TV
    /// </summary>
    public interface IProgramsService
    {
        /// <summary>
        /// Récupère tous les programmes TV
        /// </summary>
        /// <returns>Liste des programmes TV</returns>
        Task<IEnumerable<TvProgram>> GetAllProgramsAsync();

        /// <summary>
        /// Récupère un programme TV par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <returns>Programme TV correspondant ou null si non trouvé</returns>
        Task<TvProgram> GetProgramByIdAsync(int id);

        /// <summary>
        /// Récupère les programmes TV pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Liste des programmes TV pour la chaîne spécifiée</returns>
        Task<IEnumerable<TvProgram>> GetProgramsByChannelIdAsync(int channelId);

        /// <summary>
        /// Récupère les programmes TV pour une période spécifique
        /// </summary>
        /// <param name="startTime">Date et heure de début</param>
        /// <param name="endTime">Date et heure de fin</param>
        /// <returns>Liste des programmes TV pour la période spécifiée</returns>
        Task<IEnumerable<TvProgram>> GetProgramsByTimeRangeAsync(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Récupère les programmes TV en cours pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Programme TV en cours pour la chaîne spécifiée ou null si aucun</returns>
        Task<TvProgram> GetCurrentProgramForChannelAsync(int channelId);

        /// <summary>
        /// Récupère les programmes TV à venir pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <param name="count">Nombre de programmes à récupérer</param>
        /// <returns>Liste des programmes TV à venir pour la chaîne spécifiée</returns>
        Task<IEnumerable<TvProgram>> GetUpcomingProgramsForChannelAsync(int channelId, int count = 5);

        /// <summary>
        /// Recherche des programmes TV par titre ou description
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <returns>Liste des programmes TV correspondant au terme de recherche</returns>
        Task<IEnumerable<TvProgram>> SearchProgramsAsync(string searchTerm);

        /// <summary>
        /// Ajoute un nouveau programme TV
        /// </summary>
        /// <param name="program">Programme TV à ajouter</param>
        /// <returns>Programme TV ajouté</returns>
        Task<TvProgram> AddProgramAsync(TvProgram program);

        /// <summary>
        /// Met à jour un programme TV existant
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <param name="program">Nouvelles données du programme</param>
        /// <returns>Programme TV mis à jour ou null si non trouvé</returns>
        Task<TvProgram> UpdateProgramAsync(int id, TvProgram program);

        /// <summary>
        /// Supprime un programme TV
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <returns>True si le programme a été supprimé, false sinon</returns>
        Task<bool> DeleteProgramAsync(int id);

        /// <summary>
        /// Importe des programmes TV à partir d'un fichier EPG (Electronic Program Guide)
        /// </summary>
        /// <param name="epgUrl">URL du fichier EPG</param>
        /// <returns>Nombre de programmes importés</returns>
        Task<int> ImportProgramsFromEpgAsync(string epgUrl);

        /// <summary>
        /// Récupère les catégories de programmes TV distinctes
        /// </summary>
        /// <returns>Liste des catégories de programmes TV</returns>
        Task<IEnumerable<string>> GetDistinctCategoriesAsync();
    }
}
