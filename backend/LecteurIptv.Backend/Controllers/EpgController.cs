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
    /// <summary>
    /// Contrôleur pour la gestion de l'EPG (Electronic Program Guide)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EpgController : ControllerBase
    {
        private readonly ILogger<EpgController> _logger;
        private readonly IProgramsService _programsService;
        private readonly IXmltvParser _xmltvParser;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="programsService">Service de gestion des programmes TV</param>
        /// <param name="xmltvParser">Parser XMLTV</param>
        public EpgController(
            ILogger<EpgController> logger,
            IProgramsService programsService,
            IXmltvParser xmltvParser)
        {
            _logger = logger;
            _programsService = programsService;
            _xmltvParser = xmltvParser;
        }

        /// <summary>
        /// Récupère les programmes TV pour une période spécifique
        /// </summary>
        /// <param name="startTime">Date et heure de début (format ISO 8601)</param>
        /// <param name="endTime">Date et heure de fin (format ISO 8601)</param>
        /// <returns>Liste des programmes TV pour la période spécifiée</returns>
        [HttpGet("guide")]
        public async Task<ActionResult<IEnumerable<TvProgram>>> GetGuide(
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            try
            {
                if (startTime >= endTime)
                {
                    return BadRequest("La date de début doit être antérieure à la date de fin");
                }

                var programs = await _programsService.GetProgramsByTimeRangeAsync(startTime, endTime);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération des programmes TV entre {startTime} et {endTime}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération des programmes TV : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les programmes TV pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Liste des programmes TV pour la chaîne spécifiée</returns>
        [HttpGet("channels/{channelId}")]
        public async Task<ActionResult<IEnumerable<TvProgram>>> GetProgramsByChannel(int channelId)
        {
            try
            {
                var programs = await _programsService.GetProgramsByChannelIdAsync(channelId);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération des programmes TV pour la chaîne avec l'ID {channelId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération des programmes TV : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère le programme TV en cours pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <returns>Programme TV en cours pour la chaîne spécifiée</returns>
        [HttpGet("channels/{channelId}/current")]
        public async Task<ActionResult<TvProgram>> GetCurrentProgramForChannel(int channelId)
        {
            try
            {
                var program = await _programsService.GetCurrentProgramForChannelAsync(channelId);
                
                if (program == null)
                {
                    return NotFound($"Aucun programme en cours pour la chaîne avec l'ID {channelId}");
                }
                
                return Ok(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération du programme TV en cours pour la chaîne avec l'ID {channelId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération du programme TV en cours : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les programmes TV à venir pour une chaîne spécifique
        /// </summary>
        /// <param name="channelId">Identifiant de la chaîne</param>
        /// <param name="count">Nombre de programmes à récupérer (par défaut : 5)</param>
        /// <returns>Liste des programmes TV à venir pour la chaîne spécifiée</returns>
        [HttpGet("channels/{channelId}/upcoming")]
        public async Task<ActionResult<IEnumerable<TvProgram>>> GetUpcomingProgramsForChannel(
            int channelId,
            [FromQuery] int count = 5)
        {
            try
            {
                var programs = await _programsService.GetUpcomingProgramsForChannelAsync(channelId, count);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération des programmes TV à venir pour la chaîne avec l'ID {channelId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération des programmes TV à venir : {ex.Message}");
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
                    return BadRequest("L'URL du fichier EPG est requise");
                }

                var importedCount = await _programsService.ImportProgramsFromEpgAsync(epgUrl);
                return Ok(new { ImportedCount = importedCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'importation des programmes TV à partir du fichier EPG : {epgUrl}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de l'importation des programmes TV : {ex.Message}");
            }
        }

        /// <summary>
        /// Importe des programmes TV à partir d'un fichier EPG uploadé
        /// </summary>
        /// <param name="file">Fichier EPG</param>
        /// <returns>Nombre de programmes importés</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<int>> UploadAndImport(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Le fichier EPG est requis");
                }

                // Parser le fichier
                XmltvParseResult parseResult;
                using (var stream = file.OpenReadStream())
                {
                    parseResult = await _xmltvParser.ParseFromStreamAsync(stream);
                }

                // Compter les programmes importés
                int importedCount = 0;
                int updatedCount = 0;

                // Traiter les programmes
                foreach (var xmltvProgramme in parseResult.Programmes)
                {
                    // Créer un programme TV à partir du programme XMLTV
                    var program = new TvProgram
                    {
                        Title = xmltvProgramme.Title,
                        Description = xmltvProgramme.Description,
                        StartTime = xmltvProgramme.StartTime,
                        EndTime = xmltvProgramme.EndTime,
                        Category = xmltvProgramme.Category,
                        Language = xmltvProgramme.Language,
                        Actors = xmltvProgramme.Actors,
                        Director = xmltvProgramme.Directors,
                        ImageUrl = xmltvProgramme.ImageUrl,
                        Year = xmltvProgramme.Year
                    };

                    // Ajouter le programme
                    await _programsService.AddProgramAsync(program);
                    importedCount++;
                }

                return Ok(new { ImportedCount = importedCount, UpdatedCount = updatedCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'importation des programmes TV à partir du fichier EPG uploadé");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de l'importation des programmes TV : {ex.Message}");
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
                _logger.LogError(ex, "Erreur lors de la récupération des catégories de programmes TV distinctes");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération des catégories de programmes TV : {ex.Message}");
            }
        }
    }
}
