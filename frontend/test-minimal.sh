#!/bin/bash

echo "=== Test avec une application minimale ==="
echo "Date: $(date)"
echo ""

# Sauvegarder le fichier app.vue original
cp app.vue app.vue.backup

# Remplacer par l'application minimale
cp minimal-app.vue app.vue

echo "=== Lancement du serveur avec l'application minimale ==="
NODE_OPTIONS=--max-old-space-size=2048 npm run dev -- --no-clear

# Restaurer le fichier app.vue original
cp app.vue.backup app.vue
rm app.vue.backup
