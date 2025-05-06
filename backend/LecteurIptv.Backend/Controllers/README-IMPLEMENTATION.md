# Guide d'implémentation des contrôleurs API RESTful

Ce guide explique comment implémenter des contrôleurs API RESTful dans une application ASP.NET Core, en prenant comme exemple le `ChannelsController`.

## Structure d'un contrôleur API

Un contrôleur API est une classe qui hérite de `ControllerBase` et qui est décorée avec les attributs `[ApiController]` et `[Route]`. Voici sa structure de base :

```csharp
[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly ILogger<ChannelsController> _logger;
    private readonly IChannelsService _channelsService;

    public ChannelsController(ILogger<ChannelsController> logger, IChannelsService channelsService)
    {
        _logger = logger;
        _channelsService = channelsService;
    }

    // Méthodes d'action...
}
```

## Méthodes d'action

Les méthodes d'action sont les méthodes du contrôleur qui répondent aux requêtes HTTP. Elles sont décorées avec des attributs comme `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`, etc.

### Exemple de méthode GET pour récupérer toutes les ressources

```csharp
/// <summary>
/// Obtient toutes les chaînes
/// </summary>
/// <returns>Liste des chaînes</returns>
[HttpGet]
public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
{
    try
    {
        var channels = await _channelsService.GetAllChannelsAsync();
        return Ok(channels);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting channels");
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting channels: {ex.Message}");
    }
}
```

### Exemple de méthode GET pour récupérer une ressource par ID

```csharp
/// <summary>
/// Obtient une chaîne par son identifiant
/// </summary>
/// <param name="id">Identifiant de la chaîne</param>
/// <returns>Chaîne</returns>
[HttpGet("{id}")]
public async Task<ActionResult<Channel>> GetChannel(int id)
{
    try
    {
        var channel = await _channelsService.GetChannelByIdAsync(id);

        if (channel == null)
        {
            return NotFound($"Channel with ID {id} not found");
        }

        return Ok(channel);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error getting channel with ID {id}");
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting channel: {ex.Message}");
    }
}
```

### Exemple de méthode GET avec paramètre de route

```csharp
/// <summary>
/// Obtient les chaînes par catégorie
/// </summary>
/// <param name="category">Catégorie</param>
/// <returns>Liste des chaînes</returns>
[HttpGet("category/{category}")]
public async Task<ActionResult<IEnumerable<Channel>>> GetChannelsByCategory(string category)
{
    try
    {
        var channels = await _channelsService.GetChannelsByCategoryAsync(category);
        return Ok(channels);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error getting channels by category: {category}");
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting channels: {ex.Message}");
    }
}
```

### Exemple de méthode GET avec paramètre de requête

```csharp
/// <summary>
/// Recherche des chaînes
/// </summary>
/// <param name="query">Requête de recherche</param>
/// <returns>Liste des chaînes</returns>
[HttpGet("search")]
public async Task<ActionResult<IEnumerable<Channel>>> SearchChannels([FromQuery] string query)
{
    try
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query is required");
        }

        var channels = await _channelsService.SearchChannelsAsync(query);
        return Ok(channels);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error searching channels with query: {query}");
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error searching channels: {ex.Message}");
    }
}
```

### Exemple de méthode POST pour créer une ressource

```csharp
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
```

### Exemple de méthode PUT pour mettre à jour une ressource

```csharp
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
```

### Exemple de méthode DELETE pour supprimer une ressource

```csharp
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
```

## Types de retour

Les méthodes d'action peuvent retourner différents types :

- `ActionResult<T>` : Permet de retourner soit un type `T`, soit un `IActionResult` (comme `Ok()`, `NotFound()`, etc.)
- `IActionResult` : Utilisé lorsque plusieurs types de retour sont possibles (comme `Ok()`, `NotFound()`, etc.)
- `T` : Utilisé lorsqu'un seul type de retour est possible

## Méthodes de retour courantes

- `Ok(object)` : Retourne un code 200 OK avec l'objet sérialisé en JSON
- `CreatedAtAction(string, object, object)` : Retourne un code 201 Created avec l'URL de la ressource créée
- `NoContent()` : Retourne un code 204 No Content
- `BadRequest(object)` : Retourne un code 400 Bad Request avec un message d'erreur
- `NotFound(object)` : Retourne un code 404 Not Found avec un message d'erreur
- `StatusCode(int, object)` : Retourne un code HTTP personnalisé avec un message

