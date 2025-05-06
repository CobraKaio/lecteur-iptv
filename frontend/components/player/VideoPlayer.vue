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

      <!-- Informations sur le flux (optionnel) -->
      <div v-if="showStreamInfo && streamData" class="absolute top-0 right-0 p-2 bg-black bg-opacity-70 text-white text-xs rounded-bl-md z-20">
        <div class="flex flex-col">
          <span>{{ streamData.streamType.toUpperCase() }}</span>
          <span v-if="streamData.resolution">{{ streamData.resolution }}</span>
          <span v-if="streamData.bitrate">{{ formatBitrate(streamData.bitrate) }}</span>
        </div>
      </div>

      <!-- Overlay de chargement -->
      <div v-if="isLoading" class="absolute inset-0 flex items-center justify-center bg-black bg-opacity-50 z-10">
        <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-white"></div>
      </div>

      <!-- Overlay des contrôles -->
      <div
        v-show="showControls || !isPlaying"
        class="absolute inset-0 flex flex-col justify-end bg-gradient-to-t from-black/70 to-transparent transition-opacity duration-300"
        @mouseenter="showControls = true"
        @mouseleave="hideControlsDebounced"
      >
        <!-- Titre (optionnel) -->
        <div v-if="title" class="absolute top-0 left-0 right-0 p-4 bg-gradient-to-b from-black/70 to-transparent">
          <h3 class="text-white text-lg font-medium">{{ title }}</h3>
          <p v-if="subtitle" class="text-gray-300 text-sm">{{ subtitle }}</p>
        </div>

        <!-- Bouton central de lecture/pause -->
        <div class="absolute inset-0 flex items-center justify-center" @click="togglePlay">
          <button v-if="!isPlaying" class="bg-white/20 hover:bg-white/30 rounded-full p-4 transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-white" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clip-rule="evenodd" />
            </svg>
          </button>
        </div>

        <!-- Barre de contrôles -->
        <div class="p-4 flex flex-col">
          <!-- Barre de progression -->
          <div class="relative w-full h-1 bg-gray-600 rounded-full mb-4 cursor-pointer" @click="onProgressBarClick">
            <div class="absolute top-0 left-0 h-full bg-blue-500 rounded-full" :style="{ width: `${progress}%` }"></div>
            <div
              class="absolute top-0 h-3 w-3 bg-white rounded-full -mt-1 transform -translate-x-1/2"
              :style="{ left: `${progress}%` }"
            ></div>
          </div>

          <!-- Contrôles -->
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-4">
              <!-- Bouton lecture/pause -->
              <button @click="togglePlay" class="text-white focus:outline-none">
                <svg v-if="isPlaying" xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </button>

              <!-- Contrôle du volume -->
              <div class="flex items-center space-x-2">
                <button @click="toggleMute" class="text-white focus:outline-none">
                  <svg v-if="isMuted || volume === 0" xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z" clip-rule="evenodd" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2" />
                  </svg>
                  <svg v-else-if="volume < 0.5" xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z" clip-rule="evenodd" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.536 8.464a5 5 0 010 7.072" />
                  </svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5.586 15H4a1 1 0 01-1-1v-4a1 1 0 011-1h1.586l4.707-4.707C10.923 3.663 12 4.109 12 5v14c0 .891-1.077 1.337-1.707.707L5.586 15z" clip-rule="evenodd" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.536 8.464a5 5 0 010 7.072M19.535 4.464a9 9 0 010 15.072" />
                  </svg>
                </button>
                <input
                  type="range"
                  min="0"
                  max="1"
                  step="0.01"
                  v-model="volume"
                  class="w-20 accent-blue-500"
                  @input="onVolumeChange"
                />
              </div>

              <!-- Temps -->
              <div class="text-white text-sm hidden sm:block">
                {{ formatTime(currentTime) }} / {{ formatTime(duration) }}
              </div>
            </div>

            <div class="flex items-center space-x-4">
              <!-- Qualité (pour le mode Live) -->
              <div v-if="mode === 'live'" class="relative">
                <button
                  @click="showQualityOptions = !showQualityOptions"
                  class="text-white text-sm focus:outline-none flex items-center"
                >
                  {{ currentQuality }}
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 ml-1" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd" />
                  </svg>
                </button>
                <div
                  v-if="showQualityOptions"
                  class="absolute bottom-full right-0 mb-2 bg-gray-900 rounded-md shadow-lg overflow-hidden z-20"
                >
                  <div class="py-1">
                    <button
                      v-for="quality in qualityOptions"
                      :key="quality"
                      @click="changeQuality(quality)"
                      class="block w-full px-4 py-2 text-sm text-white text-left hover:bg-gray-800"
                    >
                      {{ quality }}
                    </button>
                  </div>
                </div>
              </div>

              <!-- Bouton plein écran -->
              <button @click="toggleFullscreen" class="text-white focus:outline-none">
                <svg v-if="isFullscreen" xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 9L4 4m0 0l5 0m-5 0l0 5M9 15l-5 5m0 0l5 0m-5 0l0 -5M15 9l5 -5m0 0l-5 0m5 0l0 5M15 15l5 5m0 0l-5 0m5 0l0 -5" />
                </svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8V4m0 0h4M4 4l5 5m11-1V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5m11 5v-4m0 4h-4m4 0l-5-5" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch, computed } from 'vue';
