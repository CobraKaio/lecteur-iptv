# Documentation du Projet Lecteur IPTV Hybride

## Vue d'ensemble du projet

Ce projet est une application web IPTV hybride (Live + VOD) développée avec une architecture 3-tiers, utilisant Vue.js/Nuxt.js pour le frontend et C#/.NET pour le backend. L'application permet aux utilisateurs de regarder des chaînes TV en direct avec guide des programmes (EPG) et d'accéder à une bibliothèque de contenus à la demande (VOD).

## Architecture

L'application suit une architecture 3-tiers :
- **Couche Présentation** : Frontend Vue.js/Nuxt.js
- **Couche Traitement** : Backend C#/.NET
- **Couche Données** : Base de données SQLite avec Entity Framework Core

### Structure du projet

```
lecteur-iptv/
├── .github/                # Configuration GitHub (CI/CD, Dependabot)
├── frontend/               # Application Vue/Nuxt
│   ├── components/         # Composants réutilisables
│   │   ├── layout/         # Composants de mise en page
│   │   ├── player/         # Composants du lecteur vidéo
│   │   ├── live/           # Composants spécifiques à la TV en direct
│   │   └── vod/            # Composants spécifiques à la VOD
│   ├── composables/        # Logique réutilisable
│   │   ├── useAuth.js      # Gestion de l'authentification
│   │   ├── useChannels.js  # Gestion des chaînes
│   │   ├── useFavorites.js # Gestion des favoris
│   │   └── useVod.js       # Gestion des contenus VOD
│   ├── services/           # Services d'appel API
│   ├── layouts/            # Layouts de l'application
│   ├── pages/              # Pages de l'application
│   │   ├── live/           # Pages TV en direct
│   │   ├── vod/            # Pages VOD
│   │   └── profile/        # Pages profil utilisateur
│   └── ...                 # Autres fichiers frontend
├── backend/                # API C#/.NET
│   ├── Controllers/        # Endpoints API
│   ├── Services/           # Logique métier
│   ├── Models/             # Modèles de données
│   ├── Data/               # Accès aux données et migrations
│   └── LecteurIptv.Backend.Tests/ # Tests unitaires
├── data/                   # Données et utilitaires
│   ├── samples/            # Playlists M3U d'exemple
│   └── ffmpeg/             # Scripts de conversion
└── docs/                   # Documentation
```

## Technologies utilisées

### Frontend
- **Vue.js 3** avec Composition API
- **Nuxt.js 3** pour le SSR et le routing
- **JavaScript/TypeScript** pour la logique
- **TailwindCSS** pour le styling
- **Axios** pour les appels API
- **Video.js** et **HLS.js** pour la lecture vidéo

### Backend
- **ASP.NET Core 8**
- **C# 12**
- **Entity Framework Core** pour l'accès aux données
- **SQLite** comme base de données
- **JWT** pour l'authentification
- **xUnit, Moq** pour les tests unitaires

## Fonctionnalités implémentées

### Frontend

#### 1. Structure de base
- Layout principal avec header et footer
- Navigation entre les sections (Live TV, VOD, Profil)
- Design responsive avec TailwindCSS

#### 2. Authentification
- Pages de connexion et d'inscription
- Gestion des tokens JWT
- Protection des routes avec middleware

#### 3. Composant de lecteur vidéo
- Support des flux HLS
- Contrôles personnalisés (lecture/pause, volume, plein écran)
- Modes Live et VOD
- Gestion des erreurs

#### 4. Section Live TV
- Liste des chaînes avec filtrage par catégorie et recherche
- Affichage des détails d'une chaîne
- Gestion des favoris

#### 5. Section VOD
- Catalogue de contenus avec filtrage par catégorie et recherche
- Affichage des détails d'un contenu
- Gestion des favoris

#### 6. Profil utilisateur
- Gestion des favoris (chaînes Live et contenus VOD)
- Historique de visionnage

### Backend

#### 1. API RESTful
- Endpoints pour les chaînes, VOD, utilisateurs, favoris et historique
- Pagination des résultats
- Filtrage et recherche

