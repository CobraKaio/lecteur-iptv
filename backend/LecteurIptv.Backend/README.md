# LecteurIptv.Backend

Ce projet est le backend de l'application LecteurIptv, une application web optimisée pour la lecture de flux IPTV.

## Architecture

L'architecture du projet suit le modèle 3-tiers :

1. **Couche de présentation** : Contrôleurs API (dossier `Controllers`)
2. **Couche métier** : Services (dossier `Services`)
3. **Couche de données** : Contexte de base de données et modèles (dossiers `Data` et `Models`)

## Base de données

Le projet utilise Entity Framework Core avec SQLite comme système de gestion de base de données. La base de données est créée et mise à jour à l'aide des migrations Entity Framework Core.

### Fichier de base de données

Le fichier de base de données SQLite est situé à la racine du projet et s'appelle `iptv.db`. Ce fichier est créé automatiquement lors de l'application des migrations.

### Migrations

Les migrations Entity Framework Core sont utilisées pour gérer l'évolution du schéma de la base de données. Les fichiers de migration sont situés dans le dossier `Migrations`.

Pour plus d'informations sur les migrations, consultez le fichier [README.md](Migrations/README.md) dans le dossier Migrations.

### Initialisation de la base de données

La base de données est initialisée avec des données de test en mode développement. Cette initialisation est effectuée par la méthode `InitializeDatabase` dans le fichier `Extensions/ServiceCollectionExtensions.cs`.

### Application des migrations au démarrage

En mode développement, les migrations sont appliquées automatiquement au démarrage de l'application. Cette fonctionnalité est configurée dans le fichier `Program.cs`.

## Modèles

Les modèles représentent les entités de l'application et sont situés dans le dossier `Models`. Chaque modèle correspond à une table dans la base de données.

### Modèles principaux

- **Channel** : Représente une chaîne de télévision.
- **TvProgram** : Représente un programme TV associé à une chaîne.
- **VodItem** : Représente un élément VOD (Video On Demand).
- **User** : Représente un utilisateur de l'application.
- **UserFavoriteChannel** : Représente une chaîne favorite d'un utilisateur.
- **UserFavoriteVod** : Représente un élément VOD favori d'un utilisateur.

## Services

Les services implémentent la logique métier de l'application et sont situés dans le dossier `Services`. Chaque service est défini par une interface et une implémentation.

### Services principaux

- **ChannelsService** : Gère les chaînes de télévision.
- **ProgramsService** : Gère les programmes TV.
- **VodService** : Gère les éléments VOD.
- **UserService** : Gère les utilisateurs et l'authentification.
- **FavoritesService** : Gère les favoris des utilisateurs.
- **M3UParser** : Parse les fichiers M3U.
- **StreamingService** : Gère les flux vidéo.

## Contrôleurs

Les contrôleurs exposent les fonctionnalités des services via des endpoints HTTP RESTful et sont situés dans le dossier `Controllers`.

### Contrôleurs principaux

- **ChannelsController** : Expose les fonctionnalités du service ChannelsService.
- **ProgramsController** : Expose les fonctionnalités du service ProgramsService.
- **VodController** : Expose les fonctionnalités du service VodService.
- **UsersController** : Expose les fonctionnalités du service UserService.
- **FavoritesController** : Expose les fonctionnalités du service FavoritesService.
- **M3UController** : Expose les fonctionnalités du service M3UParser.
- **StreamingController** : Expose les fonctionnalités du service StreamingService.

Pour plus d'informations sur les contrôleurs, consultez le fichier [README.md](Controllers/README.md) dans le dossier Controllers.

## Configuration

La configuration de l'application est définie dans les fichiers `appsettings.json` et `appsettings.Development.json`. Ces fichiers contiennent les paramètres de l'application, notamment la chaîne de connexion à la base de données.

## Démarrage

Pour démarrer l'application, exécutez la commande suivante à la racine du projet :

```bash
dotnet run
```

L'application sera accessible à l'adresse https://localhost:5001 ou http://localhost:5000.

## Développement

Pour développer l'application, vous pouvez utiliser Visual Studio, Visual Studio Code ou tout autre IDE compatible avec .NET.

### Prérequis

- .NET 8.0 SDK
- Entity Framework Core CLI (`dotnet ef`)

### Commandes utiles

- **Compiler le projet** : `dotnet build`
- **Exécuter le projet** : `dotnet run`
- **Créer une migration** : `dotnet ef migrations add NomDeLaMigration`
- **Appliquer les migrations** : `dotnet ef database update`
- **Supprimer la dernière migration** : `dotnet ef migrations remove`
- **Générer un script SQL** : `dotnet ef migrations script`

## Déploiement

Pour déployer l'application, vous pouvez utiliser différentes méthodes :

- **Déploiement sur IIS** : Publiez l'application et déployez-la sur un serveur IIS.
- **Déploiement sur Azure** : Publiez l'application et déployez-la sur Azure App Service.
- **Déploiement avec Docker** : Créez une image Docker et déployez-la sur un serveur Docker.

### Commandes de déploiement

- **Publier l'application** : `dotnet publish -c Release -o ./publish`
- **Créer une image Docker** : `docker build -t lecteuriptv-backend .`
- **Exécuter l'image Docker** : `docker run -p 8080:80 lecteuriptv-backend`

## Ressources

- [Documentation officielle de .NET](https://docs.microsoft.com/fr-fr/dotnet/)
- [Documentation officielle d'Entity Framework Core](https://docs.microsoft.com/fr-fr/ef/core/)
- [Documentation officielle d'ASP.NET Core](https://docs.microsoft.com/fr-fr/aspnet/core/)
