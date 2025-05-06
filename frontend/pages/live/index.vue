<template>
  <div>
    <h1 class="text-3xl font-bold mb-6">Live TV</h1>

    <div v-if="loading">
      <LoadingSpinner size="lg" text="Chargement des chaînes..." />
    </div>

    <div v-else-if="error">
      <ErrorMessage
        :message="error"
        title="Erreur de chargement"
        :retry-action="fetchChannels"
        :is-retrying="isRetrying"
      />
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Sidebar avec liste des chaînes -->
      <div class="lg:col-span-1">
        <div class="bg-white dark:bg-gray-700 rounded-lg shadow-md p-4 mb-4">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-semibold">Chaînes</h2>
            <div class="relative">
              <SearchBar
                :initial-query="searchQuery"
                placeholder="Rechercher une chaîne..."
                @search="handleSearch"
                @clear="clearSearch"
              />
            </div>
          </div>

          <!-- Filtres par catégorie -->
          <div class="mb-4">
            <CategoryFilter
              :categories="categories"
              :initial-category="selectedCategory"
              all-categories-label="Toutes"
              @filter="handleCategoryFilter"
              @clear="clearCategoryFilter"
            />
          </div>

          <!-- Liste des chaînes -->
          <div class="space-y-2 max-h-[500px] overflow-y-auto pr-2">
            <div v-if="filteredChannels.length === 0" class="py-4">
              <EmptyState
                title="Aucune chaîne trouvée"
                :message="searchQuery
                  ? `Aucun résultat pour '${searchQuery}'${selectedCategory ? ` dans la catégorie '${selectedCategory}'` : ''}.`
                  : selectedCategory
                    ? `Aucune chaîne dans la catégorie '${selectedCategory}'.`
                    : 'Aucune chaîne disponible.'"
                action-text="Effacer les filtres"
                :action-handler="() => { clearSearch(); clearCategoryFilter(); }"
              >
                <template #icon>
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                  </svg>
                </template>
              </EmptyState>
            </div>

            <div
              v-for="channel in filteredChannels"
              :key="channel.id"
              @click="selectChannel(channel)"
              class="flex items-center p-2 rounded-md cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-600"
              :class="selectedChannel?.id === channel.id ? 'bg-blue-50 dark:bg-gray-600' : ''"
            >
              <div class="w-10 h-10 bg-gray-200 dark:bg-gray-600 rounded-md flex items-center justify-center mr-3 overflow-hidden">
                <img v-if="channel.logo" :src="channel.logo" :alt="channel.name" class="w-full h-full object-cover" />
                <span v-else>{{ channel.id }}</span>
              </div>
              <div>
                <h3 class="font-medium">{{ channel.name }}</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  {{ channel.category }}
                </p>
              </div>
              <button
                @click.stop="toggleFavorite(channel.id)"
                class="ml-auto text-gray-400 hover:text-yellow-500"
                :class="{ 'text-yellow-500': isFavorite('liveChannels', channel.id) }"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                </svg>
              </button>
            </div>
          </div>

          <!-- Contrôles de pagination -->
          <div class="mt-4">
            <PaginationControls
              :current-page="pagination.pageNumber"
              :page-size="pagination.pageSize"
              :total-items="pagination.totalCount"
              :total-pages="pagination.totalPages"
              :has-previous-page="pagination.hasPreviousPage"
              :has-next-page="pagination.hasNextPage"
              @previous="goToPreviousPage"
              @next="goToNextPage"
              @page-change="goToPage"
            />
          </div>
        </div>
      </div>

      <!-- Lecteur et EPG -->
      <div class="lg:col-span-2">
        <!-- Lecteur vidéo -->
        <div v-if="selectedChannel" class="mb-6">
          <VideoPlayer
            :src="selectedChannel.streamUrl"
            :title="selectedChannel.name"
            :subtitle="currentProgram?.title"
            mode="live"
            :autoplay="true"
          />
        </div>
        <div v-else class="bg-black rounded-lg overflow-hidden mb-6 aspect-video">
          <EmptyState
            title="Aucune chaîne sélectionnée"
            message="Sélectionnez une chaîne dans la liste pour commencer à regarder"
            action-text="Chaîne aléatoire"
            :action-handler="() => {
              const randomChannel = getRandomChannel();
              if (randomChannel) selectChannel(randomChannel);
            }"
          >
            <template #icon>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </template>
          </EmptyState>
        </div>

        <!-- Informations sur le programme en cours -->
        <div class="bg-white dark:bg-gray-700 rounded-lg shadow-md p-4 mb-6">
          <h2 class="text-xl font-semibold mb-2">Programme en cours</h2>
          <div v-if="currentProgram">
            <div class="flex justify-between mb-2">
              <span class="font-medium">{{ currentProgram.title }}</span>
              <span class="text-gray-600 dark:text-gray-400">
                {{ formatTime(currentProgram.startTime) }} - {{ formatTime(currentProgram.endTime) }}
              </span>
            </div>
            <p class="text-gray-700 dark:text-gray-300 mb-3">
              {{ currentProgram.description }}
            </p>
            <div class="w-full bg-gray-200 dark:bg-gray-600 rounded-full h-2.5">
              <div class="bg-blue-600 h-2.5 rounded-full" :style="{ width: `${programProgress}%` }"></div>
            </div>
          </div>
          <div v-else>
            <EmptyState
              title="Aucun programme en cours"
              message="Sélectionnez une chaîne pour voir le programme en cours"
              :title-class="'text-lg'"
            >
              <template #icon>
                <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </template>
            </EmptyState>
          </div>
        </div>

        <!-- Guide des programmes (EPG) -->
        <div class="bg-white dark:bg-gray-700 rounded-lg shadow-md p-4">
          <h2 class="text-xl font-semibold mb-4">Guide des programmes</h2>
          <div v-if="programs.length > 0" class="space-y-3">
            <div
              v-for="program in programs"
              :key="program.id"
              class="border-b border-gray-200 dark:border-gray-600 pb-3 last:border-0"
              :class="{ 'bg-blue-50 dark:bg-gray-600 rounded p-2': program.id === currentProgram?.id }"
            >
              <div class="flex justify-between mb-1">
                <span class="font-medium">{{ program.title }}</span>
                <span class="text-gray-600 dark:text-gray-400">
                  {{ formatTime(program.startTime) }} - {{ formatTime(program.endTime) }}
                </span>
              </div>
              <p class="text-sm text-gray-700 dark:text-gray-300">
                {{ program.description }}
              </p>
            </div>
          </div>
          <div v-else-if="selectedChannel" class="py-6">
            <LoadingSpinner size="md" text="Chargement des programmes..." />
          </div>
          <div v-else>
            <EmptyState
              title="Aucun programme sélectionné"
              message="Sélectionnez une chaîne pour voir le guide des programmes"
            >
              <template #icon>
                <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
              </template>
            </EmptyState>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { useChannels } from '~/composables/useChannels';
