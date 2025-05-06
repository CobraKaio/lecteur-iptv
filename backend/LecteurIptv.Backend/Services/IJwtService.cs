using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service JWT
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Génère un token JWT pour un utilisateur
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <returns>Token JWT</returns>
        string GenerateToken(User user);

        /// <summary>
        /// Valide un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>True si le token est valide, false sinon</returns>
        bool ValidateToken(string token);
    }
}
