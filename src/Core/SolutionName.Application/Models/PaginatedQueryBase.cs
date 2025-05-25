using SolutionName.Application.Common.Queries;

namespace SolutionName.Application.Common.Models
{
    /// <summary>
    /// Base class for paginated queries that provides common pagination functionality.
    /// </summary>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    public abstract record PaginatedQueryBase<TData> : IPaginatedQuery<TData>
    {
        // maximum page size you will let clients request
        protected virtual short MaxPageSize { get; set; } = 50;

        private short _pageSize = 10;
        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            init => _pageNumber = value < 1 ? 1 : value;
        }

        public short PageSize
        {
            get => _pageSize;
            init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }

    /// <summary>
    /// Base class for paginated queries that include additional metadata.
    /// </summary>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    /// <typeparam name="TMeta">The type of metadata to include with the paginated result.</typeparam>
    public abstract record PaginatedQueryBase<TData, TMeta>
    : PaginatedQueryBase<TData>,
      IPaginatedQuery<TData, TMeta>;

}


