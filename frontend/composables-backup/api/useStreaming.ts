import { useApi } from './useApi';

export interface StreamInfo {
  streamType: string;
  resolution?: string;
  bitrate?: number;
  videoCodec?: string;
  audioCodec?: string;
  duration?: number;
  isLive: boolean;
}

export function useStreaming() {
  const api = useApi();
  const loading = ref(false);
  const error = ref<string | null>(null);
  const streamInfo = ref<StreamInfo | null>(null);

  /**
   * Vérifier si un flux est disponible
   */
  async function checkStream(url: string): Promise<boolean> {
    loading.value = true;
    error.value = null;

    try {
      // Appel à l'API réelle
      const response = await api.get<boolean>('/Streaming/check', { url });
      
      if (response.data !== null) {
        return response.data;
      } else if (response.error) {
        throw new Error(response.error);
      } else {
        // Fallback si l'API n'est pas disponible
        return true;
      }
    } catch (err) {
      console.error(`Error checking stream ${url}:`, err);
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      return false;
    } finally {
      loading.value = false;
    }
  }

  /**
   * Récupérer les informations sur un flux
   */
  async function getStreamInfo(url: string): Promise<StreamInfo | null> {
    loading.value = true;
    error.value = null;
    streamInfo.value = null;

    try {
      // Appel à l'API réelle
      const response = await api.get<StreamInfo>('/Streaming/info', { url });
      
      if (response.data) {
        streamInfo.value = response.data;
        return response.data;
      } else if (response.error) {
        throw new Error(response.error);
      } else {
        // Fallback si l'API n'est pas disponible
        const mockInfo = generateMockStreamInfo(url);
        streamInfo.value = mockInfo;
        return mockInfo;
      }
    } catch (err) {
      console.error(`Error getting stream info for ${url}:`, err);
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      
      // Fallback en cas d'erreur
      const mockInfo = generateMockStreamInfo(url);
      streamInfo.value = mockInfo;
      return mockInfo;
    } finally {
      loading.value = false;
    }
  }

  /**
   * Générer l'URL du proxy pour un flux
   */
  function getProxyUrl(url: string): string {
    // Utiliser l'URL de l'API pour proxifier le flux
    const config = useRuntimeConfig();
    const baseUrl = config.public.apiBase;
    return `${baseUrl}/Streaming/proxy?url=${encodeURIComponent(url)}`;
  }

  /**
   * Déterminer le type de flux à partir de l'URL
   */
  function getStreamType(url: string): string {
    if (url.endsWith('.m3u8')) {
      return 'hls';
    } else if (url.endsWith('.mpd')) {
      return 'dash';
    } else if (url.endsWith('.mp4') || url.endsWith('.webm') || url.endsWith('.ogg')) {
      return 'mp4';
    } else {
      return 'unknown';
    }
  }

  /**
   * Formater la durée en secondes en format hh:mm:ss
   */
  function formatDuration(seconds: number): string {
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
  }

  /**
   * Formater le bitrate en kbps ou Mbps
   */
  function formatBitrate(bitrate: number): string {
    if (!bitrate) return 'N/A';
    
    if (bitrate >= 1000000) {
      return `${(bitrate / 1000000).toFixed(2)} Mbps`;
    } else {
      return `${Math.round(bitrate / 1000)} kbps`;
    }
  }

  /**
   * Générer des informations fictives sur un flux
   */
  function generateMockStreamInfo(url: string): StreamInfo {
    const streamType = getStreamType(url);
    const isLive = url.includes('live') || Math.random() > 0.5;
    
    return {
      streamType,
      resolution: ['720p', '1080p', '480p', '360p'][Math.floor(Math.random() * 4)],
      bitrate: Math.floor(Math.random() * 5000 + 1000) * 1000,
      videoCodec: ['H.264', 'H.265', 'VP9'][Math.floor(Math.random() * 3)],
      audioCodec: ['AAC', 'MP3', 'Opus'][Math.floor(Math.random() * 3)],
      duration: isLive ? undefined : Math.floor(Math.random() * 7200 + 300),
      isLive
    };
  }

  return {
    loading,
    error,
    streamInfo,
    checkStream,
    getStreamInfo,
    getProxyUrl,
    getStreamType,
    formatDuration,
    formatBitrate
  };
}
