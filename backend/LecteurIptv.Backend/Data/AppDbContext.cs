using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LecteurIptv.Backend.Data
{
    /// <summary>
    /// Contexte de base de données de l'application
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
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
        }
    }
}
