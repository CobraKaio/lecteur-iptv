# Couche de Données (Data Layer)

Ce dossier contient les classes et les configurations liées à la couche de données de l'application LecteurIptv.Backend.

## AppDbContext

La classe `AppDbContext` est le pont entre les classes C# (Channel, TvProgram, etc.) et la base de données SQLite. Elle permet à Entity Framework Core de :

- Savoir quelles classes sont les modèles de données
- Gérer les connexions à la base de données
- Traduire les requêtes LINQ en requêtes SQL
- Suivre les changements sur les objets pour les sauvegarder en base de données
- Configurer le schéma de la base de données (relations, clés, types de colonnes)

### Fonctionnalités avancées

- **Comportement de suivi des entités** : Le mode `NoTracking` est activé par défaut pour améliorer les performances des requêtes en lecture seule.
- **Mise à jour automatique des dates** : Les dates de création et de modification sont automatiquement mises à jour lors de l'appel à `SaveChanges` ou `SaveChangesAsync`.
- **Configuration des relations** : Les relations entre les entités sont configurées dans la méthode `OnModelCreating`.
- **Index** : Des index sont définis pour améliorer les performances des requêtes.

## Extensions

Le dossier `Extensions` contient des méthodes d'extension pour le contexte de base de données :

- **GetActiveChannelsAsync** : Récupère les chaînes actives
- **GetChannelsByGroupAsync** : Récupère les chaînes par groupe
- **GetUpcomingProgramsAsync** : Récupère les programmes TV à venir pour une chaîne
- **GetCurrentProgramAsync** : Récupère le programme TV en cours pour une chaîne
- **GetUserFavoriteChannelsAsync** : Récupère les chaînes favorites d'un utilisateur
- **GetUserFavoriteVodsAsync** : Récupère les éléments VOD favoris d'un utilisateur
- **SearchChannelsAsync** : Recherche des chaînes par nom
- **SearchProgramsAsync** : Recherche des programmes TV par titre

## DbInitializer

La classe `DbInitializer` permet d'initialiser la base de données avec des données de test :

- Chaînes de télévision
- Programmes TV
- Éléments VOD
- Utilisateurs
- Chaînes favorites
- Éléments VOD favoris

## BaseEntity

La classe `BaseEntity` est une classe de base pour les entités avec des dates de création et de modification :

- **CreatedAt** : Date de création de l'entité
- **UpdatedAt** : Date de dernière mise à jour de l'entité

## Utilisation

### Configuration du contexte

```csharp
// Dans Program.cs
builder.Services.AddDatabaseServices(builder.Configuration);
```

### Initialisation de la base de données

```csharp
// Dans Program.cs
if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeDatabase();
}
```

### Utilisation des méthodes d'extension

```csharp
// Dans un contrôleur ou un service
public async Task<IActionResult> GetActiveChannels()
{
    var channels = await _context.GetActiveChannelsAsync();
    return Ok(channels);
}
```

### Utilisation du mode de suivi des entités

```csharp
// Pour les requêtes en lecture seule, aucune action supplémentaire n'est nécessaire
var channels = await _context.Channels.ToListAsync();

// Pour les requêtes nécessitant le suivi des entités (modifications, suppressions)
var channel = await _context.Channels.AsTracking().FirstOrDefaultAsync(c => c.Id == channelId);
channel.Name = "Nouveau nom";
await _context.SaveChangesAsync();
```
