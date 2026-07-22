using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Order.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCodes, problemDetails) = exception switch
            {
                FluentValidation.ValidationException validationException => (
                    StatusCodes.Status400BadRequest,
                    new ValidationProblemDetails(
                        validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "One or more validation errors occured."
                    }),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails()
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occured."
                    })
            };

            if (statusCodes == StatusCodes.Status500InternalServerError )
            {
                _logger.LogError(exception, "Unhandled exception occured.");
            }
            else
            {
                _logger.LogWarning("Handled exception of type {ExceptionType}: {Message}\", exception.GetType().Name, exception.Message", 
                    exception.GetType().Name, exception.Message);
            }

            httpContext.Response.StatusCode = statusCodes;
            await httpContext.Response.WriteAsJsonAsync((object)problemDetails, cancellationToken);

            return true;
        }
    }
}
