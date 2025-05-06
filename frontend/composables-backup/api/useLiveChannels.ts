import { useApi } from './useApi';

export interface Channel {
  id: number;
  name: string;
  logo?: string;
  category: string;
  streamUrl: string;
  epgId?: string;
}

export interface Program {
  id: number;
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  duration: number;
  channelId: number;
}

export function useLiveChannels() {
  const api = useApi();
  const channels = ref<Channel[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  /**
   * Récupérer toutes les chaînes
   */
  async function fetchChannels() {
    loading.value = true;
    error.value = null;

    try {
      // Appel à l'API réelle
      const response = await api.get<Channel[]>('/Channels');

      if (response.data) {
        // Transformer les données si nécessaire
        channels.value = response.data.map((channel: any) => ({
          id: channel.id,
          name: channel.name,
          logo: channel.logoUrl,
          category: channel.category || channel.group || 'Autre',
          streamUrl: channel.streamUrl,
          epgId: channel.epgId
        }));
      } else if (response.error) {
        throw new Error(response.error);
      } else {
        // Fallback avec des données fictives si l'API n'est pas disponible
        channels.value = Array.from({ length: 15 }, (_, i) => ({
          id: i + 1,
          name: `Chaîne ${i + 1}`,
          logo: `https://via.placeholder.com/50?text=${i + 1}`,
          category: ['Généraliste', 'Sport', 'Cinéma', 'Jeunesse', 'Info'][i % 5],
          streamUrl: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', // Flux HLS de test
          epgId: `epg_${i + 1}`
        }));
      }

      loading.value = false;
    } catch (err) {
      console.error('Error fetching channels:', err);
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      loading.value = false;
    }
  }

  /**
   * Récupérer les programmes d'une chaîne
   */
  async function fetchChannelPrograms(channelId: number) {
    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.get<Program[]>(`/channels/${channelId}/programs`);

      // Pour l'instant, simulons des données
      await new Promise(resolve => setTimeout(resolve, 300));

      const now = new Date();

      return Array.from({ length: 5 }, (_, i) => {
        const startTime = new Date(now);
        startTime.setHours(startTime.getHours() + i * 2);

        const endTime = new Date(startTime);
        endTime.setHours(endTime.getHours() + 2);

        return {
          id: i + 1,
          title: `Programme ${i + 1}`,
          description: `Description du programme ${i + 1}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.`,
          startTime: startTime.toISOString(),
          endTime: endTime.toISOString(),
          duration: 2 * 60 * 60, // 2 heures en secondes
          channelId
        };
      });
    } catch (err) {
      console.error('Erreur lors de la récupération des programmes:', err);
      return [];
    }
  }

  /**
   * Récupérer une chaîne aléatoire
   */
  async function getRandomChannel() {
    if (channels.value.length === 0) {
      await fetchChannels();
    }

    const randomIndex = Math.floor(Math.random() * channels.value.length);
    return channels.value[randomIndex];
  }

  /**
   * Filtrer les chaînes par catégorie
   */
  function filterChannelsByCategory(category: string) {
    if (!category) return channels.value;
    return channels.value.filter(channel => channel.category === category);
  }

  /**
   * Rechercher des chaînes par nom
   */
  function searchChannels(query: string) {
    if (!query) return channels.value;
    const lowerQuery = query.toLowerCase();
    return channels.value.filter(channel =>
      channel.name.toLowerCase().includes(lowerQuery)
    );
  }

  return {
    channels,
    loading,
    error,
    fetchChannels,
    fetchChannelPrograms,
    getRandomChannel,
    filterChannelsByCategory,
    searchChannels
  };
}
