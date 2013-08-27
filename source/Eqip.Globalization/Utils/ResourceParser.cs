using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.Compilation;

namespace Eqip.Globalization
{
    public sealed class ResourceParserResult
    {
        public string ResourceSet { get; set; }
        public string ResourceKey { get; set; }
    }

    public static class ResourceParser
    {

        public static ResourceParserResult Parse(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("cannot parse null or empty key");
            }
            var array  = key.Split('_');
            if (array.Length < 2) {
                throw new FormatException("key has an invalid format");
            }
            return new ResourceParserResult()
            {
                ResourceSet = array.First(),
                ResourceKey = string.Join("_", array.Skip(1))
            };
        }

    }
}