## Paramètres de méthode

Les paramètres de méthode peuvent être liés à différentes sources :

- `[FromRoute]` : Lie le paramètre à une valeur de route (par défaut pour les paramètres simples qui correspondent à un segment de route)
- `[FromQuery]` : Lie le paramètre à une valeur de chaîne de requête (par défaut pour les paramètres simples qui ne correspondent pas à un segment de route)
- `[FromBody]` : Lie le paramètre au corps de la requête (par défaut pour les types complexes)
- `[FromForm]` : Lie le paramètre à une valeur de formulaire
- `[FromHeader]` : Lie le paramètre à une valeur d'en-tête

## Gestion des erreurs

Toutes les méthodes d'action doivent gérer les erreurs de manière appropriée :

```csharp
try
{
    // Code qui peut générer une exception
}
catch (Exception ex)
{
    _logger.LogError(ex, "Message d'erreur");
    return StatusCode(StatusCodes.Status500InternalServerError, $"Message d'erreur: {ex.Message}");
}
```

## Validation

La validation peut être effectuée de plusieurs manières :

1. **Validation automatique** : Avec l'attribut `[ApiController]`, ASP.NET Core valide automatiquement les modèles et retourne un code 400 Bad Request si la validation échoue.

2. **Validation manuelle** : Vous pouvez vérifier `ModelState.IsValid` et retourner un code 400 Bad Request si la validation échoue.

3. **Validation personnalisée** : Vous pouvez effectuer des validations personnalisées dans le contrôleur ou le service.

```csharp
if (string.IsNullOrWhiteSpace(query))
{
    return BadRequest("Query is required");
}
```

## Conventions de nommage

Suivez les conventions RESTful pour nommer vos endpoints :

- `GET /api/channels` : Récupérer toutes les chaînes
- `GET /api/channels/{id}` : Récupérer une chaîne par son ID
- `POST /api/channels` : Créer une nouvelle chaîne
- `PUT /api/channels/{id}` : Mettre à jour une chaîne
- `DELETE /api/channels/{id}` : Supprimer une chaîne

Pour les actions qui ne correspondent pas à ces opérations CRUD standard, utilisez des noms descriptifs :

- `GET /api/channels/search?query={query}` : Rechercher des chaînes
- `GET /api/channels/category/{category}` : Récupérer les chaînes par catégorie
- `POST /api/channels/import?url={url}` : Importer des chaînes

## Documentation

Documentez vos API avec des commentaires XML :

```csharp
/// <summary>
/// Obtient toutes les chaînes
/// </summary>
/// <returns>Liste des chaînes</returns>
[HttpGet]
public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
{
    // ...
}
```

Ces commentaires sont utilisés par Swagger pour générer une documentation interactive de votre API.

## Bonnes pratiques

1. **Utilisez les verbes HTTP appropriés** : GET pour lire, POST pour créer, PUT pour mettre à jour, DELETE pour supprimer.

2. **Utilisez les codes de statut HTTP appropriés** : 200 OK, 201 Created, 204 No Content, 400 Bad Request, 404 Not Found, 500 Internal Server Error, etc.

3. **Validez les entrées** : Validez toujours les entrées utilisateur pour éviter les erreurs et les failles de sécurité.

4. **Gérez les erreurs** : Capturez et journalisez les exceptions, et retournez des réponses d'erreur appropriées.

5. **Utilisez l'injection de dépendances** : Injectez les services dans le constructeur du contrôleur.

6. **Suivez les conventions RESTful** : Utilisez des noms de ressources au pluriel, des verbes HTTP appropriés, etc.

7. **Documentez votre API** : Utilisez des commentaires XML pour documenter votre API.

8. **Testez votre API** : Écrivez des tests unitaires et d'intégration pour votre API.

9. **Utilisez des DTOs** : Utilisez des objets de transfert de données (DTOs) pour séparer les modèles de domaine des modèles d'API.

10. **Versionnez votre API** : Utilisez des versions pour éviter de casser les clients existants lorsque vous apportez des modifications.

## Exemple complet

Voir le fichier `ChannelsController.cs` pour un exemple complet d'implémentation d'un contrôleur API RESTful.
