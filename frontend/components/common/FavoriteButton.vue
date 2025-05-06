<script setup>
import { ref, onMounted } from 'vue'
import { useFavorites } from '~/composables/useFavorites'

const props = defineProps({
  itemId: {
    type: Number,
    required: true
  },
  type: {
    type: String,
    required: true,
    validator: (value) => ['channel', 'vod'].includes(value)
  }
})

const { addFavorite, removeFavorite, checkIsFavorite } = useFavorites()
const isFavorite = ref(false)
const isLoading = ref(false)

onMounted(async () => {
  isLoading.value = true
  try {
    isFavorite.value = await checkIsFavorite(props.itemId, props.type)
  } catch (error) {
    console.error('Error checking favorite status:', error)
  } finally {
    isLoading.value = false
  }
})

const toggleFavorite = async () => {
  if (isLoading.value) return

  isLoading.value = true
  try {
    if (isFavorite.value) {
      await removeFavorite(props.itemId, props.type)
      isFavorite.value = false
    } else {
      await addFavorite(props.itemId, props.type)
      isFavorite.value = true
    }
  } catch (error) {
    console.error('Error toggling favorite:', error)
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <button 
    class="favorite-button"
    :class="{ 'is-favorite': isFavorite, 'is-loading': isLoading }"
    @click="toggleFavorite"
    :disabled="isLoading"
  >
    <span v-if="isLoading" class="loading-spinner"></span>
    <svg v-else xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" class="heart-icon">
      <path d="M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z"/>
    </svg>
  </button>
</template>

<style scoped>
.favorite-button {
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 8px;
  transition: transform 0.2s ease;
  position: relative;
}

.favorite-button:hover {
  transform: scale(1.1);
}

.heart-icon {
  width: 24px;
  height: 24px;
  fill: none;
  stroke: currentColor;
  stroke-width: 2;
  transition: fill 0.3s ease;
}

.is-favorite .heart-icon {
  fill: #ff4757;
  stroke: #ff4757;
}

.loading-spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 71, 87, 0.3);
  border-radius: 50%;
  border-top-color: #ff4757;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.is-loading {
  cursor: wait;
}
</style>