import { useApi } from './useApi';

export interface VodItem {
  id: number;
  title: string;
  description: string;
  posterUrl?: string;
  streamUrl: string;
  category: string;
  type: 'movie' | 'series' | 'other';
  releaseYear?: number;
  duration?: number;
  rating?: number;
  genres?: string[];
}

export interface VodSeries extends VodItem {
  type: 'series';
  seasons: VodSeason[];
}

export interface VodSeason {
  id: number;
  seasonNumber: number;
  title: string;
  episodes: VodEpisode[];
}

export interface VodEpisode {
  id: number;
  episodeNumber: number;
  title: string;
  description: string;
  streamUrl: string;
  duration?: number;
  thumbnailUrl?: string;
}

export function useVod() {
  const api = useApi();
  const vodItems = ref<VodItem[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  /**
   * Récupérer tous les contenus VOD
   */
  async function fetchVodItems() {
    loading.value = true;
    error.value = null;

    try {
      // Appel à l'API réelle
      const response = await api.get<VodItem[]>('/Vod');
      
      if (response.data) {
        // Transformer les données si nécessaire
        vodItems.value = response.data.map((item: any) => ({
          id: item.id,
          title: item.title || item.name,
          description: item.description || '',
          posterUrl: item.posterUrl || item.thumbnailUrl,
          streamUrl: item.streamUrl,
          category: item.category || 'Autre',
          type: item.type || 'movie',
          releaseYear: item.releaseYear,
          duration: item.duration,
          rating: item.rating,
          genres: item.genres
        }));
      } else if (response.error) {
        throw new Error(response.error);
      } else {
        // Fallback avec des données fictives si l'API n'est pas disponible
        vodItems.value = generateMockVodItems();
      }
      
      loading.value = false;
    } catch (err) {
      console.error('Error fetching VOD items:', err);
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      loading.value = false;
      
      // Fallback avec des données fictives en cas d'erreur
      vodItems.value = generateMockVodItems();
    }
  }

  /**
   * Récupérer un contenu VOD par son ID
   */
  async function fetchVodItem(id: number) {
    loading.value = true;
    error.value = null;

    try {
      // Appel à l'API réelle
      const response = await api.get<VodItem>(`/Vod/${id}`);
      
      if (response.data) {
        // Transformer les données si nécessaire
        const item = response.data;
        return {
          id: item.id,
          title: item.title || item.name,
          description: item.description || '',
          posterUrl: item.posterUrl || item.thumbnailUrl,
          streamUrl: item.streamUrl,
          category: item.category || 'Autre',
          type: item.type || 'movie',
          releaseYear: item.releaseYear,
          duration: item.duration,
          rating: item.rating,
          genres: item.genres
        };
      } else if (response.error) {
        throw new Error(response.error);
      } else {
        // Fallback avec des données fictives si l'API n'est pas disponible
        return generateMockVodItem(id);
      }
    } catch (err) {
      console.error(`Error fetching VOD item ${id}:`, err);
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      
      // Fallback avec des données fictives en cas d'erreur
      return generateMockVodItem(id);
    } finally {
      loading.value = false;
    }
  }

  /**
   * Récupérer les contenus VOD par catégorie
   */
  async function fetchVodItemsByCategory(category: string) {
    loading.value = true;
    error.value = null;

    try {
      // Appel à l'API réelle
      const response = await api.get<VodItem[]>(`/Vod/category/${category}`);
      
      if (response.data) {
        // Transformer les données si nécessaire
        vodItems.value = response.data.map((item: any) => ({
          id: item.id,
          title: item.title || item.name,
          description: item.description || '',
          posterUrl: item.posterUrl || item.thumbnailUrl,
          streamUrl: item.streamUrl,
          category: item.category || 'Autre',
          type: item.type || 'movie',
          releaseYear: item.releaseYear,
          duration: item.duration,
          rating: item.rating,
          genres: item.genres
        }));
      } else if (response.error) {
        throw new Error(response.error);
      } else {
        // Fallback avec des données fictives si l'API n'est pas disponible
        vodItems.value = generateMockVodItems().filter(item => item.category === category);
      }
    } catch (err) {
      console.error(`Error fetching VOD items by category ${category}:`, err);
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      
      // Fallback avec des données fictives en cas d'erreur
      vodItems.value = generateMockVodItems().filter(item => item.category === category);
    } finally {
      loading.value = false;
    }
  }

  /**
   * Rechercher des contenus VOD
   */
  async function searchVodItems(query: string) {
    if (!query) return vodItems.value;
    
    loading.value = true;
    error.value = null;

    try {
      // Appel à l'API réelle
      const response = await api.get<VodItem[]>('/Vod/search', { query });
      
      if (response.data) {
        // Transformer les données si nécessaire
        vodItems.value = response.data.map((item: any) => ({
          id: item.id,
          title: item.title || item.name,
          description: item.description || '',
          posterUrl: item.posterUrl || item.thumbnailUrl,
          streamUrl: item.streamUrl,
          category: item.category || 'Autre',
          type: item.type || 'movie',
          releaseYear: item.releaseYear,
          duration: item.duration,
          rating: item.rating,
          genres: item.genres
        }));
      } else if (response.error) {
        throw new Error(response.error);
      } else {
        // Fallback avec des données fictives si l'API n'est pas disponible
        const lowerQuery = query.toLowerCase();
        vodItems.value = generateMockVodItems().filter(item => 
          item.title.toLowerCase().includes(lowerQuery) || 
          item.description.toLowerCase().includes(lowerQuery)
        );
      }
    } catch (err) {
      console.error(`Error searching VOD items with query ${query}:`, err);
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      
      // Fallback avec des données fictives en cas d'erreur
      const lowerQuery = query.toLowerCase();
      vodItems.value = generateMockVodItems().filter(item => 
        item.title.toLowerCase().includes(lowerQuery) || 
        item.description.toLowerCase().includes(lowerQuery)
      );
    } finally {
      loading.value = false;
    }
    
    return vodItems.value;
  }

  /**
   * Filtrer les contenus VOD par type
   */
  function filterVodItemsByType(type: 'movie' | 'series' | 'other') {
    return vodItems.value.filter(item => item.type === type);
  }

  /**
   * Générer des données fictives pour les contenus VOD
   */
  function generateMockVodItems(): VodItem[] {
    const categories = ['Films', 'Séries', 'Documentaires', 'Enfants'];
    const types: ('movie' | 'series' | 'other')[] = ['movie', 'series', 'other'];
    
    return Array.from({ length: 20 }, (_, i) => ({
      id: i + 1,
      title: `Contenu VOD ${i + 1}`,
      description: `Description du contenu VOD ${i + 1}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.`,
      posterUrl: `https://via.placeholder.com/300x450?text=VOD+${i + 1}`,
      streamUrl: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', // Flux HLS de test
      category: categories[i % categories.length],
      type: types[i % types.length],
      releaseYear: 2020 + (i % 5),
      duration: 90 + (i * 5),
      rating: (Math.floor(Math.random() * 10) + 1) / 2,
      genres: ['Action', 'Comédie', 'Drame', 'Science-fiction'].slice(0, (i % 4) + 1)
    }));
  }

  /**
   * Générer des données fictives pour un contenu VOD
   */
  function generateMockVodItem(id: number): VodItem {
    const categories = ['Films', 'Séries', 'Documentaires', 'Enfants'];
    const types: ('movie' | 'series' | 'other')[] = ['movie', 'series', 'other'];
    
    return {
      id,
      title: `Contenu VOD ${id}`,
      description: `Description du contenu VOD ${id}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.`,
      posterUrl: `https://via.placeholder.com/300x450?text=VOD+${id}`,
      streamUrl: 'https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8', // Flux HLS de test
      category: categories[id % categories.length],
      type: types[id % types.length],
      releaseYear: 2020 + (id % 5),
      duration: 90 + (id * 5),
      rating: (Math.floor(Math.random() * 10) + 1) / 2,
      genres: ['Action', 'Comédie', 'Drame', 'Science-fiction'].slice(0, (id % 4) + 1)
    };
  }

  // Computed properties
  const movies = computed(() => filterVodItemsByType('movie'));
  const series = computed(() => filterVodItemsByType('series'));
  const others = computed(() => filterVodItemsByType('other'));
  
  // Récupérer les catégories disponibles
  const categories = computed(() => {
    const uniqueCategories = new Set<string>();
    vodItems.value.forEach(item => {
      if (item.category) uniqueCategories.add(item.category);
    });
    return Array.from(uniqueCategories).sort();
  });

  return {
    vodItems,
    loading,
    error,
    movies,
    series,
    others,
    categories,
    fetchVodItems,
    fetchVodItem,
    fetchVodItemsByCategory,
    searchVodItems,
    filterVodItemsByType
  };
}
