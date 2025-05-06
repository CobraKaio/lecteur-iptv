/**
 * Service pour gérer les chaînes
 * Ce service utilise le composable useApi pour communiquer avec le backend
 */

import { useApi } from '~/composables/useApi';

/**
 * Récupère toutes les chaînes avec pagination
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des chaînes
 */
export const getChannels = async (pageNumber = 1, pageSize = 10) => {
  const { get } = useApi();
  return await get('/Channels', { pageNumber, pageSize });
};

/**
 * Récupère une chaîne par son ID
 * @param {number} id - ID de la chaîne
 * @returns {Promise<Object>} Chaîne
 */
export const getChannel = async (id) => {
  const { get } = useApi();
  return await get(`/Channels/${id}`);
};

/**
 * Récupère les chaînes par catégorie avec pagination
 * @param {string} category - Catégorie des chaînes
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des chaînes de la catégorie
 */
export const getChannelsByCategory = async (category, pageNumber = 1, pageSize = 10) => {
  const { get } = useApi();
  return await get(`/Channels/category/${category}`, { pageNumber, pageSize });
};

/**
 * Recherche des chaînes avec pagination
 * @param {string} query - Terme de recherche
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des chaînes correspondant à la recherche
 */
export const searchChannels = async (query, pageNumber = 1, pageSize = 10) => {
  const { get } = useApi();
  return await get('/Channels/search', { query, pageNumber, pageSize });
};

/**
 * Crée une nouvelle chaîne
 * @param {Object} channel - Données de la chaîne
 * @returns {Promise<Object>} Chaîne créée
 */
export const createChannel = async (channel) => {
  const { post } = useApi();
  return await post('/Channels', channel);
};

/**
 * Met à jour une chaîne
 * @param {number} id - ID de la chaîne
 * @param {Object} channel - Données mises à jour de la chaîne
 * @returns {Promise<Object>} Chaîne mise à jour
 */
export const updateChannel = async (id, channel) => {
  const { put } = useApi();
  return await put(`/Channels/${id}`, channel);
};

/**
 * Supprime une chaîne
 * @param {number} id - ID de la chaîne
 * @returns {Promise<void>}
 */
export const deleteChannel = async (id) => {
  const { del } = useApi();
  return await del(`/Channels/${id}`);
};

/**
 * Importe des chaînes à partir d'un fichier M3U
 * @param {string} url - URL du fichier M3U
 * @returns {Promise<Object>} Résultat de l'importation
 */
export const importChannels = async (url) => {
  const { post } = useApi();
  return await post('/Channels/import', { url });
};

/**
 * Récupère les catégories distinctes des chaînes
 * @returns {Promise<Array<string>>} Liste des catégories distinctes
 */
export const getDistinctCategories = async () => {
  const { get } = useApi();
  return await get('/Channels/categories');
};

/**
 * Récupère les groupes distincts des chaînes
 * @returns {Promise<Array<string>>} Liste des groupes distincts
 */
export const getDistinctGroups = async () => {
  const { get } = useApi();
  return await get('/Channels/groups');
};

/**
 * Récupère les langues distinctes des chaînes
 * @returns {Promise<Array<string>>} Liste des langues distinctes
 */
export const getDistinctLanguages = async () => {
  const { get } = useApi();
  return await get('/Channels/languages');
};

/**
 * Récupère les chaînes actives avec pagination
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des chaînes actives
 */
export const getActiveChannels = async (pageNumber = 1, pageSize = 10) => {
  const { get } = useApi();
  return await get('/Channels/active', { pageNumber, pageSize });
};

/**
 * Récupère les chaînes par groupe avec pagination
 * @param {string} group - Groupe des chaînes
 * @param {number} pageNumber - Numéro de page (1-based)
 * @param {number} pageSize - Nombre d'éléments par page
 * @returns {Promise<Object>} Résultat paginé des chaînes du groupe
 */
export const getChannelsByGroup = async (group, pageNumber = 1, pageSize = 10) => {
  const { get } = useApi();
  return await get(`/Channels/group/${group}`, { pageNumber, pageSize });
};

// Exporter toutes les fonctions
export default {
  getChannels,
  getChannel,
  getChannelsByCategory,
  searchChannels,
  createChannel,
  updateChannel,
  deleteChannel,
  importChannels,
  getDistinctCategories,
  getDistinctGroups,
  getDistinctLanguages,
  getActiveChannels,
  getChannelsByGroup
};
