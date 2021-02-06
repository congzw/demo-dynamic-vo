using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Common.DynamicModel.Expandos
{
    public class ExpandoQueryContext
    {
        public IList<string> Includes { get; set; } = new List<string>();
        public IList<string> Excludes { get; set; } = new List<string>();

        public static ExpandoQueryContext Parse(HttpContext httpContext)
        {
            var queryArgs = new ExpandoQueryContext();
            if (httpContext == null)
            {
                return queryArgs;
            }

            httpContext.Request.Query.TryGetValue("$includes", out var includesValue);
            var includes = includesValue.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (includes.Length > 0)
            {
                queryArgs.Includes = includes;
            }

            httpContext.Request.Query.TryGetValue("$excludes", out var excludesValue);
            var excludes = excludesValue.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (excludes.Length > 0)
            {
                queryArgs.Excludes = excludes;
            }

            return queryArgs;
        }
    }
}
