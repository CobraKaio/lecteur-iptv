#!/bin/bash

echo "=== Diagnostic du serveur de développement Nuxt ==="
echo "Date: $(date)"
echo "Node version: $(node -v)"
echo "NPM version: $(npm -v)"
echo ""

echo "=== Vérification des dépendances ==="
echo "Nuxt version: $(grep '"nuxt":' package.json)"
echo "Vue version: $(grep '"vue":' package.json)"
echo ""

echo "=== Lancement du serveur avec options de diagnostic ==="
echo "NODE_OPTIONS=--max-old-space-size=2048 npm run dev -- --no-clear"
NODE_OPTIONS=--max-old-space-size=2048 npm run dev -- --no-clear
