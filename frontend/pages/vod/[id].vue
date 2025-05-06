<template>
  <div>
    <div class="mb-4">
      <NuxtLink to="/vod" class="text-blue-600 dark:text-blue-400 hover:underline flex items-center">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M9.707 16.707a1 1 0 01-1.414 0l-6-6a1 1 0 010-1.414l6-6a1 1 0 011.414 1.414L5.414 9H17a1 1 0 110 2H5.414l4.293 4.293a1 1 0 010 1.414z" clip-rule="evenodd" />
        </svg>
        Retour au catalogue
      </NuxtLink>
    </div>

    <!-- Afficher l'état de chargement -->
    <div v-if="loading">
      <LoadingSpinner size="lg" text="Chargement du contenu VOD..." />
    </div>

    <!-- Afficher l'erreur s'il y en a une -->
    <div v-else-if="error">
      <ErrorMessage
        :message="error"
        title="Erreur de chargement"
        :retry-action="loadVodItem"
        :is-retrying="isRetrying"
      />
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Informations sur le contenu -->
      <div class="lg:col-span-1">
        <div class="bg-white dark:bg-gray-700 rounded-lg shadow-md overflow-hidden">
          <div class="aspect-[2/3] bg-gray-300 dark:bg-gray-600 relative">
            <!-- Placeholder pour image -->
            <div class="flex items-center justify-center h-full">
              <span class="text-gray-500 dark:text-gray-400">Image</span>
            </div>
          </div>
          <div class="p-4">
            <h1 class="text-2xl font-bold mb-2">{{ contentTitle }}</h1>

            <div class="flex items-center mb-3">
              <span class="text-yellow-500 mr-1">★</span>
              <span class="text-gray-600 dark:text-gray-400">4.5/5</span>
              <span class="mx-2">•</span>
              <span class="text-gray-600 dark:text-gray-400">2023</span>
              <span class="mx-2">•</span>
              <span class="text-gray-600 dark:text-gray-400">120 min</span>
            </div>

            <div class="flex flex-wrap gap-2 mb-4">
              <span class="bg-gray-200 dark:bg-gray-600 px-2 py-1 rounded-full text-sm">Action</span>
              <span class="bg-gray-200 dark:bg-gray-600 px-2 py-1 rounded-full text-sm">Aventure</span>
              <span class="bg-gray-200 dark:bg-gray-600 px-2 py-1 rounded-full text-sm">Science-Fiction</span>
            </div>

            <p class="text-gray-700 dark:text-gray-300 mb-4">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nisl vel ultricies lacinia,
              nisl nisl aliquam nisl, vel aliquam nisl nisl vel nisl. Sed euismod, nisl vel ultricies lacinia,
              nisl nisl aliquam nisl, vel aliquam nisl nisl vel nisl.
            </p>

            <div class="flex items-center justify-between">
              <button class="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md transition-colors flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clip-rule="evenodd" />
                </svg>
                Regarder
              </button>

              <button class="text-gray-600 dark:text-gray-400 hover:text-purple-600 dark:hover:text-purple-400">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Lecteur et contenus similaires -->
      <div class="lg:col-span-2">
        <!-- Lecteur vidéo -->
        <div class="bg-black rounded-lg overflow-hidden mb-6 aspect-video flex items-center justify-center text-white">
          <p>Lecteur vidéo à implémenter</p>
          <!-- Ici viendra le composant de lecteur vidéo -->
        </div>

        <!-- Contenus similaires -->
        <div class="bg-white dark:bg-gray-700 rounded-lg shadow-md p-4">
          <h2 class="text-xl font-semibold mb-4">Contenus similaires</h2>
          <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
            <div
              v-for="i in 4"
              :key="i"
              class="bg-white dark:bg-gray-700 rounded-lg shadow overflow-hidden cursor-pointer hover:shadow-lg transition-shadow"
              @click="navigateToContent(contentId + i)"
            >
              <div class="aspect-[2/3] bg-gray-300 dark:bg-gray-600">
                <!-- Placeholder pour image -->
                <div class="flex items-center justify-center h-full">
                  <span class="text-gray-500 dark:text-gray-400">Image</span>
                </div>
              </div>
              <div class="p-3">
                <h3 class="font-medium">Contenu similaire {{ i }}</h3>
                <div class="flex items-center mt-1">
                  <span class="text-yellow-500 mr-1">★</span>
                  <span class="text-sm text-gray-600 dark:text-gray-400">{{ (Math.random() * 2 + 3).toFixed(1) }}/5</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useVod } from '~/composables/useVod';
import LoadingSpinner from '~/components/ui/LoadingSpinner.vue';
import ErrorMessage from '~/components/ui/ErrorMessage.vue';
import EmptyState from '~/components/ui/EmptyState.vue';

const route = useRoute();
const router = useRouter();

// Récupérer les composables
const { fetchVodItem } = useVod();

// Récupérer l'ID du contenu depuis l'URL
const contentId = parseInt(route.params.id as string);

// État local
const vodItem = ref(null);
const loading = ref(true);
const error = ref(null);
const isRetrying = ref(false);

// Dans une vraie application, on ferait un appel API pour récupérer les détails du contenu
// Pour l'instant, on simule des données
const contentTitle = computed(() => {
  if (vodItem.value) return vodItem.value.title;
  if (contentId > 200) return `Nouveau contenu ${contentId - 200}`;
  if (contentId > 100) return `Série ${contentId - 100}`;
  return `Film ${contentId}`;
});

// Fonction pour charger les détails du contenu VOD
const loadVodItem = async () => {
  loading.value = true;
  error.value = null;
  isRetrying.value = true;

  try {
    // Vérifier si l'ID est valide
    if (isNaN(contentId) || contentId <= 0) {
      error.value = "ID de contenu invalide dans l'URL.";
      return;
    }

    // Appeler l'API pour récupérer les détails du contenu
    const item = await fetchVodItem(contentId);

    if (!item) {
      error.value = `Contenu avec l'ID ${contentId} non trouvé.`;
    } else {
      vodItem.value = item;
    }
  } catch (err) {
    console.error(`Erreur lors du chargement du contenu VOD avec l'ID ${contentId}:`, err);
    error.value = "Une erreur est survenue lors du chargement du contenu.";
  } finally {
    loading.value = false;
    isRetrying.value = false;
  }
};

// Fonction pour naviguer vers un autre contenu
const navigateToContent = (id: number) => {
  router.push(`/vod/${id}`);
};

// Charger les détails du contenu au montage du composant
onMounted(() => {
  loadVodItem();
});
</script>
