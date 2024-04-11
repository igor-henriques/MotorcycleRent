namespace MotorcycleRent.Api.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (HttpStatusCode statusCode, string problemDetails) = exception switch
        {
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidCredentialsException => (HttpStatusCode.BadRequest, exception.Message),
            EntityCreationException => (HttpStatusCode.BadRequest, exception.Message),
            MongoWriteException => (HttpStatusCode.BadRequest, Messages.InvalidMotorcycleDuplicateMessage),
            Exception => (HttpStatusCode.InternalServerError, Messages.InternalServerErrorMessage)
        };

        httpContext.Response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            Messages = problemDetails,
            httpContext.Response.StatusCode
        };

        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}
