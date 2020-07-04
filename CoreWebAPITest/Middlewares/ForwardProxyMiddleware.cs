using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace CoreWebAPITest.Middlewares
{

    public class ForwardProxyMiddleware
    {
        public const string MyClientBaseUrlProperty = "MyClientBaseUrl";
        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Forwarded
        const string _fwdHeader = "Forwarded";

        // old style, separate headers
        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Proto
        const string _fwdProtoHeader = "X-Forwarded-Proto";
        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Host
        const string _fwdHostHeader = "X-Forwarded-Host";
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ForwardProxyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ForwardProxyMiddleware>();
        }


        //https://stackoverflow.com/questions/37395227/add-response-headers-to-asp-net-core-middleware
        public async Task Invoke(HttpContext context)
        {
          var uri= new Uri( Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(context.Request));

            // first, let's start with a basic URI based on the server's view of the request
            UriBuilder builder = new UriBuilder(uri.Scheme, uri.Host, uri.Port);


            // override host and protocol if found in headers
            // first check the legacy x-forwarded headers
            if (context.Request.Headers.ContainsKey(_fwdProtoHeader))
            {
                StringValues protoVal;
                if (context.Request.Headers.TryGetValue(_fwdProtoHeader, out protoVal))
                {
                    builder.Scheme = protoVal.ToString();
                }
            }

            if (context.Request.Headers.ContainsKey(_fwdHostHeader))
            {
                StringValues hostVal;
                if (context.Request.Headers.TryGetValue(_fwdHostHeader, out hostVal))
                {
                    SetHostAndPort(builder, hostVal.ToString());
                }
            }

            // next try the newer Forwarded header
            if (context.Request.Headers.ContainsKey(_fwdHeader))
            {
                StringValues fwdVal;
                // grab the forward host string 

                if (context.Request.Headers.TryGetValue(_fwdHeader, out fwdVal))
                {
                    var fwd = fwdVal.ToString().Split(';').Select(s => s.Trim());

                    // syntax for the Forwarded header: Forwarded: by=<identifier>; for=<identifier>; host=<host>; proto=<http|https>
                    var proto = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("proto="));

                    if (!String.IsNullOrEmpty(proto))
                    {
                        proto = proto.Substring(6).Trim();
                        if (!String.IsNullOrEmpty(proto))
                            builder.Scheme = proto;
                    }
                    var host = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("host="));
                    if (!String.IsNullOrEmpty(host))
                    {
                        host = host.Substring(5).Trim();
                        if (!String.IsNullOrEmpty(host))
                            SetHostAndPort(builder, host);
                    }

                }


            }
            builder.Path = "/";
            context.Request.Headers.Add(MyClientBaseUrlProperty,new StringValues( builder.Uri.ToString()));
            await _next(context);
        }

        private static void SetHostAndPort(UriBuilder builder, string host)
        {
            var hostAndPort = host.Split(':');
            builder.Host = hostAndPort[0];
            if(hostAndPort.Length>1)
            {
                builder.Port = int.Parse(hostAndPort[1]);
            }else
            {
                builder.Port = -1;
            }
        }
    }
    public static class ForwardProxyMiddlewareExtensions
    {
        public static IApplicationBuilder RegisterForwardProxyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ForwardProxyMiddleware>();
        }

        public static Uri GetSelfReferenceBaseUrl(this HttpRequest request)
        {
            if (request == null)
                return null;

            if (request.Headers.TryGetValue(ForwardProxyMiddleware.MyClientBaseUrlProperty, out StringValues uri))
            {
                return new Uri(uri.ToString());
            }

            return null;
        }
        public static Uri RebaseUrlForClient(this HttpRequest request, Uri serverUrl)
        {
            Uri clientBase = GetSelfReferenceBaseUrl(request);

            if(clientBase == null)
            return null;

            if (serverUrl == null)
                return clientBase;


            var builder = new UriBuilder(serverUrl);

            builder.Scheme = clientBase.Scheme;
            builder.Host = clientBase.Host;
            builder.Port = clientBase.Port;

            return builder.Uri;
        }
    }
}
