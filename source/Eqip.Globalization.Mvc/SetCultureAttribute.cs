using System.Web.Mvc;
using System.Globalization;
using System.Threading;
using System.Web.Routing;
using System;


namespace Eqip.Globalization.Mvc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SetCultureAttribute : Attribute { }

    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    //public class SetCultureFilterAttribute : SetCultureFilter { }

    public class SetCultureFilter : IActionFilter
    {
        public GlobalizationManager GlobalizationManager { get; private set; }

        public SetCultureFilter() : this(null) { }

        public SetCultureFilter(GlobalizationManager globalizationManager)
        {
            this.GlobalizationManager = globalizationManager ?? new GlobalizationManager();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.GlobalizationManager.Settings.CultureStore == GlobalizationSettings.CultureLocation.None)
            {
                return;
            }
            this.GlobalizationManager.AutoSetCurrentCulture();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) { }
    }
}