<script setup>
import { defineProps } from 'vue'
import FavoriteButton from '~/components/common/FavoriteButton.vue'

const props = defineProps({
  vod: {
    type: Object,
    required: true
  }
})
</script>

<template>
  <div class="vod-card">
    <div class="vod-card__image">
      <img 
        v-if="vod.imageUrl" 
        :src="vod.imageUrl" 
        :alt="vod.title"
        class="vod-image"
      />
      <div class="vod-overlay">
        <FavoriteButton
          :itemId="vod.id"
          type="vod"
          class="favorite-btn"
        />
      </div>
    </div>
    <div class="vod-card__content">
      <h3 class="vod-title">{{ vod.title }}</h3>
      <div class="vod-info">
        <span v-if="vod.year" class="vod-year">{{ vod.year }}</span>
        <span v-if="vod.duration" class="vod-duration">{{ vod.duration }}min</span>
      </div>
      <p v-if="vod.description" class="vod-description">{{ vod.description }}</p>
    </div>
  </div>
</template>

<style scoped>
.vod-card {
  @apply bg-white dark:bg-gray-800 rounded-lg shadow-md overflow-hidden;
}

.vod-card__image {
  @apply relative aspect-video;
}

.vod-image {
  @apply w-full h-full object-cover;
}

.vod-overlay {
  @apply absolute top-2 right-2;
}

.vod-card__content {
  @apply p-4;
}

.vod-title {
  @apply text-lg font-semibold text-gray-900 dark:text-white mb-2;
}

.vod-info {
  @apply flex items-center gap-4 text-sm text-gray-500 dark:text-gray-400 mb-2;
}

.vod-description {
  @apply text-sm text-gray-600 dark:text-gray-300 line-clamp-2;
}

.favorite-btn {
  @apply bg-black/30 rounded-full p-1;
}
</style>