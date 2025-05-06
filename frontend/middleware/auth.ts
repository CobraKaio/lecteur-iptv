// frontend/middleware/auth.ts

// Importe defineNuxtRouteMiddleware pour définir ce fichier comme un middleware
// Importe navigateTo pour rediriger
// Importe useAuth pour vérifier l'état d'authentification
import { defineNuxtRouteMiddleware, navigateTo } from '#app'; // Imports Nuxt 3
import { useAuth } from '~/composables/useAuth'; // Assure-toi du bon chemin d'import

// Définit le middleware. Il reçoit l'objet route de destination (to) et d'origine (from).
export default defineNuxtRouteMiddleware((to, from) => {
  console.log('Middleware auth: checking route', to.fullPath); // Log pour le débogage

  // Utilise le composable useAuth pour obtenir l'état de connexion
  // Attention: les composables d'état comme useAuth() sont accessibles dans les middlewares !
  const { isLoggedIn } = useAuth();

  // Log pour voir l'état de connexion
  console.log('Middleware auth: isLoggedIn =', isLoggedIn.value);

  // Vérifie si la route de destination (to) nécessite une authentification
  // On utilisera la propriété meta.requiresAuth que tu ajouteras à la route via definePageMeta
  // et que tu as mentionnée dans tes rapports.
  const requiresAuth = to.meta.requiresAuth as boolean | undefined; // Caste en booléen ou undefined pour TypeScript

  console.log('Middleware auth: requiresAuth =', requiresAuth);

  // Si la route nécessite une authentification ET que l'utilisateur N'EST PAS connecté...
  if (requiresAuth && !isLoggedIn.value) {
    console.warn('Middleware auth: Route requires auth, but user is not logged in. Redirecting to login.');

    // Redirige l'utilisateur vers la page de connexion.
    // Optionnel mais très utile: ajoute l'URL de la page demandée comme query parameter 'redirect'
    // après la connexion réussie, tu pourras lire ce paramètre et rediriger l'utilisateur vers où il voulait aller initialement.
    return navigateTo({
       path: '/auth/login', // Le chemin de ta page de connexion
       query: { redirect: to.fullPath } // Ajoute le paramètre de redirection
     });
  }

  // Si la route ne nécessite pas d'authentification OU que l'utilisateur EST connecté, ne fais rien.
  // La navigation continuera vers la page demandée.
  console.log('Middleware auth: Access granted.');
});
