using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LecteurIptv.Backend.Data
{
    /// <summary>
    /// Contexte de base de données de l'application
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructeur du contexte de base de données
        /// </summary>
        /// <param name="options">Options du contexte</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Configure le comportement de suivi des entités par défaut
            // NoTracking améliore les performances pour les requêtes en lecture seule
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Chaînes de télévision
        /// </summary>
        public DbSet<Channel> Channels { get; set; } = null!;

        /// <summary>
        /// Programmes TV
        /// </summary>
        public DbSet<TvProgram> Programs { get; set; } = null!;

        /// <summary>
        /// Éléments VOD
        /// </summary>
        public DbSet<VodItem> VodItems { get; set; } = null!;

        /// <summary>
        /// Utilisateurs
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Chaînes favorites des utilisateurs
        /// </summary>
        public DbSet<UserFavoriteChannel> UserFavoriteChannels { get; set; } = null!;

        /// <summary>
        /// Éléments VOD favoris des utilisateurs
        /// </summary>
        public DbSet<UserFavoriteVod> UserFavoriteVods { get; set; } = null!;

        /// <summary>
        /// Historique de visionnage des utilisateurs
        /// </summary>
        public DbSet<UserHistory> UserHistory { get; set; } = null!;

        /// <summary>
        /// Configuration des modèles
        /// </summary>
        /// <param name="modelBuilder">Builder de modèle</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<TvProgram>()
                .HasOne(p => p.Channel)
                .WithMany(c => c.Programs)
                .HasForeignKey(p => p.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoriteChannel>()
                .HasOne(ufc => ufc.User)
                .WithMany(u => u.FavoriteChannels)
                .HasForeignKey(ufc => ufc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoriteChannel>()
                .HasOne(ufc => ufc.Channel)
                .WithMany()
                .HasForeignKey(ufc => ufc.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoriteVod>()
                .HasOne(ufv => ufv.User)
                .WithMany(u => u.FavoriteVods)
                .HasForeignKey(ufv => ufv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoriteVod>()
                .HasOne(ufv => ufv.VodItem)
                .WithMany()
                .HasForeignKey(ufv => ufv.VodItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserHistory>()
                .HasOne(uh => uh.User)
                .WithMany(u => u.ViewingHistory)
                .HasForeignKey(uh => uh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            modelBuilder.Entity<Channel>()
                .HasIndex(c => c.TvgId);

            modelBuilder.Entity<TvProgram>()
                .HasIndex(p => p.StartTime);

            modelBuilder.Entity<TvProgram>()
                .HasIndex(p => p.EndTime);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserHistory>()
                .HasIndex(uh => uh.UserId);

            modelBuilder.Entity<UserHistory>()
                .HasIndex(uh => uh.ViewedAt);
        }

        /// <summary>
        /// Méthode appelée avant la sauvegarde des changements
        /// </summary>
        /// <param name="cancellationToken">Token d'annulation</param>
        /// <returns>Nombre d'entités modifiées</returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Met à jour automatiquement les dates de création et de modification
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    var now = DateTime.UtcNow;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedAt = now;
                    }

                    entity.UpdatedAt = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Méthode appelée avant la sauvegarde des changements
        /// </summary>
        /// <returns>Nombre d'entités modifiées</returns>
        public override int SaveChanges()
        {
            // Utilise la version asynchrone pour la cohérence
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
