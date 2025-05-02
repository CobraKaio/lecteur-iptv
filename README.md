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
- ASP.NET Core 7
- C# 11
- Entity Framework Core (pour les futures fonctionnalités de persistance)

## Prérequis

- Node.js 18+ et npm pour le frontend
- .NET SDK 7.0+ pour le backend
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
dotnet run
```

## Fonctionnalités

- ✅ Parsing de playlists M3U/M3U8
- ✅ Affichage des chaînes par groupes
- ✅ Lecture des flux vidéo
- 🔄 Recherche et filtrage des chaînes
- 🔄 Gestion des favoris
- 🔄 Conversion de formats avec FFmpeg
- 🔜 Guide des programmes EPG
- 🔜 Enregistrement de flux

## Documentation

- [Architecture](docs/ARCHITECTURE.md) - Description technique de l'architecture
- [Workflow](docs/WORKFLOW.md) - Processus de développement

## Contribution

Ce projet est principalement développé à des fins personnelles, mais les suggestions et contributions sont les bienvenues via les issues et pull requests.

## Licence

Ce projet est développé à des fins personnelles de recherche et développement.
