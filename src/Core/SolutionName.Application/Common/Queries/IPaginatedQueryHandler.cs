
using SolutionName.Application.Common.Models;

namespace SolutionName.Application.Common.Queries
{
    /// <summary>
    /// Handler interface for paginated queries that return basic paginated results.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to handle.</typeparam>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    public interface IPaginatedQueryHandler<TQuery, TData>
        : IQueryHandler<TQuery, Models.PagedResult<TData>>
        where TQuery : IPaginatedQuery<TData>
    { }

    /// <summary>
    /// Handler interface for paginated queries that return results with additional metadata.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to handle.</typeparam>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    /// <typeparam name="TMeta">The type of metadata to include with the paginated result.</typeparam>
    public interface IPaginatedQueryWithMetaHandler<TQuery, TData, TMeta>
        : IQueryHandler<TQuery, PagedResult<TData, TMeta>>
        where TQuery : IPaginatedQuery<TData, TMeta>
    { }
}


