using System;
using System.Linq;
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
            fooVo.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("bad"));
            fooVo["a"] = "111";
            fooVo["bad"] = "bad";
            fooVo["good"] = "good";
            return JsonConvert.SerializeObject(fooVo, Formatting.Indented);
        }

        [HttpGet("GetVo2")]
        public string GetVo2()
        {
            var fooVo = new FooVo2();
            var expando = new ExpandoModel(fooVo);
            expando.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("bad"));

            dynamic dynamicVo = expando;
            dynamicVo.a = "222";
            dynamicVo.bad = "bad";
            dynamicVo.good = "good";
            return JsonConvert.SerializeObject(expando, Formatting.Indented);
        }

        [HttpGet("GetVo3")]
        public string GetVo3()
        {
            var expando = new ExpandoModel();
            expando.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("bad"));

            dynamic dynamicVo = expando;
            dynamicVo.a = "222";
            dynamicVo.bad = "bad";
            dynamicVo.good = "good";
            return JsonConvert.SerializeObject(expando, Formatting.Indented);
        }

        [HttpGet("GetVo4")]
        public string GetVo4()
        {
            var expando = new ExpandoModel();
            expando.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateExcludeFilter("bad"));
            expando.Set("A", () => Task.FromResult("abc"));
            expando.Set("B", async () => await Task.FromResult("abc"));
            expando.Set("bad", () => Task.FromResult("bad"));
            return JsonConvert.SerializeObject(expando, Formatting.Indented);
        }
        
        [HttpGet("GetSelect")]
        public ExpandoSelect GetSelect()
        {
           return ExpandoSelect.Parse(this.HttpContext);
        }

        [HttpGet("GetSelectVo")]
        public FooVo GetSelectVo()
        {
            var fooVo = new FooVo();
            fooVo.Id = "theId";
            fooVo.Name = "theName";
            fooVo.Title = "theTitle";
            fooVo.Description = "theDesc";

            fooVo.Set("a", "A");
            fooVo.Set("b", "B");
            fooVo.Set("C", "CC");

            //=> ~/Api/Foo/GetSelectVo?$includes=a,b&$excludes=b,c
            //=> {"a":"A","id":"theId","name":"theName","title":"theTitle","description":"theDesc"}

            //todo: add auto filter to pipelines
            var expandoQueryContext = ExpandoSelect.Parse(this.HttpContext);
            if (expandoQueryContext.Includes.Count > 0)
            {
                fooVo.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateIncludeFilter(expandoQueryContext.Includes.ToArray()));
            }
            if (expandoQueryContext.Excludes.Count > 0)
            {
                fooVo.AddPropertyFilter(ExpandoPropertyFilterFactory.CreateIncludeFilter(expandoQueryContext.Excludes.ToArray()));
            }
            return fooVo;
        }
    }

    public class FooVo : ExpandoModel
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

    public class FooLazyVo : ExpandoModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
