/**
 * Utilitaires pour la gestion des tokens JWT
 */

/**
 * Décode un token JWT sans vérification de signature
 * @param {string} token - Token JWT à décoder
 * @returns {Object|null} - Payload décodé ou null si le token est invalide
 */
export const decodeToken = (token) => {
  if (!token) return null;
  
  try {
    // Un token JWT est composé de 3 parties séparées par des points: header.payload.signature
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    
    // Décoder la partie payload (2ème partie)
    const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));
    return payload;
  } catch (error) {
    console.error('Error decoding JWT token:', error);
    return null;
  }
};

/**
 * Vérifie si un token JWT est expiré
 * @param {string} token - Token JWT à vérifier
 * @returns {boolean} - True si le token est expiré ou invalide, false sinon
 */
export const isTokenExpired = (token) => {
  const payload = decodeToken(token);
  if (!payload || !payload.exp) return true;
  
  // La date d'expiration est en secondes depuis l'epoch, convertir en millisecondes
  const expirationDate = new Date(payload.exp * 1000);
  const currentDate = new Date();
  
  return currentDate >= expirationDate;
};

/**
 * Calcule le temps restant avant l'expiration d'un token JWT en millisecondes
 * @param {string} token - Token JWT à vérifier
 * @returns {number} - Temps restant en millisecondes, 0 si le token est expiré ou invalide
 */
export const getTokenRemainingTime = (token) => {
  const payload = decodeToken(token);
  if (!payload || !payload.exp) return 0;
  
  // La date d'expiration est en secondes depuis l'epoch, convertir en millisecondes
  const expirationDate = new Date(payload.exp * 1000);
  const currentDate = new Date();
  
  const remainingTime = expirationDate.getTime() - currentDate.getTime();
  return remainingTime > 0 ? remainingTime : 0;
};

/**
 * Vérifie si un token JWT doit être rafraîchi (proche de l'expiration)
 * @param {string} token - Token JWT à vérifier
 * @param {number} refreshThreshold - Seuil en millisecondes avant l'expiration pour rafraîchir le token (défaut: 5 minutes)
 * @returns {boolean} - True si le token doit être rafraîchi, false sinon
 */
export const shouldRefreshToken = (token, refreshThreshold = 5 * 60 * 1000) => {
  const remainingTime = getTokenRemainingTime(token);
  return remainingTime > 0 && remainingTime < refreshThreshold;
};

export default {
  decodeToken,
  isTokenExpired,
  getTokenRemainingTime,
  shouldRefreshToken
};
