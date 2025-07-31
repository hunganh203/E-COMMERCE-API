using System.Net;
using WebApi.Models.Response;

namespace WebApi.Filter;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
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

        var errorResponse = new AjaxResponse(false);
        switch (exception)
        {
            case ApplicationException ex:
                if (ex.Message.Contains("Invalid token"))
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.Error = new ErrorInfo(ex.Message);
                    break;
                }
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Error = new ErrorInfo(ex.Message);
                break;

            case KeyNotFoundException ex:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Error = new ErrorInfo(ex.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //errorResponse.Error = new ErrorInfo("Internal Server errors. Check Logs!");
                 errorResponse.Error = new ErrorInfo(exception.Message);
                break;
        }
        _logger.LogError(exception.Message);

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}