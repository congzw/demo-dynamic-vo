using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DynamicModel.Expandos
{
    [TestClass]
    public class PropertyLazyBagExtensionsSpec
    {
        readonly MockBagItem _mockBagItem = MockBagItem.Instance;

        [TestMethod]
        public void IsFuncType_ObjectType_Should_Ok()
        {
            _mockBagItem.ObjectType.IsFuncType().Log("ObjectType.IsFuncType() => ").ShouldFalse();
            _mockBagItem.ObjectType.IsSyncFuncType().Log("ObjectType.IsSyncFuncType() => ").ShouldFalse();
            _mockBagItem.ObjectType.IsTaskFuncType().Log("ObjectType.IsTaskFuncType() => ").ShouldFalse();
            _mockBagItem.ObjectType.IsTaskFuncAsyncType().Log("ObjectType.IsTaskFuncAsyncType() => ").ShouldFalse();
        }

        [TestMethod]
        public void IsFuncType_SyncFuncType_Should_Ok()
        {
            _mockBagItem.SyncFuncType.IsFuncType().Log("SyncFuncType.IsFuncType() => ").ShouldTrue();
            _mockBagItem.SyncFuncType.IsSyncFuncType().Log("SyncFuncType.IsSyncFuncType() => ").ShouldTrue();
            _mockBagItem.SyncFuncType.IsTaskFuncType().Log("SyncFuncType.IsTaskFuncType() => ").ShouldFalse();
            _mockBagItem.SyncFuncType.IsTaskFuncAsyncType().Log("SyncFuncType.IsTaskFuncAsyncType() => ").ShouldFalse();
        }

        [TestMethod]
        public void IsFuncType_TaskFuncType_Should_Ok()
        {
            _mockBagItem.TaskFuncType.IsFuncType().Log("TaskFuncType.IsFuncType() => ").ShouldTrue();
            _mockBagItem.TaskFuncType.IsSyncFuncType().Log("TaskFuncType.IsSyncFuncType() => ").ShouldFalse();
            _mockBagItem.TaskFuncType.IsTaskFuncType().Log("TaskFuncType.IsTaskFuncType() => ").ShouldTrue();
            _mockBagItem.TaskFuncType.IsTaskFuncAsyncType().Log("TaskFuncType.IsTaskFuncAsyncType() => ").ShouldTrue();
        }

        [TestMethod]
        public void IsFuncType_TaskFuncAsyncType_Should_Ok()
        {
            _mockBagItem.TaskFuncAsyncType.IsFuncType().Log("TaskFuncAsyncType.IsFuncType() => ").ShouldTrue();
            _mockBagItem.TaskFuncAsyncType.IsSyncFuncType().Log("TaskFuncAsyncType.IsSyncFuncType() => ").ShouldFalse();
            _mockBagItem.TaskFuncAsyncType.IsTaskFuncType().Log("TaskFuncAsyncType.IsTaskFuncType() => ").ShouldTrue();
            _mockBagItem.TaskFuncAsyncType.IsTaskFuncAsyncType().Log("TaskFuncAsyncType.IsTaskFuncAsyncType() => ").ShouldTrue();
        }

        [TestMethod]
        public void IsFuncType_LazyObjectType_Should_Ok()
        {
            _mockBagItem.LazyObjectType.IsLazyType().Log("LazyObjectType.IsLazyType() => ").ShouldTrue();
            _mockBagItem.LazyObjectType.IsSyncFuncType().Log("LazyObjectType.IsSyncFuncType() => ").ShouldFalse();
            _mockBagItem.LazyObjectType.IsTaskFuncType().Log("LazyObjectType.IsTaskFuncType() => ").ShouldFalse();
            _mockBagItem.LazyObjectType.IsTaskFuncAsyncType().Log("LazyObjectType.IsTaskFuncAsyncType() => ").ShouldFalse();
        }

        [TestMethod]
        public void IsFuncType_LazySyncFuncType_Should_Ok()
        {
            _mockBagItem.LazySyncFuncType.IsLazyType().Log("LazySyncFuncType.IsLazyType() => ").ShouldTrue();
            _mockBagItem.LazySyncFuncType.IsSyncFuncType().Log("LazySyncFuncType.IsSyncFuncType() => ").ShouldFalse();
            _mockBagItem.LazySyncFuncType.IsTaskFuncType().Log("LazySyncFuncType.IsTaskFuncType() => ").ShouldFalse();
            _mockBagItem.LazySyncFuncType.IsTaskFuncAsyncType().Log("LazySyncFuncType.IsTaskFuncAsyncType() => ").ShouldFalse();
        }

        [TestMethod]
        public void IsFuncType_LazyTaskFuncType_Should_Ok()
        {
            _mockBagItem.LazyTaskFuncType.IsLazyType().Log("LazyTaskFuncType.IsLazyType() => ").ShouldTrue();
            _mockBagItem.LazyTaskFuncType.IsSyncFuncType().Log("LazyTaskFuncType.IsSyncFuncType() => ").ShouldFalse();
            _mockBagItem.LazyTaskFuncType.IsTaskFuncType().Log("LazyTaskFuncType.IsTaskFuncType() => ").ShouldFalse();
            _mockBagItem.LazyTaskFuncType.IsTaskFuncAsyncType().Log("LazyTaskFuncType.IsTaskFuncAsyncType() => ").ShouldFalse();
        }

        [TestMethod]
        public void IsFuncType_LazyTaskFuncAsyncType_Should_Ok()
        {
            _mockBagItem.LazyTaskFuncAsyncType.IsLazyType().Log("LazyTaskFuncAsyncType.IsLazyType() => ").ShouldTrue();
            _mockBagItem.LazyTaskFuncAsyncType.IsSyncFuncType().Log("LazyTaskFuncAsyncType.IsSyncFuncType() => ").ShouldFalse();
            _mockBagItem.LazyTaskFuncAsyncType.IsTaskFuncType().Log("LazyTaskFuncAsyncType.IsTaskFuncType() => ").ShouldFalse();
            _mockBagItem.LazyTaskFuncAsyncType.IsTaskFuncAsyncType().Log("LazyTaskFuncAsyncType.IsTaskFuncAsyncType() => ").ShouldFalse();
        }

        [TestMethod]
        public void TryUnwrapFromLazy_Should_Ok()
        {
            _mockBagItem.ObjectValue.TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.SyncFunc.TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.TaskFunc.TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.TaskFuncAsync.TryUnwrapFromLazy().ShouldEqual("abc");

            _mockBagItem.LazyObject.TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.LazySyncFunc.TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.LazyTaskFunc.TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.LazyTaskFuncAsync.TryUnwrapFromLazy().ShouldEqual("abc");
        }

        [TestMethod]
        public void TryWrapAsLazy_Should_Ok()
        {
            _mockBagItem.ObjectValue.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.SyncFunc.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.TaskFunc.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.TaskFuncAsync.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");

            _mockBagItem.LazyObject.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.LazySyncFunc.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.LazyTaskFunc.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");
            _mockBagItem.LazyTaskFuncAsync.TryWrapAsLazy().TryUnwrapFromLazy().ShouldEqual("abc");
        }
    }
}
