#!/bin/bash

echo "=== Test sans les composables problématiques ==="
echo "Date: $(date)"
echo ""

# Créer un dossier de sauvegarde
mkdir -p composables-backup

# Sauvegarder les composables potentiellement problématiques
if [ -d "composables/api" ]; then
  cp -r composables/api composables-backup/
  rm -rf composables/api
fi

echo "=== Lancement du serveur sans les composables problématiques ==="
NODE_OPTIONS=--max-old-space-size=2048 npm run dev -- --no-clear

# Restaurer les composables
if [ -d "composables-backup/api" ]; then
  cp -r composables-backup/api composables/
  rm -rf composables-backup
fi
