using System.IO;
using System.Threading.Tasks;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le parser XMLTV
    /// </summary>
    public interface IXmltvParser
    {
        /// <summary>
        /// Parse un fichier XMLTV à partir d'une URL
        /// </summary>
        /// <param name="url">URL du fichier XMLTV</param>
        /// <returns>Résultat du parsing contenant les chaînes et les programmes</returns>
        Task<XmltvParseResult> ParseFromUrlAsync(string url);

        /// <summary>
        /// Parse un fichier XMLTV à partir d'un chemin local
        /// </summary>
        /// <param name="filePath">Chemin du fichier XMLTV</param>
        /// <returns>Résultat du parsing contenant les chaînes et les programmes</returns>
        Task<XmltvParseResult> ParseFromFileAsync(string filePath);

        /// <summary>
        /// Parse un fichier XMLTV à partir d'un flux
        /// </summary>
        /// <param name="stream">Flux contenant le fichier XMLTV</param>
        /// <returns>Résultat du parsing contenant les chaînes et les programmes</returns>
        Task<XmltvParseResult> ParseFromStreamAsync(Stream stream);
    }
}
