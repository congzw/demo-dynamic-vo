using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Common.AppMessages
{
    public class AppMessageNotifyFilter : IExceptionFilter, IActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public AppMessageNotifyFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private List<IAppMessageNotifier> _notifiers;
        protected List<IAppMessageNotifier> Notifiers => _notifiers ??= _serviceProvider.GetServices<IAppMessageNotifier>().ToList();

        public void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                //todo
                foreach (var notifier in Notifiers)
                {
                    var appMessage = new AppMessage();
                    notifier.Notify(appMessage);
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //not care
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //todo
            if (context.ModelState.IsValid)
            {
            }
            else
            {
                var errors = context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            }
        }
    }
}
