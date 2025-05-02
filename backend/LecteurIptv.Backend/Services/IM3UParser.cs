using System.Threading.Tasks;
using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de parsing M3U
    /// </summary>
    public interface IM3UParser
    {
        /// <summary>
        /// Parse un fichier M3U à partir d'une URL
        /// </summary>
        /// <param name="url">URL du fichier M3U</param>
        /// <returns>Playlist M3U parsée</returns>
        Task<M3UPlaylist> ParseFromUrlAsync(string url);

        /// <summary>
        /// Parse un fichier M3U à partir d'un chemin local
        /// </summary>
        /// <param name="filePath">Chemin du fichier M3U</param>
        /// <returns>Playlist M3U parsée</returns>
        Task<M3UPlaylist> ParseFromFileAsync(string filePath);
    }
}
