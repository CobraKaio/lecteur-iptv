import { useApi } from '~/composables/useApi'

class FavoritesService {
  constructor() {
    const { api } = useApi()
    this.api = api
  }

  async getFavoriteChannels(userId) {
    try {
      const response = await this.api.get(`/favorites/channels/${userId}`)
      return response.data
    } catch (error) {
      console.error('Error fetching favorite channels:', error)
      throw error
    }
  }

  async getFavoriteVods(userId) {
    try {
      const response = await this.api.get(`/favorites/vods/${userId}`)
      return response.data
    } catch (error) {
      console.error('Error fetching favorite VODs:', error)
      throw error
    }
  }

  async checkIsFavorite(userId, itemId, type) {
    try {
      const response = await this.api.get(`/favorites/${type}s/${userId}/${itemId}`)
      return response.data.isFavorite
    } catch (error) {
      console.error('Error checking favorite status:', error)
      return false
    }
  }

  async addFavorite(userId, itemId, type) {
    try {
      const response = await this.api.post(`/favorites/${type}s/${userId}/${itemId}`)
      return response.data.success
    } catch (error) {
      console.error('Error adding to favorites:', error)
      throw error
    }
  }

  async removeFavorite(userId, itemId, type) {
    try {
      const response = await this.api.delete(`/favorites/${type}s/${userId}/${itemId}`)
      return response.data.success
    } catch (error) {
      console.error('Error removing from favorites:', error)
      throw error
    }
  }
}

export default new FavoritesService()
