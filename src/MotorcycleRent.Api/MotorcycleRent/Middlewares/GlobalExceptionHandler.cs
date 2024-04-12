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

        (int statusCode, string details) = exception switch
        {
            InvalidOperationException or
            ValidationException or
            InvalidCredentialsException or
            DateTimeInvalidRangeException or
            NoMotorcyclesAvailableException or
            OnGoingRentException or
            PartnerUnableToRentException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, Messages.InternalServerErrorMessage)
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetail = new ProblemDetails
        {
            Status = statusCode,
            Title = "Invalid Request",
            Detail = details
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);

        return true;
    }
}
