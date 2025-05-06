/**
 * Utilitaires pour la protection CSRF
 */

/**
 * Récupère le token CSRF depuis les cookies
 * @returns {string|null} Token CSRF ou null si non trouvé
 */
export const getCsrfToken = () => {
  if (!process.client) return null;
  
  // Récupérer tous les cookies
  const cookies = document.cookie.split(';');
  
  // Chercher le cookie XSRF-TOKEN
  for (let i = 0; i < cookies.length; i++) {
    const cookie = cookies[i].trim();
    if (cookie.startsWith('XSRF-TOKEN=')) {
      return cookie.substring('XSRF-TOKEN='.length, cookie.length);
    }
  }
  
  return null;
};

/**
 * Vérifie si le token CSRF est présent
 * @returns {boolean} True si le token CSRF est présent, false sinon
 */
export const hasCsrfToken = () => {
  return !!getCsrfToken();
};

/**
 * Récupère le token CSRF depuis le serveur
 * @param {string} baseUrl - URL de base de l'API
 * @returns {Promise<boolean>} True si le token a été récupéré avec succès, false sinon
 */
export const fetchCsrfToken = async (baseUrl) => {
  if (!process.client) return false;
  
  try {
    // Appeler l'endpoint CSRF pour obtenir un nouveau token
    // Cet endpoint doit être implémenté côté serveur pour générer un token CSRF
    // et le stocker dans un cookie HttpOnly
    const response = await fetch(`${baseUrl}/csrf-token`, {
      method: 'GET',
      credentials: 'include' // Important pour inclure les cookies
    });
    
    if (!response.ok) {
      console.error('Failed to fetch CSRF token:', response.status, response.statusText);
      return false;
    }
    
    // Le token est automatiquement stocké dans les cookies par le serveur
    return true;
  } catch (error) {
    console.error('Error fetching CSRF token:', error);
    return false;
  }
};

export default {
  getCsrfToken,
  hasCsrfToken,
  fetchCsrfToken
};
