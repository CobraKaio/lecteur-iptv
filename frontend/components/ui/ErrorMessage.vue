<template>
  <div class="error-message bg-red-100 dark:bg-red-900 border border-red-400 text-red-700 dark:text-red-300 px-4 py-3 rounded">
    <div class="flex items-start">
      <div class="flex-shrink-0">
        <svg class="h-5 w-5 text-red-500" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
        </svg>
      </div>
      <div class="ml-3 flex-1">
        <p class="text-sm font-medium">{{ title || 'Erreur' }}</p>
        <p class="mt-1 text-sm">{{ message }}</p>
        <div class="mt-3">
          <div v-if="retryAction">
            <button
              @click="retryAction"
              class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded text-sm transition-colors"
              :disabled="isRetrying"
            >
              <span v-if="isRetrying">
                <span class="inline-block animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full mr-1"></span>
                Réessai en cours...
              </span>
              <span v-else>{{ retryText || 'Réessayer' }}</span>
            </button>
          </div>
          <slot name="action"></slot>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  message: {
    type: String,
    required: true
  },
  title: {
    type: String,
    default: ''
  },
  retryAction: {
    type: Function,
    default: null
  },
  retryText: {
    type: String,
    default: 'Réessayer'
  },
  isRetrying: {
    type: Boolean,
    default: false
  }
});
</script>

<style scoped>
.error-message {
  @apply mb-6;
}
</style>
