using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;


namespace CoreWebAPITest.RouteConstraints
{
    public class CustomValidationConstraint : IHttpRouteConstraint,IParameterPolicy
    {
        readonly string customParamVal;
        public CustomValidationConstraint(string paramVal)
        {
            customParamVal = paramVal;
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
            IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {

            return true;
            object value;
            if (!values.TryGetValue(parameterName, out value))
                return false;

            return IsValidAccount((string) value );
        }
      private static  bool IsValidAccount(string sAccount)
        {
            //if (sAccount == "Rasheed")
            //    throw new Exception("Rasheed is not allowed");

            return true;
            return (!String.IsNullOrEmpty(sAccount) &&
                sAccount.StartsWith("1234") &&
                sAccount.Length > 5);
        }
    }
}
