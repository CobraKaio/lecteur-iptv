# Documentation du Projet Lecteur IPTV Hybride

## Vue d'ensemble du projet

Ce projet est une application web IPTV hybride (Live + VOD) développée avec une architecture 3-tiers, utilisant Vue.js/Nuxt.js pour le frontend et C#/.NET pour le backend. L'application permet aux utilisateurs de regarder des chaînes TV en direct avec guide des programmes (EPG) et d'accéder à une bibliothèque de contenus à la demande (VOD).

## Architecture

L'application suit une architecture 3-tiers :
- **Couche Présentation** : Frontend Vue.js/Nuxt.js
- **Couche Traitement** : Backend C#/.NET
- **Couche Données** : Base de données (à implémenter)

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
│   │   ├── api/            # Fonctions d'appel API
│   │   └── player/         # Logique du lecteur
│   ├── layouts/            # Layouts de l'application
│   ├── pages/              # Pages de l'application
│   │   ├── live/           # Pages TV en direct
│   │   ├── vod/            # Pages VOD
│   │   └── profile/        # Pages profil utilisateur
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
- **Vue.js 3** avec Composition API
- **Nuxt.js 3** pour le SSR et le routing
- **TypeScript** pour la sécurité du typage
- **TailwindCSS** pour le styling
- **Pinia** pour la gestion d'état
- **Video.js** et **HLS.js** pour la lecture vidéo

### Backend (à implémenter)
- **ASP.NET Core 7**
- **C# 11**
- **Entity Framework Core** (pour les futures fonctionnalités de persistance)

## Fonctionnalités implémentées

### Frontend

#### 1. Structure de base
- Layout principal avec header et footer
- Navigation entre les sections (Live TV, VOD, Profil)
- Design responsive

#### 2. Composant de lecteur vidéo
- Support des flux HLS
- Contrôles personnalisés (lecture/pause, volume, plein écran)
- Modes Live et VOD
- Gestion des erreurs

#### 3. Section Live TV
- Liste des chaînes avec filtrage par catégorie et recherche
- Affichage du programme en cours et de la progression
- Guide des programmes (EPG)
- Fonction "Chaîne aléatoire"
- Gestion des favoris

#### 4. Section VOD
- Catalogue de contenus avec filtrage par catégorie et recherche
- Affichage des détails d'un contenu
- Suggestions de contenus similaires
- Gestion des favoris

#### 5. Profil utilisateur
- Préférences utilisateur (langue, sous-titres, qualité)
- Gestion des favoris (chaînes Live et contenus VOD)

#### 6. Composables API
- Fonction générique pour les appels API
- Composables spécifiques pour les chaînes Live, le contenu VOD et le profil utilisateur
- Simulation de données pour le développement

## Code clé

### Composant de lecteur vidéo

Le composant `VideoPlayer.vue` est un lecteur vidéo personnalisé qui supporte les flux HLS et offre une interface utilisateur riche :

```vue
<template>
  <div class="video-player-container relative" :class="{ 'fullscreen': isFullscreen }">
    <div ref="playerContainer" class="video-container bg-black w-full aspect-video relative">
      <video
        ref="videoElement"
        class="w-full h-full"
        @timeupdate="onTimeUpdate"
        @loadedmetadata="onLoadedMetadata"
        @play="isPlaying = true"
        @pause="isPlaying = false"
        @ended="onEnded"
      ></video>
      
      <!-- Contrôles et overlays -->
      <!-- ... -->
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch } from 'vue';
import Hls from 'hls.js';

// Props, état, et logique du lecteur
// ...

// Initialisation du lecteur HLS
const initPlayer = () => {
  if (!videoElement.value) return;
  
  // Réinitialiser l'état
  isLoading.value = true;
  
  // Vérifier si la source est un flux HLS
  if (props.src.includes('.m3u8')) {
    if (Hls.isSupported()) {
      // Créer une nouvelle instance HLS
      hls.value = new Hls();
      hls.value.loadSource(props.src);
      hls.value.attachMedia(videoElement.value);
      
      // Gestion des événements HLS
      // ...
    }
  } else {
    // Source vidéo standard
    videoElement.value.src = props.src;
  }
};

// Cycle de vie du composant
onMounted(() => {
  initPlayer();
});

onBeforeUnmount(() => {
  destroyPlayer();
});
</script>
```

### Composable pour les chaînes Live

Le composable `useLiveChannels.ts` gère la logique des chaînes TV en direct :

```typescript
export function useLiveChannels() {
  const api = useApi();
  const channels = ref<Channel[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // Récupérer toutes les chaînes
  async function fetchChannels() {
    // Logique d'appel API ou simulation
    // ...
  }

  // Récupérer les programmes d'une chaîne
  async function fetchChannelPrograms(channelId: number) {
    // Logique d'appel API ou simulation
    // ...
  }

  // Récupérer une chaîne aléatoire
  async function getRandomChannel() {
    // Logique de sélection aléatoire
    // ...
  }

  // Autres fonctions utilitaires
  // ...

  return {
    channels,
    loading,
    error,
    fetchChannels,
    fetchChannelPrograms,
    getRandomChannel,
    filterChannelsByCategory,
    searchChannels
  };
}
```

## Prochaines étapes

### 1. Backend
- Développer l'API C#/.NET
- Implémenter le parsing des fichiers M3U
- Gérer les flux HLS/DASH
- Mettre en place la base de données

### 2. Frontend
- Finaliser l'intégration des composables avec toutes les pages
- Améliorer le lecteur vidéo avec des fonctionnalités avancées
- Implémenter la gestion des erreurs et les états de chargement

### 3. CMS
- Développer une interface d'administration
- Gérer les chaînes Live, le contenu VOD et les données EPG

### 4. Déploiement
- Configurer l'environnement de production
- Mettre en place un CDN pour la distribution des flux vidéo

## Comment exécuter le projet

### Prérequis
- Node.js 18+ et npm
- .NET SDK 7.0+ (pour le backend, à venir)

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

L'application sera accessible à l'adresse http://localhost:3000 (ou un autre port si 3000 est déjà utilisé).

## Conclusion

Ce projet est une application IPTV hybride complète avec une architecture moderne et évolutive. Le frontend est développé avec Vue.js/Nuxt.js et offre une expérience utilisateur riche pour la TV en direct et la VOD. Le backend sera développé avec C#/.NET pour fournir une API robuste et performante.

La structure du projet est conçue pour être modulaire et facilement extensible, permettant d'ajouter de nouvelles fonctionnalités à l'avenir.
