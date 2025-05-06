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
    /// Contrôleur pour la gestion des VODs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VodController : ControllerBase
    {
        private readonly IVodService _vodService;
        private readonly ILogger<VodController> _logger;
        private readonly IHistoryService _historyService;

        public VodController(
            IVodService vodService,
            ILogger<VodController> logger,
            IHistoryService historyService)
        {
            _vodService = vodService;
            _logger = logger;
            _historyService = historyService;
        }

        /// <summary>
        /// Récupère un élément VOD par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'élément VOD</param>
        /// <param name="logView">Indique si la vue doit être enregistrée dans l'historique</param>
        /// <returns>L'élément VOD s'il est trouvé</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VodItem>> GetVodItem(int id, [FromQuery] bool logView = false)
        {
            _logger.LogInformation($"Controller: Received request to get VOD item with ID: {id}.");

            // Validation de base : s'assurer que l'ID est positif
            if (id <= 0)
            {
                _logger.LogWarning($"Controller: Received invalid VOD item ID: {id}. ID must be positive.");
                return BadRequest("L'ID du contenu VOD doit être un nombre positif.");
            }

            try
            {
                // Appel à la logique métier via le service
                var vodItem = await _vodService.GetVodItemByIdAsync(id);

                // Vérifier si l'élément VOD a été trouvé
                if (vodItem == null)
                {
                    _logger.LogWarning($"Controller: VOD item with ID {id} not found.");
                    return NotFound($"Contenu VOD avec l'ID {id} non trouvé.");
                }

                // Si l'utilisateur est authentifié et que logView est true, enregistrer la vue
                if (logView && User.Identity?.IsAuthenticated == true)
                {
                    try
                    {
                        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                        if (userId > 0)
                        {
                            await _historyService.LogViewAsync(userId, id, "vod");
                            _logger.LogInformation($"View logged for user {userId} on VOD item {id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ne pas échouer la requête principale si l'enregistrement de la vue échoue
                        _logger.LogWarning(ex, $"Failed to log view for VOD item {id}");
                    }
                }

                // Retourner la réponse
                _logger.LogInformation($"Controller: Successfully retrieved VOD item with ID: {id}.");
                return Ok(vodItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Controller: Error getting VOD item with ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Une erreur interne est survenue lors de la récupération du contenu VOD avec l'ID {id}.");
            }
        }

        /// <summary>
        /// Récupère les éléments VOD actifs avec pagination
        /// </summary>
        /// <param name="pageNumber">Numéro de page (1-based)</param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        /// <returns>Résultat paginé des éléments VOD actifs</returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<VodItem>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<VodItem>>> GetActiveVodItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Controller: Received request to get active VOD items with pagination. PageNumber={pageNumber}, PageSize={pageSize}");

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
                var paginatedItems = await _vodService.GetActiveVodItemsAsync(parameters);

                _logger.LogInformation($"Controller: Successfully retrieved paginated active VOD items (Page {paginatedItems.PageNumber}/{paginatedItems.TotalPages}).");
                return Ok(paginatedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error getting active VOD items with pagination");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting active VOD items: {ex.Message}");
            }
        }

        /// <summary>
        /// Filtre les éléments VOD selon les critères spécifiés
        /// </summary>
        /// <param name="filter">Critères de filtrage</param>
        /// <returns>Résultat paginé des éléments VOD</returns>
        [HttpGet("filter")]
        public async Task<ActionResult<PaginatedResult<VodItem>>> FilterVodItems([FromQuery] VodItemFilter filter)
        {
            try
            {
                var result = await _vodService.FilterVodItemsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering VOD items");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error filtering VOD items: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les catégories distinctes des éléments VOD
        /// </summary>
        /// <returns>Liste des catégories distinctes</returns>
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctCategories()
        {
            try
            {
                var categories = await _vodService.GetDistinctCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD categories");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting distinct VOD categories: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les types distincts des éléments VOD
        /// </summary>
        /// <returns>Liste des types distincts</returns>
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctTypes()
        {
            try
            {
                var types = await _vodService.GetDistinctTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD types");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting distinct VOD types: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les langues distinctes des éléments VOD
        /// </summary>
        /// <returns>Liste des langues distinctes</returns>
        [HttpGet("languages")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctLanguages()
        {
            try
            {
                var languages = await _vodService.GetDistinctLanguagesAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD languages");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting distinct VOD languages: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les années distinctes des éléments VOD
        /// </summary>
        /// <returns>Liste des années distinctes</returns>
        [HttpGet("years")]
        public async Task<ActionResult<IEnumerable<int>>> GetDistinctYears()
        {
            try
            {
                var years = await _vodService.GetDistinctYearsAsync();
                return Ok(years);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD years");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting distinct VOD years: {ex.Message}");
            }
        }
    }
}
