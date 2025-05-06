<template>
  <div class="flex flex-col min-h-screen bg-gray-100 dark:bg-gray-800 dark:text-white">
    <AppHeader />
    <main class="flex-grow container mx-auto px-4 py-6">
      <slot />
    </main>
    <AppFooter />
  </div>
</template>

<script setup>
import AppHeader from '~/components/layout/AppHeader.vue';
import AppFooter from '~/components/layout/AppFooter.vue';
</script>
