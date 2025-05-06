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
    public class ProgramsController : ControllerBase
    {
        private readonly ILogger<ProgramsController> _logger;
        private readonly IProgramsService _programsService;

        public ProgramsController(ILogger<ProgramsController> logger, IProgramsService programsService)
        {
            _logger = logger;
            _programsService = programsService;
        }

        /// <summary>
        /// Obtient tous les programmes TV
        /// </summary>
        /// <returns>Liste des programmes TV</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvProgram>>> GetPrograms()
        {
            try
            {
                var programs = await _programsService.GetAllProgramsAsync();
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting programs");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting programs: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient un programme TV par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <returns>Programme TV</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TvProgram>> GetProgram(int id)
        {
            try
            {
                var program = await _programsService.GetProgramByIdAsync(id);

                if (program == null)
                {
                    return NotFound($"Program with ID {id} not found");
                }

                return Ok(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting program with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting program: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les programmes TV pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Liste des programmes TV</returns>
        [HttpGet("channel/{channelId}")]
        public async Task<ActionResult<IEnumerable<TvProgram>>> GetProgramsByChannel(int channelId)
        {
            try
            {
                var programs = await _programsService.GetProgramsByChannelIdAsync(channelId);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting programs for channel with ID {channelId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting programs: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les programmes TV pour une période spécifique
        /// </summary>
        /// <param name="startTime">Date et heure de début (format ISO 8601)</param>
        /// <param name="endTime">Date et heure de fin (format ISO 8601)</param>
        /// <returns>Liste des programmes TV</returns>
        [HttpGet("timerange")]
        public async Task<ActionResult<IEnumerable<TvProgram>>> GetProgramsByTimeRange([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            try
            {
                var programs = await _programsService.GetProgramsByTimeRangeAsync(startTime, endTime);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting programs for time range {startTime} to {endTime}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting programs: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient le programme TV en cours pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Programme TV en cours</returns>
        [HttpGet("channel/{channelId}/current")]
        public async Task<ActionResult<TvProgram>> GetCurrentProgramForChannel(int channelId)
        {
            try
            {
                var program = await _programsService.GetCurrentProgramForChannelAsync(channelId);

                if (program == null)
                {
                    return NotFound($"No current program found for channel with ID {channelId}");
                }

                return Ok(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting current program for channel with ID {channelId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting current program: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les programmes TV à venir pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <param name="count">Nombre de programmes à récupérer</param>
        /// <returns>Liste des programmes TV à venir</returns>
        [HttpGet("channel/{channelId}/upcoming")]
        public async Task<ActionResult<IEnumerable<TvProgram>>> GetUpcomingProgramsForChannel(int channelId, [FromQuery] int count = 5)
        {
            try
            {
                var programs = await _programsService.GetUpcomingProgramsForChannelAsync(channelId, count);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting upcoming programs for channel with ID {channelId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting upcoming programs: {ex.Message}");
            }
        }

        /// <summary>
        /// Recherche des programmes TV par titre ou description
        /// </summary>
        /// <param name="query">Requête de recherche</param>
        /// <returns>Liste des programmes TV</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TvProgram>>> SearchPrograms([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Query is required");
                }

                var programs = await _programsService.SearchProgramsAsync(query);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching programs with query: {query}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error searching programs: {ex.Message}");
            }
        }

        /// <summary>
        /// Crée un nouveau programme TV
        /// </summary>
        /// <param name="program">Programme TV à créer</param>
        /// <returns>Programme TV créé</returns>
        [HttpPost]
        public async Task<ActionResult<TvProgram>> CreateProgram(TvProgram program)
        {
            try
            {
                var createdProgram = await _programsService.AddProgramAsync(program);
                return CreatedAtAction(nameof(GetProgram), new { id = createdProgram.Id }, createdProgram);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating program");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating program: {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour un programme TV
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <param name="program">Programme TV mis à jour</param>
        /// <returns>Aucun contenu</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgram(int id, TvProgram program)
        {
            try
            {
                if (id != program.Id)
                {
                    return BadRequest("Program ID mismatch");
                }

                var updatedProgram = await _programsService.UpdateProgramAsync(id, program);
                if (updatedProgram == null)
                {
                    return NotFound($"Program with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating program with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating program: {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un programme TV
        /// </summary>
        /// <param name="id">Identifiant du programme</param>
        /// <returns>Aucun contenu</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            try
            {
                var result = await _programsService.DeleteProgramAsync(id);
                if (!result)
                {
                    return NotFound($"Program with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting program with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting program: {ex.Message}");
            }
        }

        /// <summary>
        /// Importe des programmes TV à partir d'un fichier EPG
        /// </summary>
        /// <param name="epgUrl">URL du fichier EPG</param>
        /// <returns>Nombre de programmes importés</returns>
        [HttpPost("import")]
        public async Task<ActionResult<int>> ImportPrograms([FromQuery] string epgUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(epgUrl))
                {
                    return BadRequest("EPG URL is required");
                }

                var importedCount = await _programsService.ImportProgramsFromEpgAsync(epgUrl);
                return Ok(new { ImportedCount = importedCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing programs from EPG URL: {epgUrl}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error importing programs: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les catégories de programmes TV distinctes
        /// </summary>
        /// <returns>Liste des catégories de programmes TV</returns>
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctCategories()
        {
            try
            {
                var categories = await _programsService.GetDistinctCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct program categories");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting distinct program categories: {ex.Message}");
            }
        }
    }
}
