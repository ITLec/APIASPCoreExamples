using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreWebAPITest.Middlewares
{
    public class TimerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public TimerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<TimerMiddleware>();
        }

        const string _header = "X-API-Timer";

        //https://stackoverflow.com/questions/37395227/add-response-headers-to-asp-net-core-middleware
        public async Task Invoke(HttpContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            //To add Headers AFTER everything you need to do this
            context.Response.OnStarting(state => {
                var httpContext = (HttpContext)state;
                httpContext.Response.Headers.Add(_header, new[] { watch.ElapsedMilliseconds.ToString() });

                _logger.LogInformation("Finished handling api key.");
                return Task.CompletedTask;
            }, context);

            await _next(context);
        }
    }
    public static class TimerMiddlewareExtensions
    {
        public static IApplicationBuilder RegisterTimerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TimerMiddleware>();
        }
    }

}
