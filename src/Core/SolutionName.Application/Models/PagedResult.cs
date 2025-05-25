namespace SolutionName.Application.Common.Models
{
    /// <summary>
    /// Represents a paginated result containing a list of items and pagination information.
    /// </summary>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    public record PagedResult<TData>
    {
        public PagedResult(IList<TData> data, int currentPage, int pageSize, int count)
        {
            Data = data;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public IList<TData> Data { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Represents a paginated result with additional metadata.
    /// </summary>
    /// <typeparam name="TData">The type of data in the paginated result.</typeparam>
    /// <typeparam name="TMeta">The type of metadata to include with the paginated result.</typeparam>
    public record PagedResult<TData, TMeta> : PagedResult<TData>
    {
        public PagedResult(IList<TData> data, int currentPage, int pageSize, int count) : base(data, currentPage, pageSize, count)
        {
        }

        public TMeta? Meta { get; init; }
    }
}


