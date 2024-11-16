using Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AppCommon.GlobalHelpers
{
    public static class PaginationHelpers
    {
        public static async Task<PaginatedList<T>> GetPaginatedDataAsync<T>(
            IQueryable<T> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
            where T : class
        {
            const int MaxPageSize = 100;
            const int DefaultPageSize = 20;
            const int MinPageNumber = 1; 

            pageSize = pageSize switch
            {
                <= 0 => DefaultPageSize,
                > MaxPageSize => MaxPageSize,
                _ => pageSize
            };

            var totalRecords = await query
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            pageNumber = Math.Max(MinPageNumber, Math.Min(pageNumber, Math.Max(totalPages, 1)));

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new PaginatedList<T>
            {
                Items = items,
                TotalCount = totalRecords,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNext = pageNumber < totalPages,
                HasPrevious = pageNumber > 1
            };
        }

        public static async Task<PaginatedList<T>> GetInfiniteScrollDataAsync<T>(
            IQueryable<T> query,
            int offset,
            int limit,
            CancellationToken cancellationToken = default)
            where T : class
        {
            const int MaxLimit = 100;
            const int DefaultLimit = 20;

            var totalRecords = await query
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);

            offset = Math.Max(0, offset);

            limit = limit switch
            {
                <= 0 => DefaultLimit,
                > MaxLimit => MaxLimit, 
                _ => limit
            };

            var items = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new PaginatedList<T>
            {
                Items = items,
                TotalCount = totalRecords,
                NextOffset = offset + limit,
                HasMore = (offset + limit) < totalRecords
            };
        }


        public static async Task<PaginatedList<T>> GetAllItemsAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var totalRecords = await query
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);

            var items = await query
              .ToListAsync(cancellationToken)
              .ConfigureAwait(false);


            return new PaginatedList<T>
            {
                Items = items,
                TotalCount = totalRecords,
            };
        }

        public static PaginatedList<TDestination> MapPaginatedList<TSource, TDestination>(
        this PaginatedList<TSource> source,
        Func<TSource, TDestination> mapper)
        {
            var items = source.Items.Select(mapper).ToList();

            return new PaginatedList<TDestination>
            {
                Items = items,
                TotalCount = source.TotalCount,
                NextOffset = source.NextOffset,
                HasMore = source.HasMore,
                PageSize = source.PageSize,
                CurrentPage = source.CurrentPage,
                HasNext = source.HasNext,
                HasPrevious = source.HasPrevious,
                TotalPages = source.TotalPages,
            };
        }
    }


  
}
