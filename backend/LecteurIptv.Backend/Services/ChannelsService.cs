using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Data.Extensions;
using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Services
{
    /// <summary>
    /// Service de gestion des chaînes
    /// </summary>
    public class ChannelsService : IChannelsService
    {
        private readonly ILogger<ChannelsService> _logger;
        private readonly AppDbContext _context;
        private readonly IStreamingService _streamingService;
        private readonly IMemoryCache _memoryCache;

        // Cache keys for static lists
        private const string ChannelGroupsCacheKey = "ChannelGroupsList";
        private const string ChannelCategoriesCacheKey = "ChannelCategoriesList";

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Contexte de base de données</param>
        /// <param name="streamingService">Service de streaming</param>
        /// <param name="memoryCache">Service de cache mémoire</param>
        public ChannelsService(
            ILogger<ChannelsService> logger,
            AppDbContext context,
            IStreamingService streamingService,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _context = context;
            _streamingService = streamingService;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Récupère toutes les chaînes avec pagination
        /// </summary>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes</returns>
        public async Task<PaginatedResult<Channel>> GetAllChannelsAsync(PaginationParameters parameters)
        {
            try
            {
                _logger.LogInformation($"Service: Récupération de toutes les chaînes avec pagination. PageNumber={parameters.PageNumber}, PageSize={parameters.PageSize}");

                // Accès à la collection Channels du contexte de base de données
                var query = _context.Channels
                    .OrderBy(c => c.DisplayOrder)  // Tri principal par ordre d'affichage
                    .ThenBy(c => c.Name)           // Tri secondaire par nom
                    .AsQueryable();                // Assure que nous avons un IQueryable

                _logger.LogDebug($"Service: Exécution de la requête: {query.ToQueryString()}");

                // Utiliser la méthode de fabrique pour créer un résultat paginé
                var paginatedResult = await PaginatedResult<Channel>.CreateAsync(
                    query,
                    parameters.PageNumber,
                    parameters.PageSize);

                _logger.LogInformation($"Service: Récupération réussie de {paginatedResult.Items.Count()} chaînes pour la page {paginatedResult.PageNumber} sur {paginatedResult.TotalPages} (Total: {paginatedResult.TotalCount})");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Erreur lors de la récupération de toutes les chaînes avec pagination");
                throw; // Relance l'exception pour qu'elle soit gérée par le contrôleur
            }
        }

        /// <summary>
        /// Récupère les chaînes actives avec pagination
        /// </summary>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes actives</returns>
        public async Task<PaginatedResult<Channel>> GetActiveChannelsAsync(PaginationParameters parameters)
        {
            try
            {
                _logger.LogInformation($"Service: Récupération des chaînes actives avec pagination. PageNumber={parameters.PageNumber}, PageSize={parameters.PageSize}");

                // Construire la requête pour les chaînes actives
                var query = _context.Channels
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Name)
                    .AsQueryable();

                _logger.LogDebug($"Service: Exécution de la requête: {query.ToQueryString()}");

                // Utiliser la méthode de fabrique pour créer un résultat paginé
                var paginatedResult = await PaginatedResult<Channel>.CreateAsync(
                    query,
                    parameters.PageNumber,
                    parameters.PageSize);

                _logger.LogInformation($"Service: Récupération réussie de {paginatedResult.Items.Count()} chaînes actives pour la page {paginatedResult.PageNumber} sur {paginatedResult.TotalPages} (Total: {paginatedResult.TotalCount})");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Erreur lors de la récupération des chaînes actives avec pagination");
                throw; // Relance l'exception pour qu'elle soit gérée par le contrôleur
            }
        }

        /// <summary>
        /// Récupère une chaîne par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>Chaîne correspondante ou null si non trouvée</returns>
        public async Task<Channel> GetChannelByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Récupération de la chaîne avec l'ID {id}");

                // Utilisation de AsTracking() pour permettre la modification de l'entité si nécessaire
                // FirstOrDefaultAsync retourne null si aucune entité ne correspond au prédicat
                var channel = await _context.Channels
                    .AsTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (channel == null)
                {
                    _logger.LogWarning($"Aucune chaîne trouvée avec l'ID {id}");
                }
                else
                {
                    _logger.LogInformation($"Chaîne trouvée : {channel.Name} (ID: {channel.Id})");
                }

                return channel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de la chaîne avec l'ID {id}");
                throw; // Relance l'exception pour qu'elle soit gérée par le contrôleur
            }
        }

        /// <summary>
        /// Récupère les chaînes par groupe avec pagination
        /// </summary>
        /// <param name="group">Groupe de chaînes</param>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes du groupe spécifié</returns>
        public async Task<PaginatedResult<Channel>> GetChannelsByGroupAsync(string group, PaginationParameters parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(group))
                {
                    _logger.LogWarning("Service: Tentative de récupération des chaînes avec un groupe vide");
                    // Créer une requête vide
                    var emptyQuery = Enumerable.Empty<Channel>().AsQueryable();

                    // Utiliser la méthode de fabrique pour créer un résultat paginé
                    return await PaginatedResult<Channel>.CreateAsync(
                        emptyQuery,
                        parameters.PageNumber,
                        parameters.PageSize);
                }

                _logger.LogInformation($"Service: Récupération des chaînes du groupe '{group}' avec pagination. PageNumber={parameters.PageNumber}, PageSize={parameters.PageSize}");

                // Construire la requête pour les chaînes du groupe spécifié
                var query = _context.Channels
                    .Where(c => c.IsActive && c.Group == group)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Name)
                    .AsQueryable();

                _logger.LogDebug($"Service: Exécution de la requête: {query.ToQueryString()}");

                // Utiliser la méthode de fabrique pour créer un résultat paginé
                var paginatedResult = await PaginatedResult<Channel>.CreateAsync(
                    query,
                    parameters.PageNumber,
                    parameters.PageSize);

                _logger.LogInformation($"Service: Récupération réussie de {paginatedResult.Items.Count()} chaînes du groupe '{group}' pour la page {paginatedResult.PageNumber} sur {paginatedResult.TotalPages} (Total: {paginatedResult.TotalCount})");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Service: Erreur lors de la récupération des chaînes du groupe '{group}' avec pagination");
                throw; // Relance l'exception pour qu'elle soit gérée par le contrôleur
            }
        }

        /// <summary>
        /// Récupère les chaînes par catégorie avec pagination
        /// </summary>
        /// <param name="category">Catégorie de chaînes</param>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes de la catégorie spécifiée</returns>
        public async Task<PaginatedResult<Channel>> GetChannelsByCategoryAsync(string category, PaginationParameters parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    _logger.LogWarning("Service: Tentative de récupération des chaînes avec une catégorie vide");
                    // Créer une requête vide
                    var emptyQuery = Enumerable.Empty<Channel>().AsQueryable();

                    // Utiliser la méthode de fabrique pour créer un résultat paginé
                    return await PaginatedResult<Channel>.CreateAsync(
                        emptyQuery,
                        parameters.PageNumber,
                        parameters.PageSize);
                }

                _logger.LogInformation($"Service: Récupération des chaînes de la catégorie '{category}' avec pagination. PageNumber={parameters.PageNumber}, PageSize={parameters.PageSize}");

                // Construction de la requête pour filtrer les chaînes actives de la catégorie spécifiée
                var query = _context.Channels
                    .Where(c => c.IsActive && c.Category == category)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Name)
                    .AsQueryable();

                _logger.LogDebug($"Service: Exécution de la requête: {query.ToQueryString()}");

                // Utiliser la méthode de fabrique pour créer un résultat paginé
                var paginatedResult = await PaginatedResult<Channel>.CreateAsync(
                    query,
                    parameters.PageNumber,
                    parameters.PageSize);

                _logger.LogInformation($"Service: Récupération réussie de {paginatedResult.Items.Count()} chaînes de la catégorie '{category}' pour la page {paginatedResult.PageNumber} sur {paginatedResult.TotalPages} (Total: {paginatedResult.TotalCount})");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Service: Erreur lors de la récupération des chaînes de la catégorie '{category}' avec pagination");
                throw; // Relance l'exception pour qu'elle soit gérée par le contrôleur
            }
        }

        /// <summary>
        /// Recherche des chaînes par nom avec pagination
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <param name="parameters">Paramètres de pagination</param>
        /// <returns>Résultat paginé des chaînes correspondant au terme de recherche</returns>
        public async Task<PaginatedResult<Channel>> SearchChannelsAsync(string searchTerm, PaginationParameters parameters)
        {
            try
            {
                _logger.LogInformation($"Service: Recherche de chaînes avec le terme '{searchTerm}' et pagination. PageNumber={parameters.PageNumber}, PageSize={parameters.PageSize}");

                // Validation du terme de recherche
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogInformation("Service: Terme de recherche vide, retour de toutes les chaînes actives avec pagination");
                    return await GetActiveChannelsAsync(parameters);
                }

                // Construction de la requête pour rechercher les chaînes
                var query = _context.Channels
                    .Where(c => c.IsActive &&
                               (c.Name.Contains(searchTerm) ||
                                c.Group.Contains(searchTerm) ||
                                c.Category.Contains(searchTerm)))
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Name)
                    .AsQueryable();

                _logger.LogDebug($"Service: Exécution de la requête: {query.ToQueryString()}");

                // Utiliser la méthode de fabrique pour créer un résultat paginé
                var paginatedResult = await PaginatedResult<Channel>.CreateAsync(
                    query,
                    parameters.PageNumber,
                    parameters.PageSize);

                _logger.LogInformation($"Service: Recherche réussie : {paginatedResult.Items.Count()} chaînes trouvées pour le terme '{searchTerm}' pour la page {paginatedResult.PageNumber} sur {paginatedResult.TotalPages} (Total: {paginatedResult.TotalCount})");
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Service: Erreur lors de la recherche de chaînes avec le terme '{searchTerm}' et pagination");
                throw; // Relance l'exception pour qu'elle soit gérée par le contrôleur
            }
        }

        /// <summary>
        /// Ajoute une nouvelle chaîne
        /// </summary>
        /// <param name="channel">Chaîne à ajouter</param>
        /// <returns>Chaîne ajoutée</returns>
        public async Task<Channel> AddChannelAsync(Channel channel)
        {
            _logger.LogInformation($"Ajout d'une nouvelle chaîne : {channel.Name}");

            // Vérifier que l'URL du flux est valide
            if (!string.IsNullOrEmpty(channel.StreamUrl))
            {
                var isAvailable = await _streamingService.IsStreamAvailableAsync(channel.StreamUrl);
                if (!isAvailable)
                {
                    _logger.LogWarning($"L'URL du flux n'est pas disponible : {channel.StreamUrl}");
                }
            }

            await _context.Channels.AddAsync(channel);
            await _context.SaveChangesAsync();

            return channel;
        }

        /// <summary>
        /// Met à jour une chaîne existante
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <param name="channel">Nouvelles données de la chaîne</param>
        /// <returns>Chaîne mise à jour ou null si non trouvée</returns>
        public async Task<Channel> UpdateChannelAsync(int id, Channel channel)
        {
            _logger.LogInformation($"Mise à jour de la chaîne avec l'ID {id}");

            var existingChannel = await _context.Channels
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingChannel == null)
            {
                _logger.LogWarning($"Chaîne avec l'ID {id} non trouvée");
                return null;
            }

            // Mettre à jour les propriétés
            existingChannel.Name = channel.Name;
            existingChannel.StreamUrl = channel.StreamUrl;
            existingChannel.LogoUrl = channel.LogoUrl;
            existingChannel.TvgId = channel.TvgId;
            existingChannel.TvgName = channel.TvgName;
            existingChannel.Group = channel.Group;
            existingChannel.Category = channel.Category;
            existingChannel.Language = channel.Language;
            existingChannel.Country = channel.Country;
            existingChannel.IsActive = channel.IsActive;
            existingChannel.DisplayOrder = channel.DisplayOrder;

            // Vérifier que l'URL du flux est valide
            if (!string.IsNullOrEmpty(existingChannel.StreamUrl))
            {
                var isAvailable = await _streamingService.IsStreamAvailableAsync(existingChannel.StreamUrl);
                if (!isAvailable)
                {
                    _logger.LogWarning($"L'URL du flux n'est pas disponible : {existingChannel.StreamUrl}");
                }
            }

            await _context.SaveChangesAsync();

            return existingChannel;
        }

        /// <summary>
        /// Supprime une chaîne
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne a été supprimée, false sinon</returns>
        public async Task<bool> DeleteChannelAsync(int id)
        {
            _logger.LogInformation($"Suppression de la chaîne avec l'ID {id}");

            var channel = await _context.Channels
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (channel == null)
            {
                _logger.LogWarning($"Chaîne avec l'ID {id} non trouvée");
                return false;
            }

            _context.Channels.Remove(channel);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Importe des chaînes à partir d'une playlist M3U
        /// </summary>
        /// <param name="playlist">Playlist M3U</param>
        /// <returns>Nombre de chaînes importées</returns>
        public async Task<int> ImportChannelsFromM3UAsync(M3UPlaylist playlist)
        {
            _logger.LogInformation($"Importation de chaînes à partir d'une playlist M3U : {playlist.Name}");

            int importedCount = 0;

            foreach (var m3uChannel in playlist.Channels)
            {
                // Vérifier si la chaîne existe déjà (par TvgId ou URL)
                var existingChannel = await _context.Channels
                    .AsTracking()
                    .FirstOrDefaultAsync(c =>
                        (!string.IsNullOrEmpty(m3uChannel.TvgId) && c.TvgId == m3uChannel.TvgId) ||
                        c.StreamUrl == m3uChannel.Url);

                if (existingChannel != null)
                {
                    // Mettre à jour la chaîne existante
                    existingChannel.Name = m3uChannel.Name;
                    existingChannel.StreamUrl = m3uChannel.Url;
                    existingChannel.LogoUrl = m3uChannel.LogoUrl;
                    existingChannel.TvgId = m3uChannel.TvgId;
                    existingChannel.TvgName = m3uChannel.TvgName;
                    existingChannel.Group = m3uChannel.Group;
                    existingChannel.Language = m3uChannel.Language;

                    // Extraire d'autres attributs si disponibles
                    if (m3uChannel.Attributes.TryGetValue("tvg-country", out var country))
                    {
                        existingChannel.Country = country;
                    }

                    if (m3uChannel.Attributes.TryGetValue("tvg-category", out var category))
                    {
                        existingChannel.Category = category;
                    }
                }
                else
                {
                    // Créer une nouvelle chaîne
                    var newChannel = new Channel
                    {
                        Name = m3uChannel.Name,
                        StreamUrl = m3uChannel.Url,
                        LogoUrl = m3uChannel.LogoUrl,
                        TvgId = m3uChannel.TvgId,
                        TvgName = m3uChannel.TvgName,
                        Group = m3uChannel.Group,
                        Language = m3uChannel.Language,
                        IsActive = true
                    };

                    // Extraire d'autres attributs si disponibles
                    if (m3uChannel.Attributes.TryGetValue("tvg-country", out var country))
                    {
                        newChannel.Country = country;
                    }

                    if (m3uChannel.Attributes.TryGetValue("tvg-category", out var category))
                    {
                        newChannel.Category = category;
                    }

                    await _context.Channels.AddAsync(newChannel);
                    importedCount++;
                }
            }

            await _context.SaveChangesAsync();

            return importedCount;
        }

        /// <summary>
        /// Vérifie si une chaîne est disponible
        /// </summary>
        /// <param name="id">Identifiant de la chaîne</param>
        /// <returns>True si la chaîne est disponible, false sinon</returns>
        public async Task<bool> IsChannelAvailableAsync(int id)
        {
            _logger.LogInformation($"Vérification de la disponibilité de la chaîne avec l'ID {id}");

            var channel = await _context.Channels
                .FirstOrDefaultAsync(c => c.Id == id);

            if (channel == null)
            {
                _logger.LogWarning($"Chaîne avec l'ID {id} non trouvée");
                return false;
            }

            if (string.IsNullOrEmpty(channel.StreamUrl))
            {
                _logger.LogWarning($"La chaîne avec l'ID {id} n'a pas d'URL de flux");
                return false;
            }

            return await _streamingService.IsStreamAvailableAsync(channel.StreamUrl);
        }

        /// <summary>
        /// Récupère les groupes de chaînes distincts
        /// </summary>
        /// <returns>Liste des groupes de chaînes</returns>
        public async Task<IEnumerable<string>> GetDistinctGroupsAsync()
        {
            _logger.LogInformation("Service: Attempting to get distinct channel groups.");

            // Try to get the groups from the cache
            if (_memoryCache.TryGetValue(ChannelGroupsCacheKey, out IEnumerable<string>? cachedGroups))
            {
                _logger.LogInformation("Service: Distinct channel groups found in cache.");
                return cachedGroups!;
            }

            _logger.LogInformation("Service: Distinct channel groups not found in cache, fetching from database.");

            try
            {
                // Get the groups from the database
                var groups = await _context.Channels
                    .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Group))
                    .Select(c => c.Group)
                    .Distinct()
                    .OrderBy(g => g)
                    .ToListAsync();

                _logger.LogInformation($"Service: Successfully fetched {groups.Count} distinct channel groups from database.");

                // Store the groups in the cache with an expiration policy
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(ChannelGroupsCacheKey, groups, cacheEntryOptions);

                _logger.LogInformation($"Service: Stored distinct channel groups in cache with key '{ChannelGroupsCacheKey}'.");

                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct channel groups");
                throw;
            }
        }

        /// <summary>
        /// Récupère les catégories de chaînes distinctes
        /// </summary>
        /// <returns>Liste des catégories de chaînes</returns>
        public async Task<IEnumerable<string>> GetDistinctCategoriesAsync()
        {
            _logger.LogInformation("Service: Attempting to get distinct channel categories.");

            // Try to get the categories from the cache
            if (_memoryCache.TryGetValue(ChannelCategoriesCacheKey, out IEnumerable<string>? cachedCategories))
            {
                _logger.LogInformation("Service: Distinct channel categories found in cache.");
                return cachedCategories!;
            }

            _logger.LogInformation("Service: Distinct channel categories not found in cache, fetching from database.");

            try
            {
                // Get the categories from the database
                var categories = await _context.Channels
                    .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Category))
                    .Select(c => c.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                _logger.LogInformation($"Service: Successfully fetched {categories.Count} distinct channel categories from database.");

                // Store the categories in the cache with an expiration policy
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(ChannelCategoriesCacheKey, categories, cacheEntryOptions);

                _logger.LogInformation($"Service: Stored distinct channel categories in cache with key '{ChannelCategoriesCacheKey}'.");

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distinct channel categories");
                throw;
            }
        }
    }
}
