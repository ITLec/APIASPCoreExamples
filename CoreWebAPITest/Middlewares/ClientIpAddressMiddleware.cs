

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreWebAPITest.Middlewares
{
    public class ClientIpAddressMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;


        const string _fwdHeader = "Forwarded";
        public const string _fwdForHeader = "X-Forwarded-For";

        public ClientIpAddressMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ClientIpAddressMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            string ip = null;
            // X-Forwarded-For header - the first one in the comma-separated string is the original client if more than one
            if (context.Request.Headers.ContainsKey(_fwdForHeader))
            {
                Microsoft.Extensions.Primitives.StringValues tmpStr;

                if (context.Request.Headers.TryGetValue(_fwdForHeader, out tmpStr))
                {

                    ip = tmpStr;
                    if (!String.IsNullOrEmpty(ip) && ip.Contains(","))
                        ip = ip.Substring(0, ip.IndexOf(','));
                }
            }


            if (String.IsNullOrEmpty(ip) && context.Request.Headers.ContainsKey(_fwdHeader))
            {

                Microsoft.Extensions.Primitives.StringValues tmpStr;

                if (context.Request.Headers.TryGetValue(_fwdForHeader, out tmpStr))
                {

                    var fwd = tmpStr
                    .FirstOrDefault(s => !String.IsNullOrEmpty(s))
                    .Split(';')
                    .Select(s => s.Trim());


                    // syntax for the Forwarded header: Forwarded: by=<identifier>; for=<identifier>; host=<host>; proto=<http|https>
                    ip = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("for="));
                    if (!String.IsNullOrEmpty(ip))
                        ip = ip.Substring(4);
                }
            }
           
            // try the http context to see what it has, if none of the standard headers have worked out
            if (String.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }

            // store it off if we found it
            if (!String.IsNullOrEmpty(ip))
                context.Request.Headers.Add(_fwdForHeader, ip);


            await _next(context);
        }
    }
    public static class ClientIpAddressMiddlewareExtensions
    {
        public static IApplicationBuilder RegisterClientIpAddressMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClientIpAddressMiddleware>();
        }
    }
}
