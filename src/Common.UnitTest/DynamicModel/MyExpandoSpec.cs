using Common.DynamicModel.Expandos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Common.DynamicModel
{
    [TestClass]
    public class MyExpandoSpec
    {
        [TestMethod]
        public void Set_NoFilter_Should_Ok()
        {
            var fooVo = new FooVo();

            fooVo["a"] = "111";
            fooVo.Set("b", () => "222");

            fooVo.Set("check1Invoked", () => "");
            fooVo.Set("check1", () =>
            {
                fooVo["check1Invoked"] = "check1Invoked:true";
                return "check1";
            });

            fooVo.Set("check2Invoked", () => "");
            fooVo.Set("check2", () =>
            {
                fooVo["check2Invoked"] = "check2Invoked:true";
                return "check2";
            });


            var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
            json.Contains("check1Invoked:true").ShouldTrue();
            json.Contains("check2Invoked:true").ShouldTrue();
        }
        
        [TestMethod]
        public void Set_WithFilter_Should_Ok()
        {
            var fooVo = new FooVo();
            fooVo.SetPropertyFilter(ExpandoPropertyFilter.Create("check1"));

            fooVo["a"] = "111";
            fooVo.Set("b", () => "222");

            fooVo.Set("check1Invoked", () => "");
            fooVo.Set("check1", () =>
            {
                fooVo["check1Invoked"] = "check1Invoked:true";
                return "check1";
            });

            fooVo.Set("check2Invoked", () => "");
            fooVo.Set("check2", () =>
            {
                fooVo["check2Invoked"] = "check2Invoked:true";
                return "check2";
            });


            var json = JsonConvert.SerializeObject(fooVo, Formatting.Indented).Log();
            json.Contains("check1Invoked:true").ShouldFalse();
            json.Contains("check2Invoked:true").ShouldTrue();
        }
    }
}