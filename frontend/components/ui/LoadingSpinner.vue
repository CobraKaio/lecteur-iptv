<template>
  <div class="loading-spinner-container" :class="[sizeClass, { 'with-text': text }]">
    <div class="spinner animate-spin rounded-full border-t-2 border-b-2 border-blue-500"></div>
    <p v-if="text" class="loading-text">{{ text }}</p>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  size: {
    type: String,
    default: 'md',
    validator: (value) => ['sm', 'md', 'lg'].includes(value)
  },
  text: {
    type: String,
    default: ''
  }
});

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'size-sm';
    case 'lg':
      return 'size-lg';
    default:
      return 'size-md';
  }
});
</script>

<style scoped>
.loading-spinner-container {
  @apply flex flex-col items-center justify-center py-4;
}

.loading-spinner-container.with-text {
  @apply space-y-2;
}

.spinner {
  @apply h-8 w-8;
}

.size-sm .spinner {
  @apply h-5 w-5;
}

.size-md .spinner {
  @apply h-8 w-8;
}

.size-lg .spinner {
  @apply h-12 w-12;
}

.loading-text {
  @apply text-gray-600 dark:text-gray-300 text-sm font-medium;
}

.size-sm .loading-text {
  @apply text-xs;
}

.size-md .loading-text {
  @apply text-sm;
}

.size-lg .loading-text {
  @apply text-base;
}
</style>
