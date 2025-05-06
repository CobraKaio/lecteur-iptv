# Lecteur IPTV

Application web moderne pour la lecture de flux IPTV, développée à des fins personnelles de recherche et développement.

## Structure du projet

```
lecteur-iptv/
├── .github/                # Configuration GitHub (CI/CD, Dependabot)
├── frontend/               # Application Vue/Nuxt
│   ├── composables/        # Logique réutilisable (parsing M3U, etc.)
│   └── ...                 # Autres fichiers frontend
├── backend/                # API C#/.NET
│   ├── Controllers/        # Endpoints API
│   ├── Services/           # Logique métier
│   └── ...                 # Autres fichiers backend
├── data/                   # Données et utilitaires
│   ├── samples/            # Playlists M3U d'exemple
│   └── ffmpeg/             # Scripts de conversion
└── docs/                   # Documentation
```

## Technologies utilisées

### Frontend
- Vue.js 3 avec Composition API
- Nuxt.js 3 pour le SSR et le routing
- TypeScript pour la sécurité du typage
- TailwindCSS pour le styling

### Backend
- ASP.NET Core 8
- C# 12
- Entity Framework Core avec SQLite
- JWT pour l'authentification
- xUnit et Moq pour les tests unitaires

## Prérequis

- Node.js 18+ et npm pour le frontend
- .NET SDK 8.0+ pour le backend
- FFmpeg (optionnel, pour la conversion)
- Git

## Installation

### Cloner le dépôt

```bash
git clone https://github.com/CobraKaio/lecteur-iptv.git
cd lecteur-iptv
```

### Configuration du frontend

```bash
cd frontend
npm install
npm run dev
```

### Configuration du backend

```bash
cd backend
dotnet restore
dotnet run --project LecteurIptv.Backend
```

### Exécution des tests

```bash
cd backend
dotnet test
```

## Fonctionnalités

### Frontend
- ✅ Parsing de playlists M3U/M3U8
- ✅ Affichage des chaînes par groupes
- ✅ Lecture des flux vidéo HLS
- ✅ Authentification avec JWT
- ✅ Gestion des favoris
- ✅ Historique de visionnage
- 🔄 Recherche et filtrage avancés
- 🔜 Guide des programmes EPG
- 🔜 Enregistrement de flux

### Backend
- ✅ API RESTful pour chaînes, VOD, utilisateurs
- ✅ Authentification JWT
- ✅ Gestion des favoris et historique
- ✅ Tests unitaires pour les services
- 🔄 Pagination et filtrage
- 🔜 Parsing XMLTV pour EPG

## Documentation

- [Documentation Complète](documentation-complete.md) - Documentation exhaustive du projet (structure, technologies, fonctionnalités)
- [Documentation Projet](documentation-projet.md) - Documentation détaillée du projet
- [Architecture](docs/ARCHITECTURE.md) - Description technique de l'architecture
- [Workflow](docs/WORKFLOW.md) - Processus de développement
- [Test Plan](docs/test-plan.md) - Plan de test du projet

## Contribution

Ce projet est principalement développé à des fins personnelles, mais les suggestions et contributions sont les bienvenues via les issues et pull requests.

## Licence

Ce projet est développé à des fins personnelles de recherche et développement.
