// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  devtools: { enabled: false },

  // Modules
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt'
  ],

  // Application configuration
  app: {
    head: {
      title: 'Lecteur IPTV',
      meta: [
        { name: 'description', content: 'Application de lecture de flux IPTV et VOD' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
      ]
    }
  },

  // Runtime config for API endpoints
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5151/api'
    }
  },

  // Build configuration
  build: {
    transpile: ['video.js']
  },

  // Server configuration
  server: {
    host: '0.0.0.0', // Écouter sur toutes les interfaces réseau
    port: 3001 // Port par défaut
  }
})
