<template>
  <header class="bg-gray-900 text-white shadow-md">
    <div class="container mx-auto px-4 py-3">
      <div class="flex justify-between items-center">
        <!-- Logo et titre -->
        <div class="flex items-center space-x-2">
          <NuxtLink to="/" class="flex items-center">
            <span class="text-blue-400 text-2xl mr-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
              </svg>
            </span>
            <h1 class="font-bold text-xl">Lecteur IPTV</h1>
          </NuxtLink>
        </div>

        <!-- Navigation principale -->
        <nav class="hidden md:flex space-x-6">
          <NuxtLink to="/live" class="py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400 border-b-2 border-blue-400">
            Live TV
          </NuxtLink>
          <NuxtLink to="/vod" class="py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400 border-b-2 border-blue-400">
            VOD
          </NuxtLink>

          <!-- Liens pour utilisateur connecté -->
          <template v-if="isLoggedIn">
            <NuxtLink to="/profile" class="py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400 border-b-2 border-blue-400">
              Profil
            </NuxtLink>
            <button @click="logout" class="py-2 hover:text-blue-400 transition-colors">
              Déconnexion
            </button>
          </template>

          <!-- Liens pour utilisateur non connecté -->
          <template v-else>
            <NuxtLink to="/auth/login" class="py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400 border-b-2 border-blue-400">
              Connexion
            </NuxtLink>
            <NuxtLink to="/auth/register" class="py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400 border-b-2 border-blue-400">
              Inscription
            </NuxtLink>
          </template>
        </nav>

        <!-- Menu mobile -->
        <div class="md:hidden">
          <button @click="isMenuOpen = !isMenuOpen" class="text-white focus:outline-none">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path v-if="!isMenuOpen" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Menu mobile déroulant -->
      <div v-if="isMenuOpen" class="md:hidden mt-3 pb-2">
        <NuxtLink to="/live" class="block py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400">
          Live TV
        </NuxtLink>
        <NuxtLink to="/vod" class="block py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400">
          VOD
        </NuxtLink>

        <!-- Liens pour utilisateur connecté -->
        <template v-if="isLoggedIn">
          <NuxtLink to="/profile" class="block py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400">
            Profil
          </NuxtLink>
          <button @click="logout" class="block w-full text-left py-2 hover:text-blue-400 transition-colors">
            Déconnexion
          </button>
        </template>

        <!-- Liens pour utilisateur non connecté -->
        <template v-else>
          <NuxtLink to="/auth/login" class="block py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400">
            Connexion
          </NuxtLink>
          <NuxtLink to="/auth/register" class="block py-2 hover:text-blue-400 transition-colors" active-class="text-blue-400">
            Inscription
          </NuxtLink>
        </template>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { useAuth } from '~/composables/useAuth';

const isMenuOpen = ref(false);
const { isLoggedIn, logout } = useAuth();

// Fermer le menu mobile lors du changement de route
watch(() => useRoute().fullPath, () => {
  isMenuOpen.value = false;
});
</script>
