using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Service de gestion des utilisateurs
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Contexte de base de données</param>
        public UserService(
            ILogger<UserService> logger,
            AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Récupère tous les utilisateurs
        /// </summary>
        /// <returns>Liste des utilisateurs</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            _logger.LogInformation("Récupération de tous les utilisateurs");
            return await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère un utilisateur par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Utilisateur correspondant ou null si non trouvé</returns>
        public async Task<User> GetUserByIdAsync(int id)
        {
            _logger.LogInformation($"Récupération de l'utilisateur avec l'ID {id}");
            return await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Récupère un utilisateur par son nom d'utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <returns>Utilisateur correspondant ou null si non trouvé</returns>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            _logger.LogInformation($"Récupération de l'utilisateur avec le nom d'utilisateur '{username}'");
            return await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Récupère un utilisateur par son adresse e-mail
        /// </summary>
        /// <param name="email">Adresse e-mail</param>
        /// <returns>Utilisateur correspondant ou null si non trouvé</returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation($"Récupération de l'utilisateur avec l'adresse e-mail '{email}'");
            return await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Authentifie un utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur ou adresse e-mail</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Utilisateur authentifié ou null si l'authentification échoue</returns>
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            _logger.LogInformation($"Authentification de l'utilisateur '{username}'");
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            // Rechercher l'utilisateur par nom d'utilisateur ou adresse e-mail
            var user = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

            if (user == null)
                return null;

            // Vérifier si l'utilisateur est actif
            if (!user.IsActive)
            {
                _logger.LogWarning($"Tentative d'authentification d'un utilisateur inactif : {username}");
                return null;
            }

            // Vérifier le mot de passe
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning($"Échec de l'authentification pour l'utilisateur : {username}");
                return null;
            }

            // Mettre à jour la date de dernière connexion
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Crée un nouvel utilisateur
        /// </summary>
        /// <param name="user">Utilisateur à créer</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Utilisateur créé</returns>
        public async Task<User> CreateUserAsync(User user, string password)
        {
            _logger.LogInformation($"Création d'un nouvel utilisateur : {user.Username}");
            
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Le mot de passe ne peut pas être vide");

            // Vérifier si le nom d'utilisateur existe déjà
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                throw new ArgumentException($"Le nom d'utilisateur '{user.Username}' est déjà utilisé");

            // Vérifier si l'adresse e-mail existe déjà
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new ArgumentException($"L'adresse e-mail '{user.Email}' est déjà utilisée");

            // Créer le hash du mot de passe
            CreatePasswordHash(password, out string passwordHash, out string passwordSalt);

            // Définir les propriétés de l'utilisateur
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.IsActive = true;
            user.Role = user.Role ?? "User"; // Par défaut, le rôle est "User"

            // Ajouter l'utilisateur à la base de données
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Met à jour un utilisateur existant
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <param name="user">Nouvelles données de l'utilisateur</param>
        /// <returns>Utilisateur mis à jour ou null si non trouvé</returns>
        public async Task<User> UpdateUserAsync(int id, User user)
        {
            _logger.LogInformation($"Mise à jour de l'utilisateur avec l'ID {id}");
            
            var existingUser = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (existingUser == null)
            {
                _logger.LogWarning($"Utilisateur avec l'ID {id} non trouvé");
                return null;
            }

            // Vérifier si le nom d'utilisateur existe déjà (pour un autre utilisateur)
            if (user.Username != existingUser.Username && 
                await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                throw new ArgumentException($"Le nom d'utilisateur '{user.Username}' est déjà utilisé");
            }

            // Vérifier si l'adresse e-mail existe déjà (pour un autre utilisateur)
            if (user.Email != existingUser.Email && 
                await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new ArgumentException($"L'adresse e-mail '{user.Email}' est déjà utilisée");
            }

            // Mettre à jour les propriétés de l'utilisateur
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;

            await _context.SaveChangesAsync();
            
            return existingUser;
        }

        /// <summary>
        /// Met à jour le mot de passe d'un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <param name="currentPassword">Mot de passe actuel</param>
        /// <param name="newPassword">Nouveau mot de passe</param>
        /// <returns>True si le mot de passe a été mis à jour, false sinon</returns>
        public async Task<bool> UpdatePasswordAsync(int id, string currentPassword, string newPassword)
        {
            _logger.LogInformation($"Mise à jour du mot de passe de l'utilisateur avec l'ID {id}");
            
            var user = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (user == null)
            {
                _logger.LogWarning($"Utilisateur avec l'ID {id} non trouvé");
                return false;
            }

            // Vérifier le mot de passe actuel
            if (!VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning($"Mot de passe actuel incorrect pour l'utilisateur avec l'ID {id}");
                return false;
            }

            // Créer le hash du nouveau mot de passe
            CreatePasswordHash(newPassword, out string passwordHash, out string passwordSalt);

            // Mettre à jour le mot de passe
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>True si l'utilisateur a été supprimé, false sinon</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            _logger.LogInformation($"Suppression de l'utilisateur avec l'ID {id}");
            
            var user = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (user == null)
            {
                _logger.LogWarning($"Utilisateur avec l'ID {id} non trouvé");
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Vérifie si un nom d'utilisateur existe déjà
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <returns>True si le nom d'utilisateur existe déjà, false sinon</returns>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            _logger.LogInformation($"Vérification si le nom d'utilisateur '{username}' existe déjà");
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        /// <summary>
        /// Vérifie si une adresse e-mail existe déjà
        /// </summary>
        /// <param name="email">Adresse e-mail</param>
        /// <returns>True si l'adresse e-mail existe déjà, false sinon</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            _logger.LogInformation($"Vérification si l'adresse e-mail '{email}' existe déjà");
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Crée un hash de mot de passe
        /// </summary>
        /// <param name="password">Mot de passe</param>
        /// <param name="passwordHash">Hash du mot de passe (sortie)</param>
        /// <param name="passwordSalt">Sel du mot de passe (sortie)</param>
        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Le mot de passe ne peut pas être vide");

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = Convert.ToBase64String(hmac.Key);
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        /// <summary>
        /// Vérifie un hash de mot de passe
        /// </summary>
        /// <param name="password">Mot de passe</param>
        /// <param name="storedHash">Hash stocké</param>
        /// <param name="storedSalt">Sel stocké</param>
        /// <returns>True si le mot de passe est correct, false sinon</returns>
        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password))
                return false;
            
            if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            var salt = Convert.FromBase64String(storedSalt);
            var hash = Convert.FromBase64String(storedHash);

            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                // Comparer les hash
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
