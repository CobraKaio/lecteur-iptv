/**
 * Composable pour gérer les appels API
 * Ce composable encapsule la logique de création d'Axios et l'utilisation de useRuntimeConfig
 */

import { ref } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';
import { getCsrfToken, fetchCsrfToken } from '~/utils/csrfUtils';
import { isTokenExpired } from '~/utils/jwtUtils';

// Clés pour le stockage local (doivent correspondre à celles utilisées dans useAuth)
const AUTH_TOKEN_KEY = 'auth_token';
const AUTH_USER_KEY = 'auth_user';

export function useApi() {
  // Récupérer la configuration runtime de Nuxt via le plugin
  const { $runtimeConfig } = useNuxtApp();

  // URL de base de l'API backend
  const baseUrl = $runtimeConfig?.public?.apiBase || 'http://localhost:5151/api';

  // État
  const loading = ref(false);
  const error = ref(null);

  // Créer l'instance API
  const api = axios.create({
    baseURL: baseUrl,
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    }
  });

  // Intercepteur pour les requêtes
  api.interceptors.request.use(
    async config => {
      console.log(`[API Interceptor] Outgoing request: ${config.method?.toUpperCase()} ${config.url}`);

      // Récupérer le token JWT
      // Idéalement, le token vient d'un état réactif (Pinia store ou useAuth si accessible)
      // Pour l'instant, on peut le lire de localStorage (moins réactif mais simple)
      let token = null;
      if (process.client) { // localStorage n'existe que côté client
          token = localStorage.getItem(AUTH_TOKEN_KEY); // Utiliser la constante définie dans useAuth
      }

      // Si un token existe, vérifier s'il est valide et l'ajouter au header Authorization
      if (token) {
         // Vérifier si le token est expiré
         if (isTokenExpired(token)) {
           console.warn('[API Interceptor] Token is expired, removing from storage');
           if (process.client) {
             localStorage.removeItem(AUTH_TOKEN_KEY);
             localStorage.removeItem(AUTH_USER_KEY);
           }
         } else {
           // Assure-toi que le header est prêt, surtout si d'autres intercepteurs l'ont modifié
           config.headers = config.headers || {};
           config.headers['Authorization'] = `Bearer ${token}`;
           console.log('[API Interceptor] Added Authorization header.');
         }
      } else {
          console.log('[API Interceptor] No token found, skipping Authorization header.');
      }

      // Ajouter le token CSRF pour les requêtes non GET
      if (config.method !== 'get' && process.client) {
        // Récupérer le token CSRF
        let csrfToken = getCsrfToken();

        // Si pas de token CSRF, essayer d'en récupérer un
        if (!csrfToken) {
          console.log('[API Interceptor] No CSRF token found, fetching one...');
          await fetchCsrfToken(config.baseURL);
          csrfToken = getCsrfToken();
        }

        // Si un token CSRF existe, l'ajouter au header X-XSRF-TOKEN
        if (csrfToken) {
          config.headers = config.headers || {};
          config.headers['X-XSRF-TOKEN'] = csrfToken;
          console.log('[API Interceptor] Added CSRF token header.');
        } else {
          console.warn('[API Interceptor] Could not get CSRF token, request may fail.');
        }
      }

      return config; // Retourne la configuration de la requête modifiée
    },
    error => {
      // Gère les erreurs avant que la requête ne soit envoyée (ex: problème de configuration)
      console.error('[API Interceptor] Request error before sending:', error);
      return Promise.reject(error); // Transmet l'erreur
    }
  );

  // Intercepteur pour les réponses
  api.interceptors.response.use(
    response => {
      console.log(`[API Interceptor] Response received: ${response.config.method?.toUpperCase()} ${response.config.url} - Status: ${response.status}`);
      return response; // Transmet la réponse réussie
    },
    error => {
      console.error('[API Interceptor] Response error:', error.response || error.message);

      // Gère spécifiquement le cas d'une réponse 401 Unauthorized
      // Cela arrive si le token est manquant, invalide ou expiré sur un endpoint [Authorize]
      if (error.response && error.response.status === 401) {
        console.warn('[API Interceptor] Received 401 Unauthorized. User might be logged out or token expired.');

        // Déconnexion et redirection vers la page de login
        if (process.client) {
          // Importer useAuth directement ici peut causer des problèmes de circularité
          // Utiliser une approche plus simple pour nettoyer le localStorage
          localStorage.removeItem(AUTH_TOKEN_KEY);
          localStorage.removeItem(AUTH_USER_KEY);

          // Rediriger vers la page de login
          const router = useRouter();
          if (router) {
            router.push({
              path: '/auth/login',
              query: { redirect: window.location.pathname }
            });
          } else {
            // Fallback si le router n'est pas disponible
            window.location.href = '/auth/login';
          }
        }
      } else if (error.response && error.response.status === 403) {
        // Gérer le cas 403 Forbidden (utilisateur connecté mais pas autorisé)
        console.warn('[API Interceptor] Received 403 Forbidden. User is not allowed to access this resource.');
        // On pourrait rediriger vers une page d'erreur 403 ou afficher un message
      }

      // Propager l'erreur pour permettre une gestion spécifique dans les composants
      return Promise.reject(error);
    }
  );

  /**
   * Effectue une requête GET
   * @param {string} url - URL relative de l'endpoint
   * @param {Object} params - Paramètres de requête (query string)
   * @returns {Promise} - Promesse résolue avec les données de la réponse
   */
  const get = async (url, params = {}) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.get(url, { params });
      return response.data;
    } catch (err) {
      console.error(`Error fetching data from ${url}:`, err);
      error.value = err.message || `Failed to fetch data from ${url}`;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Effectue une requête POST
   * @param {string} url - URL relative de l'endpoint
   * @param {Object} data - Données à envoyer dans le corps de la requête
   * @returns {Promise} - Promesse résolue avec les données de la réponse
   */
  const post = async (url, data = {}) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.post(url, data);
      return response.data;
    } catch (err) {
      console.error(`Error posting data to ${url}:`, err);
      error.value = err.message || `Failed to post data to ${url}`;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Effectue une requête PUT
   * @param {string} url - URL relative de l'endpoint
   * @param {Object} data - Données à envoyer dans le corps de la requête
   * @returns {Promise} - Promesse résolue avec les données de la réponse
   */
  const put = async (url, data = {}) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.put(url, data);
      return response.data;
    } catch (err) {
      console.error(`Error updating data at ${url}:`, err);
      error.value = err.message || `Failed to update data at ${url}`;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Effectue une requête DELETE
   * @param {string} url - URL relative de l'endpoint
   * @returns {Promise} - Promesse résolue avec les données de la réponse
   */
  const del = async (url) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.delete(url);
      return response.data;
    } catch (err) {
      console.error(`Error deleting data at ${url}:`, err);
      error.value = err.message || `Failed to delete data at ${url}`;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Effectue une requête pour uploader un fichier
   * @param {string} url - URL relative de l'endpoint
   * @param {FormData} formData - FormData contenant le fichier et autres données
   * @returns {Promise} - Promesse résolue avec les données de la réponse
   */
  const upload = async (url, formData) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await api.post(url, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      return response.data;
    } catch (err) {
      console.error(`Error uploading file to ${url}:`, err);
      error.value = err.message || `Failed to upload file to ${url}`;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Génère l'URL du proxy pour un flux
   * @param {string} url - URL du flux original
   * @returns {string} URL du proxy
   */
  const getProxyUrl = (url) => {
    // Encoder l'URL pour éviter les problèmes avec les caractères spéciaux
    const encodedUrl = encodeURIComponent(url);

    // Utiliser l'URL de base de l'API
    return `${baseUrl}/Streaming/proxy?url=${encodedUrl}`;
  };

  return {
    // État
    loading,
    error,
    baseUrl,

    // Méthodes
    get,
    post,
    put,
    del,
    upload,
    getProxyUrl
  };
}
