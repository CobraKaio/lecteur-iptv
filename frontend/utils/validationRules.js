/**
 * Règles de validation réutilisables pour les formulaires
 */

import { required, email, minLength, maxLength, helpers, sameAs } from '@vuelidate/validators';

/**
 * Règle de validation pour les noms d'utilisateur
 * - Doit contenir au moins 3 caractères
 * - Doit contenir au maximum 20 caractères
 * - Doit contenir uniquement des lettres, des chiffres, des tirets et des underscores
 */
export const usernameRules = {
  required: helpers.withMessage('Le nom d\'utilisateur est requis', required),
  minLength: helpers.withMessage('Le nom d\'utilisateur doit contenir au moins 3 caractères', minLength(3)),
  maxLength: helpers.withMessage('Le nom d\'utilisateur doit contenir au maximum 20 caractères', maxLength(20)),
  pattern: helpers.withMessage(
    'Le nom d\'utilisateur doit contenir uniquement des lettres, des chiffres, des tirets et des underscores',
    helpers.regex(/^[a-zA-Z0-9_-]+$/)
  )
};

/**
 * Règle de validation pour les adresses e-mail
 * - Doit être une adresse e-mail valide
 */
export const emailRules = {
  required: helpers.withMessage('L\'adresse e-mail est requise', required),
  email: helpers.withMessage('L\'adresse e-mail n\'est pas valide', email)
};

/**
 * Règle de validation pour les mots de passe
 * - Doit contenir au moins 8 caractères
 * - Doit contenir au moins une lettre minuscule
 * - Doit contenir au moins une lettre majuscule
 * - Doit contenir au moins un chiffre ou un caractère spécial
 */
export const passwordRules = {
  required: helpers.withMessage('Le mot de passe est requis', required),
  minLength: helpers.withMessage('Le mot de passe doit contenir au moins 8 caractères', minLength(8)),
  hasLowercase: helpers.withMessage(
    'Le mot de passe doit contenir au moins une lettre minuscule',
    helpers.regex(/[a-z]/)
  ),
  hasUppercase: helpers.withMessage(
    'Le mot de passe doit contenir au moins une lettre majuscule',
    helpers.regex(/[A-Z]/)
  ),
  hasNumber: helpers.withMessage(
    'Le mot de passe doit contenir au moins un chiffre ou un caractère spécial',
    helpers.regex(/[0-9]|[^a-zA-Z0-9]/)
  )
};

/**
 * Règle de validation pour la confirmation de mot de passe
 * @param {string} passwordPath - Chemin vers le mot de passe à confirmer
 * @returns {Object} Règles de validation
 */
export const confirmPasswordRules = (passwordPath) => ({
  required: helpers.withMessage('La confirmation du mot de passe est requise', required),
  sameAs: helpers.withMessage(
    'Les mots de passe ne correspondent pas',
    sameAs(passwordPath)
  )
});

/**
 * Règle de validation pour les noms et prénoms
 * - Doit contenir au moins 2 caractères
 * - Doit contenir au maximum 50 caractères
 * - Doit contenir uniquement des lettres, des espaces, des tirets et des apostrophes
 */
export const nameRules = {
  required: helpers.withMessage('Ce champ est requis', required),
  minLength: helpers.withMessage('Ce champ doit contenir au moins 2 caractères', minLength(2)),
  maxLength: helpers.withMessage('Ce champ doit contenir au maximum 50 caractères', maxLength(50)),
  pattern: helpers.withMessage(
    'Ce champ doit contenir uniquement des lettres, des espaces, des tirets et des apostrophes',
    helpers.regex(/^[a-zA-ZÀ-ÿ\s'-]+$/)
  )
};

/**
 * Règle de validation pour les URL
 * - Doit être une URL valide
 */
export const urlRules = {
  required: helpers.withMessage('L\'URL est requise', required),
  url: helpers.withMessage(
    'L\'URL n\'est pas valide',
    helpers.regex(/^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*\/?$/)
  )
};

export default {
  usernameRules,
  emailRules,
  passwordRules,
  confirmPasswordRules,
  nameRules,
  urlRules
};
