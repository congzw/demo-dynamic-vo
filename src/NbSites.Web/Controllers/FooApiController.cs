using System;
using System.Threading.Tasks;
using Common.DynamicModel.Expandos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NbSites.Web.Controllers
{
    [Route("Api/Foo")]
    public class FooApiController : ControllerBase
    {
        [HttpGet("CheckStatus")]
        public string CheckStatus()
        {
            return this.GetType().FullName + " => " + DateTime.Now;
        }

        [HttpGet("GetVo")]
        public string GetVo()
        {
            var fooVo = new FooVo();
            fooVo.SetPropertyFilter(ExpandoPropertyFilter.Create("bad"));
            fooVo["a"] = "111";
            fooVo["bad"] = "bad";
            fooVo["good"] = "good";
            return JsonConvert.SerializeObject(fooVo, Formatting.Indented);
        }

        [HttpGet("GetVo2")]
        public string GetVo2()
        {
            var fooVo = new FooVo2();
            var expando = new MyExpando(fooVo);
            expando.SetPropertyFilter(ExpandoPropertyFilter.Create("bad"));

            dynamic dynamicVo = expando;
            dynamicVo.a = "222";
            dynamicVo.bad = "bad";
            dynamicVo.good = "good";
            return JsonConvert.SerializeObject(expando, Formatting.Indented);
        }

        [HttpGet("GetVo3")]
        public string GetVo3()
        {
            var expando = new MyExpando();
            expando.SetPropertyFilter(ExpandoPropertyFilter.Create("bad"));

            dynamic dynamicVo = expando;
            dynamicVo.a = "222";
            dynamicVo.bad = "bad";
            dynamicVo.good = "good";
            return JsonConvert.SerializeObject(expando, Formatting.Indented);
        }

        [HttpGet("GetVo4")]
        public string GetVo4()
        {
            var expando = new MyExpando();
            expando.SetPropertyFilter(ExpandoPropertyFilter.Create("bad"));
            expando.Set("A", () => Task.FromResult("abc"));
            expando.Set("B", async () => await Task.FromResult("abc"));
            expando.Set("bad", () => Task.FromResult("bad"));
            return JsonConvert.SerializeObject(expando, Formatting.Indented);
        }

        //[HttpGet("GetLazyVo")]
        //public string GetLazyVo()
        //{
        //    var fooVo = new FooLazyVo();
        //    fooVo.SetPropertyFilter(ExpandoPropertyFilter.Create("check1"));
        //    fooVo["a"] = "111";
        //    fooVo.Set("b", () => "222");
        //    fooVo.Set("check1Invoked", () => "no!");
        //    fooVo.Set("check1", () =>
        //    {
        //        fooVo["check1Invoked"] = "yes!";
        //        return "check1";
        //    });

        //    fooVo.Set("check2Invoked", () => "no!");
        //    fooVo.Set("check2", () =>
        //    {
        //        fooVo["check2Invoked"] = "yes!";
        //        return "check2";
        //    });

        //    return JsonConvert.SerializeObject(fooVo, Formatting.Indented);
        //}

        //[HttpGet("GetLazyVoAsync")]
        //public async Task<string> GetLazyVoAsync()
        //{
        //    var fooVo = new FooLazyVo();
        //    fooVo.SetPropertyFilter(ExpandoPropertyFilter.Create("check1"));
        //    fooVo["a"] = "111";
        //    fooVo.Set("b", () => "222");

        //    await fooVo.SetAsync("check1Invoked", () => Task.FromResult("async no!"));
        //    await fooVo.SetAsync("check1", () =>
        //    {
        //        fooVo["check1Invoked"] = "async yes!";
        //        return Task.FromResult("check1");
        //    });

        //    await fooVo.SetAsync("check2Invoked", () => Task.FromResult("async no!"));
        //    await fooVo.SetAsync("check2", () =>
        //    {
        //        fooVo["check2Invoked"] = "async yes!";
        //        return Task.FromResult("check2");
        //    });

        //    return JsonConvert.SerializeObject(fooVo, Formatting.Indented);
        //}
    }

    public class FooVo : MyExpando
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class FooVo2
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class FooLazyVo : MyExpando
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
