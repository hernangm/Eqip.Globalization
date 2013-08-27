using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Globalization;
using System.Web.Routing;
using System.Threading;
using System.Web.Mvc;

namespace Eqip.Globalization.Mvc
{
    public class GlobalizationManager
    {

        public GlobalizationSettings Settings { get; private set; }
        public HttpContext HttpContext { get; private set; }

        public GlobalizationManager() : this(null, null) { }

        public GlobalizationManager(GlobalizationSettings settings, HttpContext httpContext)
        {
            this.Settings = settings ?? GlobalizationSettings.Settings;
            this.HttpContext = httpContext;
        }

        #region Methods
        public CultureInfo GetPreferredLanguage()
        {
            if (HttpContext != null)
            {
                var request = HttpContext.Request;
                if (request.UserLanguages != null && request.UserLanguages.Length != 0)
                {
                    var lang_code = string.Empty;
                    foreach (string lang in request.UserLanguages)
                    {
                        lang_code = lang.Substring(0, 2);
                        if (this.IsSupportedLanguage(lang_code))
                        {
                            return new CultureInfo(lang_code);
                        }
                    }
                }
            }
            return this.Settings.DefaultLanguage;
        }


        public bool IsSupportedLanguage(string lang)
        {
            try
            {
                var cult = new CultureInfo(lang);
                return Settings.SupportedLanguages.Contains(cult);
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }

        public CultureInfo GetCultureFromStore()
        {
            //Everything but CultureLocation.URL requires a valid HttpContext
            if (this.Settings.CultureStore != GlobalizationSettings.CultureLocation.URL)
            {
                if (HttpContext == null)
                    return null;
            }

            string cultureCode = string.Empty;

            switch (this.Settings.CultureStore)
            {
                case GlobalizationSettings.CultureLocation.Cookie:

                    if (HttpContext.Request.Cookies[this.Settings.CookieName] != null
                                        && HttpContext.Request.Cookies[this.Settings.CookieName].Value != string.Empty)
                    {
                        cultureCode = HttpContext.Request.Cookies[this.Settings.CookieName].Value;
                    }
                    break;

                case GlobalizationSettings.CultureLocation.QueryString:
                    cultureCode = HttpContext.Request[this.Settings.QueryStringParamName];
                    break;

                case GlobalizationSettings.CultureLocation.Session:
                    if (HttpContext.Session[this.Settings.SessionParamName] != null
                        && HttpContext.Session[this.Settings.SessionParamName].ToString() != string.Empty)
                    {
                        cultureCode = HttpContext.Session[this.Settings.SessionParamName].ToString();
                    }
                    break;

                case GlobalizationSettings.CultureLocation.URL:
                    if (HttpContext.Request.RequestContext.RouteData.Values.ContainsKey(this.Settings.LanguageActionParamName) && !string.IsNullOrEmpty(HttpContext.Request.RequestContext.RouteData.Values[this.Settings.LanguageActionParamName].ToString()))
                    {
                        cultureCode = HttpContext.Request.RequestContext.RouteData.Values[this.Settings.LanguageActionParamName].ToString();
                    }
                    if (this.Settings.CountryActionParamName != null && HttpContext.Request.RequestContext.RouteData.Values.ContainsKey(this.Settings.CountryActionParamName) && !string.IsNullOrEmpty(HttpContext.Request.RequestContext.RouteData.Values[this.Settings.CountryActionParamName].ToString()))
                    {
                        cultureCode += "-" + HttpContext.Request.RequestContext.RouteData.Values[this.Settings.CountryActionParamName].ToString();
                    }
                    break;

                case GlobalizationSettings.CultureLocation.Subdomain:
                    if (IsLocalizedURL())
                    {
                        cultureCode = DomainParser.Parse(this.HttpContext.Request.RawUrl).Subdomains.First();
                    }
                    break;
            }

            if (string.IsNullOrEmpty(cultureCode))
            {
                return null;
            }
            return new CultureInfo(cultureCode);
        }

        public void SetCurrentCulture(CultureInfo culture)
        {
            switch (this.Settings.CultureStore)
            {
                case GlobalizationSettings.CultureLocation.Cookie:
                    if (HttpContext.Request.Cookies.AllKeys.Contains(this.Settings.CookieName))
                    {
                        HttpContext.Response.Cookies[this.Settings.CookieName].Value = culture.TwoLetterISOLanguageName;
                    }
                    else
                    {
                        HttpContext.Response.Cookies.Add(new HttpCookie(this.Settings.CookieName, culture.TwoLetterISOLanguageName));
                    }
                    break;

                case GlobalizationSettings.CultureLocation.Session:
                    HttpContext.Session[this.Settings.SessionParamName] = culture.TwoLetterISOLanguageName;
                    break;
            }
            setThreadCulture(culture);
        }

        public void AutoSetCurrentCulture()
        {
            var currentCulture = this.GetCultureFromStore();
            if (currentCulture == null || !this.IsSupportedLanguage(currentCulture.TwoLetterISOLanguageName))
            {
                currentCulture = this.GetPreferredLanguage();
            }
            setThreadCulture(currentCulture);
        }

        public CultureInfo GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        private void setThreadCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public bool IsLocalizedURL()
        {
            var domain = DomainParser.Parse(this.HttpContext.Request.RawUrl);
            return domain.HasSubdomains && this.IsSupportedLanguage(domain.Subdomains.First());
        }
        #endregion
    }
}
