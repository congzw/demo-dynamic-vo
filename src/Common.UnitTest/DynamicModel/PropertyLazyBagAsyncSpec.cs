using System;
using System.Linq;
using System.Threading.Tasks;
using Common.DynamicModel.Expandos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DynamicModel
{
    [TestClass]
    public class PropertyLazyBagAsyncSpec
    {
        //[TestMethod]
        //public void TryFuncTypes()
        //{
        //    typeof(Task<string>).IsAssignableFrom(typeof(Task)).Log("typeof(Task<string>).IsAssignableFrom(typeof(Task)) => ");
        //    typeof(Task<string>).IsGenericType.Log("typeof(Task<string>).IsGenericType => ");
        //    typeof(Task<string>).GetGenericTypeDefinition().Log("typeof(Task<string>).GetGenericTypeDefinition() => ");
        //    typeof(Task<string>).GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)).Log("typeof(Task<string>).GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)) => ");
        //    typeof(Task<string>).GetGenericTypeDefinition().GetGenericArguments().Log("typeof(Task<string>).GetGenericTypeDefinition().GetGenericArguments() => ");

        //    Func<string> syncFunc = () => "abc";
        //    var syncFuncType = syncFunc.GetType().Log("syncFuncType => ");
        //    var syncFuncTypeArguments = syncFuncType.GetGenericArguments().Log("syncFuncTypeArguments => ");
        //    var syncFuncTypeArgumentType = syncFuncTypeArguments.First().Log("syncFuncTypeArgumentType => ");
        //    syncFuncTypeArgumentType.IsAssignableFrom(typeof(string)).Log("syncFuncTypeArgumentType.IsAssignableFrom(typeof(string)) => ");
        //    syncFuncTypeArgumentType.IsAssignableFrom(typeof(Task<>)).Log("syncFuncTypeArgumentType.IsAssignableFrom(typeof(Task<>) => ");
            
        //    "-------------------".Log();

        //    var syncFuncTypeDefinition = syncFuncType.GetGenericTypeDefinition().Log("syncFuncTypeDefinition => ");
        //    syncFuncTypeDefinition.IsAssignableFrom(typeof(Func<>)).Log("syncFuncTypeDefinition.IsAssignableFrom(typeof(Func<>)) => ");
        //    var syncFuncArguments = syncFuncTypeDefinition.GetGenericArguments().Log("syncFuncArguments => ");

        //    "===============".Log();

        //    Func<Task<string>> asyncFunc = () => Task.FromResult("abc");
        //    var asyncFuncType = asyncFunc.GetType().Log("asyncFuncType => ");
        //    var asyncFuncTypeArguments = asyncFuncType.GetGenericArguments().Log("asyncFuncTypeArguments => ");
        //    var asyncFuncTypeArgumentType = asyncFuncTypeArguments.First().Log("asyncFuncTypeArgumentType => ");
        //    asyncFuncTypeArgumentType.IsAssignableFrom(typeof(Task<>)).Log("asyncFuncTypeArgumentType.IsAssignableFrom(typeof(Task<>)) => ");
        //    asyncFuncTypeArgumentType.GetGenericTypeDefinition().Log("asyncFuncTypeArgumentType.GetGenericTypeDefinition() => ");
        //    "-------------------".Log();

        //    var asyncFuncTypeDefinition = asyncFuncType.GetGenericTypeDefinition().Log("asyncFuncType.GetGenericTypeDefinition() => ");
        //    asyncFuncTypeDefinition.IsAssignableFrom(typeof(Func<>)).Log("asyncFuncType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Func<>)) => ");
        //    asyncFuncTypeDefinition.GetGenericArguments().Log("asyncFuncTypeDefinition.GetGenericArguments() => ");
        //    var asyncFuncArguments = asyncFuncTypeDefinition.GetGenericArguments().Log("asyncFuncArguments => ");
        //}
        
        [TestMethod]
        public void IsAsyncFuncType_Should_Ok()
        {
            Func<Task<string>> asyncFunc = () => Task.FromResult("abc");
            var asyncFuncType = asyncFunc.GetType();
            asyncFuncType.IsFuncType().Log("asyncFuncType.IsFuncType() => ").ShouldTrue();
            asyncFuncType.IsSyncFuncType().Log("asyncFuncType.IsSyncFuncType() => ").ShouldFalse();
            asyncFuncType.IsAsyncFuncType().Log("asyncFuncType.IsAsyncFuncType() => ").ShouldTrue();
        }
        
        [TestMethod]
        public void TryWrapAsLazy_AsyncFunc_Should_Ok()
        {
            Func<Task<string>> valueFunc = () => Task.FromResult("abc");
            var valueFuncWrap = valueFunc.TryWrapAsLazy();
            valueFuncWrap.TryUnwrapFromLazy().ShouldEqual("abc");
        }
        
        [TestMethod]
        public void TryWrapAsLazy_AwaitFunc_Should_Ok()
        {
            Func<Task<string>> valueFunc = async () => await Task.FromResult("abc");
            var valueFuncWrap = valueFunc.TryWrapAsLazy();
            valueFuncWrap.TryUnwrapFromLazy().ShouldEqual("abc");
        }
    }
}
