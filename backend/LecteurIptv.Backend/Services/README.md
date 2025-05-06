# Services métier

Ce dossier contient les services métier de l'application LecteurIptv.Backend. Ces services implémentent la logique métier de l'application et utilisent le contexte de base de données pour accéder aux données.

## Architecture

L'architecture de l'application suit le modèle 3-tiers :

1. **Couche de présentation** : Contrôleurs API (dossier `Controllers`)
2. **Couche métier** : Services (dossier `Services`)
3. **Couche de données** : Contexte de base de données et modèles (dossiers `Data` et `Models`)

## Services disponibles

### ChannelsService

Le service `ChannelsService` gère les chaînes de télévision. Il implémente l'interface `IChannelsService` et fournit les fonctionnalités suivantes :

- Récupération de toutes les chaînes
- Récupération des chaînes actives
- Récupération d'une chaîne par son identifiant
- Récupération des chaînes par groupe
- Récupération des chaînes par catégorie
- Recherche de chaînes par nom
- Ajout d'une nouvelle chaîne
- Mise à jour d'une chaîne existante
- Suppression d'une chaîne
- Importation de chaînes à partir d'une playlist M3U
- Vérification de la disponibilité d'une chaîne
- Récupération des groupes de chaînes distincts
- Récupération des catégories de chaînes distinctes

### ProgramsService

Le service `ProgramsService` gère les programmes TV. Il implémente l'interface `IProgramsService` et fournit les fonctionnalités suivantes :

- Récupération de tous les programmes TV
- Récupération d'un programme TV par son identifiant
- Récupération des programmes TV pour une chaîne spécifique
- Récupération des programmes TV pour une période spécifique
- Récupération du programme TV en cours pour une chaîne
- Récupération des programmes TV à venir pour une chaîne
- Recherche de programmes TV par titre ou description
- Ajout d'un nouveau programme TV
- Mise à jour d'un programme TV existant
- Suppression d'un programme TV
- Importation de programmes TV à partir d'un fichier EPG
- Récupération des catégories de programmes TV distinctes

### VodService

Le service `VodService` gère les éléments VOD (Video On Demand). Il implémente l'interface `IVodService` et fournit les fonctionnalités suivantes :

- Récupération de tous les éléments VOD
- Récupération des éléments VOD actifs
- Récupération d'un élément VOD par son identifiant
- Récupération des éléments VOD par catégorie
- Récupération des éléments VOD par type
- Recherche d'éléments VOD par titre ou description
- Ajout d'un nouvel élément VOD
- Mise à jour d'un élément VOD existant
- Suppression d'un élément VOD
- Vérification de la disponibilité d'un élément VOD
- Récupération des catégories d'éléments VOD distinctes
- Récupération des types d'éléments VOD distincts
- Importation d'éléments VOD à partir d'une playlist M3U

### UserService

Le service `UserService` gère les utilisateurs et l'authentification. Il implémente l'interface `IUserService` et fournit les fonctionnalités suivantes :

- Récupération de tous les utilisateurs
- Récupération d'un utilisateur par son identifiant
- Récupération d'un utilisateur par son nom d'utilisateur
- Récupération d'un utilisateur par son adresse e-mail
- Authentification d'un utilisateur
- Création d'un nouvel utilisateur
- Mise à jour d'un utilisateur existant
- Mise à jour du mot de passe d'un utilisateur
- Suppression d'un utilisateur
- Vérification de l'existence d'un nom d'utilisateur
- Vérification de l'existence d'une adresse e-mail

### FavoritesService

Le service `FavoritesService` gère les favoris des utilisateurs. Il implémente l'interface `IFavoritesService` et fournit les fonctionnalités suivantes :

- Récupération des chaînes favorites d'un utilisateur
- Récupération des éléments VOD favoris d'un utilisateur
- Ajout d'une chaîne aux favoris d'un utilisateur
- Ajout d'un élément VOD aux favoris d'un utilisateur
- Suppression d'une chaîne des favoris d'un utilisateur
- Suppression d'un élément VOD des favoris d'un utilisateur
- Vérification si une chaîne est dans les favoris d'un utilisateur
- Vérification si un élément VOD est dans les favoris d'un utilisateur

### StreamingService

Le service `StreamingService` gère les flux vidéo. Il implémente l'interface `IStreamingService` et fournit les fonctionnalités suivantes :

- Proxy d'un flux vidéo
- Vérification de la disponibilité d'un flux

### M3UParser

Le service `M3UParser` parse les fichiers M3U. Il implémente l'interface `IM3UParser` et fournit les fonctionnalités suivantes :

- Parsing d'un fichier M3U à partir d'une URL
- Parsing d'un fichier M3U à partir d'un chemin local

## Utilisation

### Injection de dépendances

Les services sont enregistrés pour l'injection de dépendances dans la méthode d'extension `AddApplicationServices` :

```csharp
// Dans Program.cs
builder.Services.AddApplicationServices();
```

### Utilisation dans un contrôleur

