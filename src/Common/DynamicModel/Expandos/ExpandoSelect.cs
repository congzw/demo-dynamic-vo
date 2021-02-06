using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Common.DynamicModel.Expandos
{
    public class ExpandoSelect
    {
        public IList<string> Includes { get; set; } = new List<string>();
        public IList<string> Excludes { get; set; } = new List<string>();

        public static ExpandoSelect Parse(HttpContext httpContext)
        {
            var queryArgs = new ExpandoSelect();
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

    public static class ExpandoSelectExtensions
    {
        /// <summary>
        /// 使用ExpandoSelect过滤expando属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expando"></param>
        /// <param name="expandoSelect"></param>
        /// <returns></returns>
        public static T ApplyExpandoSelect<T>(this T expando, ExpandoSelect expandoSelect) where T : ExpandoModel
        {
            if (expandoSelect.Includes.Count > 0)
            {
                expando.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateIncludeFilter(expandoSelect.Includes.ToArray()));
            }
            if (expandoSelect.Excludes.Count > 0)
            {
                expando.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateIncludeFilter(expandoSelect.Excludes.ToArray()));
            }
            return expando;
        }

        /// <summary>
        /// 使用ExpandoSelect过滤expando属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expandoSelect"></param>
        /// <param name="expandos"></param>
        /// <returns></returns>
        public static IEnumerable<T> ApplyExpandoSelect<T>(this ExpandoSelect expandoSelect, IEnumerable<T> expandos) where T : ExpandoModel
        {
            foreach (var expando in expandos)
            {
                if (expandoSelect.Includes.Count > 0)
                {
                    expando.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateIncludeFilter(expandoSelect.Includes.ToArray()));
                }
                if (expandoSelect.Excludes.Count > 0)
                {
                    expando.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateIncludeFilter(expandoSelect.Excludes.ToArray()));
                }
                yield return expando;
            }
        }
    }
}
