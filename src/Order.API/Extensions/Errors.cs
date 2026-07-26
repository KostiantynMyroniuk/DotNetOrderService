using FluentResults;

namespace Order.API.Extensions
{
    public abstract class ApiError : Error
    {
        public int StatusCode { get; }

        public ApiError(string message, int statusCode) : base(message) 
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundError : ApiError
    {
        public NotFoundError(string message) : base(message, StatusCodes.Status404NotFound) { }
    }

    public class ConflictError : ApiError
    {
        public ConflictError(string message) : base(message, StatusCodes.Status409Conflict) { }
    }

}
