using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DynamicModel.Expandos
{
    [TestClass]
    public class ExpandoPropertyProviderSpec
    {
        [TestMethod]
        public void ApplyProviders_NoProviders_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.Id = "001";
            fooVo.Name = "fooVo";
            var fooVoContext = ExpandoPropertyContext<FooVo>.Create(fooVo);
            AsyncHelper.RunSync(async () => { await fooVoContext.ApplyProviders(null); });
            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("provider1Invoked", StringComparison.OrdinalIgnoreCase).ShouldFalse();
            jsonFilter.Contains("provider2Invoked", StringComparison.OrdinalIgnoreCase).ShouldFalse();
        }

        [TestMethod]
        public void ApplyProviders_HasProviders_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.Id = "001";
            fooVo.Name = "fooVo";

            //fooVo.IncludeAll();

            var fooVoContext = ExpandoPropertyContext<FooVo>.Create(fooVo);
            AsyncHelper.RunSync(async () => { await fooVoContext.ApplyProviders(new List<IExpandoPropertyProvider<FooVo>> { new MockProvider1(), new MockProvider2() }); });
            
            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("provider1Invoked", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            jsonFilter.Contains("provider2Invoked", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }
    }

    public class MockProvider1 : IExpandoPropertyProvider<FooVo>
    {
        public Task Process(ExpandoPropertyContext<FooVo> context)
        {
            var fooVo = context.Result;
            fooVo.Set("provider1", () => "provider1Invoked");
            return Task.CompletedTask;
        }
    }

    public class MockProvider2 : IExpandoPropertyProvider<FooVo>
    {
        public Task Process(ExpandoPropertyContext<FooVo> context)
        {
            var fooVo = context.Result;
            fooVo.Set("provider2", () => Task.FromResult("provider2Invoked"));
            return Task.CompletedTask;
        }
    }
}
