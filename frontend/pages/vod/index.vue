<template>
  <div>
    <h1 class="text-3xl font-bold mb-6">Vidéo à la demande</h1>

    <!-- Filtres et recherche -->
    <FilterContainer
      :categories="categories"
      :initial-search-query="searchQuery"
      :initial-category="selectedCategory"
      :results-count="filteredVodItems.length"
      search-placeholder="Rechercher un film, une série..."
      all-categories-label="Toutes les catégories"
      @search="handleSearch"
      @filter="handleCategoryFilter"
      @clear-search="clearSearch"
      @clear-category="clearCategoryFilter"
      @clear-all="clearAllFilters"
    />

    <!-- Contenu VOD -->
    <div v-if="loading">
      <LoadingSpinner size="lg" text="Chargement des contenus VOD..." />
    </div>

    <div v-else-if="error">
      <ErrorMessage
        :message="error"
        title="Erreur de chargement"
        :retry-action="loadVodItems"
        :is-retrying="isRetrying"
      />
    </div>

    <div v-else-if="filteredVodItems.length === 0">
      <EmptyState
        title="Aucun contenu trouvé"
        :message="searchQuery
          ? `Aucun résultat pour '${searchQuery}'${selectedCategory ? ` dans la catégorie '${selectedCategory}'` : ''}.`
          : selectedCategory
            ? `Aucun contenu dans la catégorie '${selectedCategory}'.`
            : 'Aucun contenu disponible.'"
        action-text="Effacer les filtres"
        :action-handler="clearAllFilters"
      >
        <template #icon>
          <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 4v16M17 4v16M3 8h4m10 0h4M3 12h18M3 16h4m10 0h4M4 20h16a1 1 0 001-1V5a1 1 0 00-1-1H4a1 1 0 00-1 1v14a1 1 0 001 1z" />
          </svg>
        </template>
      </EmptyState>
    </div>

    <div v-else class="space-y-8">
      <!-- Résultats de recherche ou filtrage -->
      <section v-if="searchQuery || selectedCategory">
        <h2 class="text-2xl font-bold mb-4">
          {{ searchQuery && selectedCategory
            ? `Résultats pour "${searchQuery}" dans ${selectedCategory}`
            : searchQuery
              ? `Résultats pour "${searchQuery}"`
              : `Catégorie: ${selectedCategory}` }}
        </h2>
        <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
          <div
            v-for="item in filteredVodItems"
            :key="item.id"
            class="bg-white dark:bg-gray-700 rounded-lg shadow overflow-hidden cursor-pointer hover:shadow-lg transition-shadow"
            @click="navigateToContent(item.id)"
          >
            <div class="aspect-[2/3] bg-gray-300 dark:bg-gray-600 relative">
              <!-- Image du contenu -->
              <img
                v-if="item.posterUrl"
                :src="item.posterUrl"
                :alt="item.title"
                class="w-full h-full object-cover"
              />
              <div v-else class="flex items-center justify-center h-full">
                <span class="text-gray-500 dark:text-gray-400">Image</span>
              </div>

              <!-- Badge catégorie -->
              <div v-if="item.category" class="absolute top-2 right-2 bg-purple-600 text-white text-xs px-2 py-1 rounded-full">
                {{ item.category }}
              </div>
            </div>
            <div class="p-3">
              <h3 class="font-medium">{{ item.title || 'Sans titre' }}</h3>
              <div class="flex items-center mt-1">
                <span v-if="item.rating" class="text-yellow-500 mr-1">★</span>
                <span v-if="item.rating" class="text-sm text-gray-600 dark:text-gray-400">{{ item.rating }}/5</span>
                <span v-else class="text-sm text-gray-600 dark:text-gray-400">{{ item.year || '' }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Contrôles de pagination -->
        <div class="mt-6">
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
      </section>

      <!-- Affichage normal (sans recherche ni filtrage) -->
      <template v-else>
        <!-- Section Films populaires -->
        <section v-if="movies.length > 0">
          <h2 class="text-2xl font-bold mb-4">Films populaires</h2>
          <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
            <div
              v-for="movie in movies.slice(0, 10)"
              :key="movie.id"
              class="bg-white dark:bg-gray-700 rounded-lg shadow overflow-hidden cursor-pointer hover:shadow-lg transition-shadow"
              @click="navigateToContent(movie.id)"
            >
              <div class="aspect-[2/3] bg-gray-300 dark:bg-gray-600 relative">
                <!-- Image du film -->
                <img
                  v-if="movie.posterUrl"
                  :src="movie.posterUrl"
                  :alt="movie.title"
                  class="w-full h-full object-cover"
                />
                <div v-else class="flex items-center justify-center h-full">
                  <span class="text-gray-500 dark:text-gray-400">Image</span>
                </div>

                <!-- Badge catégorie -->
                <div v-if="movie.category" class="absolute top-2 right-2 bg-purple-600 text-white text-xs px-2 py-1 rounded-full">
                  {{ movie.category }}
                </div>
              </div>
              <div class="p-3">
                <h3 class="font-medium">{{ movie.title || 'Sans titre' }}</h3>
                <div class="flex items-center mt-1">
                  <span v-if="movie.rating" class="text-yellow-500 mr-1">★</span>
                  <span v-if="movie.rating" class="text-sm text-gray-600 dark:text-gray-400">{{ movie.rating }}/5</span>
                  <span v-else class="text-sm text-gray-600 dark:text-gray-400">{{ movie.year || '' }}</span>
                </div>
              </div>
            </div>
          </div>
        </section>

        <!-- Section Séries populaires -->
        <section v-if="series.length > 0">
          <h2 class="text-2xl font-bold mb-4">Séries populaires</h2>
          <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
            <div
              v-for="serie in series.slice(0, 5)"
              :key="serie.id"
              class="bg-white dark:bg-gray-700 rounded-lg shadow overflow-hidden cursor-pointer hover:shadow-lg transition-shadow"
              @click="navigateToContent(serie.id)"
            >
              <div class="aspect-[2/3] bg-gray-300 dark:bg-gray-600 relative">
                <!-- Image de la série -->
                <img
                  v-if="serie.posterUrl"
                  :src="serie.posterUrl"
                  :alt="serie.title"
                  class="w-full h-full object-cover"
                />
                <div v-else class="flex items-center justify-center h-full">
                  <span class="text-gray-500 dark:text-gray-400">Image</span>
                </div>

                <!-- Badge catégorie -->
                <div class="absolute top-2 right-2 bg-purple-600 text-white text-xs px-2 py-1 rounded-full">
                  Séries
                </div>
              </div>
              <div class="p-3">
                <h3 class="font-medium">{{ serie.title || 'Sans titre' }}</h3>
                <div class="flex items-center mt-1">
                  <span v-if="serie.rating" class="text-yellow-500 mr-1">★</span>
                  <span v-if="serie.rating" class="text-sm text-gray-600 dark:text-gray-400">{{ serie.rating }}/5</span>
                  <span v-else class="text-sm text-gray-600 dark:text-gray-400">{{ serie.year || '' }}</span>
                </div>
              </div>
            </div>
          </div>
        </section>

        <!-- Section Ajouts récents -->
        <section v-if="recentlyAdded.length > 0">
          <h2 class="text-2xl font-bold mb-4">Ajouts récents</h2>
          <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
            <div
              v-for="item in recentlyAdded.slice(0, 5)"
              :key="item.id"
              class="bg-white dark:bg-gray-700 rounded-lg shadow overflow-hidden cursor-pointer hover:shadow-lg transition-shadow"
              @click="navigateToContent(item.id)"
            >
              <div class="aspect-[2/3] bg-gray-300 dark:bg-gray-600 relative">
                <!-- Image du contenu -->
                <img
                  v-if="item.posterUrl"
                  :src="item.posterUrl"
                  :alt="item.title"
                  class="w-full h-full object-cover"
                />
                <div v-else class="flex items-center justify-center h-full">
                  <span class="text-gray-500 dark:text-gray-400">Image</span>
                </div>

                <!-- Badge catégorie -->
                <div v-if="item.category" class="absolute top-2 right-2 bg-purple-600 text-white text-xs px-2 py-1 rounded-full">
                  {{ item.category }}
                </div>

                <!-- Badge nouveau -->
                <div class="absolute top-2 left-2 bg-green-600 text-white text-xs px-2 py-1 rounded-full">
                  Nouveau
                </div>
              </div>
              <div class="p-3">
                <h3 class="font-medium">{{ item.title || 'Sans titre' }}</h3>
                <div class="flex items-center mt-1">
                  <span v-if="item.rating" class="text-yellow-500 mr-1">★</span>
                  <span v-if="item.rating" class="text-sm text-gray-600 dark:text-gray-400">{{ item.rating }}/5</span>
                  <span v-else class="text-sm text-gray-600 dark:text-gray-400">{{ item.year || '' }}</span>
                </div>
              </div>
            </div>
          </div>
        </section>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useVod } from '~/composables/useVod';
