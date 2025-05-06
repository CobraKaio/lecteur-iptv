using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente un utilisateur de l'application
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Adresse e-mail de l'utilisateur
        /// </summary>
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe hashé de l'utilisateur
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Sel utilisé pour le hachage du mot de passe
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PasswordSalt { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'utilisateur
        /// </summary>
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'utilisateur
        /// </summary>
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'utilisateur (admin, user, etc.)
        /// </summary>
        [MaxLength(20)]
        public string Role { get; set; } = "user";

        /// <summary>
        /// Indique si l'utilisateur est actif
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date de dernière connexion de l'utilisateur
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Chaînes favorites de l'utilisateur
        /// </summary>
        public virtual ICollection<UserFavoriteChannel> FavoriteChannels { get; set; } = new List<UserFavoriteChannel>();

        /// <summary>
        /// Éléments VOD favoris de l'utilisateur
        /// </summary>
        public virtual ICollection<UserFavoriteVod> FavoriteVods { get; set; } = new List<UserFavoriteVod>();

        /// <summary>
        /// Historique de visionnage de l'utilisateur
        /// </summary>
        public virtual ICollection<UserHistory> ViewingHistory { get; set; } = new List<UserHistory>();
    }
}
