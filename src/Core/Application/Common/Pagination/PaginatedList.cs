using Microsoft.EntityFrameworkCore;

namespace Application.Common.Pagination
{
    public class PaginatedList<T> : List<T>
    {
        public bool HasNextPage => (PageNumber < TotalPages);
        public bool HasPreviousPage => (PageNumber > 1);
        public int PageNumber { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;

            this.AddRange(items);
        }

        public PaginatedList(List<T> items, int pageNumber)
        {
            PageNumber = pageNumber;

            this.AddRange(items);
        }

        public static PaginatedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            List<T> items = [.. source.Skip((pageNumber - 1) * pageSize).Take(pageSize)];
            return new PaginatedList<T>(items, pageNumber);
        }

        public static PaginatedList<T> CreateAndCount(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            List<T> items = [.. source.Skip((pageNumber - 1) * pageSize).Take(pageSize)];
            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        public static async Task<PaginatedList<T>> CreateAndCountAsync(
            IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            int count = await source.CountAsync(cancellationToken);
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            int count = await source.CountAsync(cancellationToken);
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}