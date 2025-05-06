using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente un élément VOD favori d'un utilisateur
    /// </summary>
    public class UserFavoriteVod : BaseEntity
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
        /// Identifiant de l'élément VOD
        /// </summary>
        public int VodItemId { get; set; }

        /// <summary>
        /// Élément VOD associé
        /// </summary>
        [ForeignKey("VodItemId")]
        public virtual VodItem VodItem { get; set; } = null!;

        /// <summary>
        /// Date d'ajout aux favoris
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
