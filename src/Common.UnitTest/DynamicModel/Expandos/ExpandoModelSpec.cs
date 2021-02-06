using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Common.DynamicModel.Expandos
{
    [TestClass]
    public class ExpandoModelSpec
    {
        [TestMethod]
        public void JsonIgnore_CleanVo_Should_Ok()
        {
            var fooVo = new FooEntity();
            var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
            json.Contains(nameof(fooVo.NewtonIgnored), StringComparison.OrdinalIgnoreCase).ShouldFalse();
        }

        [TestMethod]
        public void JsonIgnore_Should_Ok()
        {
            var fooVo = new FooVo();
            var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
            json.Contains(nameof(fooVo.MsIgnored), StringComparison.OrdinalIgnoreCase).ShouldFalse();
            json.Contains(nameof(fooVo.NewtonIgnored), StringComparison.OrdinalIgnoreCase).ShouldFalse();
        }

        [TestMethod]
        public void Default_Should_Include_ALL()
        {
            var fooVo = new FooVo();
            fooVo.Id = "001";
            fooVo.Name = "fooVo";
            fooVo.Set("check1", "check1Invoked");
            fooVo.Set("check2", "check2Invoked");

            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("check1Invoked", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            jsonFilter.Contains("check2Invoked", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }

        [TestMethod]
        public void Set_Instance_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.Id = "001";
            fooVo.Name = "fooVo";
            fooVo.Set("check1", "check1Invoked");
            fooVo.Set("check2", "check2Invoked");

            fooVo.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("check1"));
            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("check1Invoked", StringComparison.OrdinalIgnoreCase).ShouldFalse();
            jsonFilter.Contains("check2Invoked", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }

        [TestMethod]
        public void Set_Func_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.Id = "001";
            fooVo.Name = "fooVo";
            fooVo.Set("check1", () => "check1Invoked");
            fooVo.Set("check2", () => "check2Invoked");

            fooVo.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("check1"));
            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("check1Invoked", StringComparison.OrdinalIgnoreCase).ShouldFalse();
            jsonFilter.Contains("check2Invoked", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }

        [TestMethod]
        public void Set_Lazy_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.Id = "001";
            fooVo.Name = "fooVo";
            fooVo.Set("check1", new Lazy<string>(() => "check1Invoked"));
            fooVo.Set("check2", new Lazy<string>(() => "check2Invoked"));

            fooVo.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("check1"));
            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("check1Invoked", StringComparison.OrdinalIgnoreCase).ShouldFalse();
            jsonFilter.Contains("check2Invoked", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }

        [TestMethod]
        public void Set_Delay_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.Id = "001";
            fooVo.Name = "fooVo";
            fooVo.Set("check1", () =>
            {
                fooVo.Name += "+check1";
                return "foo";
            });
            fooVo.Set("check2", () =>
            {
                fooVo.Name += "+check2";
                return "foo";
            });

            fooVo.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("check1"));
            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("+check1", StringComparison.OrdinalIgnoreCase).ShouldFalse();
            jsonFilter.Contains("+check2", StringComparison.OrdinalIgnoreCase).ShouldTrue();

            fooVo.Name.ShouldEqual("fooVo+check2");
        }

        [TestMethod]
        public void Merge_Should_Ok()
        {
            var fooVo = new FooVo();

            var fooEntity = new FooEntity();
            fooEntity.Id = "entity id";
            fooEntity.Name = "entity name";
            fooEntity.Desc = "entity desc";
            fooVo.Merge(fooEntity);
            
            var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
            json.Contains("entity id", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            json.Contains("entity name", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            json.Contains("entity desc", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }

        [TestMethod]
        public void AsExpandoModel_Should_Ok()
        {
            var fooEntity = new FooEntity();
            fooEntity.Id = "entity id";
            fooEntity.Name = "entity name";
            fooEntity.Desc = "entity desc";

            var excludeFilter = ExpandoPropertyFilterFactory.CreateExcludeFilter("Desc");
            var fooExpando = fooEntity.AsExpandoModel(excludeFilter);

            var json = JsonConvert.SerializeObject(fooExpando, Formatting.Indented).Log();
            json.Contains("entity id", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            json.Contains("entity name", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            json.Contains("entity desc", StringComparison.OrdinalIgnoreCase).ShouldFalse();
        }

        [TestMethod]
        public void AsExpandoVo_Should_Ok()
        {
            var fooEntity = new FooEntity();
            fooEntity.Id = "entity id";
            fooEntity.Name = "entity name";
            fooEntity.Desc = "entity desc";
            
            var fooVo = fooEntity.AsExpandoModel<FooVo>();
            fooVo.Id.ShouldEqual(fooEntity.Id);
            fooVo.Name.ShouldEqual(fooEntity.Name);

            var jsonVo = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
            jsonVo.Contains("entity id", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            jsonVo.Contains("entity name", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            jsonVo.Contains("entity desc", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }
    }
}