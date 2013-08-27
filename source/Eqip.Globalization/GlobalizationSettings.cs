using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Configuration;
using System.Web;
using System.ComponentModel;
using System.Collections.Specialized;
using Eqip.Globalization.Utils;

namespace Eqip.Globalization
{
    public class GlobalizationSettings : ConfigurationSection
    {

        #region Enums

        /// <summary>
        /// Represents the location the culture code can be found
        /// </summary>
        public enum CultureLocation
        {
            /// <summary>
            /// This option should never be used.
            /// </summary>
            None = 0,
            /// <summary>
            /// Use when the culture code is saved in a cookie.  
            /// When using be sure to specify the CookieName property.
            /// </summary>
            Cookie = 1,
            /// <summary>
            /// Use when the culture code is specified in the query string.  
            /// When using be sure to specify the QueryStringParamName property.
            /// </summary>
            QueryString = 2,
            /// <summary>
            /// Use when the culture code is saved in session state.  
            /// When using be sure to specify the SessionParamName property.
            /// </summary>
            Session = 4,
            /// <summary>
            /// Use when the culture code is specified in the URL.  
            /// This assume a format of "{language}/{country}".
            /// When using be sure to specify the CountryActionParamName and 
            /// LanguageActionParamName properties.
            /// </summary>
            URL = 16,
            /// <summary>
            /// Use when the culture code is specified as a subdomain.  
            /// This assume a format of "{language}.example.com".
            /// </summary>
            Subdomain = 32
        }
        #endregion Enums

        #region Settings
        private static GlobalizationSettings settings = ConfigurationManager.GetSection("GlobalizationSettings") as GlobalizationSettings;

        public static GlobalizationSettings Settings
        {
            get
            {
                return settings;
            }
        }
        #endregion

        #region Properties
        [ConfigurationProperty("DefaultLanguage", IsRequired = true)]
        [TypeConverter(typeof(CultureInfoConverter))]
        public CultureInfo DefaultLanguage
        {
            get { return (CultureInfo)this["DefaultLanguage"]; }
            set { this["DefaultLanguage"] = value; }
        }

        [ConfigurationProperty("SupportedLanguages", IsRequired = true)]
        [TypeConverter(typeof(CultureInfoCollectionConverter))]
        public List<CultureInfo> SupportedLanguages
        {
            get
            {
                return (List<CultureInfo>)this["SupportedLanguages"];
            }
            set { this["SupportedLanguages"] = value; }
        }

        /// <summary>
        /// The CultureLocation where the culture code is to be read from.  This is required to be set.
        /// </summary>
        [ConfigurationProperty("CultureStore", IsRequired = true, DefaultValue = CultureLocation.Session)]
        public CultureLocation CultureStore
        {
            get { return (CultureLocation)this["CultureStore"]; }
            set { this["CultureStore"] = value; }
        }

        /// <summary>
        /// The name of the cookie containing the culture code.  Specify this value when CultureStore is set to Cookie.
        /// </summary>
        [ConfigurationProperty("CookieName", IsRequired = false)]
        public string CookieName
        {
            get { return (string)this["CookieName"]; }
            set { this["CookieName"] = value; }
        }

        /// <summary>
        /// The name of the action parameter containing the country code.  Specify this value when CultureStore is set to URL.
        /// </summary>
        [ConfigurationProperty("CountryActionParamName", IsRequired = false)]
        public string CountryActionParamName
        {
            get { return (string)this["CountryActionParamName"]; }
            set { this["CountryActionParamName"] = value; }
        }

        /// <summary>
        /// The name of the action parameter containing the language code.  Specify this value when CultureStore is set to URL.
        /// </summary>
        [ConfigurationProperty("LanguageActionParamName", IsRequired = false)]
        public string LanguageActionParamName
        {
            get { return (string)this["LanguageActionParamName"]; }
            set { this["LanguageActionParamName"] = value; }
        }

        /// <summary>
        /// The name of the query string parameter containing the country code.  Specify this value when CultureStore is set to QueryString.
        /// </summary>
        [ConfigurationProperty("QueryStringParamName", IsRequired = false)]
        public string QueryStringParamName
        {
            get { return (string)this["QueryStringParamName"]; }
            set { this["QueryStringParamName"] = value; }
        }

        /// <summary>
        /// The name of the session parameter containing the country code.  Specify this value when CultureStore is set to Session.
        /// </summary>
        [ConfigurationProperty("SessionParamName", IsRequired = false)]
        public string SessionParamName
        {
            get { return (string)this["SessionParamName"]; }
            set { this["SessionParamName"] = value; }
        }
        #endregion
    }
}
