namespace Application.Common.Pagination.Models
{
    public record PaginationResultViewModel<T>(List<T> List, int PageNumber, int TotalCount, int TotalPages);
}