namespace Application.Abstractions.Messaging;

/// <summary>
/// Represents a handler for a query that returns no specific response data.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
public interface IQueryHandler<in TQuery>
    : IRequestHandler<TQuery, Result>
    where TQuery : IQuery;

/// <summary>
/// Represents a handler for a query that returns a specific response type.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of response data returned by the query.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;


