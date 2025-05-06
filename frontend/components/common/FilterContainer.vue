<template>
  <div class="filter-container bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 mb-6">
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
      <!-- Barre de recherche -->
      <div class="w-full md:w-1/3">
        <SearchBar 
          :initial-query="searchQuery" 
          :placeholder="searchPlaceholder"
          :debounce-time="debounceTime"
          :min-length="minSearchLength"
          @search="onSearch" 
          @clear="onClearSearch"
        />
      </div>
      
      <!-- Filtres par catégorie -->
      <div class="w-full md:w-2/3">
        <CategoryFilter 
          :categories="categories" 
          :initial-category="selectedCategory"
          :all-categories-label="allCategoriesLabel"
          @filter="onCategoryFilter" 
          @clear="onClearCategory"
        />
      </div>
    </div>
    
    <!-- Résultats de recherche -->
    <div v-if="isFiltering" class="mt-4 text-sm text-gray-600 dark:text-gray-400 flex items-center justify-between">
      <div>
        <span v-if="resultsCount !== undefined">{{ resultsCount }} résultat(s) trouvé(s)</span>
        <span v-else>Filtres actifs</span>
      </div>
      <button 
        @click="onClearAllFilters" 
        class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 flex items-center"
      >
        <svg class="h-4 w-4 mr-1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
        </svg>
        Effacer tous les filtres
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import SearchBar from './SearchBar.vue';
import CategoryFilter from './CategoryFilter.vue';

const props = defineProps({
  categories: {
    type: Array,
    default: () => []
  },
  initialSearchQuery: {
    type: String,
    default: ''
  },
  initialCategory: {
    type: String,
    default: ''
  },
  searchPlaceholder: {
    type: String,
    default: 'Rechercher...'
  },
  allCategoriesLabel: {
    type: String,
    default: 'Toutes les catégories'
  },
  debounceTime: {
    type: Number,
    default: 300
  },
  minSearchLength: {
    type: Number,
    default: 3
  },
  resultsCount: {
    type: Number,
    default: undefined
  }
});

const emit = defineEmits(['search', 'filter', 'clear-search', 'clear-category', 'clear-all']);

const searchQuery = ref(props.initialSearchQuery);
const selectedCategory = ref(props.initialCategory);

// Computed pour déterminer si des filtres sont actifs
const isFiltering = computed(() => {
  return searchQuery.value !== '' || selectedCategory.value !== '';
});

// Fonctions pour gérer les événements
const onSearch = (query) => {
  searchQuery.value = query;
  emit('search', query);
};

const onCategoryFilter = (category) => {
  selectedCategory.value = category;
  emit('filter', category);
};

const onClearSearch = () => {
  searchQuery.value = '';
  emit('clear-search');
};

const onClearCategory = () => {
  selectedCategory.value = '';
  emit('clear-category');
};

const onClearAllFilters = () => {
  searchQuery.value = '';
  selectedCategory.value = '';
  emit('clear-all');
};

// Réagir aux changements de props
watch(() => props.initialSearchQuery, (newValue) => {
  if (newValue !== searchQuery.value) {
    searchQuery.value = newValue;
  }
});

watch(() => props.initialCategory, (newValue) => {
  if (newValue !== selectedCategory.value) {
    selectedCategory.value = newValue;
  }
});
</script>
