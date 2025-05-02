# Lecteur IPTV Prototype

Application web prototype pour la lecture de flux IPTV, développée à des fins personnelles de recherche et développement.

## Structure du projet

- `frontend/` : Application Vue/Nuxt pour l'interface utilisateur
- `backend/` : API C#/.NET pour la gestion des flux IPTV
  - `Data/` : Couche d'accès aux données (gestion des fichiers M3U, etc.)

## Prérequis

- Node.js et npm pour le frontend
- .NET SDK pour le backend
- Git

## Installation

### Cloner le dépôt

```bash
git clone [URL_DU_DEPOT]
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

## Fonctionnalités prévues

- Lecture de flux IPTV à partir de fichiers M3U
- Interface utilisateur intuitive pour la navigation dans les chaînes
- Gestion des favoris
- Recherche de chaînes

## Licence

Ce projet est développé à des fins personnelles de recherche et développement.
