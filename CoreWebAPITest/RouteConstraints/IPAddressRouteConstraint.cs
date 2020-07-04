using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace CoreWebAPITest.RouteConstraints
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class IPAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            dynamic address = validationContext.ObjectInstance;

            long? scope = address.ScopeId;

            var isValid = IPAddresses.IsValid(value as string, scope);

            var result = ValidationResult.Success;

            if (!isValid)
                result = new ValidationResult("The provided IP Address is not a valid IPv4 or IPv6 address");

            return result;
        }
    }

    internal class IPAddresses
    {
        internal static bool IsValid(string v, long? scope)
        {
            return false;
        }
    }

    public class IPAddressRouteConstraint : IHttpRouteConstraint
    {
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value;
            if (!values.TryGetValue(parameterName, out value))
                return false;

            var attr = new IPAddressAttribute();

            return attr.IsValid(value);
        }
    }
}
