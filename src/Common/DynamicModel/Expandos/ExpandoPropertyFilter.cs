using System;
using System.Collections.Generic;

namespace Common.DynamicModel.Expandos
{
    public interface IExpandoPropertyFilter
    {
        public IEnumerable<string> Filter(IEnumerable<string> propNames);
    }

    public class ExpandoPropertyFilter : IExpandoPropertyFilter
    {
        public IEnumerable<string> Filter(IEnumerable<string> propNames)
        {
            return propNames;
        }

        //default is empty filter, support for extensions, eg: filter by context query string, etc...
        public static Func<IExpandoPropertyFilter> Resolve = () => new Lazy<ExpandoPropertyFilter>(new ExpandoPropertyFilter()).Value;
    }
}
