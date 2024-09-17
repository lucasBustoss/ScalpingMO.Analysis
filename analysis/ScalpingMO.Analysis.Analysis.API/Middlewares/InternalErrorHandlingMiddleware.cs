using System.Net;
using System.Text.Json;

namespace ScalpingMO.Analysis.Analysis.API.Middlewares
{
    public class InternalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public InternalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == 401)
                    throw new BadHttpRequestException("Unauthorized", 401);

                if (context.Response.StatusCode == 403)
                    throw new BadHttpRequestException("Forbidden", 403);

                if (context.Response.StatusCode == 404)
                    throw new BadHttpRequestException("Not found", 404);

                if (context.Response.StatusCode == 405)
                    throw new BadHttpRequestException("Not allowed", 405);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var error = "Internal Server Error";

            // Customize the response message for specific exceptions if needed.
            if (exception is InvalidOperationException)
            {
                statusCode = HttpStatusCode.InternalServerError;
                error = "Dependency Injection Error";
            }
            else if (exception is BadHttpRequestException)
            {
                var httpException = exception as BadHttpRequestException;

                if (httpException != null)
                {
                    statusCode = GetHttpStatusCode(httpException.StatusCode);
                    error = GetErrorMessage((int)statusCode);
                }
            }
            else
            {
                statusCode = HttpStatusCode.BadRequest;
                error = exception.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = (int)statusCode,
                Error = error
            };

            return JsonSerializer.SerializeAsync(context.Response.Body, response);
        }

        private static string GetErrorMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return "Not authorized.";

                case 403:
                    return "Access denied.";

                case 404:
                    return "Resource not found.";

                case 405:
                    return "Not allowed.";

                default:
                    return "Internal server error.";
            }
        }

        private static HttpStatusCode GetHttpStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return HttpStatusCode.Unauthorized;

                case 403:
                    return HttpStatusCode.Forbidden;

                case 404:
                    return HttpStatusCode.NotFound;

                case 405:
                    return HttpStatusCode.MethodNotAllowed;

                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }
}