#### 2. Services métier
- Parsing de fichiers M3U
- Gestion des flux vidéo
- Authentification et autorisation
- Gestion des favoris et de l'historique

#### 3. Modèles de données
- Utilisateurs, chaînes, programmes TV, contenus VOD
- Favoris et historique de visionnage

#### 4. Tests unitaires
- Tests pour tous les services principaux
- Utilisation de Moq pour les mocks
- Base de données en mémoire pour les tests

## Tests unitaires

Le projet inclut une suite complète de tests unitaires pour le backend, couvrant tous les services principaux :

### Services testés
- ✅ ChannelsService
- ✅ VodService
- ✅ StreamingService
- ✅ M3UParser
- ✅ UserService
- ✅ FavoritesService
- ✅ HistoryService
- ✅ JwtService

### Approche de test
- Utilisation de xUnit comme framework de test
- Moq pour simuler les dépendances
- Base de données en mémoire pour les tests d'intégration
- Génération de données de test

### Exemple de test (JwtServiceTests)

```csharp
[Fact]
public void GenerateToken_WithValidUser_ReturnsValidToken()
{
    // Arrange
    var user = new User
    {
        Id = 1,
        Username = "testuser",
        Email = "test@example.com",
        Role = "User"
    };

    // Act
    var token = _jwtService.GenerateToken(user);

    // Assert
    Assert.NotNull(token);
    Assert.NotEmpty(token);

    // Vérifier le contenu du token
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtToken = tokenHandler.ReadJwtToken(token);

    Assert.Equal(user.Id.ToString(), jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
    Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
}
```

### Exemple de test (HistoryServiceTests)

```csharp
[Fact]
public async Task LogViewAsync_WithNewEntry_CreatesNewHistoryEntry()
{
    // Arrange
    var userId = 1;
    var contentId = 3;
    var contentType = "vod";
    var durationSeconds = 300;

    // Act
    var result = await _historyService.LogViewAsync(userId, contentId, contentType, durationSeconds);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(userId, result.UserId);
    Assert.Equal(contentId, result.ContentId);
    Assert.Equal(contentType, result.ContentType);
    Assert.Equal(durationSeconds, result.DurationSeconds);

    // Vérifier que l'entrée a été ajoutée à la base de données
    var historyInDb = await _context.UserHistory.FindAsync(result.Id);
    Assert.NotNull(historyInDb);
}
```

## Prochaines étapes

### 1. Backend
- Implémenter le service EpgService et ses tests
- Développer des tests d'intégration pour les contrôleurs
- Optimiser les performances avec du caching

### 2. Frontend
- Implémenter la page de détail des chaînes Live
- Développer le guide des programmes (EPG)
- Mettre en place des tests unitaires pour les composables et services

### 3. Fonctionnalités avancées
- Implémentation de la recherche avancée
- Système de recommandations
- Support multi-langues (i18n)
- Accessibilité (ARIA)

### 4. Déploiement
- Configurer l'intégration continue (CI/CD)
- Mettre en place un environnement de production
- Optimiser les performances

## Comment exécuter le projet

### Prérequis
- Node.js 18+ et npm
- .NET SDK 8.0+
- Git

### Installation et démarrage du frontend

```bash
# Cloner le dépôt
git clone https://github.com/CobraKaio/lecteur-iptv.git
cd lecteur-iptv

# Installer les dépendances frontend
cd frontend
npm install

# Démarrer le serveur de développement
npm run dev
```

### Installation et démarrage du backend

```bash
# Dans un nouveau terminal, depuis la racine du projet
cd backend
dotnet restore
dotnet run --project LecteurIptv.Backend
```

### Exécution des tests

```bash
# Exécuter les tests backend
cd backend
dotnet test
```

## Conclusion

Ce projet est une application IPTV hybride complète avec une architecture moderne et évolutive. Le frontend est développé avec Vue.js/Nuxt.js et offre une expérience utilisateur riche pour la TV en direct et la VOD. Le backend est développé avec C#/.NET et fournit une API robuste et performante.

La structure du projet est conçue pour être modulaire et facilement extensible, avec une couverture de tests complète pour assurer la qualité et la fiabilité du code.
