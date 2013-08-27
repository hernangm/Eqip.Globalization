using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Eqip.Globalization
{
    public class ResourceFormatter
    {

        public static string Format(string resourceValue)
        {
            return Format(resourceValue, null);
        }

        public static string Format(string resourceValue, params object[] args)
        {
            var replacementsCount = PlaceHoldersCount(resourceValue);
            if (replacementsCount == 0)
            {
                return resourceValue;
            }
            if (args == null)
            {
                throw new ArgumentNullException("The resource value cointains placeholders and no replacement values have been provided");
            }
            if (replacementsCount != args.Length)
            {
                throw new ArgumentException(string.Format("The number of replace parameters ({0}) does not match with the number of resource placeholders ({1}).", args.Length, replacementsCount));
            }
            return string.Format(resourceValue, args);
        }

        private static int PlaceHoldersCount(string text)
        {
            var matches = Regex.Matches(text.Replace(Environment.NewLine, ""), @"\{\{([^}]*)\}\}");
            return matches.Count;
        }
    }
}
