using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente une chaîne favorite d'un utilisateur
    /// </summary>
    public class UserFavoriteChannel : BaseEntity
    {
        /// <summary>
        /// Identifiant unique de la relation
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
        /// Identifiant de la chaîne
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Chaîne associée
        /// </summary>
        [ForeignKey("ChannelId")]
        public virtual Channel Channel { get; set; } = null!;

        /// <summary>
        /// Date d'ajout aux favoris
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
