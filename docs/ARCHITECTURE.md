# Architecture du Lecteur IPTV

Ce document décrit l'architecture technique du projet Lecteur IPTV.

## Vue d'ensemble

L'application est divisée en deux parties principales :

1. **Frontend** : Application Vue/Nuxt pour l'interface utilisateur
2. **Backend** : API C#/.NET pour la gestion des flux IPTV

## Architecture Frontend

Le frontend est construit avec Nuxt.js 3, un framework basé sur Vue.js qui offre une expérience de développement améliorée et des performances optimisées.

### Composants clés

- **Composables** : Logique réutilisable encapsulée dans des fonctions composables Vue 3
  - `useM3U.ts` : Gestion du parsing et de l'affichage des playlists M3U
  - (à venir) `usePlayer.ts` : Contrôle du lecteur vidéo

- **Pages** :
  - Page d'accueil avec sélection de playlist
  - Page de lecture avec grille de chaînes et lecteur vidéo
  - Page de gestion des favoris

- **Composants UI** :
  - Grille de chaînes avec filtrage et recherche
  - Lecteur vidéo avec contrôles personnalisés
  - Navigation entre groupes de chaînes

## Architecture Backend

Le backend est développé avec ASP.NET Core, offrant une API RESTful pour la gestion des playlists IPTV.

### Composants clés

- **Contrôleurs** :
  - `M3UController` : Endpoints pour le parsing et la gestion des playlists

- **Services** :
  - `M3UParser` : Service de parsing des fichiers M3U
  - (à venir) `StreamingService` : Gestion des flux vidéo

- **Modèles** :
  - `Channel` : Représentation d'une chaîne IPTV
  - `Playlist` : Collection de chaînes avec métadonnées

## Communication Frontend-Backend

- Le frontend communique avec le backend via des requêtes HTTP/REST
- Les données sont échangées au format JSON
- CORS est configuré pour permettre les requêtes cross-origin en développement

## Gestion des données

- Les playlists M3U peuvent être chargées depuis :
  - Des URLs distantes
  - Des fichiers locaux
  - Des exemples préchargés

- Les favoris et préférences utilisateur sont stockés dans :
  - LocalStorage pour la persistance côté client
  - (future évolution) Base de données pour la synchronisation multi-appareils

## Sécurité

- Validation des URLs de flux pour éviter les injections
- Sanitization des données M3U avant parsing
- Limitation de débit pour les requêtes API

## Performance

- Mise en cache des playlists parsées
- Chargement différé des images de chaînes
- Préchargement des flux pour une lecture instantanée
