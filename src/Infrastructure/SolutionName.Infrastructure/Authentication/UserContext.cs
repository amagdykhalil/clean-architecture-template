using SolutionName.Application.Abstractions.UserContext;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication;

/// <summary>
/// Implementation of IUserContext that provides access to the current user's information.
/// </summary>
internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("User context is unavailable");
}


