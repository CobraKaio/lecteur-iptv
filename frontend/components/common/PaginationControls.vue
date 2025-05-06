<template>
  <div class="pagination-controls flex items-center justify-between py-4">
    <!-- Information sur la pagination -->
    <div class="pagination-info text-sm text-gray-600 dark:text-gray-400">
      <span v-if="totalItems > 0">
        Affichage de {{ startItem }}-{{ endItem }} sur {{ totalItems }} éléments
      </span>
      <span v-else>Aucun élément</span>
    </div>

    <!-- Contrôles de pagination -->
    <div class="pagination-buttons flex space-x-2">
      <!-- Bouton page précédente -->
      <button
        @click="onPrevious"
        :disabled="!hasPreviousPage"
        class="px-3 py-1 rounded-md border border-gray-300 dark:border-gray-600 text-sm font-medium"
        :class="hasPreviousPage 
          ? 'bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-200' 
          : 'bg-gray-100 dark:bg-gray-800 text-gray-400 dark:text-gray-500 cursor-not-allowed'"
      >
        Précédent
      </button>

      <!-- Indicateur de page -->
      <div class="px-3 py-1 text-sm text-gray-700 dark:text-gray-300">
        Page {{ currentPage }} sur {{ totalPages || 1 }}
      </div>

      <!-- Bouton page suivante -->
      <button
        @click="onNext"
        :disabled="!hasNextPage"
        class="px-3 py-1 rounded-md border border-gray-300 dark:border-gray-600 text-sm font-medium"
        :class="hasNextPage 
          ? 'bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-200' 
          : 'bg-gray-100 dark:bg-gray-800 text-gray-400 dark:text-gray-500 cursor-not-allowed'"
      >
        Suivant
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  currentPage: {
    type: Number,
    required: true
  },
  pageSize: {
    type: Number,
    required: true
  },
  totalItems: {
    type: Number,
    required: true
  },
  totalPages: {
    type: Number,
    required: true
  },
  hasPreviousPage: {
    type: Boolean,
    required: true
  },
  hasNextPage: {
    type: Boolean,
    required: true
  }
});

const emit = defineEmits(['previous', 'next', 'page-change']);

// Calculer l'index du premier élément affiché
const startItem = computed(() => {
  if (props.totalItems === 0) return 0;
  return (props.currentPage - 1) * props.pageSize + 1;
});

// Calculer l'index du dernier élément affiché
const endItem = computed(() => {
  if (props.totalItems === 0) return 0;
  return Math.min(props.currentPage * props.pageSize, props.totalItems);
});

// Méthodes pour la navigation
const onPrevious = () => {
  if (props.hasPreviousPage) {
    emit('previous');
    emit('page-change', props.currentPage - 1);
  }
};

const onNext = () => {
  if (props.hasNextPage) {
    emit('next');
    emit('page-change', props.currentPage + 1);
  }
};
</script>
