# Documentation Complète du Projet Lecteur IPTV

## 1. Structure du Projet

Le projet Lecteur IPTV est organisé selon une architecture 3-tiers avec une séparation claire entre le frontend et le backend :

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
│   │   ├── useApi.js       # Gestion des appels API
│   │   ├── useChannels.js  # Gestion des chaînes
│   │   ├── useFavorites.js # Gestion des favoris
│   │   └── useVod.js       # Gestion des contenus VOD
│   ├── services/           # Services d'appel API
│   │   ├── authService.js  # Service d'authentification
│   │   ├── channelsService.js # Service pour les chaînes
│   │   ├── vodService.js   # Service pour les contenus VOD
│   │   ├── favoritesService.js # Service pour les favoris
│   │   └── historyService.js # Service pour l'historique
│   ├── utils/              # Utilitaires
│   │   ├── jwtUtils.js     # Utilitaires pour les tokens JWT
│   │   └── formatters.js   # Formatage des données
│   ├── layouts/            # Layouts de l'application
│   ├── pages/              # Pages de l'application
│   │   ├── live/           # Pages TV en direct
│   │   ├── vod/            # Pages VOD
│   │   ├── auth/           # Pages d'authentification
│   │   └── profile/        # Pages profil utilisateur
│   ├── public/             # Fichiers statiques
│   ├── nuxt.config.ts      # Configuration Nuxt
│   └── package.json        # Dépendances frontend
├── backend/                # API C#/.NET
│   ├── LecteurIptv.Backend/
│   │   ├── Controllers/    # Endpoints API
│   │   │   ├── ChannelsController.cs
│   │   │   ├── VodController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── FavoritesController.cs
│   │   │   └── HistoryController.cs
│   │   ├── Services/       # Logique métier
│   │   │   ├── ChannelsService.cs
│   │   │   ├── VodService.cs
│   │   │   ├── UserService.cs
│   │   │   ├── FavoritesService.cs
│   │   │   ├── HistoryService.cs
│   │   │   ├── JwtService.cs
│   │   │   ├── M3UParser.cs
│   │   │   └── StreamingService.cs
│   │   ├── Models/         # Modèles de données
│   │   │   ├── Channel.cs
│   │   │   ├── VodItem.cs
│   │   │   ├── User.cs
│   │   │   ├── UserFavoriteChannel.cs
│   │   │   ├── UserFavoriteVod.cs
│   │   │   ├── UserHistory.cs
│   │   │   ├── TvProgram.cs
│   │   │   └── BaseEntity.cs
│   │   ├── Data/           # Accès aux données
│   │   │   ├── AppDbContext.cs
│   │   │   └── DbInitializer.cs
│   │   ├── Extensions/     # Extensions
│   │   │   └── ServiceCollectionExtensions.cs
│   │   ├── Program.cs      # Point d'entrée de l'application
│   │   └── appsettings.json # Configuration
│   └── LecteurIptv.Backend.Tests/ # Tests unitaires
│       ├── Services/       # Tests des services
│       │   ├── ChannelsServiceTests.cs
│       │   ├── VodServiceTests.cs
│       │   ├── UserServiceTests.cs
│       │   ├── FavoritesServiceTests.cs
│       │   ├── HistoryServiceTests.cs
│       │   ├── JwtServiceTests.cs
│       │   ├── M3UParserTests.cs
│       │   └── StreamingServiceTests.cs
│       └── Helpers/        # Utilitaires pour les tests
├── data/                   # Données et utilitaires
│   ├── samples/            # Playlists M3U d'exemple
│   └── ffmpeg/             # Scripts de conversion
└── docs/                   # Documentation
    ├── ARCHITECTURE.md     # Description de l'architecture
    ├── WORKFLOW.md         # Processus de développement
    └── test-plan.md        # Plan de test
