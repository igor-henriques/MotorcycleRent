namespace MotorcycleRent.Api.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private const string InternalServerErrorMessage = "Internal server error: {exception}";

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        bool isInternalError = exception is
            InternalErrorException or
            EntityCreationException or
            Exception;

        if (isInternalError)
        {            
            _logger.LogError(InternalServerErrorMessage, exception.Message);
        }

        (HttpStatusCode statusCode, string problemDetails) = exception switch
        {
            InvalidOperationException or
            ValidationException or
            InvalidCredentialsException or
            DateTimeInvalidRangeException or
            NoMotorcyclesAvailableException or
            OnGoingRentException or
            PartnerUnableToRentException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, Messages.InternalServerErrorMessage)
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
