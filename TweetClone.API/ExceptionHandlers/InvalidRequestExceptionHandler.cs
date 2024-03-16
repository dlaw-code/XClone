using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TweetClone.API.Services.Implementation;

namespace TweetClone.API.Exceptions
{
    public class InvalidRequestExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not InvalidRequestException)
                return false;
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            
            await httpContext.Response
                .WriteAsJsonAsync( new ProblemDetails()
                {
                    Status = httpContext.Response.StatusCode,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Detail = exception.Message
                }, cancellationToken: cancellationToken);

            return true;
        }
    }
}
