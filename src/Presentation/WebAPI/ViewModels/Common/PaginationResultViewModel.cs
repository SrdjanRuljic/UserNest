using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.ViewModels.Common
{
    [SwaggerSchema("Paginated result view model wrapper")]
    public record PaginationResultViewModel<T>(
        [SwaggerSchema("Array of items for current page")]
        List<T> List,
        [SwaggerSchema("Current page number")]
        int PageNumber,
        [SwaggerSchema("Total number of items")]
        int TotalCount,
        [SwaggerSchema("Total number of pages")]
        int TotalPages);
}
