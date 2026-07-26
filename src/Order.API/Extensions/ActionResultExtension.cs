using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Order.API.Extensions
{
    public static class ActionResultExtension
    {
        public static ActionResult ToActionResult(this ResultBase result, ControllerBase controller)
        {
            var statusCode = result.Errors
                .OfType<ApiError>()
                .Select(x => x.StatusCode)
                .DefaultIfEmpty(StatusCodes.Status500InternalServerError)
                .Max();

            var problem = new ProblemDetails()
            {
                Status = statusCode,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Detail = string.Join(", ", result.Errors.Select(x => x.Message))
            };

            return controller.StatusCode(statusCode, problem);
        }
    }
}
