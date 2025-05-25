using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace SolutionName.Application.Common.Validator
{
    /// <summary>
    /// Custom factory for creating validation result responses that integrate with the application's API response format.
    /// </summary>
    public class ValidationResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(
            ActionExecutingContext context,
            ValidationProblemDetails? validationProblemDetails)
        {
            // collect only the first error per property
            var firstErrors = new Dictionary<string, string>();
            if (validationProblemDetails?.Errors != null)
            {
                foreach (var error in validationProblemDetails.Errors)
                {
                    if (error.Value is { Length: > 0 } && !firstErrors.ContainsKey(error.Key))
                    {
                        firstErrors[error.Key] = error.Value.First();
                    }
                }
            }

            var errorResponses = firstErrors
                .Select(kvp => new ApiErrorResponse($"{kvp.Key}: {kvp.Value}"))
                .ToList();

            var apiResponse = BadRequest(errorResponses);

            return new ObjectResult(apiResponse)
            {
                StatusCode = apiResponse.StatusCode
            };
        }
    }
}


