using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Service pour la gestion des VODs
    /// </summary>
    public class VodService : IVodService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VodService> _logger;
        private readonly IMemoryCache _memoryCache;

        // Cache keys for static lists
        private const string VodCategoriesCacheKey = "VodCategoriesList";
        private const string VodTypesCacheKey = "VodTypesList";
        private const string VodLanguagesCacheKey = "VodLanguagesList";
        private const string VodYearsCacheKey = "VodYearsList";

        public VodService(AppDbContext context, ILogger<VodService> logger, IMemoryCache memoryCache)
        {
            _context = context;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public async Task<VodItem?> GetVodItemByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Service: Attempting to retrieve VOD item with ID: {id}.");

                // Utilisation de FindAsync pour récupérer l'élément par sa clé primaire
                var vodItem = await _context.VodItems.FindAsync(id);

                if (vodItem != null)
                {
                    _logger.LogInformation($"Service: Found VOD item with ID: {id}.");
                }
                else
                {
                    _logger.LogWarning($"Service: VOD item with ID {id} not found in database.");
                }

                return vodItem; // Retourne l'élément ou null
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Service: Error retrieving VOD item with ID: {id}.");
                // Relance l'exception pour que le contrôleur puisse la gérer
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<VodItem>> GetActiveVodItemsAsync(PaginationParameters parameters)
        {
            try
            {
                _logger.LogInformation($"Service: Récupération des éléments VOD actifs avec pagination. PageNumber={parameters.PageNumber}, PageSize={parameters.PageSize}");

                // Construire la requête pour les éléments VOD actifs
                var query = _context.VodItems
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.DisplayOrder)
                    .ThenBy(v => v.Title)
                    .AsQueryable();

                _logger.LogDebug($"Service: Exécution de la requête: {query.ToQueryString()}");

                // Utiliser la méthode de fabrique pour créer un résultat paginé
                var paginatedResult = await PaginatedResult<VodItem>.CreateAsync(
                    query,
                    parameters.PageNumber,
                    parameters.PageSize);

                _logger.LogInformation($"Service: Récupération réussie de {paginatedResult.Items.Count()} éléments VOD actifs pour la page {paginatedResult.PageNumber} sur {paginatedResult.TotalPages} (Total: {paginatedResult.TotalCount})");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Erreur lors de la récupération des éléments VOD actifs avec pagination");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<VodItem>> FilterVodItemsAsync(VodItemFilter filter)
        {
            try
            {
                var query = _context.VodItems.AsQueryable();

                // Filtrage par statut actif
                query = query.Where(v => v.IsActive);

                // Recherche par texte
                if (!string.IsNullOrWhiteSpace(filter.Query))
                {
                    var searchTerm = filter.Query.ToLower();
                    query = query.Where(v =>
                        v.Title.ToLower().Contains(searchTerm) ||
                        v.Description.ToLower().Contains(searchTerm) ||
                        v.Actors.ToLower().Contains(searchTerm) ||
                        v.Director.ToLower().Contains(searchTerm));
                }

                // Filtrage par catégorie
                if (!string.IsNullOrWhiteSpace(filter.Category))
                {
                    query = query.Where(v => v.Category == filter.Category);
                }

                // Filtrage par type
                if (!string.IsNullOrWhiteSpace(filter.Type))
                {
                    query = query.Where(v => v.Type == filter.Type);
                }

                // Filtrage par année
                if (filter.Year.HasValue)
                {
                    query = query.Where(v => v.Year == filter.Year);
                }

                // Filtrage par langue
                if (!string.IsNullOrWhiteSpace(filter.Language))
                {
                    query = query.Where(v => v.Language == filter.Language);
                }

                // Tri
                query = filter.SortOrder.ToLower() == "desc" ?
                    ApplySortingDescending(query, filter.SortBy) :
                    ApplySortingAscending(query, filter.SortBy);

                // Utiliser la méthode de fabrique pour créer un résultat paginé
                var paginatedResult = await PaginatedResult<VodItem>.CreateAsync(
                    query,
                    filter.Page,
                    filter.PageSize);

                _logger.LogInformation($"Service: Filtrage réussi : {paginatedResult.Items.Count()} éléments VOD trouvés pour la page {paginatedResult.PageNumber} sur {paginatedResult.TotalPages} (Total: {paginatedResult.TotalCount})");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering VOD items");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetDistinctCategoriesAsync()
        {
            _logger.LogInformation("Service: Attempting to get distinct VOD categories.");

            // Try to get the categories from the cache
            if (_memoryCache.TryGetValue(VodCategoriesCacheKey, out IEnumerable<string>? cachedCategories))
            {
                _logger.LogInformation("Service: Distinct VOD categories found in cache.");
                return cachedCategories!;
            }

            _logger.LogInformation("Service: Distinct VOD categories not found in cache, fetching from database.");

            try
            {
                // Get the categories from the database
                var categories = await _context.VodItems
                    .Where(v => v.IsActive && !string.IsNullOrEmpty(v.Category))
                    .Select(v => v.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                _logger.LogInformation($"Service: Successfully fetched {categories.Count} distinct VOD categories from database.");

                // Store the categories in the cache with an expiration policy
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(VodCategoriesCacheKey, categories, cacheEntryOptions);

                _logger.LogInformation($"Service: Stored distinct VOD categories in cache with key '{VodCategoriesCacheKey}'.");

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD categories");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetDistinctTypesAsync()
        {
            _logger.LogInformation("Service: Attempting to get distinct VOD types.");

            // Try to get the types from the cache
            if (_memoryCache.TryGetValue(VodTypesCacheKey, out IEnumerable<string>? cachedTypes))
            {
                _logger.LogInformation("Service: Distinct VOD types found in cache.");
                return cachedTypes!;
            }

            _logger.LogInformation("Service: Distinct VOD types not found in cache, fetching from database.");

            try
            {
                // Get the types from the database
                var types = await _context.VodItems
                    .Where(v => v.IsActive && !string.IsNullOrEmpty(v.Type))
                    .Select(v => v.Type)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToListAsync();

                _logger.LogInformation($"Service: Successfully fetched {types.Count} distinct VOD types from database.");

                // Store the types in the cache with an expiration policy
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(VodTypesCacheKey, types, cacheEntryOptions);

                _logger.LogInformation($"Service: Stored distinct VOD types in cache with key '{VodTypesCacheKey}'.");

                return types;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD types");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetDistinctLanguagesAsync()
        {
            _logger.LogInformation("Service: Attempting to get distinct VOD languages.");

            // Try to get the languages from the cache
            if (_memoryCache.TryGetValue(VodLanguagesCacheKey, out IEnumerable<string>? cachedLanguages))
            {
                _logger.LogInformation("Service: Distinct VOD languages found in cache.");
                return cachedLanguages!;
            }

            _logger.LogInformation("Service: Distinct VOD languages not found in cache, fetching from database.");

            try
            {
                // Get the languages from the database
                var languages = await _context.VodItems
                    .Where(v => v.IsActive && !string.IsNullOrEmpty(v.Language))
                    .Select(v => v.Language)
                    .Distinct()
                    .OrderBy(l => l)
                    .ToListAsync();

                _logger.LogInformation($"Service: Successfully fetched {languages.Count} distinct VOD languages from database.");

                // Store the languages in the cache with an expiration policy
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(VodLanguagesCacheKey, languages, cacheEntryOptions);

                _logger.LogInformation($"Service: Stored distinct VOD languages in cache with key '{VodLanguagesCacheKey}'.");

                return languages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD languages");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<int>> GetDistinctYearsAsync()
        {
            _logger.LogInformation("Service: Attempting to get distinct VOD years.");

            // Try to get the years from the cache
            if (_memoryCache.TryGetValue(VodYearsCacheKey, out IEnumerable<int>? cachedYears))
            {
                _logger.LogInformation("Service: Distinct VOD years found in cache.");
                return cachedYears!;
            }

            _logger.LogInformation("Service: Distinct VOD years not found in cache, fetching from database.");

            try
            {
                // Get the years from the database
                var years = await _context.VodItems
                    .Where(v => v.IsActive && v.Year.HasValue)
                    .Select(v => v.Year.Value)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToListAsync();

                _logger.LogInformation($"Service: Successfully fetched {years.Count} distinct VOD years from database.");

                // Store the years in the cache with an expiration policy
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(VodYearsCacheKey, years, cacheEntryOptions);

                _logger.LogInformation($"Service: Stored distinct VOD years in cache with key '{VodYearsCacheKey}'.");

                return years;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct VOD years");
                throw;
            }
        }

        /// <summary>
        /// Applique le tri ascendant selon le champ spécifié
        /// </summary>
        private IQueryable<VodItem> ApplySortingAscending(IQueryable<VodItem> query, string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "title" => query.OrderBy(v => v.Title),
                "year" => query.OrderBy(v => v.Year),
                "rating" => query.OrderBy(v => v.Rating),
                "category" => query.OrderBy(v => v.Category),
                "type" => query.OrderBy(v => v.Type),
                "language" => query.OrderBy(v => v.Language),
                "displayorder" => query.OrderBy(v => v.DisplayOrder),
                _ => query.OrderBy(v => v.Title)
            };
        }

        /// <summary>
        /// Applique le tri descendant selon le champ spécifié
        /// </summary>
        private IQueryable<VodItem> ApplySortingDescending(IQueryable<VodItem> query, string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "title" => query.OrderByDescending(v => v.Title),
                "year" => query.OrderByDescending(v => v.Year),
                "rating" => query.OrderByDescending(v => v.Rating),
                "category" => query.OrderByDescending(v => v.Category),
                "type" => query.OrderByDescending(v => v.Type),
                "language" => query.OrderByDescending(v => v.Language),
                "displayorder" => query.OrderByDescending(v => v.DisplayOrder),
                _ => query.OrderByDescending(v => v.Title)
            };
        }
    }
}