```csharp
public class ChannelsController : ControllerBase
{
    private readonly IChannelsService _channelsService;

    public ChannelsController(IChannelsService channelsService)
    {
        _channelsService = channelsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
    {
        var channels = await _channelsService.GetAllChannelsAsync();
        return Ok(channels);
    }
}
```

### Utilisation dans un autre service

```csharp
public class MyService : IMyService
{
    private readonly IChannelsService _channelsService;

    public MyService(IChannelsService channelsService)
    {
        _channelsService = channelsService;
    }

    public async Task<IEnumerable<Channel>> GetActiveChannelsAsync()
    {
        return await _channelsService.GetActiveChannelsAsync();
    }
}
```

## Bonnes pratiques pour l'implémentation des méthodes de service

### 1. Gestion des erreurs

Toutes les méthodes de service doivent gérer les erreurs de manière appropriée avec un bloc try-catch :

```csharp
public async Task<IEnumerable<Channel>> GetAllChannelsAsync()
{
    try
    {
        _logger.LogInformation("Récupération de toutes les chaînes");

        var channels = await _context.Channels
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        _logger.LogInformation($"Récupération réussie de {channels.Count} chaînes");
        return channels;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erreur lors de la récupération de toutes les chaînes");
        throw; // Relance l'exception pour qu'elle soit gérée par le contrôleur
    }
}
```

### 2. Journalisation

Utilisez le logger injecté pour journaliser les informations importantes :

- **LogInformation** : Pour les opérations normales
- **LogWarning** : Pour les situations anormales mais non critiques
- **LogError** : Pour les erreurs
- **LogDebug** : Pour les informations de débogage

```csharp
_logger.LogInformation($"Récupération de la chaîne avec l'ID {id}");
_logger.LogWarning($"Aucune chaîne trouvée avec l'ID {id}");
_logger.LogError(ex, $"Erreur lors de la récupération de la chaîne avec l'ID {id}");
_logger.LogDebug($"Exécution de la requête: {query.ToQueryString()}");
```

### 3. Validation des entrées

Validez toujours les entrées au début de la méthode :

```csharp
if (string.IsNullOrWhiteSpace(category))
{
    _logger.LogWarning("Tentative de récupération des chaînes avec une catégorie vide");
    return new List<Channel>();
}
```

### 4. Requêtes Entity Framework Core

Utilisez les méthodes asynchrones d'Entity Framework Core pour les opérations de base de données :

```csharp
// Construction de la requête
var query = _context.Channels
    .Where(c => c.IsActive && c.Category == category)
    .OrderBy(c => c.DisplayOrder)
    .ThenBy(c => c.Name);

// Exécution de la requête
var channels = await query.ToListAsync();
```

### 5. Méthodes d'extension

Utilisez des méthodes d'extension pour factoriser les requêtes courantes :

```csharp
// Dans le service
var channels = await _context.GetActiveChannelsAsync();

// Dans la classe d'extension
public static async Task<List<Channel>> GetActiveChannelsAsync(this AppDbContext context)
{
    return await context.Channels
        .Where(c => c.IsActive)
        .OrderBy(c => c.DisplayOrder)
        .ThenBy(c => c.Name)
        .ToListAsync();
}
```

### 6. Tracking des entités

Utilisez `AsTracking()` ou `AsNoTracking()` selon que vous avez besoin de modifier l'entité ou non :

```csharp
// Avec tracking pour modification ultérieure
var channel = await _context.Channels
    .AsTracking()
    .FirstOrDefaultAsync(c => c.Id == id);

// Sans tracking pour lecture seule (plus performant)
var channel = await _context.Channels
    .AsNoTracking()
    .FirstOrDefaultAsync(c => c.Id == id);
```

### 7. Retour des résultats

Retournez des interfaces plutôt que des implémentations concrètes :

```csharp
// Bon : Retourne une interface
public async Task<IEnumerable<Channel>> GetAllChannelsAsync()

// Moins bon : Retourne une implémentation concrète
public async Task<List<Channel>> GetAllChannelsAsync()
```

### 8. Documentation XML

Documentez toutes les méthodes avec des commentaires XML :

```csharp
/// <summary>
/// Récupère toutes les chaînes
/// </summary>
/// <returns>Liste des chaînes</returns>
public async Task<IEnumerable<Channel>> GetAllChannelsAsync()
{
    // ...
}
```

### 9. Autres bonnes pratiques générales

- Toujours utiliser l'interface (par exemple, `IChannelsService`) plutôt que l'implémentation concrète (par exemple, `ChannelsService`) pour l'injection de dépendances.
- Utiliser les méthodes asynchrones (`async`/`await`) pour les opérations de base de données.
- Gérer les exceptions et les journaliser.
- Valider les entrées utilisateur.
- Utiliser les méthodes d'extension du contexte de base de données pour les requêtes courantes.
- Utiliser le mode de suivi des entités (`AsTracking`) uniquement lorsque c'est nécessaire (pour les opérations de modification).
