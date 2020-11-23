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

        void Set<T>(string name, Func<T> func, bool ignoreFilter = false);
        Task SetAsync<T>(string name, Func<Task<T>> func, bool ignoreFilter = false);

        void Merge(object instance, bool greedy = true);
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
        
        protected override IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = false)
        {
            var props = base.GetProperties(includeInstanceProperties);

            var filter = GetPropertyFilter();
            var ignorePropInfos = new List<PropertyInfo>();
            foreach (var propertyInfo in InstancePropertyInfo)
            {
                if (!filter.IncludeProp(propertyInfo))
                {
                    ignorePropInfos.Add(propertyInfo);
                }
            }

            foreach (var prop in props)
            {
                if (!filter.Include(prop.Key))
                {
                    continue;
                }

                if (ignorePropInfos.Any(x => prop.Key.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                yield return prop;
            }
        }

        public void Set<TProperty>(string name, TProperty value, bool ignoreFilter = false)
        {
            Set(name, () => value, ignoreFilter);
        }

        public void Set<T>(string name, Func<T> func, bool ignoreFilter = false)
        {
            if (ignoreFilter)
            {
                this[name] = func.Invoke();
                return;
            }

            if (GetPropertyFilter().Include(name))
            {
                this[name] = func.Invoke();
            }
        }

        public async Task SetAsync<T>(string name, Func<Task<T>> func, bool ignoreFilter = false)
        {
            if (ignoreFilter)
            {
                this[name] = func.Invoke();
                return;
            }

            if (GetPropertyFilter().Include(name))
            {
                this[name] = await func.Invoke();
            }
        }

        public void Merge(object instance, bool greedy = true)
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
                var prop = propertyInfo;
                if (!GetPropertyFilter().IncludeProp(propertyInfo))
                {
                    continue;
                }

                if (greedy)
                {
                    Set(prop.Name, () => prop.GetValue(instance, null));
                }
                else
                {
                    if (PropertyExist(prop.Name))
                    {
                        Set(prop.Name, () => prop.GetValue(instance, null));
                    }
                }

            }
        }

        protected bool PropertyExist(string prop)
        {
            var selfPropNames = GetProperties(true).Select(x => x.Key);
            foreach (var selfPropName in selfPropNames)
            {
                if (selfPropName.Equals(prop, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
