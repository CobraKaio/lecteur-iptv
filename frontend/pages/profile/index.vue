<template>
  <div class="profile-page">
    <h1 class="page-title">Mon Profil</h1>
    
    <!-- Informations de l'utilisateur -->
    <section class="user-info">
      <h2 class="section-title">Informations personnelles</h2>
      <div class="info-card">
        <p><strong>Nom d'utilisateur:</strong> {{ user?.username }}</p>
        <p><strong>Email:</strong> {{ user?.email }}</p>
      </div>
    </section>

    <!-- Chaînes favorites -->
    <section class="favorites-section">
      <h2 class="section-title">Mes chaînes favorites</h2>
      <div v-if="isLoading" class="loading">
        Chargement des favoris...
      </div>
      <div v-else-if="error" class="error">
        {{ error }}
      </div>
      <div v-else-if="favoriteChannels.length === 0" class="empty-state">
        Vous n'avez pas encore de chaînes favorites
      </div>
      <div v-else class="channels-grid">
        <ChannelCard
          v-for="channel in favoriteChannels"
          :key="channel.id"
          :channel="channel"
        />
      </div>
    </section>

    <!-- VOD favorites -->
    <section class="favorites-section">
      <h2 class="section-title">Mes VOD favorites</h2>
      <div v-if="isLoading" class="loading">
        Chargement des favoris...
      </div>
      <div v-else-if="error" class="error">
        {{ error }}
      </div>
      <div v-else-if="favoriteVods.length === 0" class="empty-state">
        Vous n'avez pas encore de VOD favorites
      </div>
      <div v-else class="vod-grid">
        <VodCard
          v-for="vod in favoriteVods"
          :key="vod.id"
          :vod="vod"
        />
      </div>
    </section>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuth } from '~/composables/useAuth'
import { useFavorites } from '~/composables/useFavorites'
import ChannelCard from '~/components/channels/ChannelCard.vue'
import VodCard from '~/components/vod/VodCard.vue'

const { user } = useAuth()
const { favoriteChannels, favoriteVods, loadFavorites, isLoading, error } = useFavorites()

onMounted(() => {
  loadFavorites()
})
</script>

<style scoped>
.profile-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}

.page-title {
  @apply text-3xl font-bold text-gray-900 dark:text-white mb-8;
}

.section-title {
  @apply text-2xl font-semibold text-gray-800 dark:text-gray-200 mb-4;
}

.info-card {
  @apply bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-8;
}

.info-card p {
  @apply mb-2 text-gray-600 dark:text-gray-300;
}

.info-card strong {
  @apply text-gray-900 dark:text-white;
}

.favorites-section {
  @apply mt-8;
}

.channels-grid,
.vod-grid {
  @apply grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6;
}

.loading {
  @apply text-gray-600 dark:text-gray-400 text-center py-8;
}

.error {
  @apply text-red-600 dark:text-red-400 text-center py-8;
}

.empty-state {
  @apply text-gray-500 dark:text-gray-400 text-center py-8 italic;
}
</style>
