using System;
using System.Collections.Generic;

namespace LecteurIptv.Backend.Models.M3U
{
    /// <summary>
    /// Représente une playlist M3U complète
    /// </summary>
    public class M3UPlaylist
    {
        /// <summary>
        /// Obtient ou définit l'en-tête de la playlist (généralement #EXTM3U)
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Obtient ou définit le nom de la playlist
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtient ou définit la liste des chaînes dans la playlist
        /// </summary>
        public List<M3UChannelItem> Channels { get; set; }

        /// <summary>
        /// Obtient ou définit les attributs supplémentaires de la playlist
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Obtient ou définit l'URL source de la playlist
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// Obtient ou définit la date de dernière mise à jour de la playlist
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe M3UPlaylist
        /// </summary>
        public M3UPlaylist()
        {
            Header = "#EXTM3U";
            Name = "Playlist";
            Channels = new List<M3UChannelItem>();
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Ajoute une chaîne à la playlist
        /// </summary>
        /// <param name="channel">Chaîne à ajouter</param>
        public void AddChannel(M3UChannelItem channel)
        {
            if (channel != null)
            {
                Channels.Add(channel);
            }
        }

        /// <summary>
        /// Obtient le nombre de chaînes dans la playlist
        /// </summary>
        /// <returns>Nombre de chaînes</returns>
        public int GetChannelCount()
        {
            return Channels.Count;
        }

        /// <summary>
        /// Filtre les chaînes par groupe
        /// </summary>
        /// <param name="groupName">Nom du groupe</param>
        /// <returns>Liste des chaînes du groupe spécifié</returns>
        public List<M3UChannelItem> GetChannelsByGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return Channels;
            }

            return Channels.FindAll(c => 
                c.Attributes.TryGetValue("group-title", out string group) && 
                group.Equals(groupName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Recherche des chaînes par nom
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <returns>Liste des chaînes correspondant au terme de recherche</returns>
        public List<M3UChannelItem> SearchChannels(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Channels;
            }

            return Channels.FindAll(c => 
                c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                (c.Attributes.TryGetValue("tvg-name", out string tvgName) && 
                 tvgName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
