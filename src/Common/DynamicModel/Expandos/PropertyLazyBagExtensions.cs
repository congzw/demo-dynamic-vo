using System;
using System.Linq;

namespace Common.DynamicModel.Expandos
{
    public static class PropertyLazyBagExtensions
    {
        public static object TryUnwrapFromLazy(this object value)
        {
            //support => object, Func<>, Lazy<>
            if (value == null)
            {
                return null;
            }

            var valueType = value.GetType();
            if (valueType.IsLazyType())
            {
                return ((dynamic)value).Value;
            }
            
            if (valueType.IsFuncType())
            {
                return ((dynamic)value)();
            }

            return value;
        }

        public static object TryWrapAsLazy(this object value)
        {
            //support => object, Func<>, Lazy<>
            if (value == null)
            {
                return null;
            }

            var valueType = value.GetType();
            if (valueType.IsLazyType())
            {
                return value;
            }

            if (valueType.IsFuncType())
            {
                //Func<T> => T
                var genericArguments = valueType.GetGenericArguments();
                var argType = genericArguments.First();
                //T => Lazy<T>
                var lazy = CreateGenericLazy(argType, value);
                return lazy;
            }

            //T => Lazy<T>
            return CreateGenericLazy(valueType, value);
        }

        public static bool IsLazyType(this Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        public static bool IsFuncType(this Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Func<>);
        }

        private static object CreateGenericLazy(Type argType, object valueFunc)
        {
            var lazyType = typeof(Lazy<>).MakeGenericType(argType);
            var result = Activator.CreateInstance(lazyType, valueFunc);
            return result;
        }

        private static object CreateGenericFunc(Type argType)
        {
            //var genericArguments = valueType.GetGenericArguments();
            //var argType = genericArguments.First();
            //var test = CreateGenericLazy(argType, value);

            var funcType = CreateGenericFuncType(argType);
            var result = Activator.CreateInstance(funcType);
            return result;
        }

        private static Type CreateGenericFuncType(Type argType)
        {
            var funcType = typeof(Func<>).MakeGenericType(argType);
            return funcType;
        }
    }
}