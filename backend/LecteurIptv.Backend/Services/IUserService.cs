using System.Collections.Generic;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Interface pour le service de gestion des utilisateurs
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Récupère tous les utilisateurs
        /// </summary>
        /// <returns>Liste des utilisateurs</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Récupère un utilisateur par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Utilisateur correspondant ou null si non trouvé</returns>
        Task<User> GetUserByIdAsync(int id);

        /// <summary>
        /// Récupère un utilisateur par son nom d'utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <returns>Utilisateur correspondant ou null si non trouvé</returns>
        Task<User> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Récupère un utilisateur par son adresse e-mail
        /// </summary>
        /// <param name="email">Adresse e-mail</param>
        /// <returns>Utilisateur correspondant ou null si non trouvé</returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Authentifie un utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur ou adresse e-mail</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Utilisateur authentifié ou null si l'authentification échoue</returns>
        Task<User> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Crée un nouvel utilisateur
        /// </summary>
        /// <param name="user">Utilisateur à créer</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Utilisateur créé</returns>
        Task<User> CreateUserAsync(User user, string password);

        /// <summary>
        /// Met à jour un utilisateur existant
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <param name="user">Nouvelles données de l'utilisateur</param>
        /// <returns>Utilisateur mis à jour ou null si non trouvé</returns>
        Task<User> UpdateUserAsync(int id, User user);

        /// <summary>
        /// Met à jour le mot de passe d'un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <param name="currentPassword">Mot de passe actuel</param>
        /// <param name="newPassword">Nouveau mot de passe</param>
        /// <returns>True si le mot de passe a été mis à jour, false sinon</returns>
        Task<bool> UpdatePasswordAsync(int id, string currentPassword, string newPassword);

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>True si l'utilisateur a été supprimé, false sinon</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Vérifie si un nom d'utilisateur existe déjà
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <returns>True si le nom d'utilisateur existe déjà, false sinon</returns>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Vérifie si une adresse e-mail existe déjà
        /// </summary>
        /// <param name="email">Adresse e-mail</param>
        /// <returns>True si l'adresse e-mail existe déjà, false sinon</returns>
        Task<bool> EmailExistsAsync(string email);
    }
}
