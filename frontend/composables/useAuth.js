/**
 * Composable pour gérer l'authentification
 * Ce composable utilise le service authService pour communiquer avec le backend
 * et gère l'état d'authentification de l'utilisateur
 */

import { ref, computed, onMounted, onUnmounted } from 'vue';
import * as authService from '~/services/authService';
import { shouldRefreshToken, getTokenRemainingTime } from '~/utils/jwtUtils';

// Clés pour le stockage local
const AUTH_TOKEN_KEY = 'auth_token';
const AUTH_USER_KEY = 'auth_user';

export function useAuth() {
  // État
  const user = ref(null);
  const token = ref(null);
  const loading = ref(false);
  const error = ref(null);
  const tokenExpiresAt = ref(null);
  const refreshTimer = ref(null);
  const sessionExpiryWarning = ref(false);

  // Computed
  const isLoggedIn = computed(() => !!token.value);

  // Temps restant avant expiration du token en millisecondes
  const tokenRemainingTime = computed(() => {
    if (!token.value) return 0;
    return getTokenRemainingTime(token.value);
  });

  // Formater le temps restant en minutes et secondes
  const formattedRemainingTime = computed(() => {
    const remainingMs = tokenRemainingTime.value;
    if (remainingMs <= 0) return '0:00';

    const minutes = Math.floor(remainingMs / 60000);
    const seconds = Math.floor((remainingMs % 60000) / 1000);
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  });

  /**
   * Connecte un utilisateur
   * @param {string} username - Nom d'utilisateur ou adresse e-mail
   * @param {string} password - Mot de passe
   * @returns {Promise<Object>} Utilisateur connecté
   */
  const login = async (username, password) => {
    loading.value = true;
    error.value = null;

    try {
      const result = await authService.login(username, password);

      // Mettre à jour l'état
      user.value = result.user;
      token.value = result.token;

      // Sauvegarder dans le stockage local
      saveAuthToStorage(result.token, result.user);

      return result.user;
    } catch (err) {
      console.error('Login error:', err);
      error.value = err.response?.data || err.message || 'Failed to login';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Inscrit un nouvel utilisateur
   * @param {Object} userData - Données de l'utilisateur
   * @returns {Promise<Object>} Utilisateur créé
   */
  const register = async (userData) => {
    loading.value = true;
    error.value = null;

    try {
      const result = await authService.register(userData);
      return result;
    } catch (err) {
      console.error('Register error:', err);
      error.value = err.response?.data || err.message || 'Failed to register';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Déconnecte l'utilisateur
   */
  const logout = () => {
    // Réinitialiser l'état
    user.value = null;
    token.value = null;

    // Supprimer du stockage local
    clearAuthFromStorage();
  };

  /**
   * Récupère les informations de l'utilisateur connecté
   * @returns {Promise<Object>} Informations de l'utilisateur
   */
  const fetchUserProfile = async () => {
    if (!user.value || !user.value.id) {
      return null;
    }

    loading.value = true;
    error.value = null;

    try {
      const result = await authService.getUserProfile(user.value.id);

      // Mettre à jour l'état
      user.value = result;

      // Mettre à jour le stockage local
      saveAuthToStorage(token.value, result);

      return result;
    } catch (err) {
      console.error('Fetch user profile error:', err);
      error.value = err.response?.data || err.message || 'Failed to fetch user profile';

      // Si l'erreur est 401, déconnecter l'utilisateur
      if (err.response?.status === 401) {
        logout();
      }

      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Met à jour le mot de passe de l'utilisateur
   * @param {string} currentPassword - Mot de passe actuel
   * @param {string} newPassword - Nouveau mot de passe
   * @returns {Promise<boolean>} True si la mise à jour a réussi
   */
  const updatePassword = async (currentPassword, newPassword) => {
    if (!user.value || !user.value.id) {
      throw new Error('User not logged in');
    }

    loading.value = true;
    error.value = null;

    try {
      const result = await authService.updatePassword(user.value.id, currentPassword, newPassword);
      return result;
    } catch (err) {
      console.error('Update password error:', err);
      error.value = err.response?.data || err.message || 'Failed to update password';
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Sauvegarde l'état d'authentification dans le stockage local
   * @param {string} authToken - Token d'authentification
   * @param {Object} authUser - Utilisateur authentifié
   */
  const saveAuthToStorage = (authToken, authUser) => {
    if (process.client) {
      localStorage.setItem(AUTH_TOKEN_KEY, authToken);
      localStorage.setItem(AUTH_USER_KEY, JSON.stringify(authUser));
    }
  };

  /**
   * Rafraîchit le token JWT
   * @returns {Promise<boolean>} True si le rafraîchissement a réussi, false sinon
   */
  const refreshAuthToken = async () => {
    if (!token.value) return false;

    try {
      loading.value = true;
      error.value = null;
      sessionExpiryWarning.value = false;

      console.log('Refreshing auth token...');
      const result = await authService.refreshToken();

      // Mettre à jour l'état
      user.value = result.user;
      token.value = result.token;

      // Sauvegarder dans le stockage local
      saveAuthToStorage(result.token, result.user);

      // Planifier le prochain rafraîchissement
      scheduleTokenRefresh();

      console.log('Token refreshed successfully');
      return true;
    } catch (err) {
      console.error('Token refresh error:', err);
      error.value = err.message || 'Failed to refresh token';

      // Si l'erreur est 401, déconnecter l'utilisateur
      if (err.response?.status === 401) {
        logout();
      }

      return false;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Planifie le rafraîchissement du token avant son expiration
   */
  const scheduleTokenRefresh = () => {
    // Annuler le timer existant s'il y en a un
    if (refreshTimer.value) {
      clearTimeout(refreshTimer.value);
      refreshTimer.value = null;
    }

    // Si pas de token, ne rien faire
    if (!token.value) return;

    // Calculer le temps restant avant l'expiration
    const remainingTime = getTokenRemainingTime(token.value);
    if (remainingTime <= 0) {
      // Token déjà expiré, déconnecter l'utilisateur
      console.warn('Token already expired');
      logout();
      return;
    }

    // Calculer le temps avant le rafraîchissement (5 minutes avant l'expiration ou la moitié du temps restant)
    const refreshThreshold = 5 * 60 * 1000; // 5 minutes
    const timeUntilRefresh = remainingTime - refreshThreshold;
    const warningThreshold = 1 * 60 * 1000; // 1 minute

    // Si le token expire dans moins de 5 minutes, le rafraîchir immédiatement
    if (timeUntilRefresh <= 0) {
      console.log('Token expires soon, refreshing now');
      refreshAuthToken();
      return;
    }

    // Planifier le rafraîchissement
    console.log(`Scheduling token refresh in ${Math.round(timeUntilRefresh / 1000)} seconds`);
    refreshTimer.value = setTimeout(refreshAuthToken, timeUntilRefresh);

    // Planifier un avertissement d'expiration si le token expire dans moins de 1 minute
    if (remainingTime <= warningThreshold) {
      sessionExpiryWarning.value = true;
    } else {
      // Planifier un avertissement 1 minute avant l'expiration
      setTimeout(() => {
        sessionExpiryWarning.value = true;
      }, remainingTime - warningThreshold);
    }
  };

  /**
   * Charge l'état d'authentification depuis le stockage local
   */
  const loadAuthFromStorage = () => {
    if (process.client) {
      const storedToken = localStorage.getItem(AUTH_TOKEN_KEY);
      const storedUser = localStorage.getItem(AUTH_USER_KEY);

      if (storedToken && storedUser) {
        // Vérifier si le token est valide
        if (authService.validateToken(storedToken)) {
          token.value = storedToken;
          try {
            user.value = JSON.parse(storedUser);

            // Planifier le rafraîchissement du token
            scheduleTokenRefresh();
          } catch (err) {
            console.error('Error parsing stored user:', err);
            user.value = null;
            clearAuthFromStorage();
          }
        } else {
          // Token expiré, nettoyer le stockage
          console.warn('Stored token is expired, clearing auth storage');
          clearAuthFromStorage();
        }
      }
    }
  };

  /**
   * Supprime l'état d'authentification du stockage local
   */
  const clearAuthFromStorage = () => {
    if (process.client) {
      localStorage.removeItem(AUTH_TOKEN_KEY);
      localStorage.removeItem(AUTH_USER_KEY);
    }
  };

  // Nettoyer le timer lors de la destruction du composant
  onUnmounted(() => {
    if (refreshTimer.value) {
      clearTimeout(refreshTimer.value);
      refreshTimer.value = null;
    }
  });

  // Charger l'état d'authentification au montage du composant
  onMounted(() => {
    loadAuthFromStorage();
  });

  return {
    // État
    user,
    token,
    loading,
    error,
    isLoggedIn,
    sessionExpiryWarning,
    tokenRemainingTime,
    formattedRemainingTime,

    // Méthodes
    login,
    register,
    logout,
    fetchUserProfile,
    updatePassword,
    refreshAuthToken,
    loadAuthFromStorage
  };
}

export default useAuth;
