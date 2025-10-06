using System.Net;
using System.Text.Json;

namespace Api.Configuration;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;
        
        var errorResponse = new ErrorResponse
        {
            Errors = "Internal Server error"
        };
        logger.LogError("Unhandled exception: {Exception}", exception);

        if (exception is OperationCanceledException ocException)
        {
            response.StatusCode = 499; // see: https://stackoverflow.com/a/46361806
            errorResponse.Errors = "Internal Server error - Operation cancelled";
        }
        else if (exception is BadHttpRequestException badRequestException)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            errorResponse.Errors = badRequestException.Message;
        }
        else if (exception is HttpRequestException hrException)
        {
            response.StatusCode = hrException.StatusCode switch
            {
                HttpStatusCode.BadRequest => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.ServiceUnavailable
            };
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            errorResponse.Errors = "Internal Server error";
        }

        var result = JsonSerializer.Serialize(errorResponse);
        logger.LogWarning("Returning error response: route:{Url}, statusCode:{StatusCode}, traceId:{TraceId}, body:{Body}",
            context.Request.Method+" "+context.Request.Path, response.StatusCode, response.Headers["TraceId"].ToString(), result);
       
        await context.Response.WriteAsync(result);
    }
}

public record ErrorResponse
{
    public required string Errors { get; set; }
}