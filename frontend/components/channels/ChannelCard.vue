<script setup>
import { defineProps } from 'vue'
import FavoriteButton from '~/components/common/FavoriteButton.vue'

const props = defineProps({
  channel: {
    type: Object,
    required: true
  }
})
</script>

<template>
  <div class="channel-card">
    <div class="channel-card__header">
      <img 
        v-if="channel.logoUrl" 
        :src="channel.logoUrl" 
        :alt="channel.name"
        class="channel-logo"
      />
      <div class="channel-info">
        <h3 class="channel-name">{{ channel.name }}</h3>
        <p v-if="channel.group" class="channel-group">{{ channel.group }}</p>
      </div>
      <FavoriteButton
        :itemId="channel.id"
        type="channel"
        class="favorite-btn"
      />
    </div>
  </div>
</template>

<style scoped>
.channel-card {
  @apply bg-white dark:bg-gray-800 rounded-lg shadow-md overflow-hidden;
}

.channel-card__header {
  @apply p-4 flex items-center justify-between;
}

.channel-logo {
  @apply w-12 h-12 object-contain rounded;
}

.channel-info {
  @apply flex-grow mx-4;
}

.channel-name {
  @apply text-lg font-semibold text-gray-900 dark:text-white;
}

.channel-group {
  @apply text-sm text-gray-500 dark:text-gray-400;
}

.favorite-btn {
  @apply flex-shrink-0;
}
</style>