import FilterContainer from '~/components/common/FilterContainer.vue';
import LoadingSpinner from '~/components/ui/LoadingSpinner.vue';
import ErrorMessage from '~/components/ui/ErrorMessage.vue';
import EmptyState from '~/components/ui/EmptyState.vue';
import PaginationControls from '~/components/common/PaginationControls.vue';

const router = useRouter();
const route = useRoute();

// Récupérer les composables
const {
  vodItems,
  loading,
  error,
  categories,
  filteredVodItems,
  movies,
  series,
  recentlyAdded,
  fetchVodItems,
  fetchVodItemsByCategory,
  searchVodItems,
  pagination,
  goToPreviousPage,
  goToNextPage,
  goToPage,
  changePageSize
} = useVod();

// État local
const searchQuery = ref('');
const selectedCategory = ref('');
const isRetrying = ref(false);

// Fonctions pour gérer la recherche et le filtrage
const handleSearch = async (query) => {
  searchQuery.value = query;
  updateUrlParams();

  if (selectedCategory.value) {
    // Si une catégorie est sélectionnée, rechercher dans cette catégorie
    await fetchVodItemsByCategory(selectedCategory.value, query, 1, pagination.value.pageSize);
  } else {
    // Sinon, rechercher dans tous les contenus VOD
    await searchVodItems(query, '', 1, pagination.value.pageSize);
  }
};

