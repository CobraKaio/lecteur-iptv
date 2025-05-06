using System.ComponentModel.DataAnnotations;

namespace LecteurIptv.Backend.Models
{
    /// <summary>
    /// Représente les paramètres de pagination envoyés par le client
    /// </summary>
    public class PaginationParameters
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private const int MaxPageSize = 100;

        /// <summary>
        /// Numéro de page (1-based)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Le numéro de page doit être supérieur à 0")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Nombre d'éléments par page
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "La taille de page doit être supérieure à 0")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
        }
    }
}
