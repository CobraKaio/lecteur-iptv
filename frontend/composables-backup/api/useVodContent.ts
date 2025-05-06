import { useApi } from './useApi';

export interface VodContent {
  id: number;
  title: string;
  description: string;
  poster?: string;
  streamUrl: string;
  type: 'movie' | 'series';
  category: string;
  year: number;
  duration: number; // en minutes
  rating: number; // sur 5
  isNew?: boolean;
}

export interface Episode {
  id: number;
  seriesId: number;
  season: number;
  episode: number;
  title: string;
  description: string;
  streamUrl: string;
  duration: number; // en minutes
}

export function useVodContent() {
  const api = useApi();
  const vodContent = ref<VodContent[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  /**
   * Récupérer tout le contenu VOD
   */
  async function fetchVodContent() {
    loading.value = true;
    error.value = null;

    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.get<VodContent[]>('/vod');
      
      // Pour l'instant, simulons des données
      await new Promise(resolve => setTimeout(resolve, 500));
      
      const categories = ['Films', 'Séries', 'Documentaires', 'Jeunesse'];
      
      vodContent.value = Array.from({ length: 20 }, (_, i) => ({
        id: i + 1,
        title: `Contenu ${i + 1}`,
        description: `Description du contenu ${i + 1}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.`,
        poster: `https://via.placeholder.com/300x450?text=Contenu${i + 1}`,
        streamUrl: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', // Flux HLS de test
        type: i % 3 === 0 ? 'series' : 'movie',
        category: categories[i % categories.length],
        year: 2020 + (i % 4),
        duration: 90 + (i * 5),
        rating: 3 + (Math.random() * 2),
        isNew: i > 15
      }));
      
      loading.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      loading.value = false;
    }
  }

  /**
   * Récupérer un contenu VOD par ID
   */
  async function fetchVodContentById(id: number) {
    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.get<VodContent>(`/vod/${id}`);
      
      // Pour l'instant, simulons des données
      await new Promise(resolve => setTimeout(resolve, 300));
      
      if (vodContent.value.length === 0) {
        await fetchVodContent();
      }
      
      return vodContent.value.find(content => content.id === id) || null;
    } catch (err) {
      console.error('Erreur lors de la récupération du contenu VOD:', err);
      return null;
    }
  }

  /**
   * Récupérer les épisodes d'une série
   */
  async function fetchSeriesEpisodes(seriesId: number) {
    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.get<Episode[]>(`/vod/series/${seriesId}/episodes`);
      
      // Pour l'instant, simulons des données
      await new Promise(resolve => setTimeout(resolve, 300));
      
      return Array.from({ length: 10 }, (_, i) => ({
        id: i + 1,
        seriesId,
        season: 1,
        episode: i + 1,
        title: `Épisode ${i + 1}`,
        description: `Description de l'épisode ${i + 1}. Lorem ipsum dolor sit amet.`,
        streamUrl: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', // Flux HLS de test
        duration: 45
      }));
    } catch (err) {
      console.error('Erreur lors de la récupération des épisodes:', err);
      return [];
    }
  }

  /**
   * Récupérer du contenu similaire
   */
  async function fetchSimilarContent(id: number, limit = 4) {
    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.get<VodContent[]>(`/vod/${id}/similar?limit=${limit}`);
      
      // Pour l'instant, simulons des données
      await new Promise(resolve => setTimeout(resolve, 300));
      
      if (vodContent.value.length === 0) {
        await fetchVodContent();
      }
      
      // Exclure le contenu actuel et prendre quelques éléments aléatoires
      const filteredContent = vodContent.value.filter(content => content.id !== id);
      const shuffled = [...filteredContent].sort(() => 0.5 - Math.random());
      return shuffled.slice(0, limit);
    } catch (err) {
      console.error('Erreur lors de la récupération du contenu similaire:', err);
      return [];
    }
  }

  /**
   * Filtrer le contenu VOD par catégorie
   */
  function filterContentByCategory(category: string) {
    if (!category) return vodContent.value;
    return vodContent.value.filter(content => content.category === category);
  }

  /**
   * Rechercher du contenu VOD par titre
   */
  function searchContent(query: string) {
    if (!query) return vodContent.value;
    const lowerQuery = query.toLowerCase();
    return vodContent.value.filter(content => 
      content.title.toLowerCase().includes(lowerQuery) || 
      content.description.toLowerCase().includes(lowerQuery)
    );
  }

  /**
   * Récupérer le contenu récemment ajouté
   */
  function getNewContent() {
    return vodContent.value.filter(content => content.isNew);
  }

  /**
   * Récupérer les films populaires (simulé par le rating)
   */
  function getPopularMovies(limit = 10) {
    return vodContent.value
      .filter(content => content.type === 'movie')
      .sort((a, b) => b.rating - a.rating)
      .slice(0, limit);
  }

  /**
   * Récupérer les séries populaires (simulé par le rating)
   */
  function getPopularSeries(limit = 5) {
    return vodContent.value
      .filter(content => content.type === 'series')
      .sort((a, b) => b.rating - a.rating)
      .slice(0, limit);
  }

  return {
    vodContent,
    loading,
    error,
    fetchVodContent,
    fetchVodContentById,
    fetchSeriesEpisodes,
    fetchSimilarContent,
    filterContentByCategory,
    searchContent,
    getNewContent,
    getPopularMovies,
    getPopularSeries
  };
}