import { useStreaming } from '~/composables/useStreaming';

// Importation conditionnelle de hls.js pour éviter les problèmes côté serveur
let Hls: any = null;
if (process.client) {
  // Importer hls.js uniquement côté client
  Hls = (await import('hls.js')).default;
}

// Props
const props = defineProps({
  src: {
    type: String,
    required: true
  },
  poster: {
    type: String,
    default: ''
  },
  title: {
    type: String,
    default: ''
  },
  subtitle: {
    type: String,
    default: ''
  },
  autoplay: {
    type: Boolean,
    default: false
  },
  mode: {
    type: String,
    default: 'vod', // 'vod' ou 'live'
    validator: (value: string) => ['vod', 'live'].includes(value)
  },
  showStreamInfo: {
    type: Boolean,
    default: false
  }
});

// Émissions d'événements
const emit = defineEmits(['play', 'pause', 'timeupdate', 'ended', 'error', 'qualitychange']);

// Références
const videoElement = ref<HTMLVideoElement | null>(null);
const playerContainer = ref<HTMLElement | null>(null);
const player = ref<any>(null);
const hls = ref<Hls | null>(null);

// Utiliser le service de streaming
const {
  getProxyUrl,
  getStreamInfo,
  checkStream,
  getStreamType,
  formatDuration,
  formatBitrate,
  streamInfo: streamData,
  loading: streamLoading,
  error: streamError
} = useStreaming();

// État du lecteur
const isPlaying = ref(false);
const isLoading = ref(true);
const isMuted = ref(false);
const volume = ref(1);
const currentTime = ref(0);
const duration = ref(0);
const progress = ref(0);
const showControls = ref(true);
const isFullscreen = ref(false);
const showQualityOptions = ref(false);
const currentQuality = ref('Auto');
const qualityOptions = ['Auto', '1080p', '720p', '480p', '360p'];

// Fonction pour formater le temps en MM:SS
const formatTime = (seconds: number) => {
  if (isNaN(seconds)) return '00:00';

  const mins = Math.floor(seconds / 60);
  const secs = Math.floor(seconds % 60);
  return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
};

// Gestionnaires d'événements
const onTimeUpdate = () => {
  if (!videoElement.value) return;

  currentTime.value = videoElement.value.currentTime;
  progress.value = (currentTime.value / duration.value) * 100 || 0;

  emit('timeupdate', {
    currentTime: currentTime.value,
    progress: progress.value
  });
};

const onLoadedMetadata = () => {
  if (!videoElement.value) return;

  duration.value = videoElement.value.duration;
  isLoading.value = false;
};

const onEnded = () => {
  isPlaying.value = false;
  emit('ended');
};

const onProgressBarClick = (event: MouseEvent) => {
  if (!videoElement.value || !event.currentTarget) return;

  const progressBar = event.currentTarget as HTMLElement;
  const rect = progressBar.getBoundingClientRect();
  const pos = (event.clientX - rect.left) / rect.width;

  videoElement.value.currentTime = pos * duration.value;
};

const onVolumeChange = () => {
  if (!videoElement.value) return;

  videoElement.value.volume = volume.value;
  isMuted.value = volume.value === 0;
};

// Actions du lecteur
const togglePlay = () => {
  if (!videoElement.value) return;

  if (isPlaying.value) {
    videoElement.value.pause();
    emit('pause');
  } else {
    videoElement.value.play();
    emit('play');
  }
};

const toggleMute = () => {
  if (!videoElement.value) return;

  if (isMuted.value) {
    isMuted.value = false;
    volume.value = videoElement.value.volume > 0 ? videoElement.value.volume : 0.5;
  } else {
    isMuted.value = true;
    volume.value = 0;
  }

  videoElement.value.volume = volume.value;
};

const toggleFullscreen = () => {
  if (!playerContainer.value) return;

  if (!isFullscreen.value) {
    if (playerContainer.value.requestFullscreen) {
      playerContainer.value.requestFullscreen();
    }
  } else {
    if (document.exitFullscreen) {
      document.exitFullscreen();
    }
  }
};

const changeQuality = (quality: string) => {
  currentQuality.value = quality;
  showQualityOptions.value = false;

  // Dans une vraie application, cela changerait la qualité du flux HLS
  emit('qualitychange', quality);
};

// Gestion des contrôles
const hideControlsDebounced = () => {
  if (isPlaying.value) {
    setTimeout(() => {
      showControls.value = false;
    }, 2000);
  }
};

