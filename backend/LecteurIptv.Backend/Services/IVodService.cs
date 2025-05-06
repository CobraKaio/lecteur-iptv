using System.Collections.Generic;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de gestion des VODs
    /// </summary>
    public interface IVodService
    {
        /// <summary>
        /// Récupère un élément VOD par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'élément VOD</param>
        /// <returns>L'élément VOD s'il est trouvé, sinon null</returns>
        Task<VodItem?> GetVodItemByIdAsync(int id);

        /// <summary>
        /// Récupère les éléments VOD actifs avec pagination
        /// </summary>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des éléments VOD actifs</returns>
        Task<PaginatedResult<VodItem>> GetActiveVodItemsAsync(PaginationParameters parameters);

        /// <summary>
        /// Filtre les éléments VOD selon les critères spécifiés
        /// </summary>
        /// <param name="filter">Critères de filtrage</param>
        /// <returns>Résultat paginé des éléments VOD</returns>
        Task<PaginatedResult<VodItem>> FilterVodItemsAsync(VodItemFilter filter);

        /// <summary>
        /// Récupère les catégories distinctes des éléments VOD
        /// </summary>
        /// <returns>Liste des catégories distinctes</returns>
        Task<IEnumerable<string>> GetDistinctCategoriesAsync();

        /// <summary>
        /// Récupère les types distincts des éléments VOD
        /// </summary>
        /// <returns>Liste des types distincts</returns>
        Task<IEnumerable<string>> GetDistinctTypesAsync();

        /// <summary>
        /// Récupère les langues distinctes des éléments VOD
        /// </summary>
        /// <returns>Liste des langues distinctes</returns>
        Task<IEnumerable<string>> GetDistinctLanguagesAsync();

        /// <summary>
        /// Récupère les années distinctes des éléments VOD
        /// </summary>
        /// <returns>Liste des années distinctes</returns>
        Task<IEnumerable<int>> GetDistinctYearsAsync();
    }
}
