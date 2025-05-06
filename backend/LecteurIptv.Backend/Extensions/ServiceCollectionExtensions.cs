using LecteurIptv.Backend.Data;
using LecteurIptv.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LecteurIptv.Backend.Extensions
{
    /// <summary>
    /// Extensions pour la collection de services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Ajoute les services de base de données à la collection de services
        /// </summary>
        /// <param name="services">Collection de services</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Collection de services</returns>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Ajoute le contexte de base de données
            services.AddDbContext<AppDbContext>(options =>
            {
                // Utilise SQLite comme fournisseur de base de données
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

                // Configure le comportement de journalisation en développement
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            return services;
        }

        /// <summary>
        /// Initialise la base de données
        /// </summary>
        /// <param name="services">Collection de services</param>
        /// <returns>Collection de services</returns>
        public static IServiceCollection InitializeDatabase(this IServiceCollection services)
        {
            // Récupère le service provider
            var serviceProvider = services.BuildServiceProvider();

            // Récupère le contexte de base de données
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Initialise la base de données
            DbInitializer.InitializeAsync(dbContext).GetAwaiter().GetResult();

            return services;
        }

        /// <summary>
        /// Ajoute les services métier à la collection de services
        /// </summary>
        /// <param name="services">Collection de services</param>
        /// <returns>Collection de services</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add memory cache
            services.AddMemoryCache();

            // Register the VOD service
            services.AddScoped<IVodService, VodService>();

            // Register other application services
            services.AddScoped<IStreamingService, StreamingService>();
            services.AddScoped<IM3UParser, M3UParser>();
            services.AddScoped<IXmltvParser, XmltvParser>();

            // Ajoute les services d'infrastructure
            services.AddHttpClient();

            // Ajoute les services métier
            services.AddScoped<IChannelsService, ChannelsService>();
            services.AddScoped<IProgramsService, ProgramsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFavoritesService, FavoritesService>();
            services.AddScoped<IHistoryService, HistoryService>();

            return services;
        }
    }
}