// Initialisation du lecteur
const initPlayer = async () => {
  if (!videoElement.value) return;

  // Réinitialiser l'état
  isLoading.value = true;
  currentTime.value = 0;
  duration.value = 0;
  progress.value = 0;

  // Configurer le volume
  videoElement.value.volume = volume.value;
  videoElement.value.muted = isMuted.value;

  try {
    // Vérifier si le flux est disponible
    const isAvailable = await checkStream(props.src);

    if (!isAvailable) {
      console.error('Stream not available:', props.src);
      emit('error', { type: 'STREAM_NOT_AVAILABLE', message: 'Le flux n\'est pas disponible' });
      isLoading.value = false;
      return;
    }

    // Récupérer les informations sur le flux
    await getStreamInfo(props.src);

    // Obtenir l'URL du proxy
    const proxyUrl = getProxyUrl(props.src);

    // Déterminer le type de flux
    const streamType = getStreamType(props.src);

    // Vérifier si la source est un flux HLS
    if (streamType === 'hls') {
      if (Hls && Hls.isSupported()) {
        // Nettoyer l'instance HLS précédente si elle existe
        if (hls.value) {
          hls.value.destroy();
        }

        // Créer une nouvelle instance HLS
        hls.value = new Hls({
          enableWorker: true,
          lowLatencyMode: props.mode === 'live'
        });

        // Utiliser l'URL du proxy
        hls.value.loadSource(proxyUrl);
        hls.value.attachMedia(videoElement.value);

        hls.value.on(Hls.Events.MANIFEST_PARSED, () => {
          isLoading.value = false;

          if (props.autoplay) {
            videoElement.value?.play().catch(error => {
              console.error('Autoplay failed:', error);
            });
          }
        });

        hls.value.on(Hls.Events.ERROR, (event, data) => {
          console.error('HLS error:', data);
          emit('error', data);

          if (data.fatal) {
            switch (data.type) {
              case Hls.ErrorTypes.NETWORK_ERROR:
                // Essayer de récupérer
                hls.value?.startLoad();
                break;
              case Hls.ErrorTypes.MEDIA_ERROR:
                // Essayer de récupérer
                hls.value?.recoverMediaError();
                break;
              default:
                // Erreur fatale, détruire l'instance
                destroyPlayer();
                break;
            }
          }
        });
      } else if (videoElement.value && videoElement.value.canPlayType('application/vnd.apple.mpegurl')) {
        // Support natif HLS (Safari)
        videoElement.value.src = proxyUrl;
        if (props.autoplay) {
          videoElement.value.play().catch(error => {
            console.error('Autoplay failed:', error);
          });
        }
        isLoading.value = false;
      }
    } else if (streamType === 'dash' && window.dashjs) {
      // Utiliser dash.js pour les flux DASH
      const dashjs = window.dashjs;
      const dashPlayer = dashjs.MediaPlayer().create();

      dashPlayer.initialize(videoElement.value, proxyUrl, props.autoplay);
      dashPlayer.on('error', (e) => {
        console.error('DASH error:', e);
        emit('error', e);
      });

      dashPlayer.on('playbackMetaDataLoaded', () => {
        isLoading.value = false;
      });
    } else {
      // Source vidéo standard
      videoElement.value.src = proxyUrl;
      if (props.autoplay) {
        videoElement.value.play().catch(error => {
          console.error('Autoplay failed:', error);
        });
      }
      isLoading.value = false;
    }
  } catch (error) {
    console.error('Error initializing player:', error);
    emit('error', { type: 'INIT_ERROR', message: 'Erreur lors de l\'initialisation du lecteur', error });
    isLoading.value = false;
  }
};

// Nettoyage du lecteur
const destroyPlayer = () => {
  if (hls.value && typeof hls.value.destroy === 'function') {
    hls.value.destroy();
    hls.value = null;
  }

  if (videoElement.value) {
    videoElement.value.pause();
    videoElement.value.src = '';
    videoElement.value.load();
  }
};

// Gestionnaire d'événements pour le plein écran
const handleFullscreenChange = () => {
  isFullscreen.value = !!document.fullscreenElement;
};

// Cycle de vie du composant
onMounted(() => {
  // Initialiser le lecteur
  initPlayer();

  // Ajouter les écouteurs d'événements
  document.addEventListener('fullscreenchange', handleFullscreenChange);
});

onBeforeUnmount(() => {
  // Nettoyer le lecteur
  destroyPlayer();

  // Supprimer les écouteurs d'événements
  document.removeEventListener('fullscreenchange', handleFullscreenChange);
});

// Observer les changements de source
watch(() => props.src, (newSrc, oldSrc) => {
  if (newSrc !== oldSrc) {
    destroyPlayer();
    initPlayer();
  }
});
</script>

<style scoped>
.video-player-container {
  width: 100%;
}

.video-player-container.fullscreen {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9999;
  background: black;
}

.video-container {
  overflow: hidden;
}

/* Masquer les contrôles natifs */
video::-webkit-media-controls {
  display: none !important;
}
</style>
