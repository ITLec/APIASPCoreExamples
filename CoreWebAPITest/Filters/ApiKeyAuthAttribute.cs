using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPITest.Filters
{
    public class ApiKeyAuthAttribute : Attribute, Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter
    {
        const string ApiKeyHeaderName = "ApiKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
          if(!  context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
            }
            string secretApiKey = "ITLec";

            if(potentialApiKey != secretApiKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //Before
            await next();
            //After
        }
    }
}
