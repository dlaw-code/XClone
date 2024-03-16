using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TweetClone.API.Services.Implementation;

namespace TweetClone.API.Exceptions
{
    public class ExceptionLoggingHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionLoggingHandler> _logger;

        public ExceptionLoggingHandler(ILogger<ExceptionLoggingHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {

            if (exception is InvalidRequestException)
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            _logger.LogError(
                exception, "Exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error"
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return false;
        }
    }
}
