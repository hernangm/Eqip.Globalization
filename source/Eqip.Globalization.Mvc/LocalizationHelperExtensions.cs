using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Eqip.Globalization.Mvc
{
    public static class LocalizationHelperExtensions
    {
        public static IHtmlString Get(this LocalizationHelper helper, string key)
        {
            return helper.Get(key, null);
        }

        public static IHtmlString Get(this LocalizationHelper helper, string key, params object[] replaceValues)
        {
            var parsedResource = ResourceParser.Parse(key);
            var resourceValue = helper.HtmlHelper.ViewContext.HttpContext.GetGlobalResourceObject(parsedResource.ResourceSet, parsedResource.ResourceKey).ToString();
            return helper.HtmlHelper.Raw(ResourceFormatter.Format(resourceValue, replaceValues));
        }
    }
}
