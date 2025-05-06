using LecteurIptv.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LecteurIptv.Backend.Data
{
    /// <summary>
    /// Classe d'initialisation de la base de données
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Initialise la base de données avec des données de test
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        public static async Task InitializeAsync(AppDbContext context)
        {
            // S'assure que la base de données est créée
            await context.Database.EnsureCreatedAsync();

            // Vérifie si la base de données contient déjà des données
            if (await context.Channels.AnyAsync())
            {
                return; // La base de données a déjà été initialisée
            }

            // Ajoute des chaînes de test
            var channels = new List<Channel>
            {
                new Channel
                {
                    Name = "France 2",
                    StreamUrl = "https://example.com/france2.m3u8",
                    LogoUrl = "https://example.com/logos/france2.png",
                    TvgId = "france2.fr",
                    TvgName = "France 2",
                    Group = "France TV",
                    Category = "Généraliste",
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new Channel
                {
                    Name = "France 3",
                    StreamUrl = "https://example.com/france3.m3u8",
                    LogoUrl = "https://example.com/logos/france3.png",
                    TvgId = "france3.fr",
                    TvgName = "France 3",
                    Group = "France TV",
                    Category = "Généraliste",
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    DisplayOrder = 2
                },
                new Channel
                {
                    Name = "France 5",
                    StreamUrl = "https://example.com/france5.m3u8",
                    LogoUrl = "https://example.com/logos/france5.png",
                    TvgId = "france5.fr",
                    TvgName = "France 5",
                    Group = "France TV",
                    Category = "Documentaire",
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    DisplayOrder = 3
                },
                new Channel
                {
                    Name = "M6",
                    StreamUrl = "https://example.com/m6.m3u8",
                    LogoUrl = "https://example.com/logos/m6.png",
                    TvgId = "m6.fr",
                    TvgName = "M6",
                    Group = "RTL Group",
                    Category = "Généraliste",
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    DisplayOrder = 4
                },
                new Channel
                {
                    Name = "Arte",
                    StreamUrl = "https://example.com/arte.m3u8",
                    LogoUrl = "https://example.com/logos/arte.png",
                    TvgId = "arte.fr",
                    TvgName = "Arte",
                    Group = "Arte Group",
                    Category = "Culture",
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    DisplayOrder = 5
                },
                new Channel
                {
                    Name = "BBC One",
                    StreamUrl = "https://example.com/bbcone.m3u8",
                    LogoUrl = "https://example.com/logos/bbcone.png",
                    TvgId = "bbcone.uk",
                    TvgName = "BBC One",
                    Group = "BBC",
                    Category = "Généraliste",
                    Language = "en",
                    Country = "UK",
                    IsActive = true,
                    DisplayOrder = 6
                },
                new Channel
                {
                    Name = "BBC Two",
                    StreamUrl = "https://example.com/bbctwo.m3u8",
                    LogoUrl = "https://example.com/logos/bbctwo.png",
                    TvgId = "bbctwo.uk",
                    TvgName = "BBC Two",
                    Group = "BBC",
                    Category = "Généraliste",
                    Language = "en",
                    Country = "UK",
                    IsActive = true,
                    DisplayOrder = 7
                },
                new Channel
                {
                    Name = "CNN",
                    StreamUrl = "https://example.com/cnn.m3u8",
                    LogoUrl = "https://example.com/logos/cnn.png",
                    TvgId = "cnn.com",
                    TvgName = "CNN",
                    Group = "News",
                    Category = "Actualités",
                    Language = "en",
                    Country = "US",
                    IsActive = true,
                    DisplayOrder = 8
                },
                new Channel
                {
                    Name = "Eurosport",
                    StreamUrl = "https://example.com/eurosport.m3u8",
                    LogoUrl = "https://example.com/logos/eurosport.png",
                    TvgId = "eurosport.fr",
                    TvgName = "Eurosport",
                    Group = "Sport",
                    Category = "Sport",
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    DisplayOrder = 9
                },
                new Channel
                {
                    Name = "Canal+",
                    StreamUrl = "https://example.com/canalplus.m3u8",
                    LogoUrl = "https://example.com/logos/canalplus.png",
                    TvgId = "canalplus.fr",
                    TvgName = "Canal+",
                    Group = "Canal",
                    Category = "Cinéma",
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    DisplayOrder = 10
                }
            };

            await context.Channels.AddRangeAsync(channels);
            await context.SaveChangesAsync();

            // Ajoute des programmes TV de test
            var now = DateTime.UtcNow;
            var programs = new List<TvProgram>();

            foreach (var channel in channels)
            {
                // Programme actuel
                programs.Add(new TvProgram
                {
                    ChannelId = channel.Id,
                    Title = $"Programme actuel sur {channel.Name}",
                    Description = $"Description du programme actuel sur {channel.Name}",
                    StartTime = now.AddHours(-1),
                    EndTime = now.AddHours(1),
                    Category = "Divers",
                    ImageUrl = $"https://example.com/programs/{channel.Name.ToLower().Replace(" ", "")}_current.jpg",
                    Language = channel.Language
                });

                // Programmes à venir
                for (int i = 1; i <= 5; i++)
                {
                    programs.Add(new TvProgram
                    {
                        ChannelId = channel.Id,
                        Title = $"Programme {i} sur {channel.Name}",
                        Description = $"Description du programme {i} sur {channel.Name}",
                        StartTime = now.AddHours(i * 2 - 1),
                        EndTime = now.AddHours(i * 2 + 1),
                        Category = i % 3 == 0 ? "Film" : (i % 3 == 1 ? "Série" : "Documentaire"),
                        ImageUrl = $"https://example.com/programs/{channel.Name.ToLower().Replace(" ", "")}_program{i}.jpg",
                        Language = channel.Language,
                        Year = 2020 + (i % 3),
                        Country = channel.Country,
                        Actors = i % 3 == 0 ? "Acteur 1, Acteur 2, Acteur 3" : "",
                        Director = i % 3 == 0 ? "Réalisateur" : "",
                        Episode = i % 3 == 1 ? $"S01E0{i}" : "",
                        Season = i % 3 == 1 ? "Saison 1" : ""
                    });
                }
            }

            await context.Programs.AddRangeAsync(programs);
            await context.SaveChangesAsync();

            // Ajoute des éléments VOD de test
            var vodItems = new List<VodItem>
            {
                new VodItem
                {
                    Title = "Film 1",
                    Description = "Description du film 1",
                    StreamUrl = "https://example.com/vod/film1.mp4",
                    ImageUrl = "https://example.com/vod/thumbnails/film1.jpg",
                    Category = "Film",
                    Year = 2021,
                    Duration = 120,
                    Director = "Réalisateur 1",
                    Actors = "Acteur 1, Acteur 2, Acteur 3",
                    Rating = 4.5m,
                    Language = "fr",
                    Country = "FR",
                    IsActive = true
                },
                new VodItem
                {
                    Title = "Film 2",
                    Description = "Description du film 2",
                    StreamUrl = "https://example.com/vod/film2.mp4",
                    ImageUrl = "https://example.com/vod/thumbnails/film2.jpg",
                    Category = "Film",
                    Year = 2022,
                    Duration = 110,
                    Director = "Réalisateur 2",
                    Actors = "Acteur 4, Acteur 5, Acteur 6",
                    Rating = 4.2m,
                    Language = "en",
                    Country = "US",
                    IsActive = true
                },
                new VodItem
                {
                    Title = "Série 1 - S01E01",
                    Description = "Description de l'épisode 1 de la série 1",
                    StreamUrl = "https://example.com/vod/serie1_s01e01.mp4",
                    ImageUrl = "https://example.com/vod/thumbnails/serie1_s01e01.jpg",
                    Category = "Série",
                    Year = 2020,
                    Duration = 45,
                    Director = "Réalisateur 3",
                    Actors = "Acteur 7, Acteur 8, Acteur 9",
                    Rating = 4.8m,
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    Season = "Saison 1",
                    Episode = "S01E01"
                },
                new VodItem
                {
                    Title = "Série 1 - S01E02",
                    Description = "Description de l'épisode 2 de la série 1",
                    StreamUrl = "https://example.com/vod/serie1_s01e02.mp4",
                    ImageUrl = "https://example.com/vod/thumbnails/serie1_s01e02.jpg",
                    Category = "Série",
                    Year = 2020,
                    Duration = 45,
                    Director = "Réalisateur 3",
                    Actors = "Acteur 7, Acteur 8, Acteur 9",
                    Rating = 4.7m,
                    Language = "fr",
                    Country = "FR",
                    IsActive = true,
                    Season = "Saison 1",
                    Episode = "S01E02"
                },
                new VodItem
                {
                    Title = "Documentaire 1",
                    Description = "Description du documentaire 1",
                    StreamUrl = "https://example.com/vod/documentaire1.mp4",
                    ImageUrl = "https://example.com/vod/thumbnails/documentaire1.jpg",
                    Category = "Documentaire",
                    Year = 2023,
                    Duration = 90,
                    Director = "Réalisateur 4",
                    Rating = 4.6m,
                    Language = "fr",
                    Country = "FR",
                    IsActive = true
                }
            };

            await context.VodItems.AddRangeAsync(vodItems);
            await context.SaveChangesAsync();

            // Ajoute des utilisateurs de test
            var users = new List<User>
            {
                new User
                {
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = "password1", // En production, utilisez BCrypt.Net.BCrypt.HashPassword("password1")
                    FirstName = "Prénom1",
                    LastName = "Nom1",
                    IsActive = true,
                    Role = "User"
                },
                new User
                {
                    Username = "user2",
                    Email = "user2@example.com",
                    PasswordHash = "password2", // En production, utilisez BCrypt.Net.BCrypt.HashPassword("password2")
                    FirstName = "Prénom2",
                    LastName = "Nom2",
                    IsActive = true,
                    Role = "User"
                },
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "admin", // En production, utilisez BCrypt.Net.BCrypt.HashPassword("admin")
                    FirstName = "Admin",
                    LastName = "Admin",
                    IsActive = true,
                    Role = "Admin"
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Ajoute des chaînes favorites pour les utilisateurs
            var userFavoriteChannels = new List<UserFavoriteChannel>
            {
                new UserFavoriteChannel
                {
                    UserId = users[0].Id,
                    ChannelId = channels[0].Id
                },
                new UserFavoriteChannel
                {
                    UserId = users[0].Id,
                    ChannelId = channels[2].Id
                },
                new UserFavoriteChannel
                {
                    UserId = users[0].Id,
                    ChannelId = channels[4].Id
                },
                new UserFavoriteChannel
                {
                    UserId = users[1].Id,
                    ChannelId = channels[1].Id
                },
                new UserFavoriteChannel
                {
                    UserId = users[1].Id,
                    ChannelId = channels[3].Id
                }
            };

            await context.UserFavoriteChannels.AddRangeAsync(userFavoriteChannels);
            await context.SaveChangesAsync();

            // Ajoute des éléments VOD favoris pour les utilisateurs
            var userFavoriteVods = new List<UserFavoriteVod>
            {
                new UserFavoriteVod
                {
                    UserId = users[0].Id,
                    VodItemId = vodItems[0].Id
                },
                new UserFavoriteVod
                {
                    UserId = users[0].Id,
                    VodItemId = vodItems[2].Id
                },
                new UserFavoriteVod
                {
                    UserId = users[1].Id,
                    VodItemId = vodItems[1].Id
                },
                new UserFavoriteVod
                {
                    UserId = users[1].Id,
                    VodItemId = vodItems[4].Id
                }
            };

            await context.UserFavoriteVods.AddRangeAsync(userFavoriteVods);
            await context.SaveChangesAsync();
        }
    }
}
