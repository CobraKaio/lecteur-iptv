using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de streaming
    /// </summary>
    public interface IStreamingService
    {
        /// <summary>
        /// Proxy un flux vidéo
        /// </summary>
        /// <param name="url">URL du flux à proxifier</param>
        /// <param name="response">Réponse HTTP</param>
        /// <returns>Tâche asynchrone</returns>
        Task ProxyStreamAsync(string url, HttpResponse response);

        /// <summary>
        /// Vérifie si un flux est disponible
        /// </summary>
        /// <param name="url">URL du flux à vérifier</param>
        /// <returns>True si le flux est disponible, false sinon</returns>
        Task<bool> IsStreamAvailableAsync(string url);

        /// <summary>
        /// Obtient les informations sur un flux
        /// </summary>
        /// <param name="url">URL du flux</param>
        /// <returns>Informations sur le flux</returns>
        Task<StreamInfo> GetStreamInfoAsync(string url);
    }

    /// <summary>
    /// Informations sur un flux
    /// </summary>
    public class StreamInfo
    {
        /// <summary>
        /// Type de flux (HLS, DASH, MP4, etc.)
        /// </summary>
        public string StreamType { get; set; } = string.Empty;

        /// <summary>
        /// Résolution du flux
        /// </summary>
        public string Resolution { get; set; } = string.Empty;

        /// <summary>
        /// Bitrate du flux
        /// </summary>
        public int Bitrate { get; set; }

        /// <summary>
        /// Codec vidéo
        /// </summary>
        public string VideoCodec { get; set; } = string.Empty;

        /// <summary>
        /// Codec audio
        /// </summary>
        public string AudioCodec { get; set; } = string.Empty;

        /// <summary>
        /// Durée du flux en secondes (pour les VOD)
        /// </summary>
        public double? Duration { get; set; }

        /// <summary>
        /// Indique si le flux est en direct
        /// </summary>
        public bool IsLive { get; set; }
    }
}
