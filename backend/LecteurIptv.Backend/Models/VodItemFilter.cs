using System.ComponentModel.DataAnnotations;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Filtre pour les éléments VOD
    /// </summary>
    public class VodItemFilter
    {
        /// <summary>
        /// Terme de recherche
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Catégorie
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Type (film, série, etc.)
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Année
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Langue
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Champ de tri
        /// </summary>
        public string SortBy { get; set; } = "title";

        /// <summary>
        /// Ordre de tri (asc/desc)
        /// </summary>
        public string SortOrder { get; set; } = "asc";

        /// <summary>
        /// Numéro de page (1-based)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Nombre d'éléments par page
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}