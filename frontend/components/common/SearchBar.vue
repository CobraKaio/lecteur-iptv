<template>
  <div class="search-bar">
    <div class="relative">
      <input
        type="text"
        v-model="searchQuery"
        @input="onSearch"
        :placeholder="placeholder"
        class="pl-10 pr-4 py-2 w-full rounded-lg border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
      <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
        <svg class="h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z" clip-rule="evenodd" />
        </svg>
      </div>
      <button 
        v-if="searchQuery" 
        @click="clearSearch" 
        class="absolute inset-y-0 right-0 pr-3 flex items-center"
      >
        <svg class="h-5 w-5 text-gray-400 hover:text-gray-600" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import { debounce } from 'lodash-es';

const props = defineProps({
  initialQuery: {
    type: String,
    default: ''
  },
  debounceTime: {
    type: Number,
    default: 300
  },
  placeholder: {
    type: String,
    default: 'Rechercher...'
  },
  minLength: {
    type: Number,
    default: 3
  }
});

const emit = defineEmits(['search', 'clear']);

const searchQuery = ref(props.initialQuery);

// Fonction debounce pour éviter trop d'appels pendant la saisie
const debouncedSearch = debounce((query) => {
  if (query.length === 0 || query.length >= props.minLength) {
    emit('search', query);
  }
}, props.debounceTime);

const onSearch = () => {
  debouncedSearch(searchQuery.value);
};

const clearSearch = () => {
  searchQuery.value = '';
  emit('clear');
  emit('search', '');
};

// Réagir aux changements de props.initialQuery
watch(() => props.initialQuery, (newValue) => {
  if (newValue !== searchQuery.value) {
    searchQuery.value = newValue;
  }
});
</script>
