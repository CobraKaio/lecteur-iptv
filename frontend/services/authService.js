/**
 * Service d'authentification pour communiquer avec le backend
 * Ce service utilise le composable useApi pour effectuer les requêtes d'authentification
 */

import { useApi } from '~/composables/useApi';
import { isTokenExpired } from '~/utils/jwtUtils';

/**
 * Connecte un utilisateur
 * @param {string} username - Nom d'utilisateur ou adresse e-mail
 * @param {string} password - Mot de passe
 * @returns {Promise<Object>} Utilisateur connecté avec token
 */
export const login = async (username, password) => {
  try {
    const { post } = useApi();

    // Appel à l'API backend pour authentifier l'utilisateur
    const response = await post('/Users/login', { username, password });

    // Dans une vraie implémentation, le backend devrait renvoyer un token JWT
    // Pour l'instant, nous simulons un token en attendant que le backend soit mis à jour
    const user = response;
    const token = `simulated-jwt-token-${Date.now()}-${username}`;

    return {
      user,
      token
    };
  } catch (error) {
    console.error('Login error:', error);
    throw error;
  }
};

/**
 * Inscrit un nouvel utilisateur
 * @param {Object} userData - Données de l'utilisateur
 * @returns {Promise<Object>} Utilisateur créé
 */
export const register = async (userData) => {
  try {
    const { post } = useApi();

    // Appel à l'API backend pour créer un nouvel utilisateur
    const response = await post('/Users/register', userData);
    return response;
  } catch (error) {
    console.error('Register error:', error);
    throw error;
  }
};

/**
 * Récupère les informations de l'utilisateur connecté
 * @param {number} userId - ID de l'utilisateur
 * @returns {Promise<Object>} Informations de l'utilisateur
 */
export const getUserProfile = async (userId) => {
  try {
    const { get } = useApi();

    // Appel à l'API backend pour récupérer les informations de l'utilisateur
    const response = await get(`/Users/${userId}`);
    return response;
  } catch (error) {
    console.error('Get user profile error:', error);
    throw error;
  }
};

/**
 * Met à jour le mot de passe de l'utilisateur
 * @param {number} userId - ID de l'utilisateur
 * @param {string} currentPassword - Mot de passe actuel
 * @param {string} newPassword - Nouveau mot de passe
 * @returns {Promise<boolean>} True si la mise à jour a réussi
 */
export const updatePassword = async (userId, currentPassword, newPassword) => {
  try {
    const { post } = useApi();

    // Appel à l'API backend pour mettre à jour le mot de passe
    await post(`/Users/${userId}/password`, {
      currentPassword,
      newPassword
    });
    return true;
  } catch (error) {
    console.error('Update password error:', error);
    throw error;
  }
};

/**
 * Rafraîchit le token JWT
 * @returns {Promise<Object>} Nouveau token JWT et utilisateur
 */
export const refreshToken = async () => {
  try {
    const { post } = useApi();

    // Appel à l'API backend pour rafraîchir le token
    const response = await post('/Users/refresh-token');

    // Le backend devrait renvoyer un nouvel objet avec user et token
    return response;
  } catch (error) {
    console.error('Refresh token error:', error);

    // Améliorer les messages d'erreur
    if (error.response) {
      switch (error.response.status) {
        case 401:
          throw new Error('Session expirée. Veuillez vous reconnecter.');
        default:
          throw new Error(`Erreur de rafraîchissement du token: ${error.response.data || error.message}`);
      }
    }

    throw new Error('Impossible de se connecter au serveur. Veuillez vérifier votre connexion internet');
  }
};

/**
 * Vérifie si un token est valide
 * @param {string} token - Token JWT à vérifier
 * @returns {boolean} True si le token est valide, false sinon
 */
export const validateToken = (token) => {
  if (!token) return false;

  // Utiliser la fonction isTokenExpired de jwtUtils
  return !isTokenExpired(token);
};

export default {
  login,
  register,
  getUserProfile,
  updatePassword,
  refreshToken,
  validateToken
};
