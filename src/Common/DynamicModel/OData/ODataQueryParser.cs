using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Common.DynamicModel.OData
{
    /// <summary>
    ///目前就支持$select
    /// </summary>
	internal class ODataQueryParser
    {
        public const string Query_Record = "$Record";
        public static ODataQueryResult Parse(string queryString)
		{
			return Parse(HttpUtility.ParseQueryString(queryString));
		}

        public static ODataQueryResult Parse(NameValueCollection parameters)
		{

			var filterString = parameters["$filter"];
			if (!string.IsNullOrEmpty(filterString))
			{
                //
			}

			var orderByString = parameters["$orderby"];
			if (!string.IsNullOrEmpty(orderByString))
			{
			}

			int? top = null;
			var topString = parameters["$top"];
			if (!string.IsNullOrEmpty(topString))
			{
				int topValue;
				if (!int.TryParse(topString, out topValue))
				{
                    throw new ODataParseException("Expected an integer value for $top: " + topString);
				}
				top = topValue;
			}

			var skip = 0;
			var skipString = parameters["$skip"];
			if (!string.IsNullOrEmpty(skipString))
			{
				if (!int.TryParse(skipString, out skip))
				{
					throw new ODataParseException("Expected an integer value for $skip: " + skipString);
				}
			}

            var select = new ODataSelectColumnCollection();
            var selectString = parameters["$select"];
            if (!string.IsNullOrEmpty(selectString))
            {
                //todo 二级属性的处理
                var selectParts = selectString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => !s.Contains(".")).ToList();
                if (selectParts.Any(x => x == "*"))
                {
                    select.AllColumns = true;
                    if (selectParts.Any(x=>x.Equals(Query_Record, StringComparison.OrdinalIgnoreCase)))
                    {
                        select.Record = true;
                    }
                }
                else
                {
                    selectParts.ForEach(s => @select.Add(new ODataSelectColumn
                    {
                        ColumnName = s
                    }));
                }
            }
            else
            {
                select.DefaultColumns = true;
            }
            var formatParameter = parameters["$format"];
            string format;
            if (!string.IsNullOrEmpty(formatParameter))
            {
                format = formatParameter;
            }
            else
            {
                format = null;
            }
			return new ODataQueryResult
			{
			    Select=select,
                Top=top,
                Skip = skip
			};
		}
	}
}
