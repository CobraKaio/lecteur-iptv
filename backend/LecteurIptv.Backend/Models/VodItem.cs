using System;
using System.ComponentModel.DataAnnotations;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente un élément VOD (Video On Demand)
    /// </summary>
    public class VodItem : BaseEntity
    {
        /// <summary>
        /// Identifiant unique de l'élément VOD
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Titre de l'élément VOD
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description de l'élément VOD
        /// </summary>
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// URL du flux de l'élément VOD
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string StreamUrl { get; set; } = string.Empty;

        /// <summary>
        /// URL de l'image de l'élément VOD
        /// </summary>
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Type de l'élément VOD (film, série, etc.)
        /// </summary>
        [MaxLength(50)]
        public string Type { get; set; } = "movie";

        /// <summary>
        /// Catégorie de l'élément VOD
        /// </summary>
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Année de production
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Durée en minutes
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Pays de production
        /// </summary>
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Langue de l'élément VOD
        /// </summary>
        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Acteurs de l'élément VOD
        /// </summary>
        [MaxLength(500)]
        public string Actors { get; set; } = string.Empty;

        /// <summary>
        /// Réalisateur de l'élément VOD
        /// </summary>
        [MaxLength(200)]
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Épisode (pour les séries)
        /// </summary>
        [MaxLength(50)]
        public string Episode { get; set; } = string.Empty;

        /// <summary>
        /// Saison (pour les séries)
        /// </summary>
        [MaxLength(50)]
        public string Season { get; set; } = string.Empty;

        /// <summary>
        /// Note de l'élément VOD (sur 10)
        /// </summary>
        public decimal? Rating { get; set; }

        /// <summary>
        /// Indique si l'élément VOD est actif
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Ordre d'affichage de l'élément VOD
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
    }
}
