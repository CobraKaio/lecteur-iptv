<template>
  <div class="category-filter">
    <div class="flex flex-wrap gap-2">
      <button 
        @click="selectCategory('')" 
        class="px-3 py-1 text-sm rounded-full transition-colors"
        :class="selectedCategory === '' ? 'bg-blue-600 text-white' : 'bg-gray-200 dark:bg-gray-600 hover:bg-gray-300 dark:hover:bg-gray-500'"
      >
        {{ allCategoriesLabel }}
      </button>
      <button 
        v-for="category in categories" 
        :key="category" 
        @click="selectCategory(category)" 
        class="px-3 py-1 text-sm rounded-full transition-colors"
        :class="selectedCategory === category ? 'bg-blue-600 text-white' : 'bg-gray-200 dark:bg-gray-600 hover:bg-gray-300 dark:hover:bg-gray-500'"
      >
        {{ category }}
      </button>
    </div>
    
    <div v-if="selectedCategory" class="mt-2">
      <button 
        @click="clearFilter" 
        class="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 flex items-center"
      >
        <svg class="h-4 w-4 mr-1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
        </svg>
        Effacer le filtre
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
  categories: {
    type: Array,
    default: () => []
  },
  initialCategory: {
    type: String,
    default: ''
  },
  allCategoriesLabel: {
    type: String,
    default: 'Toutes les catégories'
  }
});

const emit = defineEmits(['filter', 'clear']);

const selectedCategory = ref(props.initialCategory);

const selectCategory = (category) => {
  selectedCategory.value = category;
  emit('filter', category);
};

const clearFilter = () => {
  selectedCategory.value = '';
  emit('clear');
  emit('filter', '');
};

// Réagir aux changements de props.initialCategory
watch(() => props.initialCategory, (newValue) => {
  if (newValue !== selectedCategory.value) {
    selectedCategory.value = newValue;
  }
});
</script>
