using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente un résultat paginé
    /// </summary>
    /// <typeparam name="T">Type des éléments</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Liste des éléments
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Nombre total d'éléments
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Page courante
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Taille de la page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Nombre total de pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indique s'il existe une page précédente
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indique s'il existe une page suivante
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Constructeur par défaut pour la sérialisation
        /// </summary>
        public PaginatedResult()
        {
        }

        /// <summary>
        /// Constructeur privé pour forcer l'utilisation de la méthode CreateAsync
        /// </summary>
        private PaginatedResult(int pageNumber, int pageSize, int totalCount, IEnumerable<T> items)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            Items = items;
        }

        /// <summary>
        /// Méthode de fabrique pour créer un résultat paginé à partir d'une source IQueryable
        /// </summary>
        /// <param name="source">Source de données IQueryable</param>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Résultat paginé</returns>
        public static async Task<PaginatedResult<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // Validation des paramètres
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 1 : pageSize;

            // Obtenir le nombre total d'éléments
            var count = await source.CountAsync();

            // Obtenir les éléments pour la page courante
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Créer et retourner le résultat paginé
            return new PaginatedResult<T>(pageNumber, pageSize, count, items);
        }
    }
}