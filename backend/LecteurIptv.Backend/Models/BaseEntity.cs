using System;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Classe de base pour les entités avec dates de création et de modification
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Date de création de l'entité
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de dernière mise à jour de l'entité
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
