namespace MotorcycleRent.Api.Middlewares;

public sealed class HttpLoggingDetailsMiddleware
{
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpLoggingDetailsMiddleware> _logger;

    public HttpLoggingDetailsMiddleware(RequestDelegate next, ILogger<HttpLoggingDetailsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        var originalResponseBody = context.Response.Body;
        using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            await LogDetailsAsync(context);
        }
        catch (Exception ex)
        {
            await LogDetailsAsync(context, ex);
            throw;
        }
        finally
        {
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;
        }
    }

    private async Task LogDetailsAsync(HttpContext context, Exception? ex = null)
    {
        var requestBody = await GetRequestBodyAsync(context);
        var responseBody = await GetResponseBodyAsync(context);

        _logger.LogInformation("RequestBody: {RequestBody}", requestBody);
        _logger.LogInformation("ResponseBody: {ResponseBody}", responseBody);

        if (ex != null)
        {
            _logger.LogError(ex, "An exception occurred");
        }
    }

    private async Task<string> GetRequestBodyAsync(HttpContext context)
    {
        await using var requestStream = _recyclableMemoryStreamManager.GetStream();

        var oldPosition = context.Request.Body.Position;
        context.Request.Body.Position = 0;

        await context.Request.Body.CopyToAsync(requestStream);
        context.Request.Body.Position = oldPosition;

        return await ReadStreamInBlocksAsync(requestStream);
    }

    private static async Task<string> GetResponseBodyAsync(HttpContext context)
    {
        var stream = context.Response.Body;

        var oldPosition = stream.Position;
        stream.Position = 0;

        var body = await ReadStreamInBlocksAsync(stream);
        stream.Position = oldPosition;

        return body;
    }

    private static async Task<string> ReadStreamInBlocksAsync(
        Stream stream,
        int count = 4096,
        int maxReadLength = int.MaxValue)
    {
        stream.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StringWriter();
        var reader = new StreamReader(stream);

        var readBlockBuffer = new char[count];
        int readBlockLength;
        int readLength = 0;

        do
        {
            var countToRead = Math.Min(count, maxReadLength - readLength);
            readBlockLength = await reader.ReadBlockAsync(readBlockBuffer, 0, countToRead);
            textWriter.Write(readBlockBuffer, 0, readBlockLength);
            readLength += readBlockLength;

        } while (readBlockLength > 0 && readLength < maxReadLength);

        return textWriter.ToString();
    }
}
