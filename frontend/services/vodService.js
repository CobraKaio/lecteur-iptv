/**
 * Service pour gérer les contenus VOD
 * Ce service utilise le service API pour communiquer avec le backend
 */

import { get, post, put, del } from './api';

/**
 * Récupère tous les contenus VOD avec pagination
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des contenus VOD
 */
export const getVodItems = async (pageNumber = 1, pageSize = 10) => {
  return await get('/Vod', { pageNumber, pageSize });
};

/**
 * Récupère un contenu VOD par son ID
 * @param {number} id - ID du contenu VOD
 * @returns {Promise<Object>} Contenu VOD
 */
export const getVodItem = async (id) => {
  return await get(`/Vod/${id}`);
};

/**
 * Récupère les contenus VOD par catégorie avec pagination
 * @param {string} category - Catégorie des contenus VOD
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des contenus VOD de la catégorie
 */
export const getVodItemsByCategory = async (category, pageNumber = 1, pageSize = 10) => {
  return await get(`/Vod/category/${category}`, { pageNumber, pageSize });
};

/**
 * Recherche des contenus VOD avec pagination
 * @param {string} query - Terme de recherche
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des contenus VOD correspondant à la recherche
 */
export const searchVodItems = async (query, pageNumber = 1, pageSize = 10) => {
  return await get('/Vod/search', { query, pageNumber, pageSize });
};

/**
 * Crée un nouveau contenu VOD
 * @param {Object} vodItem - Données du contenu VOD
 * @returns {Promise<Object>} Contenu VOD créé
 */
export const createVodItem = async (vodItem) => {
  return await post('/Vod', vodItem);
};

/**
 * Met à jour un contenu VOD
 * @param {number} id - ID du contenu VOD
 * @param {Object} vodItem - Données mises à jour du contenu VOD
 * @returns {Promise<Object>} Contenu VOD mis à jour
 */
export const updateVodItem = async (id, vodItem) => {
  return await put(`/Vod/${id}`, vodItem);
};

/**
 * Supprime un contenu VOD
 * @param {number} id - ID du contenu VOD
 * @returns {Promise<void>}
 */
export const deleteVodItem = async (id) => {
  return await del(`/Vod/${id}`);
};

/**
 * Importe des contenus VOD à partir d'un fichier M3U
 * @param {string} url - URL du fichier M3U
 * @returns {Promise<Object>} Résultat de l'importation
 */
export const importVodItems = async (url) => {
  return await post('/Vod/import', null, { url });
};

/**
 * Récupère les contenus VOD actifs avec pagination
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des contenus VOD actifs
 */
export const getActiveVodItems = async (pageNumber = 1, pageSize = 10) => {
  return await get('/Vod/active', { pageNumber, pageSize });
};

/**
 * Filtre les contenus VOD selon les critères spécifiés
 * @param {Object} filter - Critères de filtrage
 * @returns {Promise<Object>} Résultat paginé des contenus VOD filtrés
 */
export const filterVodItems = async (filter) => {
  return await get('/Vod/filter', filter);
};

/**
 * Récupère les catégories distinctes des contenus VOD
 * @returns {Promise<Array<string>>} Liste des catégories distinctes
 */
export const getDistinctCategories = async () => {
  return await get('/Vod/categories');
};

/**
 * Récupère les types distincts des contenus VOD
 * @returns {Promise<Array<string>>} Liste des types distincts
 */
export const getDistinctTypes = async () => {
  return await get('/Vod/types');
};

/**
 * Récupère les langues distinctes des contenus VOD
 * @returns {Promise<Array<string>>} Liste des langues distinctes
 */
export const getDistinctLanguages = async () => {
  return await get('/Vod/languages');
};

/**
 * Récupère les années distinctes des contenus VOD
 * @returns {Promise<Array<number>>} Liste des années distinctes
 */
export const getDistinctYears = async () => {
  return await get('/Vod/years');
};

// Exporter toutes les fonctions
export default {
  getVodItems,
  getVodItem,
  getVodItemsByCategory,
  searchVodItems,
  createVodItem,
  updateVodItem,
  deleteVodItem,
  importVodItems,
  getActiveVodItems,
  filterVodItems,
  getDistinctCategories,
  getDistinctTypes,
  getDistinctLanguages,
  getDistinctYears
};
