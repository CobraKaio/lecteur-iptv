using System;
using System.Collections.Generic;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente une playlist M3U
    /// </summary>
    public class M3UPlaylist
    {
        /// <summary>
        /// Identifiant unique de la playlist
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Nom de la playlist
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Liste des chaînes dans la playlist
        /// </summary>
        public List<M3UChannel> Channels { get; set; } = new List<M3UChannel>();

        /// <summary>
        /// Date de création de la playlist
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de dernière mise à jour de la playlist
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// URL source de la playlist
        /// </summary>
        public string SourceUrl { get; set; } = string.Empty;

        /// <summary>
        /// Attributs supplémentaires de la playlist
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
