using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Text;
using System.Globalization;


namespace Eqip.Globalization.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string ChangeCultureLink(this UrlHelper urlHelper, CultureInfo lang)
        {
            return urlHelper.Action(null, null, null, "http", BuildDomain(urlHelper.RequestContext.HttpContext.Request.RawUrl, lang.TwoLetterISOLanguageName, null, null, false));
        }

        private static string BuildDomain(string url, string subdomains, string domain, string tlds, bool addWWW)
        {
            var dn = DomainParser.Parse(url);
            var output = new List<string>();
            if (addWWW)
            {
                output.Add("www");
            }
            //output.Add(subdomains ?? dn.Subdomains);
            output.Add(domain ?? dn.Domain);
            output.Add(tlds ?? dn.TopLevelDomainsAsString);
            return string.Join(".", output);
        }
    }
}
