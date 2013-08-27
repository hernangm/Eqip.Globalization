using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

namespace Eqip.Globalization.Mvc
{
    public class LocalizationHelper
    {
        public HtmlHelper HtmlHelper { get; private set; }

        public LocalizationHelper(HtmlHelper htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }


    }
}