import { useStreaming } from '~/composables/useStreaming';
import { useRoute, useRouter } from 'vue-router';
import VideoPlayer from '~/components/player/VideoPlayer.vue';
import SearchBar from '~/components/common/SearchBar.vue';
import CategoryFilter from '~/components/common/CategoryFilter.vue';
import LoadingSpinner from '~/components/ui/LoadingSpinner.vue';
import ErrorMessage from '~/components/ui/ErrorMessage.vue';
import EmptyState from '~/components/ui/EmptyState.vue';
import PaginationControls from '~/components/common/PaginationControls.vue';

// Types
interface Channel {
  id: number;
  name: string;
  streamUrl: string;
  logo?: string;
  category?: string;
  group?: string;
}

interface Program {
  id: number;
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  channelId: number;
}

// Récupérer les composables
const {
  channels,
  loading,
  error,
  fetchChannels,
  fetchChannel,
  getRandomChannel,
  categories,
  filteredChannels,
  searchChannels,
  fetchChannelsByCategory,
  pagination,
  goToPreviousPage,
  goToNextPage,
  goToPage,
  changePageSize
} = useChannels();

const {
  getProxyUrl
} = useStreaming();

const router = useRouter();
const route = useRoute();

// Simuler les fonctionnalités de favoris en attendant l'implémentation du backend
const favorites = ref({
  liveChannels: [] as number[]
});

const isFavorite = (type: string, id: number) => {
  if (type === 'liveChannels') {
    return favorites.value.liveChannels.includes(id);
  }
  return false;
};

const addFavorite = async (type: string, id: number) => {
  if (type === 'liveChannels' && !favorites.value.liveChannels.includes(id)) {
    favorites.value.liveChannels.push(id);
  }
};

const removeFavorite = async (type: string, id: number) => {
  if (type === 'liveChannels') {
    favorites.value.liveChannels = favorites.value.liveChannels.filter(channelId => channelId !== id);
  }
};

// État local
const searchQuery = ref('');
const selectedCategory = ref('');
const selectedChannel = ref<Channel | null>(null);
const programs = ref<Program[]>([]);
const currentProgram = ref<Program | null>(null);
const programProgress = ref(0);
const isRetrying = ref(false);

// Fonctions pour gérer la recherche et le filtrage
const handleSearch = async (query) => {
  searchQuery.value = query;
  updateUrlParams();

  if (selectedCategory.value) {
    // Si une catégorie est sélectionnée, rechercher dans cette catégorie
    await fetchChannelsByCategory(selectedCategory.value, query, 1, pagination.value.pageSize);
  } else {
    // Sinon, rechercher dans toutes les chaînes
    await searchChannels(query, '', 1, pagination.value.pageSize);
  }
};

const handleCategoryFilter = async (category) => {
  selectedCategory.value = category;
  updateUrlParams();

  if (searchQuery.value) {
    // Si une recherche est active, filtrer les résultats de recherche par catégorie
    await fetchChannelsByCategory(category, searchQuery.value, 1, pagination.value.pageSize);
  } else {
    // Sinon, filtrer toutes les chaînes par catégorie
    await fetchChannelsByCategory(category, '', 1, pagination.value.pageSize);
  }
};

