// Ce plugin corrige le problème d'importation de hls.js avec le module global/window.js
// en définissant une variable globale window si elle n'existe pas déjà

export default defineNuxtPlugin(() => {
  // Ce plugin ne fait rien côté client car window existe déjà
  // Mais côté serveur, il fournit un polyfill pour window
  if (process.server) {
    // @ts-ignore
    global.window = global.window || global;
  }
});
