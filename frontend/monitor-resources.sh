#!/bin/bash

echo "=== Surveillance des ressources système ==="
echo "Date: $(date)"
echo ""

echo "=== Utilisation de la mémoire avant le lancement ==="
free -h
echo ""

echo "=== Processus Node.js en cours d'exécution ==="
ps aux | grep node
echo ""

echo "=== Lancement du serveur et surveillance des ressources ==="
echo "Appuyez sur Ctrl+C pour arrêter la surveillance"
echo ""

# Lancer le serveur en arrière-plan
cd lecteur-iptv/frontend
NODE_OPTIONS=--max-old-space-size=2048 npm run dev -- --no-clear > nuxt-output.log 2>&1 &
NUXT_PID=$!

# Surveiller les ressources toutes les 5 secondes
while true; do
  echo "=== $(date) ==="
  echo "Mémoire:"
  free -h | grep "Mem:"
  echo "CPU pour le processus Nuxt (PID: $NUXT_PID):"
  ps -p $NUXT_PID -o %cpu,%mem,rss
  echo ""
  sleep 5
done
