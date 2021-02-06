using System;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DynamicModel.Expandos.Web
{
    public class ExpandoOptions
    {
        public ExpandoPropertyIncludeFilterDefaultMode DefaultMode { get; set; } = ExpandoPropertyIncludeFilterDefaultMode.None;
    }

    public static class ExpandoSetup
    {
        public static IMvcBuilder AddExpandoSelectFilter(this IMvcBuilder mvcBuilder, Action<ExpandoOptions> setupAction = null)
        {
            var expandoOption = new ExpandoOptions();
            setupAction?.Invoke(expandoOption);
            ExpandoPropertyFilterFactory.ResolveDefaultMode = () => expandoOption.DefaultMode;
            
            mvcBuilder.AddMvcOptions(options =>
            {
                options.Filters.Add<ApplyExpandoSelectFilter>();
            });
            return mvcBuilder;
        }
    }
}
