<template>
  <div class="container mx-auto p-4">
    <h1 class="text-2xl font-bold mb-4">Mes Favoris</h1>

    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="mb-4 flex justify-between items-center">
        <h2 class="text-xl font-semibold">Chaînes favorites</h2>
        <button
          @click="fetchFavorites"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md transition-colors"
          :disabled="loading"
        >
          <span v-if="loading">Chargement...</span>
          <span v-else>Rafraîchir</span>
        </button>
      </div>

      <!-- Message d'erreur -->
      <div v-if="error" class="mb-4">
        <ErrorMessage
          :message="error"
          title="Erreur"
          :retry-action="fetchFavorites"
          :is-retrying="isRetrying"
        />
      </div>

      <!-- Liste des favoris -->
      <div v-if="favorites.length > 0" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
        <div
          v-for="favorite in favorites"
          :key="favorite.id"
          class="bg-gray-100 dark:bg-gray-700 rounded-lg p-4 flex items-center"
        >
          <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center mr-3">
            <svg class="w-6 h-6 text-blue-500" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
              <path d="M10 12a2 2 0 100-4 2 2 0 000 4z"></path>
              <path fill-rule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clip-rule="evenodd"></path>
            </svg>
          </div>
          <div class="flex-grow">
            <h3 class="font-medium">{{ favorite.name }}</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ favorite.category }}</p>
          </div>
          <button @click="removeFavorite(favorite.id)" class="text-gray-500 hover:text-red-500">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Message si aucun favori -->
      <div v-else-if="!loading">
        <EmptyState
          title="Aucun favori"
          message="Vous n'avez pas encore de chaînes favorites."
        >
          <template #icon>
            <svg class="h-16 w-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"></path>
            </svg>
          </template>
          <template #action>
            <NuxtLink to="/live" class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded transition-colors">
              Parcourir les chaînes
            </NuxtLink>
          </template>
        </EmptyState>
      </div>

      <!-- Loader -->
      <div v-else>
        <LoadingSpinner size="lg" text="Chargement de vos favoris..." />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useApi } from '~/composables/useApi';
import LoadingSpinner from '~/components/ui/LoadingSpinner.vue';
import ErrorMessage from '~/components/ui/ErrorMessage.vue';
import EmptyState from '~/components/ui/EmptyState.vue';

// IMPORTANT: Définir les métadonnées de la page
// Ceci indique à Nuxt d'appliquer le middleware 'auth' (nom du fichier sans '.ts')
// et de définir une propriété 'requiresAuth' à true pour que le middleware le sache.
definePageMeta({
  middleware: ['auth'], // Applique le middleware 'auth'
  requiresAuth: true // Marque cette route comme nécessitant une authentification
});

// État
const favorites = ref([]);
const loading = ref(false);
const error = ref(null);
const isRetrying = ref(false);

// Récupérer l'API
const { get, del } = useApi();

// Fonction pour récupérer les favoris
const fetchFavorites = async () => {
  loading.value = true;
  error.value = null;
  isRetrying.value = true;

  try {
    // Appel à l'API protégée qui nécessite un token JWT
    const response = await get('/Favorites/channels');
    favorites.value = response;
  } catch (err) {
    console.error('Error fetching favorites:', err);
    error.value = err.response?.data || err.message || 'Erreur lors de la récupération des favoris';
  } finally {
    loading.value = false;
    isRetrying.value = false;
  }
};

// Fonction pour supprimer un favori
const removeFavorite = async (id) => {
  try {
    // Appel à l'API protégée qui nécessite un token JWT
    await del(`/Favorites/channels/${id}`);

    // Mettre à jour la liste des favoris
    favorites.value = favorites.value.filter(fav => fav.id !== id);
  } catch (err) {
    console.error('Error removing favorite:', err);
    error.value = err.response?.data || err.message || 'Erreur lors de la suppression du favori';
  }
};

// Charger les favoris au montage du composant
onMounted(() => {
  fetchFavorites();
});
</script>
