using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.DynamicModel.Expandos
{
    /// <summary>
    /// 用于Expando的属性过滤器(用于Expando内部)
    /// </summary>
    public interface IExpandoPropertyFilter
    {
        /// <summary>
        /// 是否包含动态属性
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public bool IncludeDynamicProperty(string propName);
        /// <summary>
        /// 是否包含实例属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public bool IncludeInstanceProperty(PropertyInfo propertyInfo);
    }
    
    public class ExpandoPropertyIncludeFilter : IExpandoPropertyFilter
    {
        public IList<string> Includes { get; set; } = new List<string>();
        
        public bool IncludeDynamicProperty(string propName)
        {
            if (Includes.Contains("*"))
            {
                return true;
            }
            return Includes.Any(x => propName.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public bool IncludeInstanceProperty(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes().ToList();
            foreach (var attribute in attributes)
            {
                //JsonIgnore, XmlIgnore, SoapIgnore, JsonIgnore[System.Text.Json.Serialization, Newtonsoft.Json]
                if (attribute.GetType().Name.Contains("Ignore", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class ExpandoPropertyExcludeFilter : IExpandoPropertyFilter
    {
        public IList<string> Excludes { get; set; } = new List<string>();

        public bool IncludeDynamicProperty(string propName)
        {
            if (Excludes.Contains("*"))
            {
                return false;
            }
            return !Excludes.Any(x => propName.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public bool IncludeInstanceProperty(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes().ToList();
            foreach (var attribute in attributes)
            {
                //JsonIgnore, XmlIgnore, SoapIgnore, JsonIgnore[System.Text.Json.Serialization, Newtonsoft.Json]
                if (attribute.GetType().Name.Contains("Ignore", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class ExpandoPropertyCompositeFilter : IExpandoPropertyFilter
    {
        public IList<IExpandoPropertyFilter> Filters { get; set; } = new List<IExpandoPropertyFilter>();
        public void AddPropertyFilter(IExpandoPropertyFilter filter)
        {
            if (filter != null)
            {
                if (!Filters.Contains(filter))
                {
                    Filters.Add(filter);
                }
            }
        }

        public bool IncludeDynamicProperty(string propName)
        {
            var shouldInclude = Filters.Any(f => f.IncludeDynamicProperty(propName));
            if (!shouldInclude)
            {
                return false;
            }

            //Dynamic: 排除 > 包含
            var excludeFilters = Filters.Where(x => x.GetType() == typeof(ExpandoPropertyExcludeFilter)).ToList();
            var shouldExclude = excludeFilters.Any(f => !f.IncludeDynamicProperty(propName));
            return !shouldExclude;
        }

        public bool IncludeInstanceProperty(PropertyInfo propertyInfo)
        {
            return Filters.Any(f => f.IncludeInstanceProperty(propertyInfo));
        }
    }

    public class ExpandoPropertyFilterFactory
    {
        #region static helpers for easy use

        public static ExpandoPropertyIncludeFilter CreateIncludeFilter(params string[] includes)
        {
            return new ExpandoPropertyIncludeFilter() { Includes = includes };
        }
        public static ExpandoPropertyExcludeFilter CreateExcludeFilter(params string[] excludes)
        {
            return new ExpandoPropertyExcludeFilter() { Excludes = excludes };
        }
        public static ExpandoPropertyCompositeFilter CreateCompositeFilter(params IExpandoPropertyFilter[] filters)
        {
            var compositeFilter = new ExpandoPropertyCompositeFilter();
            foreach (var filter in filters)
            {
                compositeFilter.AddPropertyFilter(filter);
            }
            return compositeFilter;
        }

        //default is empty filter, support for extensions, eg: filter by context query string, etc...
        private static readonly Lazy<ExpandoPropertyIncludeFilter> IncludeAllLazy = new Lazy<ExpandoPropertyIncludeFilter>(() => new ExpandoPropertyIncludeFilter() { Includes = new List<string> { "*" } });
        private static readonly Lazy<ExpandoPropertyIncludeFilter> IncludeNoneLazy = new Lazy<ExpandoPropertyIncludeFilter>(() => new ExpandoPropertyIncludeFilter());
        public static Func<IExpandoPropertyFilter> Resolve = () => ResolveDefaultMode() == ExpandoPropertyIncludeFilterDefaultMode.None ? IncludeNoneLazy.Value : IncludeAllLazy.Value;

        /// <summary>
        /// 自动过滤的默认模式：None, All
        /// </summary>
        public static Func<ExpandoPropertyIncludeFilterDefaultMode> ResolveDefaultMode = () => ExpandoPropertyIncludeFilterDefaultMode.All;

        #endregion
    }

    public enum ExpandoPropertyIncludeFilterDefaultMode
    {
        None = 0,
        All = 1
    }
}
