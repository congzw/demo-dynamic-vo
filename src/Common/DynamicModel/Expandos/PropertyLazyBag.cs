using System;
using System.Collections.Generic;

namespace Common.DynamicModel.Expandos
{
    public class PropertyLazyBag
    {
        public IDictionary<string, object> CachedLazyProperties { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        private readonly PropertyBag _propertyBag;

        public PropertyLazyBag(PropertyBag propertyBag)
        {
            _propertyBag = propertyBag;
        }

        public void TrySetLazyProp(string key, object value)
        {
            CachedLazyProperties[key] = value.TryWrapAsLazy();
            if (_propertyBag.ContainsKey(key))
            {
                _propertyBag.Remove(key);
            }
        }

        public bool TryGetLazyProp(string key, out object result)
        {
            if (_propertyBag.ContainsKey(key))
            {
                result = _propertyBag[key];
                return true;
            }

            if (!CachedLazyProperties.ContainsKey(key))
            {
                result = null;
                return false;
            }

            var cachedLazyProperty = CachedLazyProperties[key];
            result = cachedLazyProperty.TryUnwrapFromLazy();
            _propertyBag[key] = result;
            return true;
        }
    }
}