```

## 2. Technologies Précises Utilisées

### 2.1 Frontend

| Technologie | Version | Description |
|-------------|---------|-------------|
| Vue.js | 3.5.13 | Framework JavaScript progressif pour construire des interfaces utilisateur |
| Nuxt.js | 3.17.1 | Framework Vue.js pour créer des applications universelles |
| TailwindCSS | 6.14.0 | Framework CSS utilitaire pour le styling |
| Axios | 1.9.0 | Client HTTP basé sur les promesses pour effectuer des requêtes API |
| Pinia | 3.0.2 | Gestionnaire d'état pour Vue.js |
| Video.js | 8.22.0 | Lecteur vidéo HTML5 avec support pour HLS |
| HLS.js | 1.6.2 | Bibliothèque pour la lecture de flux HLS |
| Vuelidate | 2.0.3 | Validation de formulaires pour Vue.js |
| Lodash-es | 4.17.21 | Bibliothèque d'utilitaires JavaScript |

### 2.2 Backend

| Technologie | Version | Description |
|-------------|---------|-------------|
| ASP.NET Core | 8.0 | Framework pour construire des applications web et des API |
| C# | 12 | Langage de programmation orienté objet |
| Entity Framework Core | 8.0 | ORM (Object-Relational Mapper) pour .NET |
| SQLite | 3 | Base de données légère et autonome |
| xUnit | 2.4.2 | Framework de test unitaire pour .NET |
| Moq | 4.18.4 | Framework de mocking pour les tests unitaires |
| JWT | - | JSON Web Tokens pour l'authentification |
| Swagger/OpenAPI | - | Documentation et test des API |

## 3. Fonctionnalités Implémentées

Le projet Lecteur IPTV implémente un ensemble complet de fonctionnalités pour la gestion et la lecture de contenus IPTV (chaînes en direct et VOD) :

### 3.1 Backend

#### 3.1.1 Modèles de Données
- **Channel** : Représente une chaîne de télévision avec ses propriétés (nom, URL du flux, logo, groupe, catégorie, etc.)
- **VodItem** : Représente un élément VOD (film, série) avec ses propriétés (titre, description, URL du flux, image, catégorie, etc.)
- **User** : Représente un utilisateur de l'application avec ses informations d'authentification et son profil
- **UserFavoriteChannel** : Relation entre un utilisateur et ses chaînes favorites
- **UserFavoriteVod** : Relation entre un utilisateur et ses éléments VOD favoris
- **UserHistory** : Historique de visionnage d'un utilisateur
- **TvProgram** : Programme TV associé à une chaîne

#### 3.1.2 Services
- **ChannelsService** : Gestion des chaînes (CRUD, filtrage, recherche)
- **VodService** : Gestion des éléments VOD (CRUD, filtrage, recherche)
- **UserService** : Gestion des utilisateurs (inscription, authentification, profil)
- **FavoritesService** : Gestion des favoris (ajout, suppression, liste)
- **HistoryService** : Gestion de l'historique de visionnage (ajout, suppression, liste)
- **JwtService** : Gestion des tokens JWT pour l'authentification
- **M3UParser** : Parsing des fichiers M3U/M3U8
- **StreamingService** : Gestion des flux vidéo
- **XmltvParser** : Parsing des fichiers XMLTV pour les données EPG

#### 3.1.3 Contrôleurs API
- **ChannelsController** : Endpoints pour les chaînes
- **VodController** : Endpoints pour les éléments VOD
- **UsersController** : Endpoints pour les utilisateurs
- **FavoritesController** : Endpoints pour les favoris
- **HistoryController** : Endpoints pour l'historique

#### 3.1.4 Tests Unitaires
- Tests pour tous les services principaux
- Utilisation de Moq pour simuler les dépendances
- Base de données en mémoire pour les tests d'intégration
- Génération de données de test

### 3.2 Frontend

#### 3.2.1 Composables
- **useAuth** : Gestion de l'authentification (connexion, inscription, déconnexion, rafraîchissement du token)
- **useApi** : Gestion des appels API avec Axios
- **useChannels** : Gestion des chaînes (liste, filtrage, recherche)
- **useFavorites** : Gestion des favoris (ajout, suppression, liste)
- **useVod** : Gestion des éléments VOD (liste, filtrage, recherche)

#### 3.2.2 Services
- **authService** : Service d'authentification
- **channelsService** : Service pour les chaînes
- **vodService** : Service pour les contenus VOD
- **favoritesService** : Service pour les favoris
- **historyService** : Service pour l'historique

#### 3.2.3 Composants
- **VideoPlayer** : Lecteur vidéo personnalisé avec support HLS
- **ChannelCard** : Carte pour afficher une chaîne
- **VodCard** : Carte pour afficher un élément VOD
- **FavoriteButton** : Bouton pour ajouter/supprimer des favoris
- **Pagination** : Composant de pagination pour les listes

#### 3.2.4 Pages
- **live/index.vue** : Liste des chaînes
- **live/[id].vue** : Détail d'une chaîne
- **vod/index.vue** : Liste des éléments VOD
- **vod/[id].vue** : Détail d'un élément VOD
- **auth/login.vue** : Page de connexion
- **auth/register.vue** : Page d'inscription
- **profile/index.vue** : Profil utilisateur
- **profile/favorites.vue** : Favoris de l'utilisateur
- **profile/history.vue** : Historique de visionnage

## 4. Flux de Données et Communication

### 4.1 Flux Frontend-Backend

1. **Authentification**
   - Le frontend envoie les identifiants utilisateur au backend via `/api/Users/login`
   - Le backend vérifie les identifiants et génère un token JWT
   - Le frontend stocke le token et l'utilise pour les requêtes ultérieures

2. **Récupération des Chaînes**
   - Le frontend demande la liste des chaînes au backend via `/api/Channels`
   - Le backend récupère les chaînes depuis la base de données
   - Le frontend affiche les chaînes et permet le filtrage/la recherche côté client

3. **Lecture d'une Chaîne**
   - L'utilisateur sélectionne une chaîne
   - Le frontend récupère les détails de la chaîne via `/api/Channels/{id}`
   - Le lecteur vidéo charge le flux HLS directement depuis l'URL du flux
   - Le backend enregistre l'événement dans l'historique via `/api/History`

4. **Gestion des Favoris**
   - L'utilisateur ajoute/supprime un favori via l'interface
   - Le frontend envoie la requête au backend via `/api/Favorites`
   - Le backend met à jour la base de données
   - Le frontend met à jour l'interface utilisateur

### 4.2 Stockage des Données

1. **Base de Données SQLite**
   - Stockage persistant des données (chaînes, VOD, utilisateurs, favoris, historique)
   - Migrations Entity Framework Core pour la gestion du schéma
   - Initialisation avec des données de test en développement

2. **Stockage Local Frontend**
   - Token JWT pour l'authentification
   - Informations utilisateur pour éviter des requêtes répétées
   - Préférences utilisateur (volume, qualité, etc.)

## 5. Sécurité

### 5.1 Authentification
- Utilisation de JWT (JSON Web Tokens) pour l'authentification
- Stockage sécurisé des mots de passe avec hachage et sel
- Rafraîchissement automatique des tokens avant expiration
- Déconnexion automatique en cas d'expiration du token

### 5.2 Autorisation
- Contrôle d'accès basé sur les rôles (utilisateur, administrateur)
- Protection des routes frontend avec middleware d'authentification
- Vérification des permissions côté backend pour chaque requête

### 5.3 Sécurité des Données
- Validation des entrées utilisateur côté frontend et backend
- Protection contre les injections SQL avec Entity Framework Core
- CORS configuré pour limiter les origines autorisées

## 6. Performance et Optimisation

### 6.1 Backend
- Mise en cache en mémoire pour les données statiques
- Pagination des résultats pour les grandes collections
- Tracking désactivé par défaut dans Entity Framework Core
- Indexation des colonnes fréquemment utilisées

### 6.2 Frontend
- Chargement paresseux des composants
- Mise en cache des résultats d'API
- Optimisation des requêtes avec pagination et filtrage côté serveur
- Utilisation de Tailwind CSS pour un CSS minimal

## 7. Tests

### 7.1 Tests Unitaires Backend
- Tests pour tous les services principaux avec xUnit
- Utilisation de Moq pour simuler les dépendances
- Base de données en mémoire pour les tests d'intégration
- Tests de validation des modèles
- Tests des méthodes CRUD pour chaque service
- Tests de la logique métier spécifique

### 7.2 Tests Manuels
- Tests fonctionnels pour vérifier le comportement de l'application
- Tests de compatibilité sur différents navigateurs
- Tests de performance pour les grandes collections de données
- Tests d'utilisabilité pour l'interface utilisateur

## 8. Déploiement

### 8.1 Prérequis
- .NET SDK 8.0+ pour le backend
- Node.js 18+ et npm pour le frontend
- Serveur web (Nginx, Apache) pour le frontend en production
- Serveur d'application pour le backend (Kestrel, IIS)

### 8.2 Étapes de Déploiement
1. **Backend**
   - Compilation en mode Release
   - Publication sur le serveur d'application
   - Configuration des variables d'environnement
   - Mise en place des migrations de base de données

2. **Frontend**
   - Construction en mode Production
   - Déploiement des fichiers statiques sur le serveur web
   - Configuration des redirections pour le routage SPA

### 8.3 CI/CD
- Intégration continue avec GitHub Actions
- Tests automatisés à chaque push
- Déploiement automatique en environnement de test
- Déploiement manuel en production

## 9. Prochaines Étapes

### 9.1 Fonctionnalités Planifiées
- Implémentation complète du guide des programmes (EPG)
- Support pour l'enregistrement des flux
- Système de recommandation basé sur l'historique
- Support multi-langues (i18n)
- Amélioration de l'accessibilité (ARIA)

### 9.2 Améliorations Techniques
- Mise en place de tests automatisés pour le frontend
- Optimisation des performances de lecture vidéo
- Mise en cache avancée pour les données fréquemment utilisées
- Support pour les sous-titres et le multi-audio
- Implémentation de WebSockets pour les notifications en temps réel

## 10. Conclusion

Le projet Lecteur IPTV est une application complète pour la gestion et la lecture de contenus IPTV, avec une architecture moderne et évolutive. Le frontend offre une expérience utilisateur riche et intuitive, tandis que le backend fournit une API robuste et performante.

La structure du projet est conçue pour être modulaire et facilement extensible, avec une couverture de tests complète pour assurer la qualité et la fiabilité du code. Les fonctionnalités implémentées couvrent les besoins essentiels d'une application IPTV, avec des plans d'évolution pour ajouter des fonctionnalités avancées à l'avenir.
