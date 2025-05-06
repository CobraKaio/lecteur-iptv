# Migrations Entity Framework Core

Ce dossier contient les migrations Entity Framework Core pour le projet LecteurIptv.Backend. Les migrations permettent de gérer l'évolution du schéma de la base de données de manière contrôlée.

## Migrations existantes

1. **20250502163535_InitialCreate** : Migration initiale qui a créé la base de données avec toutes les tables nécessaires.
2. **20250503140717_AddBaseEntityToFavorites** : Ajout des propriétés CreatedAt et UpdatedAt aux tables UserFavoriteChannels et UserFavoriteVods.
3. **20250503140906_AddIsFeaturedToChannel** : Ajout de la propriété IsFeatured à la table Channels.

## Commandes utiles

### Créer une nouvelle migration

```bash
dotnet ef migrations add NomDeLaMigration
```

Cette commande crée une nouvelle migration basée sur les modifications apportées aux modèles depuis la dernière migration. Elle génère un fichier de migration dans le dossier Migrations avec deux méthodes principales :

- **Up()** : Contient les opérations pour appliquer la migration (créer des tables, ajouter des colonnes, etc.).
- **Down()** : Contient les opérations pour annuler la migration (supprimer des tables, supprimer des colonnes, etc.).

### Appliquer les migrations

```bash
dotnet ef database update
```

Cette commande applique toutes les migrations en attente à la base de données.

### Appliquer une migration spécifique

```bash
dotnet ef database update NomDeLaMigration
```

Cette commande applique les migrations jusqu'à la migration spécifiée (incluse).

### Revenir à une migration spécifique

```bash
dotnet ef database update NomDeLaMigration
```

Cette commande annule toutes les migrations après la migration spécifiée.

### Supprimer la dernière migration

```bash
dotnet ef migrations remove
```

Cette commande supprime la dernière migration. Utile si vous avez créé une migration mais que vous souhaitez la modifier avant de l'appliquer.

### Générer un script SQL

```bash
dotnet ef migrations script
```

Cette commande génère un script SQL pour toutes les migrations.

### Générer un script SQL pour une plage de migrations

```bash
dotnet ef migrations script MigrationDébut MigrationFin
```

Cette commande génère un script SQL pour les migrations entre MigrationDébut et MigrationFin.

## Bonnes pratiques

1. **Créer des migrations atomiques** : Chaque migration doit représenter un changement logique et cohérent du schéma de la base de données.
2. **Tester les migrations** : Avant de déployer une migration en production, testez-la dans un environnement de développement ou de test.
3. **Versionner les migrations** : Les fichiers de migration doivent être versionnés dans le système de contrôle de version (Git).
4. **Ne pas modifier les migrations existantes** : Une fois qu'une migration a été appliquée et partagée avec d'autres développeurs, ne la modifiez pas. Créez plutôt une nouvelle migration pour corriger les problèmes.
5. **Appliquer les migrations au démarrage** : En développement, il est courant d'appliquer automatiquement les migrations au démarrage de l'application.

## Application des migrations au démarrage

Dans le fichier Program.cs, vous pouvez ajouter le code suivant pour appliquer automatiquement les migrations au démarrage de l'application en mode développement :

```csharp
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
```

## Gestion des erreurs de migration

Si vous rencontrez des erreurs lors de l'application des migrations, voici quelques étapes à suivre :

1. **Vérifier les logs** : Les logs d'Entity Framework Core contiennent souvent des informations utiles sur les erreurs de migration.
2. **Vérifier le schéma de la base de données** : Assurez-vous que le schéma de la base de données correspond à ce que vous attendez.
3. **Vérifier les contraintes** : Les erreurs de migration sont souvent dues à des contraintes de clé étrangère ou d'unicité.
4. **Revenir à une migration précédente** : Si nécessaire, vous pouvez revenir à une migration précédente et créer une nouvelle migration pour corriger les problèmes.

## Migrations et SQLite

SQLite a certaines limitations en ce qui concerne les migrations :

1. **Renommer des colonnes** : SQLite ne prend pas en charge le renommage de colonnes. Entity Framework Core contourne cette limitation en créant une nouvelle table, en copiant les données, puis en supprimant l'ancienne table.
2. **Supprimer des colonnes** : SQLite ne prend pas en charge la suppression de colonnes. Entity Framework Core utilise la même approche que pour le renommage de colonnes.
3. **Modifier le type de colonnes** : SQLite a une gestion des types différente des autres SGBD. Assurez-vous que les types de colonnes sont compatibles.

## Migrations et production

En production, l'application des migrations doit être gérée avec soin :

1. **Sauvegarder la base de données** : Avant d'appliquer des migrations en production, sauvegardez toujours la base de données.
2. **Tester les migrations** : Testez les migrations dans un environnement de test avant de les appliquer en production.
3. **Planifier les migrations** : Planifiez les migrations pendant les périodes de faible activité.
4. **Utiliser des transactions** : Assurez-vous que les migrations sont appliquées dans une transaction pour éviter les états incohérents en cas d'erreur.
5. **Avoir un plan de rollback** : Préparez un plan pour revenir à l'état précédent en cas de problème.

## Ressources

- [Documentation officielle d'Entity Framework Core sur les migrations](https://docs.microsoft.com/fr-fr/ef/core/managing-schemas/migrations/)
- [Guide des migrations Entity Framework Core](https://www.learnentityframeworkcore.com/migrations)
- [Bonnes pratiques pour les migrations Entity Framework Core](https://docs.microsoft.com/fr-fr/ef/core/managing-schemas/migrations/operations)
