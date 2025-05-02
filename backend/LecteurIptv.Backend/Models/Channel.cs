using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente une chaîne de télévision
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Identifiant unique de la chaîne
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nom de la chaîne
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL du flux de la chaîne
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string StreamUrl { get; set; } = string.Empty;

        /// <summary>
        /// URL du logo de la chaîne
        /// </summary>
        [MaxLength(500)]
        public string LogoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant TVG pour les données EPG
        /// </summary>
        [MaxLength(100)]
        public string TvgId { get; set; } = string.Empty;

        /// <summary>
        /// Nom TVG pour les données EPG
        /// </summary>
        [MaxLength(100)]
        public string TvgName { get; set; } = string.Empty;

        /// <summary>
        /// Groupe auquel appartient la chaîne
        /// </summary>
        [MaxLength(100)]
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Catégorie de la chaîne
        /// </summary>
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Langue de la chaîne
        /// </summary>
        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Pays de la chaîne
        /// </summary>
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Indique si la chaîne est active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Ordre d'affichage de la chaîne
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Date de création de la chaîne
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de dernière mise à jour de la chaîne
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Programmes associés à la chaîne
        /// </summary>
        public virtual ICollection<TvProgram> Programs { get; set; } = new List<TvProgram>();
    }
}
