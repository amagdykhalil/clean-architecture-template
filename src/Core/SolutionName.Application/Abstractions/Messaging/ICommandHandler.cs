namespace Application.Abstractions.Messaging;

/// <summary>
/// Represents a handler for a command that returns no specific response data.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand, ApiResponse<object>>
    where TCommand : ICommand;

/// <summary>
/// Represents a handler for a command that returns a specific response type.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <typeparam name="TResponse">The type of response data returned by the command.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, ApiResponse<TResponse>>
    where TCommand : ICommand<TResponse>;


