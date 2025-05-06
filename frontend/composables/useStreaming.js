/**
 * Composable pour gérer le streaming
 * Ce composable utilise le service streamingService pour gérer les flux vidéo
 */

import { ref } from 'vue';
import * as streamingService from '~/services/streamingService';

export function useStreaming() {
  // État
  const loading = ref(false);
  const error = ref(null);
  const streamInfo = ref(null);
  const isAvailable = ref(false);

  /**
   * Vérifie si un flux est disponible
   * @param {string} url - URL du flux
   * @returns {Promise<boolean>} True si le flux est disponible, false sinon
   */
  const checkStream = async (url) => {
    loading.value = true;
    error.value = null;
    
    try {
      isAvailable.value = await streamingService.checkStream(url);
      return isAvailable.value;
    } catch (err) {
      console.error(`Error checking stream ${url}:`, err);
      error.value = err.message || `Failed to check stream: ${url}`;
      isAvailable.value = false;
      return false;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Récupère les informations sur un flux
   * @param {string} url - URL du flux
   * @returns {Promise<Object>} Informations sur le flux
   */
  const getStreamInfo = async (url) => {
    loading.value = true;
    error.value = null;
    
    try {
      streamInfo.value = await streamingService.getStreamInfo(url);
      return streamInfo.value;
    } catch (err) {
      console.error(`Error getting stream info for ${url}:`, err);
      error.value = err.message || `Failed to get stream info: ${url}`;
      streamInfo.value = null;
      return null;
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
    return streamingService.getProxyUrl(url);
  };

  /**
   * Détermine le type de flux à partir de l'URL
   * @param {string} url - URL du flux
   * @returns {string} Type de flux ('hls', 'dash', 'mp4', 'unknown')
   */
  const getStreamType = (url) => {
    return streamingService.getStreamType(url);
  };

  /**
   * Formate la durée en secondes en format hh:mm:ss
   * @param {number} seconds - Durée en secondes
   * @returns {string} Durée formatée
   */
  const formatDuration = (seconds) => {
    return streamingService.formatDuration(seconds);
  };

  /**
   * Formate le bitrate en kbps ou Mbps
   * @param {number} bitrate - Bitrate en bps
   * @returns {string} Bitrate formaté
   */
  const formatBitrate = (bitrate) => {
    return streamingService.formatBitrate(bitrate);
  };

  return {
    // État
    loading,
    error,
    streamInfo,
    isAvailable,
    
    // Méthodes
    checkStream,
    getStreamInfo,
    getProxyUrl,
    getStreamType,
    formatDuration,
    formatBitrate
  };
}
