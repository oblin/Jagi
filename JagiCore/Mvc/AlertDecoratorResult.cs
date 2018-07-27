using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Mvc
{
    public class AlertDecoratorResult : ActionResult
    {
        public ActionResult InnerResult { get; set; }
        public string AlertClass { get; set; }
        public string Message { get; set; }


        public AlertDecoratorResult(ActionResult innerResult, string alertClass, string message)
        {
            InnerResult = innerResult;
            AlertClass = alertClass;
            Message = message;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = factory.GetTempData(context.HttpContext);
            tempData.AddAlert(new Alert(AlertClass, Message));
            InnerResult.ExecuteResult(context);
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            //var factory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            //var tempData = factory.GetTempData(context.HttpContext);
            var factory = context.HttpContext.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
            var tempData = factory.GetTempData(context.HttpContext);

            tempData.AddAlert(new Alert(AlertClass, Message));

            await InnerResult.ExecuteResultAsync(context);
        }
    }
}