const handleCategoryFilter = async (category) => {
  selectedCategory.value = category;
  updateUrlParams();

  if (searchQuery.value) {
    // Si une recherche est active, filtrer les résultats de recherche par catégorie
    await fetchVodItemsByCategory(category, searchQuery.value, 1, pagination.value.pageSize);
  } else {
    // Sinon, filtrer tous les contenus VOD par catégorie
    await fetchVodItemsByCategory(category, '', 1, pagination.value.pageSize);
  }
};

const clearSearch = async () => {
  searchQuery.value = '';
  updateUrlParams();

  if (selectedCategory.value) {
    await fetchVodItemsByCategory(selectedCategory.value, '', 1, pagination.value.pageSize);
  } else {
    await fetchVodItems(1, pagination.value.pageSize);
  }
};

const clearCategoryFilter = async () => {
  selectedCategory.value = '';
  updateUrlParams();

  if (searchQuery.value) {
    await searchVodItems(searchQuery.value, '', 1, pagination.value.pageSize);
  } else {
    await fetchVodItems(1, pagination.value.pageSize);
  }
};

const clearAllFilters = async () => {
  searchQuery.value = '';
  selectedCategory.value = '';
  updateUrlParams();
  await fetchVodItems(1, pagination.value.pageSize);
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

// Fonction pour naviguer vers la page de détail d'un contenu
const navigateToContent = (id: number) => {
  router.push(`/vod/${id}`);
};

// Fonction pour charger les contenus VOD avec gestion de l'état de chargement
const loadVodItems = async () => {
  isRetrying.value = true;
  try {
    // Charger les contenus VOD en fonction des filtres
    if (searchQuery.value && selectedCategory.value) {
      await fetchVodItemsByCategory(selectedCategory.value, searchQuery.value, 1, pagination.value.pageSize);
    } else if (searchQuery.value) {
      await searchVodItems(searchQuery.value, '', 1, pagination.value.pageSize);
    } else if (selectedCategory.value) {
      await fetchVodItemsByCategory(selectedCategory.value, '', 1, pagination.value.pageSize);
    } else {
      await fetchVodItems(1, pagination.value.pageSize);
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

  // Charger les contenus VOD
  await loadVodItems();
});
</script>
