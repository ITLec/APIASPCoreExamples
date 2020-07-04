using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebAPITest.Handlers
{
    //In .net core we should use Middleware instead of handlers
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1#middleware-order
    public class FullPipelineTimerHandler : DelegatingHandler
    {
        const string _header = "X-API-Timer--";
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timer = Stopwatch.StartNew();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            var elapsed = timer.ElapsedMilliseconds;

            Trace.WriteLine("--Pipleline+Action timemsec: " + elapsed.ToString());

            response.Headers.Add(_header, elapsed + " msec");
            return response;
        }
    }
}
