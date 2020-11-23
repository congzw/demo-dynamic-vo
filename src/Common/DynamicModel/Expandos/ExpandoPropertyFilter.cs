using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.DynamicModel.Expandos
{
    public interface IExpandoPropertyFilter
    {
        public bool Include(string propName);
    }

    public class ExpandoPropertyFilter : IExpandoPropertyFilter
    {
        public IList<string> Excludes { get; set; } = new List<string>();

        public bool Include(string propName)
        {
            return !Excludes.Any(x => propName.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public static ExpandoPropertyFilter Create(params string[] excludes)
        {
            return new ExpandoPropertyFilter() { Excludes = excludes };
        }
        //default is empty filter, support for extensions, eg: filter by context query string, etc...
        public static Func<IExpandoPropertyFilter> Resolve = () => new Lazy<ExpandoPropertyFilter>(new ExpandoPropertyFilter()).Value;
    }
}
