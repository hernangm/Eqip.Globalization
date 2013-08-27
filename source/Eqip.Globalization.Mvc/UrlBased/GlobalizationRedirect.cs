using System.Web;
using System.Web.Mvc;

namespace Eqip.Globalization.Mvc.UrlBased
{
    public class GlobalizationRedirect
    {
        private GlobalizationManager Globalization { get; set; }


        public GlobalizationRedirect(GlobalizationManager globalization)
        {
            this.Globalization = globalization;
        }

        public void RedirectToLanguage()
        {
            if (Globalization.Settings.CultureStore == GlobalizationSettings.CultureLocation.Subdomain)
            {
                if (!Globalization.IsLocalizedURL())
                {
                    var request = HttpContext.Current.Request;
                    var lang = this.Globalization.GetPreferredLanguage();
                    var u = new UrlHelper(request.RequestContext);
                    HttpContext.Current.Response.Redirect(u.ChangeCultureLink(lang));
                }
            }
        }


    }
}
