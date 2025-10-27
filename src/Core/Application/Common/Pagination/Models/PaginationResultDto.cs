namespace Application.Common.Pagination.Models
{
    public record PaginationResultDto<T>(
        List<T> List,
        int PageNumber,
        int TotalCount,
        int TotalPages);
}
