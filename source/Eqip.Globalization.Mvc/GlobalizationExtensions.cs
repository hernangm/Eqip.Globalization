using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace Eqip.Globalization.Mvc
{
    public static class GlobalizationExtensions
    {

        public static string Translate(this HtmlHelper helper, object obj, string property)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException("property");
            }

            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var prop = obj.GetType().GetProperty(property + "_" + lang);
            if (prop == null)
            {
                throw new ArgumentNullException(string.Format("Property \"{0}\" does not exist for the {1} language.", property, lang));
            }
            var value = prop.GetValue(obj, null);
            if (value == null)
                return string.Empty;

            return prop.GetValue(obj, null).ToString();
        }


        public static MvcHtmlString LanguageSelector(this HtmlHelper helper)
        {
            var g = new GlobalizationManager();
            var sb = new StringBuilder();

            if (g.Settings.SupportedLanguages.Count > 1)
            {
                var cc = g.GetCurrentCulture();
                foreach (var l in g.Settings.SupportedLanguages)
                {
                    if (!l.Equals(cc))
                    {
                        sb.Append("<li>{0}</li>".FormatWith(CreateLanguageOption(helper, g, l)));
                    }
                }

                var ul = new TagBuilder("ul");
                ul.GenerateId("language-selector");
                ul.InnerHtml = sb.ToString();
                return MvcHtmlString.Create(ul.ToString());
            }
            return MvcHtmlString.Empty;
        }


        private static string CreateLanguageOption(HtmlHelper helper, GlobalizationManager g, CultureInfo lang)
        {
            switch (g.Settings.CultureStore)
            {

                case GlobalizationSettings.CultureLocation.None:
                    return string.Empty;
                case GlobalizationSettings.CultureLocation.Cookie:
                    return string.Empty;
                case GlobalizationSettings.CultureLocation.Session:
                    return string.Empty;
                case GlobalizationSettings.CultureLocation.URL:
                    return string.Empty;
                case GlobalizationSettings.CultureLocation.QueryString:
                    return string.Empty;
                case GlobalizationSettings.CultureLocation.Subdomain:
                    var u = new UrlHelper(helper.ViewContext.RequestContext);
                    return string.Format("<a href=\"{0}\">{1}</a>", u.ChangeCultureLink(lang), lang.DisplayName);
            }
            return string.Empty;
        }
    }
}
