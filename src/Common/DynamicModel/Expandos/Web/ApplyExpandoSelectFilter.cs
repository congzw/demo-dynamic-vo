using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.DynamicModel.Expandos.Web
{
    public class ApplyExpandoSelectFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            var expandoSelect = ExpandoSelect.Parse(context.HttpContext);
            if (expandoSelect.HasSelectParams())
            {
                context.HttpContext.Response.Headers.Add("my-expando-select", new[] { $"$includes={expandoSelect.IncludesRawValue}&$excludes={expandoSelect.ExcludesRawValue}" });
            }

            var objectResult = context.Result as ObjectResult;
            var returnResult = objectResult?.Value;
            var processedResult = TryProcessExpandoModel(returnResult, expandoSelect, out var processed);
            if (processed)
            {
                context.Result = new ObjectResult(processedResult);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Can't add to headers here because the response has already begun.
        }

        private object TryProcessExpandoModel(object model, ExpandoSelect expandoSelect, out bool processed)
        {
            processed = false;
            if (model is ExpandoModel expandoModel)
            {
                processed = true;
                return expandoModel.ApplyExpandoSelect(expandoSelect);
            }

            if (model is IEnumerable<ExpandoModel> expandoModels)
            {
                processed = true;
                return expandoModels.ApplyExpandoSelect(expandoSelect);
            }

            //todo: auto apply expando select
            //1 ExpandoModel
            //2 IEnumerable<ExpandoModel>
            //3 todo: convert IEnumerable<ExpandoModel> to IPagerResult<ExpandoModel> by need!
            //4 todo: MessageResult.Data processed with 1,2,3

            return model;
        }
    }
}
