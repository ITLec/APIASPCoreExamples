using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace CoreWebAPITest.RouteConstraints
{
    public class EnumerationConstraint : IHttpRouteConstraint, IParameterPolicy

    {
        /// <summary>
        /// Holds the type of the Enum class to validate against
        /// </summary>
        public readonly Type EnumType;

        /// <summary>
        /// Constructor taking a namespace-qualified type name of the Enum type to use
        /// </summary>
        public EnumerationConstraint(string type)
        
        {
            var t = GetType(type);

            if (t == null || !t.IsEnum)
                throw new ArgumentException("Argument is not an Enum type", "type");
            EnumType = t;
        }

        /// <summary>
        /// Internal method to convert the string enum type name into a Type instance
        /// by checking all of the currently loaded assemblies
        /// </summary>
        private static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// IHttpRouteConstraint.Match implementation to validate a parameter against
        /// the Enum members.  String comparison is NOT case-sensitive.
        /// </summary>
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
            IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            return false;
            object value;

            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                var stringVal = value as string;
                if (!String.IsNullOrEmpty(stringVal))
                {
                    // see if we can find the string in the enumeration members
                    stringVal = stringVal.ToLower();
                    if (null != EnumType.GetEnumNames().FirstOrDefault(a => a.ToLower().Equals(stringVal)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
