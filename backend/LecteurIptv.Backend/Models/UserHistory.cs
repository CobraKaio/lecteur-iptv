using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente une entrée dans l'historique de visionnage d'un utilisateur
    /// </summary>
    public class UserHistory : BaseEntity
    {
        /// <summary>
        /// Identifiant unique de l'entrée d'historique
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Utilisateur associé
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Identifiant du contenu (chaîne ou VOD)
        /// </summary>
        public int ContentId { get; set; }

        /// <summary>
        /// Type de contenu ("channel" ou "vod")
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Titre du contenu (pour faciliter l'affichage sans jointure)
        /// </summary>
        [MaxLength(200)]
        public string ContentTitle { get; set; } = string.Empty;

        /// <summary>
        /// URL de l'image du contenu (pour faciliter l'affichage sans jointure)
        /// </summary>
        [MaxLength(500)]
        public string ContentImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Date et heure de visionnage
        /// </summary>
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Durée de visionnage en secondes (optionnel, principalement pour les VOD)
        /// </summary>
        public int? DurationSeconds { get; set; }

        /// <summary>
        /// Position de lecture en secondes (optionnel, principalement pour les VOD)
        /// </summary>
        public int? PositionSeconds { get; set; }
    }
}
