/**
 * Composable pour gérer les chaînes
 * Ce composable utilise le service channelsService pour récupérer et manipuler les chaînes
 */

import { ref, computed } from 'vue';
import * as channelsService from '~/services/channelsService';

export function useChannels() {
  // État
  const channels = ref([]);
  const loading = ref(false);
  const error = ref(null);
  const selectedCategory = ref(null);
  const categories = ref([]);
  const categoriesLoading = ref(false);
  const categoriesError = ref(null);
  const groups = ref([]);
  const groupsLoading = ref(false);
  const groupsError = ref(null);
  const languages = ref([]);
  const languagesLoading = ref(false);
  const languagesError = ref(null);

  // État de pagination
  const pagination = ref({
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 1,
    hasPreviousPage: false,
    hasNextPage: false
  });

  /**
   * Récupère toutes les chaînes avec pagination
   * @param {number} pageNumber - Numéro de page (1-based)
   * @param {number} pageSize - Nombre d'éléments par page
   */
  const fetchChannels = async (pageNumber = pagination.value.pageNumber, pageSize = pagination.value.pageSize) => {
    loading.value = true;
    error.value = null;

    try {
      // Appeler le service avec les paramètres de pagination
      const result = await channelsService.getChannels(pageNumber, pageSize);

      // Mettre à jour les chaînes et l'état de pagination
      channels.value = result.items;
      pagination.value = {
        pageNumber: result.pageNumber,
        pageSize: result.pageSize,
        totalCount: result.totalCount,
        totalPages: result.totalPages,
        hasPreviousPage: result.hasPreviousPage,
        hasNextPage: result.hasNextPage
      };

      // Si les catégories n'ont pas encore été chargées, les extraire des chaînes
      if (categories.value.length === 0) {
        extractCategoriesFromChannels();
      }
    } catch (err) {
      console.error('Error fetching channels:', err);
      error.value = err.message || 'Failed to fetch channels';
    } finally {
      loading.value = false;
    }
  };

  /**
   * Extrait les catégories uniques des chaînes
   */
  const extractCategoriesFromChannels = () => {
    const uniqueCategories = new Set();
    channels.value.forEach(channel => {
      if (channel.category) uniqueCategories.add(channel.category);
      if (channel.group) uniqueCategories.add(channel.group);
    });
    categories.value = Array.from(uniqueCategories).sort();
  };

  /**
   * Récupère une chaîne par son ID
   * @param {number} id - ID de la chaîne
   * @returns {Promise<Object>} Chaîne
   */
  const fetchChannel = async (id) => {
    loading.value = true;
    error.value = null;

    try {
      return await channelsService.getChannel(id);
    } catch (err) {
      console.error(`Error fetching channel ${id}:`, err);
      error.value = err.message || `Failed to fetch channel with ID: ${id}`;
      return null;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Récupère les chaînes par catégorie avec pagination
   * @param {string} category - Catégorie des chaînes
   * @param {string} query - Terme de recherche optionnel
   * @param {number} pageNumber - Numéro de page (1-based)
   * @param {number} pageSize - Nombre d'éléments par page
   */
  const fetchChannelsByCategory = async (
    category,
    query = '',
    pageNumber = pagination.value.pageNumber,
    pageSize = pagination.value.pageSize
  ) => {
    loading.value = true;
    error.value = null;
    selectedCategory.value = category;

    try {
      if (query && query.length >= 3) {
        // Si on a aussi une recherche, on recherche dans cette catégorie
        // Note: Idéalement, le backend devrait supporter la recherche filtrée par catégorie
        // Pour l'instant, on filtre côté client
        const result = await channelsService.searchChannels(query, pageNumber, pageSize);
        channels.value = result.items.filter(channel =>
          channel.category === category || channel.group === category
        );

        // Mettre à jour l'état de pagination (mais avec le nombre filtré)
        pagination.value = {
          ...pagination.value,
          pageNumber: result.pageNumber,
          pageSize: result.pageSize,
          totalCount: channels.value.length, // Nombre après filtrage
          totalPages: Math.ceil(channels.value.length / result.pageSize),
          hasPreviousPage: pageNumber > 1,
          hasNextPage: pageNumber < Math.ceil(channels.value.length / result.pageSize)
        };
      } else {
        // Sinon on filtre juste par catégorie
        const result = await channelsService.getChannelsByCategory(category, pageNumber, pageSize);
        channels.value = result.items;

        // Mettre à jour l'état de pagination
        pagination.value = {
          pageNumber: result.pageNumber,
          pageSize: result.pageSize,
          totalCount: result.totalCount,
          totalPages: result.totalPages,
          hasPreviousPage: result.hasPreviousPage,
          hasNextPage: result.hasNextPage
        };
      }
    } catch (err) {
      console.error(`Error fetching channels by category ${category}:`, err);
      error.value = err.message || `Failed to fetch channels for category: ${category}`;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Recherche des chaînes avec pagination
   * @param {string} query - Terme de recherche
   * @param {string} category - Catégorie optionnelle pour filtrer les résultats
   * @param {number} pageNumber - Numéro de page (1-based)
   * @param {number} pageSize - Nombre d'éléments par page
   */
  const searchChannels = async (
    query,
    category = '',
    pageNumber = pagination.value.pageNumber,
    pageSize = pagination.value.pageSize
  ) => {
    if (query.length === 0) {
      // Si la recherche est vide, on revient à la liste complète ou filtrée par catégorie
      if (category) {
        return await fetchChannelsByCategory(category, '', pageNumber, pageSize);
      } else {
        return await fetchChannels(pageNumber, pageSize);
      }
    }

    if (query.length < 3) return;

    loading.value = true;
    error.value = null;

    try {
      const result = await channelsService.searchChannels(query, pageNumber, pageSize);

      // Si une catégorie est spécifiée, on filtre les résultats
      if (category) {
        channels.value = result.items.filter(channel =>
          channel.category === category || channel.group === category
        );

        // Mettre à jour l'état de pagination (mais avec le nombre filtré)
        pagination.value = {
          ...pagination.value,
          pageNumber: result.pageNumber,
          pageSize: result.pageSize,
          totalCount: channels.value.length, // Nombre après filtrage
          totalPages: Math.ceil(channels.value.length / result.pageSize),
          hasPreviousPage: pageNumber > 1,
          hasNextPage: pageNumber < Math.ceil(channels.value.length / result.pageSize)
        };
      } else {
        channels.value = result.items;

        // Mettre à jour l'état de pagination
        pagination.value = {
          pageNumber: result.pageNumber,
          pageSize: result.pageSize,
          totalCount: result.totalCount,
          totalPages: result.totalPages,
          hasPreviousPage: result.hasPreviousPage,
          hasNextPage: result.hasNextPage
        };
      }
    } catch (err) {
      console.error(`Error searching channels with query ${query}:`, err);
      error.value = err.message || `Failed to search channels with query: ${query}`;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Importe des chaînes à partir d'un fichier M3U
   * @param {string} url - URL du fichier M3U
   * @returns {Promise<Object>} Résultat de l'importation
   */
  const importChannels = async (url) => {
    loading.value = true;
    error.value = null;

    try {
      const result = await channelsService.importChannels(url);
      // Rafraîchir la liste des chaînes après l'importation
      await fetchChannels();
      return result;
    } catch (err) {
      console.error(`Error importing channels from ${url}:`, err);
      error.value = err.message || `Failed to import channels from URL: ${url}`;
      return null;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Chaînes filtrées par catégorie sélectionnée
   */
  const filteredChannels = computed(() => {
    if (!selectedCategory.value) {
      return channels.value;
    }

    return channels.value.filter(channel =>
      channel.category === selectedCategory.value ||
      channel.group === selectedCategory.value
    );
  });

  /**
   * Obtient une chaîne aléatoire
   * @returns {Object|null} Chaîne aléatoire ou null si aucune chaîne n'est disponible
   */
  const getRandomChannel = () => {
    if (channels.value.length === 0) return null;

    const randomIndex = Math.floor(Math.random() * channels.value.length);
    return channels.value[randomIndex];
  };

  /**
   * Récupère les catégories distinctes des chaînes
   */
  const fetchDistinctCategories = async () => {
    categoriesLoading.value = true;
    categoriesError.value = null;

    try {
      categories.value = await channelsService.getDistinctCategories();
    } catch (err) {
      console.error('Error fetching distinct categories:', err);
      categoriesError.value = err.message || 'Failed to fetch distinct categories';
    } finally {
      categoriesLoading.value = false;
    }
  };

  /**
   * Récupère les groupes distincts des chaînes
   */
  const fetchDistinctGroups = async () => {
    groupsLoading.value = true;
    groupsError.value = null;

    try {
      groups.value = await channelsService.getDistinctGroups();
    } catch (err) {
      console.error('Error fetching distinct groups:', err);
      groupsError.value = err.message || 'Failed to fetch distinct groups';
    } finally {
      groupsLoading.value = false;
    }
  };

  /**
   * Récupère les langues distinctes des chaînes
   */
  const fetchDistinctLanguages = async () => {
    languagesLoading.value = true;
    languagesError.value = null;

    try {
      languages.value = await channelsService.getDistinctLanguages();
    } catch (err) {
      console.error('Error fetching distinct languages:', err);
      languagesError.value = err.message || 'Failed to fetch distinct languages';
    } finally {
      languagesLoading.value = false;
    }
  };

  /**
   * Navigue vers la page précédente
   */
  const goToPreviousPage = async () => {
    if (pagination.value.hasPreviousPage) {
      const newPageNumber = pagination.value.pageNumber - 1;

      if (selectedCategory.value) {
        await fetchChannelsByCategory(selectedCategory.value, '', newPageNumber, pagination.value.pageSize);
      } else {
        await fetchChannels(newPageNumber, pagination.value.pageSize);
      }
    }
  };

  /**
   * Navigue vers la page suivante
   */
  const goToNextPage = async () => {
    if (pagination.value.hasNextPage) {
      const newPageNumber = pagination.value.pageNumber + 1;

      if (selectedCategory.value) {
        await fetchChannelsByCategory(selectedCategory.value, '', newPageNumber, pagination.value.pageSize);
      } else {
        await fetchChannels(newPageNumber, pagination.value.pageSize);
      }
    }
  };

  /**
   * Navigue vers une page spécifique
   * @param {number} pageNumber - Numéro de page (1-based)
   */
  const goToPage = async (pageNumber) => {
    if (pageNumber >= 1 && pageNumber <= pagination.value.totalPages) {
      if (selectedCategory.value) {
        await fetchChannelsByCategory(selectedCategory.value, '', pageNumber, pagination.value.pageSize);
      } else {
        await fetchChannels(pageNumber, pagination.value.pageSize);
      }
    }
  };

  /**
   * Change la taille de page et recharge les données
   * @param {number} newPageSize - Nouvelle taille de page
   */
  const changePageSize = async (newPageSize) => {
    if (newPageSize > 0) {
      if (selectedCategory.value) {
        await fetchChannelsByCategory(selectedCategory.value, '', 1, newPageSize);
      } else {
        await fetchChannels(1, newPageSize);
      }
    }
  };

  return {
    // État
    channels,
    loading,
    error,
    selectedCategory,
    categories,
    categoriesLoading,
    categoriesError,
    groups,
    groupsLoading,
    groupsError,
    languages,
    languagesLoading,
    languagesError,
    filteredChannels,
    pagination,

    // Méthodes
    fetchChannels,
    fetchChannel,
    fetchChannelsByCategory,
    searchChannels,
    importChannels,
    getRandomChannel,
    fetchDistinctCategories,
    fetchDistinctGroups,
    fetchDistinctLanguages,
    goToPreviousPage,
    goToNextPage,
    goToPage,
    changePageSize
  };
}
