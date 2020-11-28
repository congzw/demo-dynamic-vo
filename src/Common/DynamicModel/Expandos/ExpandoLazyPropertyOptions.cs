using System;

namespace Common.DynamicModel.Expandos
{
    public class ExpandoLazyPropertyOptions
    {
        /// <summary>
        /// 是否支持Lazy属性（默认支持）
        /// </summary>
        public bool SupportLazyProperty = true;

        //default is empty filter, support for extensions, eg: filter by context query string, etc...
        public static Func<ExpandoLazyPropertyOptions> Resolve = () => new Lazy<ExpandoLazyPropertyOptions>(new ExpandoLazyPropertyOptions()).Value;
    }
}