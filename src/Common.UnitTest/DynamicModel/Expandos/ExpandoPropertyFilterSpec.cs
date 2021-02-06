using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DynamicModel.Expandos
{
    [TestClass]
    public class ExpandoPropertyFilterSpec
    {
        [TestMethod]
        public void ExcludeFilter_Should_Ok()
        {
            var propertyFilter = ExpandoPropertyFilterFactory.CreateExcludeFilter("c", "d");
            propertyFilter.IncludeDynamicProperty("A").ShouldTrue();
            propertyFilter.IncludeDynamicProperty("b").ShouldTrue();
            propertyFilter.IncludeDynamicProperty("C").ShouldFalse();
            propertyFilter.IncludeDynamicProperty("c").ShouldFalse();
            
            var propertyFilter2 = ExpandoPropertyFilterFactory.CreateExcludeFilter("*");
            propertyFilter2.IncludeDynamicProperty("C").ShouldFalse();
            
            var propertyFilter3 = ExpandoPropertyFilterFactory.CreateExcludeFilter();
            propertyFilter3.IncludeDynamicProperty("C").ShouldTrue();
        }

        [TestMethod]
        public void IncludeFilter_Should_Ok()
        {
            var propertyFilter = ExpandoPropertyFilterFactory.CreateIncludeFilter("a", "b");
            propertyFilter.IncludeDynamicProperty("A").ShouldTrue();
            propertyFilter.IncludeDynamicProperty("b").ShouldTrue();
            propertyFilter.IncludeDynamicProperty("C").ShouldFalse();
            propertyFilter.IncludeDynamicProperty("c").ShouldFalse();


            var propertyFilter2 = ExpandoPropertyFilterFactory.CreateIncludeFilter("*");
            propertyFilter2.IncludeDynamicProperty("C").ShouldTrue();
            
            var propertyFilter3 = ExpandoPropertyFilterFactory.CreateIncludeFilter();
            propertyFilter3.IncludeDynamicProperty("C").ShouldFalse();
        }

        [TestMethod]
        public void CompositeFilter_Should_Ok()
        {
            var includeFilter = ExpandoPropertyFilterFactory.CreateIncludeFilter("a", "b");
            var includeFilter2 = ExpandoPropertyFilterFactory.CreateIncludeFilter("d");
            var propertyFilter = ExpandoPropertyFilterFactory.CreateCompositeFilter(includeFilter, includeFilter2);
            propertyFilter.IncludeDynamicProperty("A").ShouldTrue();
            propertyFilter.IncludeDynamicProperty("b").ShouldTrue();
            propertyFilter.IncludeDynamicProperty("C").ShouldFalse();
            propertyFilter.IncludeDynamicProperty("D").ShouldTrue();
            
            var includeFilter3 = ExpandoPropertyFilterFactory.CreateIncludeFilter("*");
            propertyFilter.AddPropertyFilter(includeFilter3);
            propertyFilter.IncludeDynamicProperty("D").ShouldTrue();

            var excludeFilter = ExpandoPropertyFilterFactory.CreateExcludeFilter("d");
            propertyFilter.AddPropertyFilter(excludeFilter);
            propertyFilter.IncludeDynamicProperty("D").ShouldFalse();

            var excludeFilter2 = ExpandoPropertyFilterFactory.CreateExcludeFilter("e");
            propertyFilter.AddPropertyFilter(excludeFilter2);
            propertyFilter.IncludeDynamicProperty("e").ShouldFalse();

            var excludeFilter3 = ExpandoPropertyFilterFactory.CreateExcludeFilter("a");
            propertyFilter.AddPropertyFilter(excludeFilter3);
            propertyFilter.IncludeDynamicProperty("a").ShouldFalse();

            propertyFilter.IncludeDynamicProperty("b").ShouldTrue();
            propertyFilter.IncludeDynamicProperty("x").ShouldTrue();
            var excludeFilter4 = ExpandoPropertyFilterFactory.CreateExcludeFilter("*");
            propertyFilter.AddPropertyFilter(excludeFilter4);
            propertyFilter.IncludeDynamicProperty("b").ShouldFalse();
            propertyFilter.IncludeDynamicProperty("x").ShouldFalse();
        }
    }
}
