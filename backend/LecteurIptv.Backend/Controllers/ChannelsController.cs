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
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelsController : ControllerBase
    {
        private readonly ILogger<ChannelsController> _logger;
        private readonly IChannelsService _channelsService;
        private readonly IM3UParser _m3uParser;
        private readonly IHistoryService _historyService;

        public ChannelsController(
            ILogger<ChannelsController> logger,
            IChannelsService channelsService,
            IM3UParser m3uParser,
            IHistoryService historyService)
        {
            _logger = logger;
            _channelsService = channelsService;
            _m3uParser = m3uParser;
            _historyService = historyService;
        }

        /// <summary>
        /// Obtient toutes les chaînes avec pagination
        /// </summary>
        /// <param name="pageNumber">Numéro de page (1-based)</param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        /// <returns>Résultat paginé des chaînes</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<Channel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<Channel>>> GetChannels([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Controller: Received request to get channels with pagination. PageNumber={pageNumber}, PageSize={pageSize}");

                // Validation des paramètres de pagination
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Les paramètres de pagination (pageNumber, pageSize) doivent être supérieurs à 0.");
                }

                // Créer l'objet PaginationParameters
                var parameters = new PaginationParameters
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Appeler le service avec les paramètres de pagination
                var paginatedChannels = await _channelsService.GetAllChannelsAsync(parameters);

                _logger.LogInformation($"Controller: Successfully retrieved paginated channels (Page {paginatedChannels.PageNumber}/{paginatedChannels.TotalPages}).");
                return Ok(paginatedChannels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error getting channels with pagination");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting channels: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient une chaîne par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <param name="logView">Indique si la vue doit être enregistrée dans l'historique</param>
        /// <returns>Chaîne</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Channel>> GetChannel(int id, [FromQuery] bool logView = false)
        {
            try
            {
                var channel = await _channelsService.GetChannelByIdAsync(id);

                if (channel == null)
                {
                    return NotFound($"Channel with ID {id} not found");
                }

                // Si l'utilisateur est authentifié et que logView est true, enregistrer la vue
                if (logView && User.Identity?.IsAuthenticated == true)
                {
                    try
                    {
                        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                        if (userId > 0)
                        {
                            await _historyService.LogViewAsync(userId, id, "channel");
                            _logger.LogInformation($"View logged for user {userId} on channel {id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ne pas échouer la requête principale si l'enregistrement de la vue échoue
                        _logger.LogWarning(ex, $"Failed to log view for channel {id}");
                    }
                }

                return Ok(channel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting channel with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting channel: {ex.Message}");
            }
        }

        /// <summary>
        /// Crée une nouvelle chaîne
        /// </summary>
        /// <param name="channel">Chaîne à créer</param>
        /// <returns>Chaîne créée</returns>
        [HttpPost]
        public async Task<ActionResult<Channel>> CreateChannel(Channel channel)
        {
            try
            {
                var createdChannel = await _channelsService.AddChannelAsync(channel);
                return CreatedAtAction(nameof(GetChannel), new { id = createdChannel.Id }, createdChannel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating channel");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating channel: {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour une chaîne
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <param name="channel">Chaîne mise à jour</param>
        /// <returns>Aucun contenu</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChannel(int id, Channel channel)
        {
            try
            {
                if (id != channel.Id)
                {
                    return BadRequest("Channel ID mismatch");
                }

                var updatedChannel = await _channelsService.UpdateChannelAsync(id, channel);
                if (updatedChannel == null)
                {
                    return NotFound($"Channel with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating channel with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating channel: {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime une chaîne
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>Aucun contenu</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChannel(int id)
        {
            try
            {
                var result = await _channelsService.DeleteChannelAsync(id);
                if (!result)
                {
                    return NotFound($"Channel with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting channel with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting channel: {ex.Message}");
            }
        }

        /// <summary>
        /// Importe des chaînes à partir d'un fichier M3U
        /// </summary>
        /// <param name="url">URL du fichier M3U</param>
        /// <returns>Nombre de chaînes importées</returns>
        [HttpPost("import")]
        public async Task<ActionResult<int>> ImportChannels([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return BadRequest("URL is required");
                }

                var playlist = await _m3uParser.ParseFromUrlAsync(url);
                var importedCount = await _channelsService.ImportChannelsFromM3UAsync(playlist);

                return Ok(new { ImportedCount = importedCount, TotalCount = playlist.Channels.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing channels from URL: {url}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error importing channels: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les chaînes par catégorie avec pagination
        /// </summary>
        /// <param name="category">Catégorie</param>
        /// <param name="pageNumber">Numéro de page (1-based)</param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        /// <returns>Résultat paginé des chaînes de la catégorie spécifiée</returns>
        [HttpGet("category/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<Channel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<Channel>>> GetChannelsByCategory(string category, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Controller: Received request to get channels by category '{category}' with pagination. PageNumber={pageNumber}, PageSize={pageSize}");

                // Validation des paramètres de pagination
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Les paramètres de pagination (pageNumber, pageSize) doivent être supérieurs à 0.");
                }

                // Créer l'objet PaginationParameters
                var parameters = new PaginationParameters
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Appeler le service avec les paramètres de pagination
                var paginatedChannels = await _channelsService.GetChannelsByCategoryAsync(category, parameters);

                _logger.LogInformation($"Controller: Successfully retrieved paginated channels for category '{category}' (Page {paginatedChannels.PageNumber}/{paginatedChannels.TotalPages}).");
                return Ok(paginatedChannels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controller: Error getting channels by category '{category}' with pagination");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting channels: {ex.Message}");
            }
        }

        /// <summary>
        /// Recherche des chaînes avec pagination
        /// </summary>
        /// <param name="query">Requête de recherche</param>
        /// <param name="pageNumber">Numéro de page (1-based)</param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        /// <returns>Résultat paginé des chaînes correspondant au terme de recherche</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<Channel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<Channel>>> SearchChannels([FromQuery] string query, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Controller: Received request to search channels with query '{query}' and pagination. PageNumber={pageNumber}, PageSize={pageSize}");

                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Query is required");
                }

                // Validation des paramètres de pagination
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Les paramètres de pagination (pageNumber, pageSize) doivent être supérieurs à 0.");
                }

                // Créer l'objet PaginationParameters
                var parameters = new PaginationParameters
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Appeler le service avec les paramètres de pagination
                var paginatedChannels = await _channelsService.SearchChannelsAsync(query, parameters);

                _logger.LogInformation($"Controller: Successfully searched channels with query '{query}' (Page {paginatedChannels.PageNumber}/{paginatedChannels.TotalPages}).");
                return Ok(paginatedChannels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controller: Error searching channels with query '{query}' and pagination");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error searching channels: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie si une chaîne est disponible
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>Résultat de la vérification</returns>
        [HttpGet("{id}/check")]
        public async Task<ActionResult<bool>> CheckChannelAvailability(int id)
        {
            try
            {
                var isAvailable = await _channelsService.IsChannelAvailableAsync(id);
                return Ok(new { IsAvailable = isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking channel availability with ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error checking channel availability: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les chaînes actives avec pagination
        /// </summary>
        /// <param name="pageNumber">Numéro de page (1-based)</param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        /// <returns>Résultat paginé des chaînes actives</returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<Channel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<Channel>>> GetActiveChannels([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Controller: Received request to get active channels with pagination. PageNumber={pageNumber}, PageSize={pageSize}");

                // Validation des paramètres de pagination
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Les paramètres de pagination (pageNumber, pageSize) doivent être supérieurs à 0.");
                }

                // Créer l'objet PaginationParameters
                var parameters = new PaginationParameters
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Appeler le service avec les paramètres de pagination
                var paginatedChannels = await _channelsService.GetActiveChannelsAsync(parameters);

                _logger.LogInformation($"Controller: Successfully retrieved paginated active channels (Page {paginatedChannels.PageNumber}/{paginatedChannels.TotalPages}).");
                return Ok(paginatedChannels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error getting active channels with pagination");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting active channels: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les groupes de chaînes distincts
        /// </summary>
        /// <returns>Liste des groupes de chaînes</returns>
        [HttpGet("groups")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctGroups()
        {
            try
            {
                var groups = await _channelsService.GetDistinctGroupsAsync();
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct channel groups");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting distinct channel groups: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les catégories de chaînes distinctes
        /// </summary>
        /// <returns>Liste des catégories de chaînes</returns>
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctCategories()
        {
            try
            {
                var categories = await _channelsService.GetDistinctCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct channel categories");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting distinct channel categories: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les chaînes par groupe avec pagination
        /// </summary>
        /// <param name="group">Groupe</param>
        /// <param name="pageNumber">Numéro de page (1-based)</param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        /// <returns>Résultat paginé des chaînes du groupe spécifié</returns>
        [HttpGet("group/{group}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<Channel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<Channel>>> GetChannelsByGroup(string group, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Controller: Received request to get channels by group '{group}' with pagination. PageNumber={pageNumber}, PageSize={pageSize}");

                if (string.IsNullOrWhiteSpace(group))
                {
                    return BadRequest("Group is required");
                }

                // Validation des paramètres de pagination
                if (pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Les paramètres de pagination (pageNumber, pageSize) doivent être supérieurs à 0.");
                }

                // Créer l'objet PaginationParameters
                var parameters = new PaginationParameters
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // Appeler le service avec les paramètres de pagination
                var paginatedChannels = await _channelsService.GetChannelsByGroupAsync(group, parameters);

                _logger.LogInformation($"Controller: Successfully retrieved paginated channels for group '{group}' (Page {paginatedChannels.PageNumber}/{paginatedChannels.TotalPages}).");
                return Ok(paginatedChannels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controller: Error getting channels by group '{group}' with pagination");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting channels: {ex.Message}");
            }
        }
    }
}
