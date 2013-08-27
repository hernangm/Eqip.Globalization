using System;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using System.ComponentModel;
using System.Web.Compilation;
using System.Collections.Generic;

namespace Eqip.Globalization
{
    /// <summary>
    /// This is the resource provider section that mimics the settings stored in DbResourceConfiguration object.
    /// </summary>
    public class DbResourceProviderSection : ConfigurationSection
    {
        [ConfigurationProperty("connectionString", DefaultValue = ""),
        Description("The connection string used to connect to the db Resourcewl provider")]
        public string ConnectionString
        {
            get { return this["connectionString"] as string; }
            set { this["connectionString"] = value; }
        }

        [Description("Determines whether any missing resources are automatically added to the Invariant culture. Defaults to true"),
         ConfigurationProperty("addMissingResources", DefaultValue = true)]
        public bool AddMissingResources
        {
            get { return (bool)this["addMissingResources"]; }
            set { this["addMissingResources"] = value; }
        }

        public DbResourceProviderSection(string connectionString, bool addMissingResources)
        {
            this.ConnectionString = connectionString;
            this.AddMissingResources = addMissingResources;
        }

        public DbResourceProviderSection()
        {

        }

    }
}

