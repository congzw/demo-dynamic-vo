using System;
using System.Linq;
using System.Threading.Tasks;

namespace Common.DynamicModel.Expandos
{
    public static class PropertyLazyBagExtensions
    {
        public static object TryWrapAsLazy(this object value)
        {
            //support => object, Func<>, Lazy<>
            //1 object _objectValue = "abc";
            //2.1 Func<string> _syncFunc = () => "abc";
            //2.2 Func<Task<string>> _taskFunc = () => Task.FromResult("abc");
            //2.3 Func<Task<string>> _taskFuncAsync = async () => await Task.FromResult("abc");
            //3.1 Lazy<string> LazyObject => new Lazy<string>("abc");
            //3.2 Lazy<Func<string>> LazySyncFunc => new Lazy<Func<string>>(() => "abc");
            //3.3 Lazy<Task<string>> LazyTaskFunc => new Lazy<Task<string>>(() => Task.FromResult("abc"));
            //3.4 Lazy<Task<string>> LazyTaskFuncAsync => new Lazy<Task<string>>(async () => await Task.FromResult("abc"));
            
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

            if (valueType.IsTaskFuncType())
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
                return TryUnwrapFromLazyReal(value);
            }

            //objectValue
            //syncFunc
            //taskFunc
            //taskFuncAsync
            if (valueType.IsSyncFuncType())
            {
                return ((dynamic)value)();
            }

            if (valueType.IsTaskFuncType())
            {
                //don't read data from theLazyObject.Result： it will potentially block the code in ASP.
                return ((dynamic)value)().GetAwaiter().GetResult();
            }

            return value;
        }

        private static object TryUnwrapFromLazyReal(this object theLazy)
        {
            var theLazyObject = ((dynamic)theLazy).Value;
            var theLazyObjectType = (Type)theLazyObject.GetType();

            var isTask = theLazyObjectType.IsTaskType();
            if (isTask)
            {
                //don't read data from theLazyObject.Result! it will potentially block the code in ASP.
                return theLazyObject.GetAwaiter().GetResult();
            }

            var isFunc = theLazyObjectType.IsFuncType();
            if (isFunc)
            {
                return theLazyObject();
            }

            return theLazyObject;
        }

        public static bool IsLazyType(this Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        public static bool IsFuncType(this Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Func<>));
            //return theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Func<>);
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

        public static bool IsTaskFuncType(this Type funcType)
        {
            var isFuncType = IsFuncType(funcType);
            if (!isFuncType)
            {
                return false;
            }
            
            //syncFuncType => System.Func`1[System.String]
            //asyncFuncType => System.Func`1[System.Threading.Tasks.Task`1[System.String]]
            var funcTypeArguments = funcType.GetGenericArguments();
            var argumentType = funcTypeArguments.FirstOrDefault();
            if (argumentType == null)
            {
                return false;
            }

            return argumentType.IsTaskType();
        }

        public static bool IsTaskFuncAsyncType(this Type theType)
        {
            //todo?
            return theType.IsTaskFuncType();
        }
        
        public static bool IsTaskType(this Type theType)
        {
            //System.String
            //System.Threading.Tasks.Task`1[System.String]
            //typeof(Task<string>).GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)) => True
            return theType.IsGenericType && theType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>));
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
        //    dynamic awaitable = methodInfo.Invoke(obj, parameters);
        //    await awaitable;
        //    return (T)awaitable.GetAwaiter().GetResult();
        //}

        //private static async Task InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
        //{
        //    dynamic awaitable = methodInfo.Invoke(obj, parameters);
        //    await awaitable;
        //}
    }
}