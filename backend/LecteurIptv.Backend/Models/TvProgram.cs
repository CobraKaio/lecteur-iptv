using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente un programme TV
    /// </summary>
    public class TvProgram
    {
        /// <summary>
        /// Identifiant unique du programme
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de la chaîne
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Chaîne associée au programme
        /// </summary>
        [ForeignKey("ChannelId")]
        public virtual Channel Channel { get; set; } = null!;

        /// <summary>
        /// Titre du programme
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description du programme
        /// </summary>
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Date et heure de début du programme
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Date et heure de fin du programme
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Catégorie du programme
        /// </summary>
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// URL de l'image du programme
        /// </summary>
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Année de production
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Pays de production
        /// </summary>
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Langue du programme
        /// </summary>
        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Acteurs du programme
        /// </summary>
        [MaxLength(500)]
        public string Actors { get; set; } = string.Empty;

        /// <summary>
        /// Réalisateur du programme
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
        /// Date de création du programme
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de dernière mise à jour du programme
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
