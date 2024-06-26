﻿using FluentValidation;
using MotorcycleRent.Domain.Exceptions;

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
        if (exception is DomainException or ValidationException)
        {            
            _logger.LogError(InternalServerErrorMessage, exception.Message);
        }

        (int statusCode, string details) = exception switch
        {
            DomainException or ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, Messages.InternalServerError)
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
