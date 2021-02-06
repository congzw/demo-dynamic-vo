using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.DynamicModel.Expandos
{
    public interface IExpandoModel
    {
        void AddPropertyFilter(IExpandoPropertyFilter filter);
        IExpandoPropertyFilter GetPropertyFilter();
    }

    public class ExpandoModel : Expando, IExpandoModel
    {
        public ExpandoModel()
        {
        }
        public ExpandoModel(object instance)
            : base(instance)
        {
        }
        public ExpandoModel(IDictionary<string, object> dict)
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

            var thisPropertyInfos = this.GetReturnInstancePropertyInfos().ToList();
            foreach (var propertyInfo in propertyInfos)
            {
                var theOne = thisPropertyInfos.FirstOrDefault(x => x.Name == propertyInfo.Name);
                if (theOne != null)
                {
                    theOne.SetValue(this, propertyInfo.GetValue(instance, null));
                }
                else
                {
                    Set(propertyInfo.Name, propertyInfo.GetValue(instance, null));
                }
            }
        }

        #region helpers

        public static ExpandoModel CreateExpandoModel(object instance, IExpandoPropertyFilter filter = null)
        {
            var expando = new ExpandoModel();
            expando.AddPropertyFilter(filter);
            expando.Merge(instance);
            return expando;
        }

        #endregion
    }

    public static class ExpandoModelExtensions
    {
        public static ExpandoModel AsExpandoModel(this object instance, IExpandoPropertyFilter filter = null)
        {
            if (instance is ExpandoModel expandoModel)
            {
                if (filter != null)
                {
                    expandoModel.AddPropertyFilter(filter);
                }
                return expandoModel;
            }

            var expando = new ExpandoModel();
            expando.AddPropertyFilter(filter);
            expando.Merge(instance);
            return expando;
        }

        public static T AsExpandoModel<T>(this object instance, IExpandoPropertyFilter filter = null) where T : ExpandoModel, new()
        {
            if (instance is T expandoModel)
            {
                expandoModel.AddPropertyFilter(filter);
                return expandoModel;
            }

            var expando = new T();
            expando.AddPropertyFilter(filter);
            expando.Merge(instance);
            return expando;
        }
    }
}
