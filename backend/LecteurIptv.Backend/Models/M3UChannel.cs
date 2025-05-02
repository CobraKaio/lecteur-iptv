using System;
using System.Collections.Generic;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente une chaîne dans un fichier M3U
    /// </summary>
    public class M3UChannel
    {
        /// <summary>
        /// Identifiant unique de la chaîne
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Nom de la chaîne
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL du flux de la chaîne
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// URL du logo de la chaîne
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Groupe auquel appartient la chaîne
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant TVG pour les données EPG
        /// </summary>
        public string TvgId { get; set; } = string.Empty;

        /// <summary>
        /// Nom TVG pour les données EPG
        /// </summary>
        public string TvgName { get; set; } = string.Empty;

        /// <summary>
        /// Langue de la chaîne
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Indique si la chaîne est une chaîne de TV en direct
        /// </summary>
        public bool IsLive { get; set; } = true;

        /// <summary>
        /// Indique si la chaîne est un contenu VOD
        /// </summary>
        public bool IsVod { get; set; } = false;

        /// <summary>
        /// Attributs supplémentaires de la chaîne
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
