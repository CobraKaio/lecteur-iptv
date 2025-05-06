# Contrôleurs API

Ce dossier contient les contrôleurs API de l'application LecteurIptv.Backend. Ces contrôleurs exposent les fonctionnalités des services métier via des endpoints HTTP RESTful.

## Architecture

L'architecture de l'application suit le modèle 3-tiers :

1. **Couche de présentation** : Contrôleurs API (dossier `Controllers`)
2. **Couche métier** : Services (dossier `Services`)
3. **Couche de données** : Contexte de base de données et modèles (dossiers `Data` et `Models`)

## Contrôleurs disponibles

### ChannelsController

Le contrôleur `ChannelsController` expose les fonctionnalités du service `IChannelsService` via les endpoints suivants :

- `GET /api/channels` : Récupère toutes les chaînes
- `GET /api/channels/{id}` : Récupère une chaîne par son identifiant
- `GET /api/channels/active` : Récupère les chaînes actives
- `GET /api/channels/category/{category}` : Récupère les chaînes par catégorie
- `GET /api/channels/search?query={query}` : Recherche des chaînes par nom
- `GET /api/channels/{id}/check` : Vérifie si une chaîne est disponible
- `GET /api/channels/groups` : Récupère les groupes de chaînes distincts
- `GET /api/channels/categories` : Récupère les catégories de chaînes distinctes
- `POST /api/channels` : Ajoute une nouvelle chaîne
- `POST /api/channels/import?url={url}` : Importe des chaînes à partir d'une playlist M3U
- `PUT /api/channels/{id}` : Met à jour une chaîne existante
- `DELETE /api/channels/{id}` : Supprime une chaîne

### ProgramsController

Le contrôleur `ProgramsController` expose les fonctionnalités du service `IProgramsService` via les endpoints suivants :

- `GET /api/programs` : Récupère tous les programmes TV
- `GET /api/programs/{id}` : Récupère un programme TV par son identifiant
- `GET /api/programs/channel/{channelId}` : Récupère les programmes TV pour une chaîne spécifique
- `GET /api/programs/timerange?startTime={startTime}&endTime={endTime}` : Récupère les programmes TV pour une période spécifique
- `GET /api/programs/channel/{channelId}/current` : Récupère le programme TV en cours pour une chaîne spécifique
- `GET /api/programs/channel/{channelId}/upcoming?count={count}` : Récupère les programmes TV à venir pour une chaîne spécifique
- `GET /api/programs/search?query={query}` : Recherche des programmes TV par titre ou description
- `GET /api/programs/categories` : Récupère les catégories de programmes TV distinctes
- `POST /api/programs` : Ajoute un nouveau programme TV
- `POST /api/programs/import?epgUrl={epgUrl}` : Importe des programmes TV à partir d'un fichier EPG
- `PUT /api/programs/{id}` : Met à jour un programme TV existant
- `DELETE /api/programs/{id}` : Supprime un programme TV

### VodController

Le contrôleur `VodController` expose les fonctionnalités du service `IVodService` via les endpoints suivants :

- `GET /api/vod` : Récupère tous les éléments VOD
- `GET /api/vod/{id}` : Récupère un élément VOD par son identifiant
- `GET /api/vod/category/{category}` : Récupère les éléments VOD par catégorie
- `GET /api/vod/search?query={query}` : Recherche des éléments VOD par titre ou description
- `GET /api/vod/{id}/check` : Vérifie si un élément VOD est disponible
- `GET /api/vod/categories` : Récupère les catégories d'éléments VOD distinctes
- `GET /api/vod/types` : Récupère les types d'éléments VOD distincts
- `POST /api/vod` : Ajoute un nouvel élément VOD
- `POST /api/vod/import?url={url}` : Importe des éléments VOD à partir d'une playlist M3U
- `PUT /api/vod/{id}` : Met à jour un élément VOD existant
- `DELETE /api/vod/{id}` : Supprime un élément VOD

### UsersController

Le contrôleur `UsersController` expose les fonctionnalités du service `IUserService` via les endpoints suivants :

- `GET /api/users` : Récupère tous les utilisateurs
- `GET /api/users/{id}` : Récupère un utilisateur par son identifiant
- `POST /api/users/login` : Authentifie un utilisateur
- `POST /api/users/register` : Crée un nouvel utilisateur
- `PUT /api/users/{id}` : Met à jour un utilisateur existant
- `PUT /api/users/{id}/password` : Met à jour le mot de passe d'un utilisateur
- `DELETE /api/users/{id}` : Supprime un utilisateur

### FavoritesController

Le contrôleur `FavoritesController` expose les fonctionnalités du service `IFavoritesService` via les endpoints suivants :

- `GET /api/favorites/channels/{userId}` : Récupère les chaînes favorites d'un utilisateur
- `GET /api/favorites/vods/{userId}` : Récupère les éléments VOD favoris d'un utilisateur
- `GET /api/favorites/channels/{userId}/{channelId}` : Vérifie si une chaîne est dans les favoris d'un utilisateur
- `GET /api/favorites/vods/{userId}/{vodItemId}` : Vérifie si un élément VOD est dans les favoris d'un utilisateur
- `POST /api/favorites/channels/{userId}/{channelId}` : Ajoute une chaîne aux favoris d'un utilisateur
- `POST /api/favorites/vods/{userId}/{vodItemId}` : Ajoute un élément VOD aux favoris d'un utilisateur
- `DELETE /api/favorites/channels/{userId}/{channelId}` : Supprime une chaîne des favoris d'un utilisateur
- `DELETE /api/favorites/vods/{userId}/{vodItemId}` : Supprime un élément VOD des favoris d'un utilisateur

### StreamingController

Le contrôleur `StreamingController` expose les fonctionnalités du service `IStreamingService` via les endpoints suivants :

- `GET /api/streaming/proxy?url={url}` : Proxy d'un flux vidéo

### M3UController

Le contrôleur `M3UController` expose les fonctionnalités du service `IM3UParser` via les endpoints suivants :

- `GET /api/m3u/parse?url={url}` : Parse un fichier M3U à partir d'une URL

## Modèles de requête et de réponse

### LoginModel

```json
{
  "username": "string",
  "password": "string"
}
```

### RegisterModel

```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string"
}
```

### PasswordModel

```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```

## Gestion des erreurs

Tous les contrôleurs gèrent les erreurs de manière cohérente :

- **400 Bad Request** : Requête invalide (par exemple, données manquantes ou invalides)
- **401 Unauthorized** : Authentification échouée
- **404 Not Found** : Ressource non trouvée
- **500 Internal Server Error** : Erreur interne du serveur

## Bonnes pratiques

- Utiliser les verbes HTTP appropriés (GET, POST, PUT, DELETE)
- Utiliser les codes de statut HTTP appropriés
- Utiliser les routes RESTful
- Utiliser les modèles de requête et de réponse
- Gérer les erreurs de manière cohérente
- Journaliser les erreurs
- Valider les entrées utilisateur
- Utiliser l'injection de dépendances
- Utiliser les attributs de routage et de validation
