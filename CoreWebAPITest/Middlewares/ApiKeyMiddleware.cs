using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace CoreWebAPITest.Middlewares
{

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ApiKeyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ApiKeyMiddleware>();
        }

      public  const string API_KEY = "x-api-key";


        public async Task Invoke(HttpContext context)
        {

            if (context.Request.Path.Value.ToLower().StartsWith("/swagger"))
            {

                await _next(context);
            }
            else
            {


                string apiKey = null;

                Microsoft.Extensions.Primitives.StringValues val;
                if (context.Request.Headers.ContainsKey(API_KEY) &&
                    context.Request.Headers.TryGetValue(API_KEY, out val))
                {
                    apiKey = val.ToString();
                }
                else if (context.Request.Query.ContainsKey(API_KEY) &&
                    context.Request.Query.TryGetValue(API_KEY, out val))
                {
                    apiKey = val.ToString();
                }

                if (string.IsNullOrEmpty(apiKey))
                {
                    //var response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                    //{
                    //    Content = new StringContent("Missing Api Key")
                    //};
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                  await  context.Response.WriteAsync( "Missing Api Key");

                }
                else
                {

                    await _next(context);
                }
            }

        }
    }
    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder RegisterApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
