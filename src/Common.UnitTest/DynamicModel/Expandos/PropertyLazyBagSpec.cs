using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DynamicModel.Expandos
{
    [TestClass]
    public class PropertyLazyBagSpec
    {
        readonly MockBagItem _mockBagItem = MockBagItem.Instance;

        [TestMethod]
        public void TrySetGetLazyProp_NoLazy_Should_Ok()
        {
            var bag =new PropertyLazyBag(new PropertyBag());
            bag.TrySetLazyProp("ObjectValue", _mockBagItem.ObjectValue);
            bag.TryGetLazyProp("ObjectValue", out var theValue);
            theValue.ShouldEqual("abc");
            
            bag.TrySetLazyProp("SyncFunc", _mockBagItem.SyncFunc);
            bag.TryGetLazyProp("SyncFunc", out theValue);
            theValue.ShouldEqual("abc");

            bag.TrySetLazyProp("TaskFunc", _mockBagItem.TaskFunc);
            bag.TryGetLazyProp("TaskFunc", out theValue);
            theValue.ShouldEqual("abc");

            bag.TrySetLazyProp("TaskFuncAsync", _mockBagItem.TaskFuncAsync);
            bag.TryGetLazyProp("TaskFuncAsync", out var taskFuncAsync);
            taskFuncAsync.ShouldEqual("abc");
        }
        
        [TestMethod]
        public void TrySetGetLazyProp_Lazy_Should_Ok()
        {
            var bag = new PropertyLazyBag(new PropertyBag());

            bag.TrySetLazyProp("LazyObject", _mockBagItem.LazyObject);
            bag.TryGetLazyProp("LazyObject", out var theValue);
            theValue.ShouldEqual("abc");

            bag.TrySetLazyProp("LazyObject", _mockBagItem.LazyObject);
            bag.TryGetLazyProp("LazyObject", out theValue);
            theValue.ShouldEqual("abc");

            bag.TrySetLazyProp("LazySyncFunc", _mockBagItem.LazySyncFunc);
            bag.TryGetLazyProp("LazySyncFunc", out theValue);
            theValue.ShouldEqual("abc");

            bag.TrySetLazyProp("LazyTaskFunc", _mockBagItem.LazyTaskFunc);
            bag.TryGetLazyProp("LazyTaskFunc", out theValue);
            theValue.ShouldEqual("abc");

            bag.TrySetLazyProp("LazyTaskFuncAsync", _mockBagItem.LazyTaskFuncAsync);
            bag.TryGetLazyProp("LazyTaskFuncAsync", out theValue);
            theValue.ShouldEqual("abc");
        }
    }
}
