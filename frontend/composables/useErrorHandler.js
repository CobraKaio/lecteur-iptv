/**
 * Composable pour la gestion centralisée des erreurs
 * Ce composable fournit des fonctions pour gérer les erreurs de manière cohérente
 */

import { ref, computed } from 'vue';

export function useErrorHandler() {
  // État
  const errors = ref({});
  const globalError = ref(null);
  const notifications = ref([]);
  
  // Computed
  const hasErrors = computed(() => Object.keys(errors.value).length > 0 || globalError.value !== null);
  const hasNotifications = computed(() => notifications.value.length > 0);
  
  /**
   * Ajoute une erreur pour un champ spécifique
   * @param {string} field - Nom du champ
   * @param {string} message - Message d'erreur
   */
  const addFieldError = (field, message) => {
    errors.value = {
      ...errors.value,
      [field]: message
    };
  };
  
  /**
   * Supprime une erreur pour un champ spécifique
   * @param {string} field - Nom du champ
   */
  const removeFieldError = (field) => {
    const newErrors = { ...errors.value };
    delete newErrors[field];
    errors.value = newErrors;
  };
  
  /**
   * Définit une erreur globale
   * @param {string} message - Message d'erreur
   */
  const setGlobalError = (message) => {
    globalError.value = message;
  };
  
  /**
   * Supprime l'erreur globale
   */
  const clearGlobalError = () => {
    globalError.value = null;
  };
  
  /**
   * Supprime toutes les erreurs
   */
  const clearAllErrors = () => {
    errors.value = {};
    globalError.value = null;
  };
  
  /**
   * Gère une erreur HTTP
   * @param {Error} error - Erreur à gérer
   * @param {string} defaultMessage - Message par défaut si l'erreur n'a pas de message
   * @returns {string} Message d'erreur
   */
  const handleHttpError = (error, defaultMessage = 'Une erreur est survenue') => {
    console.error('HTTP Error:', error);
    
    let errorMessage = defaultMessage;
    
    if (error.response) {
      // Le serveur a répondu avec un code d'erreur
      const status = error.response.status;
      const data = error.response.data;
      
      switch (status) {
        case 400:
          // Erreur de validation
          errorMessage = data || 'Données invalides';
          break;
        case 401:
          // Non authentifié
          errorMessage = 'Vous devez être connecté pour effectuer cette action';
          break;
        case 403:
          // Non autorisé
          errorMessage = 'Vous n\'êtes pas autorisé à effectuer cette action';
          break;
        case 404:
          // Ressource non trouvée
          errorMessage = 'La ressource demandée n\'existe pas';
          break;
        case 422:
          // Erreur de validation
          errorMessage = data || 'Données invalides';
          break;
        case 429:
          // Trop de requêtes
          errorMessage = 'Trop de requêtes. Veuillez réessayer plus tard';
          break;
        case 500:
          // Erreur serveur
          errorMessage = 'Erreur serveur. Veuillez réessayer plus tard';
          break;
        default:
          // Autre erreur
          errorMessage = data || `Erreur ${status}`;
          break;
      }
    } else if (error.request) {
      // La requête a été faite mais aucune réponse n'a été reçue
      errorMessage = 'Aucune réponse du serveur. Veuillez vérifier votre connexion internet';
    } else {
      // Une erreur s'est produite lors de la configuration de la requête
      errorMessage = error.message || defaultMessage;
    }
    
    setGlobalError(errorMessage);
    return errorMessage;
  };
  
  /**
   * Ajoute une notification
   * @param {string} message - Message de la notification
   * @param {string} type - Type de notification (info, success, warning, error)
   * @param {number} timeout - Délai avant la disparition de la notification (en ms)
   */
  const addNotification = (message, type = 'info', timeout = 5000) => {
    const id = Date.now();
    
    notifications.value.push({
      id,
      message,
      type,
      timestamp: new Date()
    });
    
    // Supprimer la notification après le délai
    if (timeout > 0) {
      setTimeout(() => {
        removeNotification(id);
      }, timeout);
    }
    
    return id;
  };
  
  /**
   * Supprime une notification
   * @param {number} id - ID de la notification
   */
  const removeNotification = (id) => {
    notifications.value = notifications.value.filter(notification => notification.id !== id);
  };
  
  /**
   * Supprime toutes les notifications
   */
  const clearAllNotifications = () => {
    notifications.value = [];
  };
  
  return {
    // État
    errors,
    globalError,
    notifications,
    hasErrors,
    hasNotifications,
    
    // Méthodes
    addFieldError,
    removeFieldError,
    setGlobalError,
    clearGlobalError,
    clearAllErrors,
    handleHttpError,
    addNotification,
    removeNotification,
    clearAllNotifications
  };
}

export default useErrorHandler;
