# Workflow de développement

Ce document décrit le processus de développement pour le projet Lecteur IPTV.

## Branches Git

Le projet utilise le workflow Git Flow adapté :

- `main` : Code de production stable
- `develop` : Branche d'intégration principale
- `feature/*` : Nouvelles fonctionnalités
- `bugfix/*` : Corrections de bugs
- `release/*` : Préparation des versions

## Cycle de développement

1. **Planification** :
   - Création d'issues GitHub pour les fonctionnalités et bugs
   - Priorisation dans le tableau de projet

2. **Développement** :
   - Création d'une branche à partir de `develop`
   - Implémentation de la fonctionnalité ou correction
   - Tests unitaires et d'intégration

3. **Revue** :
   - Création d'une Pull Request vers `develop`
   - Revue de code par un autre développeur
   - Validation des tests automatisés

4. **Intégration** :
   - Merge dans `develop` après approbation
   - Tests d'intégration continus

5. **Release** :
   - Création d'une branche `release/X.Y.Z`
   - Tests finaux et corrections mineures
   - Merge dans `main` et tag de version

## CI/CD

Le projet utilise GitHub Actions pour l'intégration et le déploiement continus :

- **frontend-ci.yml** : Build et tests du frontend Vue/Nuxt
- **backend-ci.yml** : Build et tests du backend .NET

Les workflows sont déclenchés automatiquement sur :
- Push sur `main` et `develop`
- Pull Requests vers ces branches

## Standards de code

### Frontend (Vue/Nuxt)
- ESLint avec configuration Vue recommandée
- Prettier pour le formatage
- TypeScript strict mode

### Backend (.NET)
- .NET Code Style avec EditorConfig
- StyleCop pour l'analyse statique
- Tests unitaires avec xUnit

## Gestion des dépendances

Le projet utilise Dependabot pour maintenir les dépendances à jour :
- Vérification hebdomadaire des mises à jour npm
- Vérification hebdomadaire des mises à jour NuGet
- Vérification mensuelle des actions GitHub

## Déploiement

Le déploiement est géré via GitHub Actions :

1. **Environnement de développement** :
   - Déploiement automatique sur push vers `develop`
   - URL: dev.lecteur-iptv.example.com

2. **Environnement de staging** :
   - Déploiement automatique sur création d'une branche `release/*`
   - URL: staging.lecteur-iptv.example.com

3. **Production** :
   - Déploiement manuel après approbation sur push vers `main`
   - URL: lecteur-iptv.example.com
