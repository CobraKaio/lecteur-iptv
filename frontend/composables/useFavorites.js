import { ref } from 'vue'
import { useAuth } from './useAuth'
import favoritesService from '~/services/favoritesService'

export const useFavorites = () => {
  const { user } = useAuth()
  const favoriteChannels = ref([])
  const favoriteVods = ref([])
  const isLoading = ref(false)
  const error = ref(null)

  const loadFavorites = async () => {
    if (!user.value) return
    
    isLoading.value = true
    error.value = null
    
    try {
      const [channels, vods] = await Promise.all([
        favoritesService.getFavoriteChannels(user.value.id),
        favoritesService.getFavoriteVods(user.value.id)
      ])
      
      favoriteChannels.value = channels
      favoriteVods.value = vods
    } catch (err) {
      error.value = 'Error loading favorites'
      console.error(err)
    } finally {
      isLoading.value = false
    }
  }

  const checkIsFavorite = async (itemId, type) => {
    if (!user.value) return false
    
    try {
      return await favoritesService.checkIsFavorite(user.value.id, itemId, type)
    } catch (err) {
      console.error('Error checking favorite status:', err)
      return false
    }
  }

  const addFavorite = async (itemId, type) => {
    if (!user.value) return false
    
    error.value = null
    try {
      const result = await favoritesService.addFavorite(user.value.id, itemId, type)
      if (result) {
        await loadFavorites()
      }
      return result
    } catch (err) {
      error.value = 'Error adding to favorites'
      console.error(err)
      return false
    }
  }

  const removeFavorite = async (itemId, type) => {
    if (!user.value) return false
    
    error.value = null
    try {
      const result = await favoritesService.removeFavorite(user.value.id, itemId, type)
      if (result) {
        if (type === 'channel') {
          favoriteChannels.value = favoriteChannels.value.filter(c => c.id !== itemId)
        } else {
          favoriteVods.value = favoriteVods.value.filter(v => v.id !== itemId)
        }
      }
      return result
    } catch (err) {
      error.value = 'Error removing from favorites'
      console.error(err)
      return false
    }
  }

  return {
    favoriteChannels,
    favoriteVods,
    isLoading,
    error,
    loadFavorites,
    checkIsFavorite,
    addFavorite,
    removeFavorite
  }
}