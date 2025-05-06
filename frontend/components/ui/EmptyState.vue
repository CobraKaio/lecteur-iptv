<template>
  <div class="empty-state bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 flex flex-col items-center justify-center">
    <div class="icon-container mb-4">
      <slot name="icon">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
      </slot>
    </div>

    <h2 :class="[titleClass || 'text-xl font-medium text-gray-700 dark:text-gray-300 mb-2']">{{ title }}</h2>

    <p class="text-gray-600 dark:text-gray-400 text-center mb-4">
      {{ message }}
    </p>

    <div v-if="$slots.action || actionText" class="action-container">
      <slot name="action">
        <button
          v-if="actionText && actionHandler"
          @click="actionHandler"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded transition-colors"
        >
          {{ actionText }}
        </button>
      </slot>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  message: {
    type: String,
    required: true
  },
  titleClass: {
    type: String,
    default: ''
  },
  actionText: {
    type: String,
    default: ''
  },
  actionHandler: {
    type: Function,
    default: null
  }
});
</script>

<style scoped>
.empty-state {
  @apply min-h-[200px];
}

@media (min-width: 768px) {
  .empty-state {
    @apply min-h-[300px];
  }
}
</style>
