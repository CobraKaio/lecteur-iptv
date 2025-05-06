using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente une chaîne de télévision
    /// </summary>
    public class Channel : BaseEntity, IValidatableObject
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
        [Url(ErrorMessage = "L'URL du flux doit être une URL valide")]
        public string StreamUrl { get; set; } = string.Empty;

        /// <summary>
        /// URL du logo de la chaîne
        /// </summary>
        [MaxLength(500)]
        [Url(ErrorMessage = "L'URL du logo doit être une URL valide")]
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
        /// Indique si la chaîne est mise en avant
        /// </summary>
        public bool IsFeatured { get; set; } = false;

        /// <summary>
        /// Ordre d'affichage de la chaîne
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Programmes associés à la chaîne
        /// </summary>
        public virtual ICollection<TvProgram> Programs { get; set; } = new List<TvProgram>();

        /// <summary>
        /// Valide que les URLs sont correctement formatées
        /// </summary>
        /// <param name="validationContext">Contexte de validation</param>
        /// <returns>Liste des erreurs de validation</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Valider StreamUrl
            if (!string.IsNullOrEmpty(StreamUrl) && !Uri.TryCreate(StreamUrl, UriKind.Absolute, out _))
            {
                yield return new ValidationResult(
                    "L'URL du flux doit être une URL absolue valide",
                    new[] { nameof(StreamUrl) });
            }

            // Valider LogoUrl si elle n'est pas vide
            if (!string.IsNullOrEmpty(LogoUrl) && !Uri.TryCreate(LogoUrl, UriKind.Absolute, out _))
            {
                yield return new ValidationResult(
                    "L'URL du logo doit être une URL absolue valide",
                    new[] { nameof(LogoUrl) });
            }
        }
    }
}
