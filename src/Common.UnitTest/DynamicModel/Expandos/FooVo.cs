using System.Collections.Generic;
using System.Linq;

namespace Common.DynamicModel.Expandos
{
    public class FooVo : MyExpando
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string NewtonIgnored { get; set; } = "NewtonIgnored";

        [System.Text.Json.Serialization.JsonIgnore]
        public string MsIgnored { get; set; } = "MsIgnored";

        //for test
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var memberNames = base.GetDynamicMemberNames();
            var list = memberNames.ToList();
            ("get dynamic names => " + string.Join(',', list)).Log();
            return list;
        }
    }

    public class FooEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string NewtonIgnored { get; set; } = "NewtonIgnored";

        [System.Text.Json.Serialization.JsonIgnore]
        public string MsIgnored { get; set; } = "MsIgnored";
    }
}
