/**
 * Service API pour communiquer avec le backend
 * Ce service utilise Axios pour effectuer les requêtes HTTP
 */

import { useApi } from '~/composables/useApi';

/**
 * Effectue une requête GET
 * @param {string} url - URL de la requête
 * @param {Object} params - Paramètres de la requête
 * @returns {Promise<any>} Réponse de la requête
 */
export const get = async (url, params = {}) => {
  const api = useApi();
  try {
    const response = await api.get(url, { params });
    return response.data;
  } catch (error) {
    console.error(`GET request failed for ${url}:`, error);
    throw error;
  }
};

/**
 * Effectue une requête POST
 * @param {string} url - URL de la requête
 * @param {Object} data - Données à envoyer
 * @param {Object} params - Paramètres de la requête
 * @returns {Promise<any>} Réponse de la requête
 */
export const post = async (url, data = {}, params = {}) => {
  const api = useApi();
  try {
    const response = await api.post(url, data, { params });
    return response.data;
  } catch (error) {
    console.error(`POST request failed for ${url}:`, error);
    throw error;
  }
};

/**
 * Effectue une requête PUT
 * @param {string} url - URL de la requête
 * @param {Object} data - Données à envoyer
 * @param {Object} params - Paramètres de la requête
 * @returns {Promise<any>} Réponse de la requête
 */
export const put = async (url, data = {}, params = {}) => {
  const api = useApi();
  try {
    const response = await api.put(url, data, { params });
    return response.data;
  } catch (error) {
    console.error(`PUT request failed for ${url}:`, error);
    throw error;
  }
};

/**
 * Effectue une requête DELETE
 * @param {string} url - URL de la requête
 * @param {Object} params - Paramètres de la requête
 * @returns {Promise<any>} Réponse de la requête
 */
export const del = async (url, params = {}) => {
  const api = useApi();
  try {
    const response = await api.delete(url, { params });
    return response.data;
  } catch (error) {
    console.error(`DELETE request failed for ${url}:`, error);
    throw error;
  }
};

// Exporter toutes les fonctions
export default {
  get,
  post,
  put,
  del
};
