namespace Application.Abstractions.Messaging;

/// <summary>
/// Represents a query that returns no specific response data.
/// </summary>
public interface IQuery : IRequest<ApiResponse<object>>;

/// <summary>
/// Represents a query that returns a specific response type.
/// </summary>
/// <typeparam name="TResponse">The type of response data returned by the query.</typeparam>
public interface IQuery<TResponse> : IRequest<ApiResponse<TResponse>>;


