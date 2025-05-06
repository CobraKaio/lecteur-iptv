using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion de l'historique de visionnage
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly ILogger<HistoryController> _logger;
        private readonly IHistoryService _historyService;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="historyService">Service de gestion de l'historique</param>
        public HistoryController(ILogger<HistoryController> logger, IHistoryService historyService)
        {
            _logger = logger;
            _historyService = historyService;
        }

        /// <summary>
        /// Récupère l'historique de visionnage de l'utilisateur connecté
        /// </summary>
        /// <param name="limit">Nombre maximum d'entrées à récupérer</param>
        /// <param name="offset">Nombre d'entrées à ignorer</param>
        /// <returns>Liste des entrées d'historique</returns>
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<UserHistory>>> GetMyHistory([FromQuery] int limit = 50, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                var history = await _historyService.GetHistoryAsync(userId, limit, offset);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'historique de visionnage");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération de l'historique : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère l'historique de visionnage de l'utilisateur connecté pour un type de contenu spécifique
        /// </summary>
        /// <param name="contentType">Type de contenu ("channel" ou "vod")</param>
        /// <param name="limit">Nombre maximum d'entrées à récupérer</param>
        /// <param name="offset">Nombre d'entrées à ignorer</param>
        /// <returns>Liste des entrées d'historique</returns>
        [HttpGet("me/{contentType}")]
        public async Task<ActionResult<IEnumerable<UserHistory>>> GetMyHistoryByType(string contentType, [FromQuery] int limit = 50, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                if (contentType != "channel" && contentType != "vod")
                {
                    return BadRequest($"Type de contenu invalide : {contentType}. Les valeurs acceptées sont 'channel' ou 'vod'");
                }

                var history = await _historyService.GetHistoryByTypeAsync(userId, contentType, limit, offset);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de l'historique de visionnage pour le type {contentType}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération de l'historique : {ex.Message}");
            }
        }

        /// <summary>
        /// Enregistre une entrée dans l'historique de visionnage
        /// </summary>
        /// <param name="contentId">Identifiant du contenu</param>
        /// <param name="contentType">Type de contenu ("channel" ou "vod")</param>
        /// <param name="durationSeconds">Durée de visionnage en secondes (optionnel)</param>
        /// <param name="positionSeconds">Position de lecture en secondes (optionnel)</param>
        /// <returns>Entrée d'historique créée</returns>
        [HttpPost("log")]
        public async Task<ActionResult<UserHistory>> LogView(
            [FromQuery] int contentId,
            [FromQuery] string contentType,
            [FromQuery] int? durationSeconds = null,
            [FromQuery] int? positionSeconds = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                if (contentType != "channel" && contentType != "vod")
                {
                    return BadRequest($"Type de contenu invalide : {contentType}. Les valeurs acceptées sont 'channel' ou 'vod'");
                }

                var historyEntry = await _historyService.LogViewAsync(userId, contentId, contentType, durationSeconds, positionSeconds);
                return Ok(historyEntry);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Erreur de validation lors de l'enregistrement d'une entrée dans l'historique");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'enregistrement d'une entrée dans l'historique");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de l'enregistrement dans l'historique : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime une entrée de l'historique de visionnage
        /// </summary>
        /// <param name="historyId">Identifiant de l'entrée d'historique</param>
        /// <returns>Résultat de la suppression</returns>
        [HttpDelete("{historyId}")]
        public async Task<ActionResult> DeleteHistoryEntry(int historyId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                var result = await _historyService.DeleteHistoryEntryAsync(userId, historyId);
                if (!result)
                {
                    return NotFound($"Entrée d'historique avec l'ID {historyId} non trouvée");
                }

                return Ok(new { Message = $"Entrée d'historique avec l'ID {historyId} supprimée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression de l'entrée d'historique {historyId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la suppression de l'entrée d'historique : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime tout l'historique de visionnage de l'utilisateur connecté
        /// </summary>
        /// <returns>Nombre d'entrées supprimées</returns>
        [HttpDelete("clear")]
        public async Task<ActionResult> ClearHistory()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                var count = await _historyService.ClearHistoryAsync(userId);
                return Ok(new { DeletedCount = count, Message = $"{count} entrées d'historique supprimées avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'historique de visionnage");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la suppression de l'historique : {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour la position de lecture d'une entrée d'historique
        /// </summary>
        /// <param name="historyId">Identifiant de l'entrée d'historique</param>
        /// <param name="positionSeconds">Nouvelle position de lecture en secondes</param>
        /// <returns>Entrée d'historique mise à jour</returns>
        [HttpPut("{historyId}/position")]
        public async Task<ActionResult<UserHistory>> UpdatePosition(int historyId, [FromQuery] int positionSeconds)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                {
                    return Unauthorized("Utilisateur non authentifié");
                }

                var historyEntry = await _historyService.UpdatePositionAsync(userId, historyId, positionSeconds);
                return Ok(historyEntry);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Erreur de validation lors de la mise à jour de la position de lecture");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise à jour de la position de lecture pour l'entrée d'historique {historyId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la mise à jour de la position de lecture : {ex.Message}");
            }
        }
    }
}
