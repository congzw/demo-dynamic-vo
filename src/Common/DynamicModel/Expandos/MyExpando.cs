using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Common.DynamicModel.Expandos
{
    public interface IMyExpando
    {
        void SetPropertyFilter(IExpandoPropertyFilter filter);
        IExpandoPropertyFilter GetPropertyFilter();

        //void Set<T>(string name, Func<T> func, bool ignoreFilter = false);
        //Task SetAsync<T>(string name, Func<Task<T>> func, bool ignoreFilter = false);
        //void Merge(object instance, bool greedy = true);
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
        private IExpandoPropertyFilter _propertyFilter;
        public void SetPropertyFilter(IExpandoPropertyFilter filter)
        {
            _propertyFilter = filter;
        }
        public IExpandoPropertyFilter GetPropertyFilter()
        {
            return _propertyFilter ?? ExpandoPropertyFilter.Resolve();
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
