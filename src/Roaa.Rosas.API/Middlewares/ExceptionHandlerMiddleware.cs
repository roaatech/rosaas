using Newtonsoft.Json;
using Roaa.Rosas.Common.Extensions;
using System.Net;

namespace Roaa.StarsKnight.Education.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            _next = next;
            _environment = env;
            _logger = loggerFactory.CreateLogger(next.GetType());
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();

                _logger.LogError(ex, $"errorId:{errorId}, sys-exception: {ex.GetErrorMessage()}");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                string text = _environment.IsProductionEnvironment() ?
                             $"An error occurred while processing your request, Error TenantId:{errorId}, Please contact your system administrator for more details" :
                             JsonConvert.SerializeObject(ex);

                await httpContext.Response.WriteAsync(text);
            }
        }




    }
}
