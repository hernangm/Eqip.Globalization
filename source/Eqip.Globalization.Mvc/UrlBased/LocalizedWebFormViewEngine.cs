using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading;

namespace Eqip.Globalization.Mvc.UrlBased
{
    public class LocalizedWebFormViewEngine : WebFormViewEngine
    {
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            string localizedPartialViewName = partialViewName;
            if (!string.IsNullOrEmpty(partialViewName))
                localizedPartialViewName += "." + Thread.CurrentThread.CurrentUICulture.Name;

            var result = base.FindPartialView(controllerContext, localizedPartialViewName, useCache);

            if (result.View == null)
                result = base.FindPartialView(controllerContext, partialViewName, useCache);

            return result;
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            string localizedMasterName = masterName;
            if (!string.IsNullOrEmpty(masterName))
                localizedMasterName += "." + Thread.CurrentThread.CurrentUICulture.Name;


            string localizedViewName = string.Empty;
            if (!string.IsNullOrEmpty(viewName))
                localizedViewName = viewName + "." + Thread.CurrentThread.CurrentUICulture.Name;

            var result = base.FindView(controllerContext, localizedViewName, localizedMasterName, useCache);

            if (result.View != null)
                return result;

            if (!string.IsNullOrEmpty(viewName))
                localizedViewName = viewName + "." + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            result = base.FindView(controllerContext, localizedViewName, localizedMasterName, useCache);

            if (result.View != null)
                return result;

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
    }
}
