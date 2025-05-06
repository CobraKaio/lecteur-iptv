/**
 * Plugin pour exposer la configuration runtime de Nuxt
 * Ce plugin est exécuté au démarrage de l'application
 */

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig();
  
  // Exposer la configuration runtime pour qu'elle soit accessible dans toute l'application
  return {
    provide: {
      runtimeConfig: config
    }
  };
});
