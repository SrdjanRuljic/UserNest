using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Common.Pagination.Models
{
    [SwaggerSchema("Base pagination parameters")]
    public class PaginationViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        [SwaggerSchema("Page number (1-based)")]
        public int PageNumber { get; set; }
        
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        [SwaggerSchema("Number of items per page")]
        public int PageSize { get; set; }
        
        public PaginationViewModel()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        
        public PaginationViewModel(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}