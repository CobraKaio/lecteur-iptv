/**
 * Plugin d'authentification pour Nuxt
 * Ce plugin s'exécute uniquement côté client et charge l'état d'authentification
 * depuis le stockage local au démarrage de l'application
 */

import { useAuth } from '~/composables/useAuth';

export default defineNuxtPlugin(() => {
  // Récupérer le composable d'authentification
  const { loadAuthFromStorage } = useAuth();
  
  // Charger l'état d'authentification depuis le stockage local
  loadAuthFromStorage();
});
