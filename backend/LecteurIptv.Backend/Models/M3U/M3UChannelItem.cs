using System;
using System.Collections.Generic;

namespace LecteurIptv.Backend.Models.M3U
{
    /// <summary>
    /// Représente une chaîne dans une playlist M3U
    /// </summary>
    public class M3UChannelItem
    {
        /// <summary>
        /// Obtient ou définit la ligne #EXTINF brute
        /// </summary>
        public string ExtInf { get; set; }

        /// <summary>
        /// Obtient ou définit le nom de la chaîne
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtient ou définit l'URL du flux
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Obtient ou définit la durée (en secondes) si spécifiée dans #EXTINF
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Obtient ou définit l'ID TVG (Television Guide)
        /// </summary>
        public string TvgId { get; set; }

        /// <summary>
        /// Obtient ou définit le nom TVG
        /// </summary>
        public string TvgName { get; set; }

        /// <summary>
        /// Obtient ou définit l'URL du logo
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Obtient ou définit le groupe de la chaîne
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Obtient ou définit la langue de la chaîne
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Obtient ou définit les attributs supplémentaires de la chaîne
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe M3UChannelItem
        /// </summary>
        public M3UChannelItem()
        {
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Duration = -1; // Valeur par défaut pour indiquer que la durée n'est pas spécifiée
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe M3UChannelItem avec des valeurs spécifiées
        /// </summary>
        /// <param name="extInf">Ligne #EXTINF brute</param>
        /// <param name="name">Nom de la chaîne</param>
        /// <param name="url">URL du flux</param>
        public M3UChannelItem(string extInf, string name, string url)
            : this()
        {
            ExtInf = extInf;
            Name = name;
            Url = url;
        }

        /// <summary>
        /// Extrait les attributs communs du dictionnaire Attributes vers les propriétés dédiées
        /// </summary>
        public void ExtractCommonAttributes()
        {
            if (Attributes.TryGetValue("tvg-id", out string tvgId))
            {
                TvgId = tvgId;
            }

            if (Attributes.TryGetValue("tvg-name", out string tvgName))
            {
                TvgName = tvgName;
            }

            if (Attributes.TryGetValue("tvg-logo", out string logoUrl))
            {
                LogoUrl = logoUrl;
            }

            if (Attributes.TryGetValue("group-title", out string group))
            {
                Group = group;
            }

            if (Attributes.TryGetValue("tvg-language", out string language))
            {
                Language = language;
            }
        }

        /// <summary>
        /// Vérifie si la chaîne appartient à un groupe spécifique
        /// </summary>
        /// <param name="groupName">Nom du groupe</param>
        /// <returns>True si la chaîne appartient au groupe spécifié, sinon False</returns>
        public bool IsInGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return true;
            }

            return Group != null && Group.Equals(groupName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Vérifie si le nom de la chaîne contient un terme de recherche
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <returns>True si le nom de la chaîne contient le terme de recherche, sinon False</returns>
        public bool NameContains(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return true;
            }

            return Name != null && Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retourne une représentation sous forme de chaîne de caractères de l'objet M3UChannelItem
        /// </summary>
        /// <returns>Représentation sous forme de chaîne de caractères</returns>
        public override string ToString()
        {
            return $"{Name} ({Url})";
        }
    }
}
