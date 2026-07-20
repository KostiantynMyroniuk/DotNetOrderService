using FluentResults;

namespace Order.API.Extensions
{
    public class NotFoundError : Error
    {
        public NotFoundError(string message) : base(message) { }
    }
}
