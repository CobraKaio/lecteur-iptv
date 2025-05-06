<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8 bg-white dark:bg-gray-800 p-8 rounded-lg shadow-md">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white">
          Créer un nouveau compte
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600 dark:text-gray-400">
          Ou
          <NuxtLink to="/auth/login" class="font-medium text-blue-600 hover:text-blue-500 dark:text-blue-400 dark:hover:text-blue-300">
            connectez-vous à votre compte existant
          </NuxtLink>
        </p>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="handleRegister">
        <!-- Alerte d'erreur -->
        <div v-if="error" class="bg-red-100 dark:bg-red-900 border border-red-400 text-red-700 dark:text-red-300 px-4 py-3 rounded relative" role="alert">
          <div class="flex items-center">
            <svg class="w-5 h-5 mr-2 text-red-500" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"></path>
            </svg>
            <span class="block sm:inline">{{ error }}</span>
          </div>
          <button @click="clearError" class="absolute top-0 bottom-0 right-0 px-4 py-3">
            <svg class="h-4 w-4 text-red-500" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
              <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path>
            </svg>
          </button>
        </div>

        <div class="rounded-md shadow-sm space-y-4">
          <div>
            <label for="username" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Nom d'utilisateur
            </label>
            <input
              id="username"
              v-model="form.username"
              name="username"
              type="text"
              autocomplete="username"
              required
              @blur="v$.username.$touch()"
              :class="[
                'mt-1 appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white sm:text-sm',
                v$.username.$error ? 'border-red-500 dark:border-red-500' : 'border-gray-300 dark:border-gray-700'
              ]"
              placeholder="Nom d'utilisateur"
            />
            <div v-if="v$.username.$error" class="text-red-500 text-xs mt-1">
              {{ v$.username.$errors[0].$message }}
            </div>
          </div>

          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Adresse e-mail
            </label>
            <input
              id="email"
              v-model="form.email"
              name="email"
              type="email"
              autocomplete="email"
              required
              @blur="v$.email.$touch()"
              :class="[
                'mt-1 appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white sm:text-sm',
                v$.email.$error ? 'border-red-500 dark:border-red-500' : 'border-gray-300 dark:border-gray-700'
              ]"
              placeholder="Adresse e-mail"
            />
            <div v-if="v$.email.$error" class="text-red-500 text-xs mt-1">
              {{ v$.email.$errors[0].$message }}
            </div>
          </div>

          <div>
            <label for="firstName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Prénom
            </label>
            <input
              id="firstName"
              v-model="form.firstName"
              name="firstName"
              type="text"
              autocomplete="given-name"
              required
              @blur="v$.firstName.$touch()"
              :class="[
                'mt-1 appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white sm:text-sm',
                v$.firstName.$error ? 'border-red-500 dark:border-red-500' : 'border-gray-300 dark:border-gray-700'
              ]"
              placeholder="Prénom"
            />
            <div v-if="v$.firstName.$error" class="text-red-500 text-xs mt-1">
              {{ v$.firstName.$errors[0].$message }}
            </div>
          </div>

          <div>
            <label for="lastName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Nom
            </label>
            <input
              id="lastName"
              v-model="form.lastName"
              name="lastName"
              type="text"
              autocomplete="family-name"
              required
              @blur="v$.lastName.$touch()"
              :class="[
                'mt-1 appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white sm:text-sm',
                v$.lastName.$error ? 'border-red-500 dark:border-red-500' : 'border-gray-300 dark:border-gray-700'
              ]"
              placeholder="Nom"
            />
            <div v-if="v$.lastName.$error" class="text-red-500 text-xs mt-1">
              {{ v$.lastName.$errors[0].$message }}
            </div>
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Mot de passe
            </label>
            <input
              id="password"
              v-model="form.password"
              name="password"
              type="password"
              autocomplete="new-password"
              required
              @blur="v$.password.$touch()"
              :class="[
                'mt-1 appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white sm:text-sm',
                v$.password.$error ? 'border-red-500 dark:border-red-500' : 'border-gray-300 dark:border-gray-700'
              ]"
              placeholder="Mot de passe"
            />
            <div v-if="v$.password.$error" class="text-red-500 text-xs mt-1">
              {{ v$.password.$errors[0].$message }}
            </div>
          </div>

          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Confirmer le mot de passe
            </label>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              name="confirmPassword"
              type="password"
              autocomplete="new-password"
              required
              @blur="v$.confirmPassword.$touch()"
              :class="[
                'mt-1 appearance-none block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white sm:text-sm',
                v$.confirmPassword.$error ? 'border-red-500 dark:border-red-500' : 'border-gray-300 dark:border-gray-700'
              ]"
              placeholder="Confirmer le mot de passe"
            />
            <div v-if="v$.confirmPassword.$error" class="text-red-500 text-xs mt-1">
              {{ v$.confirmPassword.$errors[0].$message }}
            </div>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading || !isFormValid"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="loading" class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            <span v-else class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg class="h-5 w-5 text-blue-500 group-hover:text-blue-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
              </svg>
            </span>
            {{ loading ? 'Création en cours...' : 'Créer un compte' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useAuth } from '~/composables/useAuth';
import { useRouter } from 'vue-router';
import { useVuelidate } from '@vuelidate/core';
import { usernameRules, emailRules, passwordRules, confirmPasswordRules, nameRules } from '~/utils/validationRules';

// Récupérer le composable d'authentification
const { register, loading, error: authError } = useAuth();
const router = useRouter();
const error = authError; // Utiliser la référence réactive du composable

// État du formulaire
const form = ref({
  username: '',
  email: '',
  firstName: '',
  lastName: '',
  password: '',
  confirmPassword: ''
});

// Règles de validation
const rules = {
  username: usernameRules,
  email: emailRules,
  firstName: nameRules,
  lastName: nameRules,
  password: passwordRules,
  confirmPassword: confirmPasswordRules(() => form.value.password)
};

// Initialiser Vuelidate
const v$ = useVuelidate(rules, form);

// Effacer le message d'erreur
const clearError = () => {
  error.value = null;
};

// Validation du formulaire
const isFormValid = computed(() => {
  return !v$.value.$invalid;
});

// Gérer la soumission du formulaire
const handleRegister = async () => {
  // Effacer les erreurs précédentes
  clearError();

  // Valider le formulaire
  const isValid = await v$.value.$validate();
  if (!isValid) {
    // Afficher un message d'erreur global
    error.value = 'Veuillez corriger les erreurs dans le formulaire';
    return;
  }

  try {
    // Préparer les données d'inscription
    const userData = {
      username: form.value.username,
      email: form.value.email,
      firstName: form.value.firstName,
      lastName: form.value.lastName,
      password: form.value.password
    };

    // Appeler la méthode register du composable
    await register(userData);

    // Rediriger vers la page de connexion après l'inscription
    router.push('/auth/login?registered=true');
  } catch (err) {
    // L'erreur est déjà gérée par le composable
    console.error('Registration failed:', err);

    // Réinitialiser la validation
    v$.value.$reset();
  }
};
</script>