const clearSearch = async () => {
  searchQuery.value = '';
  updateUrlParams();

  if (selectedCategory.value) {
    await fetchChannelsByCategory(selectedCategory.value, '', 1, pagination.value.pageSize);
  } else {
    await fetchChannels(1, pagination.value.pageSize);
  }
};

const clearCategoryFilter = async () => {
  selectedCategory.value = '';
  updateUrlParams();

  if (searchQuery.value) {
    await searchChannels(searchQuery.value, '', 1, pagination.value.pageSize);
  } else {
    await fetchChannels(1, pagination.value.pageSize);
  }
};

const updateUrlParams = () => {
  const query = { ...route.query };

  if (searchQuery.value) {
    query.q = searchQuery.value;
  } else {
    delete query.q;
  }

  if (selectedCategory.value) {
    query.category = selectedCategory.value;
  } else {
    delete query.category;
  }

  router.replace({ query });
};

// Fonction pour sélectionner une chaîne
const selectChannel = async (channel: Channel) => {
  selectedChannel.value = channel;

  // Simuler le chargement des programmes de la chaîne
  // Dans une vraie application, cela ferait un appel API
  programs.value = generateMockPrograms(channel.id);

  // Déterminer le programme en cours
  updateCurrentProgram();
};

// Fonction pour générer des programmes fictifs
const generateMockPrograms = (channelId: number): Program[] => {
  const now = new Date();
  const programs: Program[] = [];

  // Générer 5 programmes (2 passés, 1 en cours, 2 futurs)
  for (let i = -2; i <= 2; i++) {
    const startTime = new Date(now);
    startTime.setHours(now.getHours() + i);
    startTime.setMinutes(0);
    startTime.setSeconds(0);

    const endTime = new Date(startTime);
    endTime.setHours(endTime.getHours() + 1);

    programs.push({
      id: channelId * 100 + (i + 3),
      title: `Programme ${i + 3}`,
      description: `Description du programme ${i + 3}. Ceci est un programme fictif généré pour la démonstration.`,
      startTime: startTime.toISOString(),
      endTime: endTime.toISOString(),
      channelId: channelId
    });
  }

  return programs;
};

// Fonction pour mettre à jour le programme en cours
const updateCurrentProgram = () => {
  if (programs.value.length === 0) return;

  const now = new Date();

  // Trouver le programme en cours
  currentProgram.value = programs.value.find(program => {
    const start = new Date(program.startTime);
    const end = new Date(program.endTime);
    return now >= start && now <= end;
  }) || programs.value[0];

  // Calculer la progression du programme
  if (currentProgram.value) {
    const start = new Date(currentProgram.value.startTime).getTime();
    const end = new Date(currentProgram.value.endTime).getTime();
    const current = now.getTime();

    programProgress.value = Math.min(100, Math.max(0, ((current - start) / (end - start)) * 100));
  }
};

// Fonction pour formater l'heure
const formatTime = (isoString: string) => {
  const date = new Date(isoString);
  return date.toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' });
};

// Fonction pour basculer un favori
const toggleFavorite = async (channelId: number) => {
  if (isFavorite('liveChannels', channelId)) {
    await removeFavorite('liveChannels', channelId);
  } else {
    await addFavorite('liveChannels', channelId);
  }
};

// Vérifier si on arrive sur la page avec le paramètre random=true

// Fonction pour charger les chaînes avec gestion de l'état de chargement
const loadChannels = async () => {
  isRetrying.value = true;
  try {
    // Charger les chaînes en fonction des filtres
    if (searchQuery.value && selectedCategory.value) {
      await fetchChannelsByCategory(selectedCategory.value, searchQuery.value, 1, pagination.value.pageSize);
    } else if (searchQuery.value) {
      await searchChannels(searchQuery.value, '', 1, pagination.value.pageSize);
    } else if (selectedCategory.value) {
      await fetchChannelsByCategory(selectedCategory.value, '', 1, pagination.value.pageSize);
    } else {
      await fetchChannels(1, pagination.value.pageSize);
    }
  } finally {
    isRetrying.value = false;
  }
};

// Initialisation
onMounted(async () => {
  // Récupérer les paramètres d'URL
  if (route.query.q) {
    searchQuery.value = route.query.q as string;
  }

  if (route.query.category) {
    selectedCategory.value = route.query.category as string;
  }

  // Charger les chaînes
  await loadChannels();

  // Si le paramètre random est présent, sélectionner une chaîne aléatoire
  if (route.query.random === 'true') {
    const randomChannel = getRandomChannel();
    if (randomChannel) {
      selectChannel(randomChannel);
    }
  }

  // Mettre à jour le programme en cours toutes les minutes
  const intervalId = setInterval(updateCurrentProgram, 60000);

  // Nettoyer l'intervalle lors de la destruction du composant
  onBeforeUnmount(() => {
    clearInterval(intervalId);
  });
});
</script>
