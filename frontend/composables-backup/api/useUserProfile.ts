import { useApi } from './useApi';

export interface UserPreferences {
  language: string;
  subtitles: string;
  quality: 'auto' | 'low' | 'medium' | 'high';
  autoplay: boolean;
}

export interface UserFavorites {
  liveChannels: number[];
  vodContent: number[];
}

export function useUserProfile() {
  const api = useApi();
  const preferences = ref<UserPreferences>({
    language: 'fr',
    subtitles: 'fr',
    quality: 'auto',
    autoplay: true
  });
  
  const favorites = ref<UserFavorites>({
    liveChannels: [],
    vodContent: []
  });
  
  const loading = ref(false);
  const error = ref<string | null>(null);

  /**
   * Récupérer les préférences utilisateur
   */
  async function fetchUserPreferences() {
    loading.value = true;
    error.value = null;

    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.get<UserPreferences>('/user/preferences');
      
      // Pour l'instant, simulons des données
      await new Promise(resolve => setTimeout(resolve, 300));
      
      // Simuler la récupération depuis le localStorage
      const storedPreferences = localStorage.getItem('userPreferences');
      if (storedPreferences) {
        preferences.value = JSON.parse(storedPreferences);
      } else {
        // Valeurs par défaut
        preferences.value = {
          language: 'fr',
          subtitles: 'fr',
          quality: 'auto',
          autoplay: true
        };
      }
      
      loading.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      loading.value = false;
    }
  }

  /**
   * Enregistrer les préférences utilisateur
   */
  async function saveUserPreferences(newPreferences: UserPreferences) {
    loading.value = true;
    error.value = null;

    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.put<UserPreferences>('/user/preferences', newPreferences);
      
      // Pour l'instant, simulons l'enregistrement
      await new Promise(resolve => setTimeout(resolve, 300));
      
      // Enregistrer dans le localStorage
      localStorage.setItem('userPreferences', JSON.stringify(newPreferences));
      
      preferences.value = newPreferences;
      loading.value = false;
      
      return true;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      loading.value = false;
      return false;
    }
  }

  /**
   * Récupérer les favoris de l'utilisateur
   */
  async function fetchUserFavorites() {
    loading.value = true;
    error.value = null;

    try {
      // Dans une vraie application, cela ferait un appel API
      // const response = await api.get<UserFavorites>('/user/favorites');
      
      // Pour l'instant, simulons des données
      await new Promise(resolve => setTimeout(resolve, 300));
      
      // Simuler la récupération depuis le localStorage
      const storedFavorites = localStorage.getItem('userFavorites');
      if (storedFavorites) {
        favorites.value = JSON.parse(storedFavorites);
      } else {
        // Valeurs par défaut pour la démo
        favorites.value = {
          liveChannels: [1, 3, 5, 7],
          vodContent: [2, 4, 6, 8, 10]
        };
      }
      
      loading.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Une erreur est survenue';
      loading.value = false;
    }
  }

  /**
   * Ajouter un favori
   */
  async function addFavorite(type: 'liveChannels' | 'vodContent', id: number) {
    if (!favorites.value[type].includes(id)) {
      favorites.value[type].push(id);
      
      // Dans une vraie application, cela ferait un appel API
      // await api.post('/user/favorites', { type, id });
      
      // Pour l'instant, simulons l'enregistrement
      localStorage.setItem('userFavorites', JSON.stringify(favorites.value));
      
      return true;
    }
    return false;
  }

  /**
   * Supprimer un favori
   */
  async function removeFavorite(type: 'liveChannels' | 'vodContent', id: number) {
    favorites.value[type] = favorites.value[type].filter(item => item !== id);
    
    // Dans une vraie application, cela ferait un appel API
    // await api.delete(`/user/favorites/${type}/${id}`);
    
    // Pour l'instant, simulons l'enregistrement
    localStorage.setItem('userFavorites', JSON.stringify(favorites.value));
    
    return true;
  }

  /**
   * Vérifier si un élément est en favori
   */
  function isFavorite(type: 'liveChannels' | 'vodContent', id: number) {
    return favorites.value[type].includes(id);
  }

  return {
    preferences,
    favorites,
    loading,
    error,
    fetchUserPreferences,
    saveUserPreferences,
    fetchUserFavorites,
    addFavorite,
    removeFavorite,
    isFavorite
  };
}
