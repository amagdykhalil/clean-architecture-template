using SolutionName.Application.Common.Models;

namespace SolutionName.Application.Common.Queries
{
    /// <summary>
    /// Interface for queries that return paginated results.
    /// </summary>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    public interface IPaginatedQuery<TData> : IQuery<Models.PagedResult<TData>>;

    /// <summary>
    /// Interface for queries that return paginated results with additional metadata.
    /// </summary>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    /// <typeparam name="TMeta">The type of metadata to include with the paginated result.</typeparam>
    public interface IPaginatedQuery<TData, TMeta> : IQuery<PagedResult<TData, TMeta>>;
}


