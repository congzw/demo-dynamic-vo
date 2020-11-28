using System;
using System.Collections.Generic;

namespace Common.DynamicModel.Expandos
{
    public class PropertyLazyBag
    {
        private readonly IDictionary<string, object> _cachedLazyProperties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly PropertyBag _propertyBag;

        public PropertyLazyBag(PropertyBag propertyBag)
        {
            _propertyBag = propertyBag;
        }

        public bool LazyDisabled { get; set; } = ExpandoLazyPropertyOptions.Resolve().SupportLazyProperty;

        public void TrySetLazyProp(string key, object value)
        {
            if (LazyDisabled)
            {
                _propertyBag[key] = value;
                return;
            }

            _cachedLazyProperties[key] = value.TryWrapAsLazy();
            if (_propertyBag.ContainsKey(key))
            {
                _propertyBag.Remove(key);
            }
        }

        public bool TryGetLazyProp(string key, out object result)
        {
            if (LazyDisabled)
            {
                return _propertyBag.TryGetValue(key, out result);
            }

            if (_propertyBag.ContainsKey(key))
            {
                result = _propertyBag[key];
                return true;
            }

            if (!_cachedLazyProperties.ContainsKey(key))
            {
                result = null;
                return false;
            }

            var cachedLazyProperty = _cachedLazyProperties[key];
            _propertyBag[key] = cachedLazyProperty.TryUnwrapFromLazy();
            _cachedLazyProperties.Remove(key);
            result = _propertyBag[key];
            return true;
        }
    }

    public static class ExpandoLazyPropertyExtensions
    {
        internal static object TryUnwrapFromLazy(this object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType().IsLazyType())
            {
                return ((dynamic)value).Value;
            }
            return value;
        }

        internal static object TryWrapAsLazy(this object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType().IsLazyType())
            {
                return value;
            }
            return new Lazy<object>(() => value);
        }

        private static bool IsLazyType(this Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Lazy<>);
        }
    }
}
