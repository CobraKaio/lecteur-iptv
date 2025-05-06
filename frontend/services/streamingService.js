/**
 * Service pour gérer le streaming
 * Ce service utilise le composable useApi pour communiquer avec le backend
 */

import { useApi } from '~/composables/useApi';

/**
 * Génère l'URL du proxy pour un flux
 * @param {string} url - URL du flux original
 * @returns {string} URL du proxy
 */
export const getProxyUrl = (url) => {
  const { getProxyUrl: getProxyUrlFromApi } = useApi();
  return getProxyUrlFromApi(url);
};

/**
 * Vérifie si un flux est disponible
 * @param {string} url - URL du flux
 * @returns {Promise<boolean>} True si le flux est disponible, false sinon
 */
export const checkStream = async (url) => {
  const { get } = useApi();
  return await get('/Streaming/check', { url });
};

/**
 * Récupère les informations sur un flux
 * @param {string} url - URL du flux
 * @returns {Promise<Object>} Informations sur le flux
 */
export const getStreamInfo = async (url) => {
  const { get } = useApi();
  return await get('/Streaming/info', { url });
};

/**
 * Détermine le type de flux à partir de l'URL
 * @param {string} url - URL du flux
 * @returns {string} Type de flux ('hls', 'dash', 'mp4', 'unknown')
 */
export const getStreamType = (url) => {
  if (url.endsWith('.m3u8')) {
    return 'hls';
  } else if (url.endsWith('.mpd')) {
    return 'dash';
  } else if (url.endsWith('.mp4') || url.endsWith('.webm') || url.endsWith('.ogg')) {
    return 'mp4';
  } else {
    return 'unknown';
  }
};

/**
 * Formate la durée en secondes en format hh:mm:ss
 * @param {number} seconds - Durée en secondes
 * @returns {string} Durée formatée
 */
export const formatDuration = (seconds) => {
  if (!seconds) return '00:00';

  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = Math.floor(seconds % 60);

  const parts = [];

  if (hours > 0) {
    parts.push(hours.toString().padStart(2, '0'));
  }

  parts.push(minutes.toString().padStart(2, '0'));
  parts.push(secs.toString().padStart(2, '0'));

  return parts.join(':');
};

/**
 * Formate le bitrate en kbps ou Mbps
 * @param {number} bitrate - Bitrate en bps
 * @returns {string} Bitrate formaté
 */
export const formatBitrate = (bitrate) => {
  if (!bitrate) return 'N/A';

  if (bitrate >= 1000000) {
    return `${(bitrate / 1000000).toFixed(2)} Mbps`;
  } else {
    return `${Math.round(bitrate / 1000)} kbps`;
  }
};

// Exporter toutes les fonctions
export default {
  getProxyUrl,
  checkStream,
  getStreamInfo,
  getStreamType,
  formatDuration,
  formatBitrate
};
