using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.DynamicModel.Expandos
{
    public interface ILazyExpando
    {
        void SetPropertyFilter(IExpandoPropertyFilter filter);
        IExpandoPropertyFilter GetPropertyFilter();

        void Set<T>(string name, Func<T> func, bool ignoreFilter = false);
        Task SetAsync<T>(string name, Func<Task<T>> func, bool ignoreFilter = false);
    }

    public class LazyExpando : Expando, ILazyExpando
    {
        public LazyExpando()
        {
        }
        public LazyExpando(object instance)
            : base(instance)
        {
        }
        public LazyExpando(IDictionary<string, object> dict)
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

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var memberNames = base.GetDynamicMemberNames();
            //add expando property filter for extensions
            var filter = GetPropertyFilter();
            foreach (var item in memberNames)
            {
                if (filter.Include(item))
                {
                    yield return item;
                }
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
    }
}
