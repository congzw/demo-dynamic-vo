using System;
using System.Linq;
using System.Threading.Tasks;

namespace Common.DynamicModel.Expandos
{
    public static class PropertyLazyBagExtensions
    {
        public static object TryUnwrapFromLazy(this object value)
        {
            //support => object, Func<>, Func<Task<>>, Lazy<>
            if (value == null)
            {
                return null;
            }

            var valueType = value.GetType();
            if (valueType.IsLazyType())
            {
                var theLazyObject = ((dynamic)value).Value;
                var theLazyObjectType = (Type)theLazyObject.GetType();

                var isTask = theLazyObjectType.IsGenericType && theLazyObjectType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>));
                if (isTask)
                {
                    //todo: dead lock?
                    return theLazyObject.GetAwaiter().GetResult();
                }

                var isFunc = theLazyObjectType.IsGenericType && theLazyObjectType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Func<>));
                if (isFunc)
                {
                    return theLazyObject();
                }
                
                return theLazyObject;
            }

            if (valueType.IsSyncFuncType())
            {
                return ((dynamic)value)();
            }
            
            if (valueType.IsAsyncFuncType())
            {
                //todo: dead lock?
                return ((dynamic)value).GetAwaiter().GetResult().Result;
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
            
            if (valueType.IsSyncFuncType())
            {
                //Func<T> => T
                var genericArguments = valueType.GetGenericArguments();
                var argType = genericArguments.First();
                //T => Lazy<T>
                var lazy = CreateGenericLazy(argType, value);
                return lazy;
            }

            if (valueType.IsAsyncFuncType())
            {
                //Func<Task<T>> => Task<T>
                var genericArguments = valueType.GetGenericArguments();
                var argType = genericArguments.First();
                //T => Lazy<Task<T>>
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

        public static bool IsSyncFuncType(this Type funcType)
        {
            var isFuncType = IsFuncType(funcType);
            if (!isFuncType)
            {
                return false;
            }

            var funcTypeArguments = funcType.GetGenericArguments();
            var argumentType = funcTypeArguments.FirstOrDefault();
            if (argumentType == null)
            {
                return false;
            }

            //typeof(Task<string>).GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)) => True
            var isTask = argumentType.IsGenericType && argumentType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>));
            return !isTask;
        }

        public static bool IsAsyncFuncType(this Type funcType)
        {
            //syncFuncType => System.Func`1[System.String]
            //asyncFuncType => System.Func`1[System.Threading.Tasks.Task`1[System.String]]
            var isFuncType = IsFuncType(funcType);
            if (!isFuncType)
            {
                return false;
            }

            var funcTypeArguments = funcType.GetGenericArguments();
            var argumentType = funcTypeArguments.FirstOrDefault();
            if (argumentType == null)
            {
                return false;
            }
            
            //typeof(Task<string>).GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)) => True
            return argumentType.IsGenericType && argumentType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>));
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

        //private static async Task<T> InvokeAsync<T>(this MethodInfo methodInfo, object obj, params object[] parameters)
        //{
        //    //todo: deadlock check
        //    dynamic awaitable = methodInfo.Invoke(obj, parameters);
        //    await awaitable;
        //    return (T)awaitable.GetAwaiter().GetResult();
        //}

        //private static async Task InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
        //{
        //    //todo: deadlock check
        //    dynamic awaitable = methodInfo.Invoke(obj, parameters);
        //    await awaitable;
        //}
    }
}