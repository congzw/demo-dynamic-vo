using System;
using Common.DynamicModel.Expandos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Common.DynamicModel
{
    [TestClass]
    public class MyExpandoSpec
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
        public void Indexer_AsLazy_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.SetLazyEnabled(true);
            fooVo.Id = "001";
            fooVo.Name = "fooVo";

            fooVo["check1"] = "check1";
            fooVo["check2"] = "check2";
            fooVo.SetPropertyFilter(ExpandoPropertyFilter.Create("check1"));

            var json = fooVo.GetJson(true);

            json.Contains("check1", StringComparison.OrdinalIgnoreCase).ShouldFalse();
            json.Contains("check2", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }

        [TestMethod]
        public void Indexer_AsNoLazy_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.SetLazyEnabled(false);

            fooVo.Id = "001";
            fooVo.Name = "fooVo";

            fooVo["check1"] = "check1";
            fooVo["check2"] = "check2";
            var json = fooVo.GetJson(true);
            json.Contains("check1", StringComparison.OrdinalIgnoreCase).ShouldTrue();
            json.Contains("check2", StringComparison.OrdinalIgnoreCase).ShouldTrue();

            fooVo.SetPropertyFilter(ExpandoPropertyFilter.Create("check1"));
            var jsonFilter = fooVo.GetJson(true);
            jsonFilter.Contains("check1", StringComparison.OrdinalIgnoreCase).ShouldFalse();
            jsonFilter.Contains("check2", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        }

        //[TestMethod]
        //public void Set_NoFilter_Should_Ok()
        //{
        //    var fooVo = new FooVo();

        //    fooVo["a"] = "111";
        //    fooVo.Set("b", () => "222");

        //    fooVo.Set("check1Invoked", () => "");
        //    fooVo.Set("check1", () =>
        //    {
        //        fooVo["check1Invoked"] = "check1Invoked:true";
        //        return "check1";
        //    });

        //    fooVo.Set("check2Invoked", () => "");
        //    fooVo.Set("check2", () =>
        //    {
        //        fooVo["check2Invoked"] = "check2Invoked:true";
        //        return "check2";
        //    });


        //    var json = fooVo.GetJson().Log();
        //    json.Contains("check1Invoked:true").ShouldTrue();
        //    json.Contains("check2Invoked:true").ShouldTrue();
        //}

        //[TestMethod]
        //public void Set_WithFilter_Should_Ok()
        //{
        //    var fooVo = new FooVo();
        //    fooVo.SetPropertyFilter(ExpandoPropertyFilter.Create("check1"));

        //    fooVo["a"] = "111";
        //    fooVo.Set("b", () => "222");

        //    fooVo.Set("check1Invoked", () => "");
        //    fooVo.Set("check1", () =>
        //    {
        //        fooVo["check1Invoked"] = "check1Invoked:true";
        //        return "check1";
        //    });

        //    fooVo.Set("check2Invoked", () => "");
        //    fooVo.Set("check2", () =>
        //    {
        //        fooVo["check2Invoked"] = "check2Invoked:true";
        //        return "check2";
        //    });


        //    var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
        //    json.Contains("check1Invoked:true").ShouldFalse();
        //    json.Contains("check2Invoked:true").ShouldTrue();
        //}

        //[TestMethod]
        //public void Merge_Greedy_Should_Ok()
        //{
        //    var fooVo = new FooVo();

        //    var fooEntity = new FooEntity();
        //    fooEntity.Id = "entity id";
        //    fooEntity.Name = "entity name";
        //    fooEntity.Desc = "entity desc";
        //    fooVo.Merge(fooEntity);

        //    var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
        //    json.Contains("entity id", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        //    json.Contains("entity name", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        //    json.Contains("entity desc", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        //}

        //[TestMethod]
        //public void Merge_NotGreedy_Should_Ok()
        //{
        //    var fooVo = new FooVo();

        //    var fooEntity = new FooEntity();
        //    fooEntity.Id = "entity id";
        //    fooEntity.Name = "entity name";
        //    fooEntity.Desc = "entity desc";
        //    fooVo.Merge(fooEntity, false);

        //    var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
        //    json.Contains("entity id", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        //    json.Contains("entity name", StringComparison.OrdinalIgnoreCase).ShouldTrue();
        //    json.Contains("entity desc", StringComparison.OrdinalIgnoreCase).ShouldFalse();
        //}
    }
}