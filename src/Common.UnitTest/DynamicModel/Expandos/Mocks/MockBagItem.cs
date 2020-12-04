using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Common.DynamicModel.Expandos
{
    public class MockBagItem
    {
        public readonly string ObjectValue = "abc";
        public readonly Func<string> SyncFunc = () => "abc";
        public readonly Func<Task<string>> TaskFunc = () => Task.FromResult("abc");
        public readonly Func<Task<string>> TaskFuncAsync = async () => await Task.FromResult("abc");

        public Type ObjectType => ObjectValue.GetType();
        public Type SyncFuncType => SyncFunc.GetType();
        public Type TaskFuncType => TaskFunc.GetType();
        public Type TaskFuncAsyncType => TaskFuncAsync.GetType();

        public Lazy<string> LazyObject => new Lazy<string>(ObjectValue);
        public Lazy<Func<string>> LazySyncFunc => new Lazy<Func<string>>(SyncFunc);
        public Lazy<Task<string>> LazyTaskFunc => new Lazy<Task<string>>(TaskFunc);
        public Lazy<Task<string>> LazyTaskFuncAsync => new Lazy<Task<string>>(TaskFuncAsync);

        public Type LazyObjectType => LazyObject.GetType();
        public Type LazySyncFuncType => LazySyncFunc.GetType();
        public Type LazyTaskFuncType => LazyTaskFunc.GetType();
        public Type LazyTaskFuncAsyncType => LazyTaskFuncAsync.GetType();
        
        public static MockBagItem Instance = new MockBagItem();
    }
}
