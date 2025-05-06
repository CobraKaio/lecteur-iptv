/**
 * Composable pour gérer les contenus VOD
 * Ce composable utilise le service vodService pour récupérer et manipuler les contenus VOD
 */

import { ref, computed } from 'vue';
import * as vodService from '~/services/vodService';

export function useVod() {
  // État
  const vodItems = ref([]);
  const loading = ref(false);
  const error = ref(null);
  const selectedCategory = ref(null);
  const categories = ref([]);
  const categoriesLoading = ref(false);
  const categoriesError = ref(null);
  const types = ref([]);
  const typesLoading = ref(false);
  const typesError = ref(null);
  const languages = ref([]);
  const languagesLoading = ref(false);
  const languagesError = ref(null);
  const years = ref([]);
  const yearsLoading = ref(false);
  const yearsError = ref(null);

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
   * Récupère tous les contenus VOD avec pagination
   * @param {number} pageNumber - Numéro de page (1-based)
   * @param {number} pageSize - Nombre d'éléments par page
   */
  const fetchVodItems = async (pageNumber = pagination.value.pageNumber, pageSize = pagination.value.pageSize) => {
    loading.value = true;
    error.value = null;

    try {
      // Appeler le service avec les paramètres de pagination
      const result = await vodService.getVodItems(pageNumber, pageSize);

      // Mettre à jour les contenus VOD et l'état de pagination
      vodItems.value = result.items;
      pagination.value = {
        pageNumber: result.pageNumber,
        pageSize: result.pageSize,
        totalCount: result.totalCount,
        totalPages: result.totalPages,
        hasPreviousPage: result.hasPreviousPage,
        hasNextPage: result.hasNextPage
      };

      // Si les catégories n'ont pas encore été chargées, les extraire des contenus VOD
      if (categories.value.length === 0) {
        extractCategoriesFromVodItems();
      }
    } catch (err) {
      console.error('Error fetching VOD items:', err);
      error.value = err.message || 'Failed to fetch VOD items';
    } finally {
      loading.value = false;
    }
  };

  /**
   * Extrait les catégories uniques des contenus VOD
   */
  const extractCategoriesFromVodItems = () => {
    const uniqueCategories = new Set();
    vodItems.value.forEach(item => {
      if (item.category) uniqueCategories.add(item.category);
      if (item.group) uniqueCategories.add(item.group);
    });
    categories.value = Array.from(uniqueCategories).sort();
  };

  /**
   * Récupère un contenu VOD par son ID
   * @param {number} id - ID du contenu VOD
   * @returns {Promise<Object>} Contenu VOD
   */
  const fetchVodItem = async (id) => {
    loading.value = true;
    error.value = null;

    try {
      return await vodService.getVodItem(id);
    } catch (err) {
      console.error(`Error fetching VOD item ${id}:`, err);
      error.value = err.message || `Failed to fetch VOD item with ID: ${id}`;
      return null;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Récupère les contenus VOD par catégorie avec pagination
   * @param {string} category - Catégorie des contenus VOD
   * @param {string} query - Terme de recherche optionnel
   * @param {number} pageNumber - Numéro de page (1-based)
   * @param {number} pageSize - Nombre d'éléments par page
   */
  const fetchVodItemsByCategory = async (
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
        const result = await vodService.searchVodItems(query, pageNumber, pageSize);
        vodItems.value = result.items.filter(item =>
          item.category === category || item.group === category
        );

        // Mettre à jour l'état de pagination (mais avec le nombre filtré)
        pagination.value = {
          ...pagination.value,
          pageNumber: result.pageNumber,
          pageSize: result.pageSize,
          totalCount: vodItems.value.length, // Nombre après filtrage
          totalPages: Math.ceil(vodItems.value.length / result.pageSize),
          hasPreviousPage: pageNumber > 1,
          hasNextPage: pageNumber < Math.ceil(vodItems.value.length / result.pageSize)
        };
      } else {
        // Sinon on filtre juste par catégorie
        const result = await vodService.getVodItemsByCategory(category, pageNumber, pageSize);
        vodItems.value = result.items;

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
      console.error(`Error fetching VOD items by category ${category}:`, err);
      error.value = err.message || `Failed to fetch VOD items for category: ${category}`;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Recherche des contenus VOD avec pagination
   * @param {string} query - Terme de recherche
   * @param {string} category - Catégorie optionnelle pour filtrer les résultats
   * @param {number} pageNumber - Numéro de page (1-based)
   * @param {number} pageSize - Nombre d'éléments par page
   */
  const searchVodItems = async (
    query,
    category = '',
    pageNumber = pagination.value.pageNumber,
    pageSize = pagination.value.pageSize
  ) => {
    if (query.length === 0) {
      // Si la recherche est vide, on revient à la liste complète ou filtrée par catégorie
      if (category) {
        return await fetchVodItemsByCategory(category, '', pageNumber, pageSize);
      } else {
        return await fetchVodItems(pageNumber, pageSize);
      }
    }

    if (query.length < 3) return;

    loading.value = true;
    error.value = null;

    try {
      const result = await vodService.searchVodItems(query, pageNumber, pageSize);

      // Si une catégorie est spécifiée, on filtre les résultats
      if (category) {
        vodItems.value = result.items.filter(item =>
          item.category === category || item.group === category
        );

        // Mettre à jour l'état de pagination (mais avec le nombre filtré)
        pagination.value = {
          ...pagination.value,
          pageNumber: result.pageNumber,
          pageSize: result.pageSize,
          totalCount: vodItems.value.length, // Nombre après filtrage
          totalPages: Math.ceil(vodItems.value.length / result.pageSize),
          hasPreviousPage: pageNumber > 1,
          hasNextPage: pageNumber < Math.ceil(vodItems.value.length / result.pageSize)
        };
      } else {
        vodItems.value = result.items;

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
      console.error(`Error searching VOD items with query ${query}:`, err);
      error.value = err.message || `Failed to search VOD items with query: ${query}`;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Importe des contenus VOD à partir d'un fichier M3U
   * @param {string} url - URL du fichier M3U
   * @returns {Promise<Object>} Résultat de l'importation
   */
  const importVodItems = async (url) => {
    loading.value = true;
    error.value = null;

    try {
      const result = await vodService.importVodItems(url);
      // Rafraîchir la liste des contenus VOD après l'importation
      await fetchVodItems();
      return result;
    } catch (err) {
      console.error(`Error importing VOD items from ${url}:`, err);
      error.value = err.message || `Failed to import VOD items from URL: ${url}`;
      return null;
    } finally {
      loading.value = false;
    }
  };

  /**
   * Contenus VOD filtrés par catégorie sélectionnée
   */
  const filteredVodItems = computed(() => {
    if (!selectedCategory.value) {
      return vodItems.value;
    }

    return vodItems.value.filter(item =>
      item.category === selectedCategory.value ||
      item.group === selectedCategory.value
    );
  });

  /**
   * Obtient les films (contenus VOD de type "movie")
   */
  const movies = computed(() => {
    return vodItems.value.filter(item => item.type === 'movie');
  });

  /**
   * Obtient les séries (contenus VOD de type "series")
   */
  const series = computed(() => {
    return vodItems.value.filter(item => item.type === 'series');
  });

  /**
   * Obtient les contenus VOD récemment ajoutés (30 derniers jours)
   */
  const recentlyAdded = computed(() => {
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);

    return vodItems.value.filter(item => {
      const createdAt = new Date(item.createdAt);
      return createdAt >= thirtyDaysAgo;
    });
  });

  /**
   * Récupère les catégories distinctes des contenus VOD
   */
  const fetchDistinctCategories = async () => {
    categoriesLoading.value = true;
    categoriesError.value = null;

    try {
      categories.value = await vodService.getDistinctCategories();
    } catch (err) {
      console.error('Error fetching distinct categories:', err);
      categoriesError.value = err.message || 'Failed to fetch distinct categories';
    } finally {
      categoriesLoading.value = false;
    }
  };

  /**
   * Récupère les types distincts des contenus VOD
   */
  const fetchDistinctTypes = async () => {
    typesLoading.value = true;
    typesError.value = null;

    try {
      types.value = await vodService.getDistinctTypes();
    } catch (err) {
      console.error('Error fetching distinct types:', err);
      typesError.value = err.message || 'Failed to fetch distinct types';
    } finally {
      typesLoading.value = false;
    }
  };

  /**
   * Récupère les langues distinctes des contenus VOD
   */
  const fetchDistinctLanguages = async () => {
    languagesLoading.value = true;
    languagesError.value = null;

    try {
      languages.value = await vodService.getDistinctLanguages();
    } catch (err) {
      console.error('Error fetching distinct languages:', err);
      languagesError.value = err.message || 'Failed to fetch distinct languages';
    } finally {
      languagesLoading.value = false;
    }
  };

  /**
   * Récupère les années distinctes des contenus VOD
   */
  const fetchDistinctYears = async () => {
    yearsLoading.value = true;
    yearsError.value = null;

    try {
      years.value = await vodService.getDistinctYears();
    } catch (err) {
      console.error('Error fetching distinct years:', err);
      yearsError.value = err.message || 'Failed to fetch distinct years';
    } finally {
      yearsLoading.value = false;
    }
  };

  /**
   * Navigue vers la page précédente
   */
  const goToPreviousPage = async () => {
    if (pagination.value.hasPreviousPage) {
      const newPageNumber = pagination.value.pageNumber - 1;

      if (selectedCategory.value) {
        await fetchVodItemsByCategory(selectedCategory.value, '', newPageNumber, pagination.value.pageSize);
      } else {
        await fetchVodItems(newPageNumber, pagination.value.pageSize);
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
        await fetchVodItemsByCategory(selectedCategory.value, '', newPageNumber, pagination.value.pageSize);
      } else {
        await fetchVodItems(newPageNumber, pagination.value.pageSize);
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
        await fetchVodItemsByCategory(selectedCategory.value, '', pageNumber, pagination.value.pageSize);
      } else {
        await fetchVodItems(pageNumber, pagination.value.pageSize);
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
        await fetchVodItemsByCategory(selectedCategory.value, '', 1, newPageSize);
      } else {
        await fetchVodItems(1, newPageSize);
      }
    }
  };

  return {
    // État
    vodItems,
    loading,
    error,
    selectedCategory,
    categories,
    categoriesLoading,
    categoriesError,
    types,
    typesLoading,
    typesError,
    languages,
    languagesLoading,
    languagesError,
    years,
    yearsLoading,
    yearsError,
    filteredVodItems,
    movies,
    series,
    recentlyAdded,
    pagination,

    // Méthodes
    fetchVodItems,
    fetchVodItem,
    fetchVodItemsByCategory,
    searchVodItems,
    importVodItems,
    fetchDistinctCategories,
    fetchDistinctTypes,
    fetchDistinctLanguages,
    fetchDistinctYears,
    goToPreviousPage,
    goToNextPage,
    goToPage,
    changePageSize
  };
}
