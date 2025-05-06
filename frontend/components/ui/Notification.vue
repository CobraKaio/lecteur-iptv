<template>
  <div
    :class="[
      'fixed z-50 p-4 rounded-md shadow-lg transition-all duration-300 transform',
      positionClasses,
      typeClasses,
      { 'translate-y-0 opacity-100': show, 'translate-y-8 opacity-0': !show }
    ]"
    role="alert"
  >
    <div class="flex items-center">
      <div class="flex-shrink-0">
        <component :is="icon" class="h-5 w-5" aria-hidden="true" />
      </div>
      <div class="ml-3">
        <p class="text-sm font-medium">
          {{ message }}
        </p>
      </div>
      <div class="ml-auto pl-3">
        <div class="-mx-1.5 -my-1.5">
          <button
            @click="$emit('close')"
            class="inline-flex rounded-md p-1.5 focus:outline-none focus:ring-2 focus:ring-offset-2"
            :class="closeButtonClasses"
          >
            <span class="sr-only">Fermer</span>
            <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted, onBeforeUnmount } from 'vue';

const props = defineProps({
  message: {
    type: String,
    required: true
  },
  type: {
    type: String,
    default: 'info',
    validator: (value) => ['info', 'success', 'warning', 'error'].includes(value)
  },
  position: {
    type: String,
    default: 'top-right',
    validator: (value) => ['top-right', 'top-left', 'bottom-right', 'bottom-left', 'top-center', 'bottom-center'].includes(value)
  },
  timeout: {
    type: Number,
    default: 5000
  },
  autoClose: {
    type: Boolean,
    default: true
  }
});

const emit = defineEmits(['close']);

// État
const show = ref(false);
const timer = ref(null);

// Classes de position
const positionClasses = computed(() => {
  switch (props.position) {
    case 'top-right':
      return 'top-4 right-4';
    case 'top-left':
      return 'top-4 left-4';
    case 'bottom-right':
      return 'bottom-4 right-4';
    case 'bottom-left':
      return 'bottom-4 left-4';
    case 'top-center':
      return 'top-4 left-1/2 -translate-x-1/2';
    case 'bottom-center':
      return 'bottom-4 left-1/2 -translate-x-1/2';
    default:
      return 'top-4 right-4';
  }
});

// Classes de type
const typeClasses = computed(() => {
  switch (props.type) {
    case 'info':
      return 'bg-blue-50 dark:bg-blue-900 text-blue-800 dark:text-blue-100';
    case 'success':
      return 'bg-green-50 dark:bg-green-900 text-green-800 dark:text-green-100';
    case 'warning':
      return 'bg-yellow-50 dark:bg-yellow-900 text-yellow-800 dark:text-yellow-100';
    case 'error':
      return 'bg-red-50 dark:bg-red-900 text-red-800 dark:text-red-100';
    default:
      return 'bg-blue-50 dark:bg-blue-900 text-blue-800 dark:text-blue-100';
  }
});

// Classes du bouton de fermeture
const closeButtonClasses = computed(() => {
  switch (props.type) {
    case 'info':
      return 'bg-blue-50 dark:bg-blue-900 text-blue-500 hover:bg-blue-100 dark:hover:bg-blue-800 focus:ring-blue-500 focus:ring-offset-blue-50 dark:focus:ring-offset-blue-900';
    case 'success':
      return 'bg-green-50 dark:bg-green-900 text-green-500 hover:bg-green-100 dark:hover:bg-green-800 focus:ring-green-500 focus:ring-offset-green-50 dark:focus:ring-offset-green-900';
    case 'warning':
      return 'bg-yellow-50 dark:bg-yellow-900 text-yellow-500 hover:bg-yellow-100 dark:hover:bg-yellow-800 focus:ring-yellow-500 focus:ring-offset-yellow-50 dark:focus:ring-offset-yellow-900';
    case 'error':
      return 'bg-red-50 dark:bg-red-900 text-red-500 hover:bg-red-100 dark:hover:bg-red-800 focus:ring-red-500 focus:ring-offset-red-50 dark:focus:ring-offset-red-900';
    default:
      return 'bg-blue-50 dark:bg-blue-900 text-blue-500 hover:bg-blue-100 dark:hover:bg-blue-800 focus:ring-blue-500 focus:ring-offset-blue-50 dark:focus:ring-offset-blue-900';
  }
});

// Icône
const icon = computed(() => {
  switch (props.type) {
    case 'info':
      return 'IconInfo';
    case 'success':
      return 'IconSuccess';
    case 'warning':
      return 'IconWarning';
    case 'error':
      return 'IconError';
    default:
      return 'IconInfo';
  }
});

// Icônes
const IconInfo = {
  template: `
    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-blue-400 dark:text-blue-300" viewBox="0 0 20 20" fill="currentColor">
      <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
    </svg>
  `
};

const IconSuccess = {
  template: `
    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-green-400 dark:text-green-300" viewBox="0 0 20 20" fill="currentColor">
      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
    </svg>
  `
};

const IconWarning = {
  template: `
    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-yellow-400 dark:text-yellow-300" viewBox="0 0 20 20" fill="currentColor">
      <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
    </svg>
  `
};

const IconError = {
  template: `
    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-red-400 dark:text-red-300" viewBox="0 0 20 20" fill="currentColor">
      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
    </svg>
  `
};

// Afficher la notification
const startTimer = () => {
  if (props.autoClose && props.timeout > 0) {
    timer.value = setTimeout(() => {
      emit('close');
    }, props.timeout);
  }
};

// Nettoyer le timer
const clearTimer = () => {
  if (timer.value) {
    clearTimeout(timer.value);
    timer.value = null;
  }
};

// Cycle de vie
onMounted(() => {
  // Afficher la notification après un court délai pour permettre l'animation
  setTimeout(() => {
    show.value = true;
  }, 10);
  
  // Démarrer le timer pour la fermeture automatique
  startTimer();
});

onBeforeUnmount(() => {
  clearTimer();
});
</script>
