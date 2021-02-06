using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.DynamicModel.Expandos
{
    public interface IMyExpando
    {
        void AddPropertyFilter(IExpandoPropertyFilter filter);
        IExpandoPropertyFilter GetPropertyFilter();
    }

    public class MyExpando : Expando, IMyExpando
    {
        public MyExpando()
        {
        }
        public MyExpando(object instance)
            : base(instance)
        {
        }
        public MyExpando(IDictionary<string, object> dict)
            : base(dict)
        {
        }
        
        //for filter extensions 
        private readonly ExpandoPropertyCompositeFilter _propertyFilters = new ExpandoPropertyCompositeFilter();
        public void AddPropertyFilter(IExpandoPropertyFilter filter)
        {
            _propertyFilters.AddPropertyFilter(filter);
        }

        public IExpandoPropertyFilter GetPropertyFilter()
        {
            if (_propertyFilters.Filters.Count == 0)
            {
                return ExpandoPropertyFilterFactory.Resolve();
            }
            return _propertyFilters;
        }

        protected override IEnumerable<string> GetReturnDynamicMembers()
        {
            var filter = GetPropertyFilter();
            return base.GetReturnDynamicMembers().Where(x => filter.IncludeDynamicProperty(x));
        }

        protected override IEnumerable<PropertyInfo> GetReturnInstancePropertyInfos()
        {
            var filter = GetPropertyFilter();
            return base.GetReturnInstancePropertyInfos().Where(x => filter.IncludeInstanceProperty(x));
        }

        public void Set<TProperty>(string name, TProperty value)
        {
            this[name] = value;
        }

        public void Set<T>(string name, Func<T> func)
        {
            this[name] = func;
        }

        public void Set<T>(string name, Lazy<T> lazy)
        {
            this[name] = lazy;
        }
        
        public void Merge(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var propertyInfos = instance.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                    .ToList();

            foreach (var propertyInfo in propertyInfos)
            {
                Set(propertyInfo.Name, propertyInfo.GetValue(instance, null));
            }
        }
    }
}
