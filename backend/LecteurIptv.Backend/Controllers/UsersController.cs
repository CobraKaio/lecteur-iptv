using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Obtient tous les utilisateurs
        /// </summary>
        /// <returns>Liste des utilisateurs</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting users: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient un utilisateur par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Utilisateur</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                // Ne pas renvoyer le hash et le sel du mot de passe
                user.PasswordHash = null;
                user.PasswordSalt = null;

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting user: {ex.Message}");
            }
        }

        /// <summary>
        /// Authentifie un utilisateur
        /// </summary>
        /// <param name="loginModel">Modèle de connexion</param>
        /// <returns>Utilisateur authentifié</returns>
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(loginModel.Username, loginModel.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Ne pas renvoyer le hash et le sel du mot de passe
                user.PasswordHash = null;
                user.PasswordSalt = null;

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating user: {loginModel.Username}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error authenticating user: {ex.Message}");
            }
        }

        /// <summary>
        /// Crée un nouvel utilisateur
        /// </summary>
        /// <param name="registerModel">Modèle d'enregistrement</param>
        /// <returns>Utilisateur créé</returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                // Vérifier si le nom d'utilisateur existe déjà
                if (await _userService.UsernameExistsAsync(registerModel.Username))
                {
                    return BadRequest($"Username '{registerModel.Username}' is already taken");
                }

                // Vérifier si l'adresse e-mail existe déjà
                if (await _userService.EmailExistsAsync(registerModel.Email))
                {
                    return BadRequest($"Email '{registerModel.Email}' is already registered");
                }

                // Créer un nouvel utilisateur
                var user = new User
                {
                    Username = registerModel.Username,
                    Email = registerModel.Email,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Role = "User", // Par défaut, le rôle est "User"
                    IsActive = true
                };

                var createdUser = await _userService.CreateUserAsync(user, registerModel.Password);

                // Ne pas renvoyer le hash et le sel du mot de passe
                createdUser.PasswordHash = null;
                createdUser.PasswordSalt = null;

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering user: {registerModel.Username}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error registering user: {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <param name="user">Utilisateur mis à jour</param>
        /// <returns>Aucun contenu</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            try
            {
                if (id != user.Id)
                {
                    return BadRequest("User ID mismatch");
                }

                var updatedUser = await _userService.UpdateUserAsync(id, user);
                if (updatedUser == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour le mot de passe d'un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <param name="passwordModel">Modèle de mot de passe</param>
        /// <returns>Aucun contenu</returns>
        [HttpPut("{id}/password")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] PasswordModel passwordModel)
        {
            try
            {
                var result = await _userService.UpdatePasswordAsync(id, passwordModel.CurrentPassword, passwordModel.NewPassword);
                if (!result)
                {
                    return BadRequest("Current password is incorrect");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating password for user with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating password: {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Aucun contenu</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound($"User with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting user: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Modèle de connexion
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Nom d'utilisateur ou adresse e-mail
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Mot de passe
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Modèle d'enregistrement
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Adresse e-mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Mot de passe
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Prénom
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Nom
        /// </summary>
        public string LastName { get; set; }
    }

    /// <summary>
    /// Modèle de mot de passe
    /// </summary>
    public class PasswordModel
    {
        /// <summary>
        /// Mot de passe actuel
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Nouveau mot de passe
        /// </summary>
        public string NewPassword { get; set; }
    }
}
