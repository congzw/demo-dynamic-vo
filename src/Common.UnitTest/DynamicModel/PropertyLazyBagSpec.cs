using System;
using System.Threading.Tasks;
using Common.DynamicModel.Expandos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DynamicModel
{
    [TestClass]
    public class PropertyLazyBagSpec
    {
        [TestMethod]
        public void IsSyncFuncType_Should_Ok()
        {
            Func<string> syncFunc = () => "abc";
            var syncFuncType = syncFunc.GetType();
            syncFuncType.IsFuncType().Log("syncFuncType.IsFuncType() => ").ShouldTrue();
            syncFuncType.IsSyncFuncType().Log("syncFuncType.IsSyncFuncType() => ").ShouldTrue();
            syncFuncType.IsAsyncFuncType().Log("syncFuncType.IsAsyncFuncType() => ").ShouldFalse();
        }

        [TestMethod]
        public void TryWrapAsLazy_Object_Should_Ok()
        {
            var value = "abc";
            var valueWrap = value.TryWrapAsLazy();
            var valueWrapType = valueWrap.GetType();
            valueWrapType.Log("valueWrapType => ");
            valueWrapType.IsAssignableFrom(typeof(Lazy<string>)).Log("valueWrapType.IsAssignableFrom(typeof(Lazy<string>)) => ").ShouldTrue();
            
            valueWrap.TryUnwrapFromLazy().ShouldEqual("abc");
        }

        [TestMethod]
        public void TryWrapAsLazy_Func_Should_Ok()
        {
            Func<string> valueFunc = () => "abc";
            var valueFuncWrap = valueFunc.TryWrapAsLazy();
            var valueFuncWrapType = valueFuncWrap.GetType();
            valueFuncWrapType.Log("valueFuncWrapType => ");
            valueFuncWrapType.IsAssignableFrom(typeof(Lazy<string>)).Log("valueFuncWrapType.IsAssignableFrom(typeof(Lazy<string>)) => ").ShouldTrue();
            
            valueFuncWrap.TryUnwrapFromLazy().ShouldEqual("abc");
        }

        [TestMethod]
        public void TryWrapAsLazy_Lazy_Should_Ok()
        {
            Lazy<string> valueLazy = new Lazy<string>(() => "abc");
            var valueLazyWrap = valueLazy.TryWrapAsLazy();
            var valueLazyWrapType = valueLazyWrap.GetType();
            valueLazyWrapType.Log("valueLazyWrapType => ");
            valueLazyWrapType.IsAssignableFrom(typeof(Lazy<string>)).Log("valueLazyWrapType.IsAssignableFrom(typeof(Lazy<string>)) => ").ShouldTrue();

            valueLazyWrap.TryUnwrapFromLazy().ShouldEqual("abc");
        }


        [TestMethod]
        public void TryUnwrapFromLazy_Lazy_Should_Ok()
        {
            new Lazy<string>(() => "abc").TryUnwrapFromLazy().ShouldEqual("abc");
            new Func<string>(() => "abc").TryUnwrapFromLazy().ShouldEqual("abc");
            "abc".TryUnwrapFromLazy().ShouldEqual("abc");
        }
    }
}
