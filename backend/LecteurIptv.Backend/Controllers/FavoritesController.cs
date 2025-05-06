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
    public class FavoritesController : ControllerBase
    {
        private readonly ILogger<FavoritesController> _logger;
        private readonly IFavoritesService _favoritesService;

        public FavoritesController(ILogger<FavoritesController> logger, IFavoritesService favoritesService)
        {
            _logger = logger;
            _favoritesService = favoritesService;
        }

        /// <summary>
        /// Obtient les chaînes favorites d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des chaînes favorites</returns>
        [HttpGet("channels/{userId}")]
        public async Task<ActionResult<IEnumerable<Channel>>> GetFavoriteChannels(int userId)
        {
            try
            {
                var channels = await _favoritesService.GetFavoriteChannelsAsync(userId);
                return Ok(channels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting favorite channels for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting favorite channels: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les éléments VOD favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des éléments VOD favoris</returns>
        [HttpGet("vods/{userId}")]
        public async Task<ActionResult<IEnumerable<VodItem>>> GetFavoriteVods(int userId)
        {
            try
            {
                var vods = await _favoritesService.GetFavoriteVodsAsync(userId);
                return Ok(vods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting favorite VODs for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting favorite VODs: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute une chaîne aux favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpPost("channels/{userId}/{channelId}")]
        public async Task<IActionResult> AddFavoriteChannel(int userId, int channelId)
        {
            try
            {
                var result = await _favoritesService.AddFavoriteChannelAsync(userId, channelId);
                if (!result)
                {
                    return BadRequest("Failed to add channel to favorites");
                }

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding channel with ID {channelId} to favorites for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding channel to favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute un élément VOD aux favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpPost("vods/{userId}/{vodItemId}")]
        public async Task<IActionResult> AddFavoriteVod(int userId, int vodItemId)
        {
            try
            {
                var result = await _favoritesService.AddFavoriteVodAsync(userId, vodItemId);
                if (!result)
                {
                    return BadRequest("Failed to add VOD item to favorites");
                }

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding VOD item with ID {vodItemId} to favorites for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding VOD item to favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime une chaîne des favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpDelete("channels/{userId}/{channelId}")]
        public async Task<IActionResult> RemoveFavoriteChannel(int userId, int channelId)
        {
            try
            {
                var result = await _favoritesService.RemoveFavoriteChannelAsync(userId, channelId);
                if (!result)
                {
                    return NotFound("Channel not found in favorites");
                }

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing channel with ID {channelId} from favorites for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error removing channel from favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un élément VOD des favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpDelete("vods/{userId}/{vodItemId}")]
        public async Task<IActionResult> RemoveFavoriteVod(int userId, int vodItemId)
        {
            try
            {
                var result = await _favoritesService.RemoveFavoriteVodAsync(userId, vodItemId);
                if (!result)
                {
                    return NotFound("VOD item not found in favorites");
                }

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing VOD item with ID {vodItemId} from favorites for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error removing VOD item from favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie si une chaîne est dans les favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Résultat de la vérification</returns>
        [HttpGet("channels/{userId}/{channelId}")]
        public async Task<ActionResult<bool>> IsChannelFavorite(int userId, int channelId)
        {
            try
            {
                var result = await _favoritesService.IsChannelFavoriteAsync(userId, channelId);
                return Ok(new { IsFavorite = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if channel with ID {channelId} is in favorites for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error checking if channel is in favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie si un élément VOD est dans les favoris d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="vodItemId">Identifiant de l'élément VOD</param>
        /// <returns>Résultat de la vérification</returns>
        [HttpGet("vods/{userId}/{vodItemId}")]
        public async Task<ActionResult<bool>> IsVodFavorite(int userId, int vodItemId)
        {
            try
            {
                var result = await _favoritesService.IsVodFavoriteAsync(userId, vodItemId);
                return Ok(new { IsFavorite = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if VOD item with ID {vodItemId} is in favorites for user with ID {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error checking if VOD item is in favorites: {ex.Message}");
            }
        }
    }
}
