<template>
  <div>
    <section class="mb-10">
      <h1 class="text-3xl font-bold mb-6">Bienvenue sur votre Lecteur IPTV</h1>
      <p class="text-lg mb-4">
        Accédez à vos chaînes TV en direct et à votre bibliothèque de contenus à la demande.
      </p>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-8">
        <!-- Section Live TV -->
        <div class="bg-white dark:bg-gray-700 rounded-lg shadow-md overflow-hidden">
          <div class="bg-blue-600 text-white p-4">
            <h2 class="text-xl font-semibold">Live TV</h2>
          </div>
          <div class="p-6">
            <p class="mb-4">Regardez vos chaînes TV préférées en direct avec guide des programmes.</p>
            <div class="flex justify-between items-center">
              <NuxtLink to="/live" class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md transition-colors">
                Accéder aux chaînes
              </NuxtLink>
              <button @click="goToRandomChannel" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
                Chaîne aléatoire
              </button>
            </div>
          </div>
        </div>

        <!-- Section VOD -->
        <div class="bg-white dark:bg-gray-700 rounded-lg shadow-md overflow-hidden">
          <div class="bg-purple-600 text-white p-4">
            <h2 class="text-xl font-semibold">Vidéo à la demande</h2>
          </div>
          <div class="p-6">
            <p class="mb-4">Explorez notre bibliothèque de films et séries disponibles à tout moment.</p>
            <NuxtLink to="/vod" class="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md transition-colors inline-block">
              Découvrir le catalogue
            </NuxtLink>
          </div>
        </div>
      </div>
    </section>

    <!-- Section Derniers ajouts -->
    <section class="mb-10">
      <h2 class="text-2xl font-bold mb-4">Derniers ajouts</h2>
      <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
        <div v-for="i in 5" :key="i" class="bg-white dark:bg-gray-700 rounded-lg shadow overflow-hidden">
          <div class="aspect-w-16 aspect-h-9 bg-gray-300 dark:bg-gray-600">
            <!-- Placeholder pour image -->
            <div class="flex items-center justify-center h-full">
              <span class="text-gray-500 dark:text-gray-400">Image</span>
            </div>
          </div>
          <div class="p-3">
            <h3 class="font-medium">Titre {{ i }}</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">Catégorie</p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { useChannels } from '~/composables/useChannels';

const router = useRouter();
const { getRandomChannel } = useChannels();

// Fonction pour aller à une chaîne aléatoire
const goToRandomChannel = async () => {
  // Rediriger vers la page live avec le paramètre random=true
  // Le composant live/index.vue s'occupera de sélectionner une chaîne aléatoire
  router.push('/live?random=true');
};
</script>
