using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Resources;

namespace Eqip.Globalization
{
    public class DbSimpleResourceProvider : IResourceProvider, IImplicitResourceProvider
    {
        /// <summary>
        /// Keep track of the 'className' passed by ASP.NET
        /// which is the ResourceSetId in the database.
        /// </summary>
        private string _ResourceSetName;

        /// <summary>
        /// Cache for each culture of this ResourceSet. Once
        /// loaded we just cache the resources.
        /// </summary>
        private IDictionary _resourceCache;

        private IResourceDataManager ResourceDataManager {get; set;}

        private DbSimpleResourceProvider(IResourceDataManager resourceDataManager)
        {
            this.ResourceDataManager = resourceDataManager;
        }

        public DbSimpleResourceProvider(IResourceDataManager resourceDataManager, string virtualPath, string className) : this(resourceDataManager)
        {
            _ResourceSetName = className;
        }

        /// <summary>
        /// Manages caching of the Resource Sets. Once loaded the values are loaded from the
        /// cache only.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        private IDictionary GetResourceCache(string cultureName)
        {
            if (cultureName == null)
                cultureName = "";

            if (this._resourceCache == null)
                this._resourceCache = new ListDictionary();

            IDictionary Resources = this._resourceCache[cultureName] as IDictionary;
            if (Resources == null)
            {
                Resources = this.ResourceDataManager.GetResourceSet(cultureName as string, this._ResourceSetName);
                this._resourceCache[cultureName] = Resources;
            }

            return Resources;
        }

        /// <summary>
        /// Clears out the resource cache which forces all resources to be reloaded from
        /// the database.
        ///
        /// This is never actually called as far as I can tell
        /// </summary>
        public void ClearResourceCache()
        {
            this._resourceCache.Clear();
        }

        /// <summary>
        /// The main worker method that retrieves a resource key for a given culture
        /// from a ResourceSet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object IResourceProvider.GetObject(string ResourceKey, CultureInfo Culture)
        {
            string CultureName = null;
            if (Culture != null)
                CultureName = Culture.Name;
            else
                CultureName = CultureInfo.CurrentUICulture.Name;

            return this.GetObjectInternal(ResourceKey, CultureName);
        }

        /// <summary>
        /// Internal lookup method that handles retrieving a resource
        /// by its resource id and culture. Realistically this method
        /// is always called with the culture being null or empty
        /// but the routine handles resource fallback in case the
        /// code is manually called.
        /// </summary>
        /// <param name="ResourceKey"></param>
        /// <param name="CultureName"></param>
        /// <returns></returns>
        object GetObjectInternal(string ResourceKey, string CultureName)
        {
            IDictionary Resources = this.GetResourceCache(CultureName);

            object value = null;
            if (Resources == null)
                value = null;
            else
                value = Resources[ResourceKey];

            // *** If we're at a specific culture (en-Us) and there's no value fall back
            // *** to the generic culture (en)
            if (value == null && CultureName.Length > 3)
            {
                // *** try again with the 2 letter locale
                return GetObjectInternal(ResourceKey, CultureName.Substring(0, 2));
            }

            // *** If the value is still null get the invariant value
            if (value == null)
            {
                Resources = this.GetResourceCache("");
                if (Resources == null)
                    value = null;
                else
                    value = Resources[ResourceKey];
            }

            // *** If the value is still null and we're at the invariant culture
            // *** let's add a marker that the value is missing
            // *** this also allows the pre-compiler to work and never return null
            if (value == null && string.IsNullOrEmpty(CultureName))
            {
                // *** No entry there
                value = "";
                if (DbResourceConfiguration.Current.AddMissingResources)
                {
                    this.ResourceDataManager.AddResource(ResourceKey, value.ToString(), "", this._ResourceSetName);
                }

            }

            return value;
        }

        /// <summary>
        /// The Resource Reader is used parse over the resource collection
        /// that the ResourceSet contains. It's basically an IEnumarable interface
        /// implementation and it's what's used to retrieve the actual keys
        /// </summary>
        public IResourceReader ResourceReader  // IResourceProvider.ResourceReader
        {
            get
            {
                if (this._ResourceReader == null)
                {
                    this._ResourceReader = new DbResourceReader(GetResourceCache(null)) as IResourceReader;
                }
                return this._ResourceReader as IResourceReader;
            }
        }
        private IResourceReader _ResourceReader = null;


        #region IImplicitResourceProvider Members

        /// <summary>
        /// Called when an ASP.NET Page is compiled asking for a collection
        /// of keys that match a given control name (keyPrefix). This
        /// routine for example returns control.Text,control.ToolTip from the
        /// Resource collection if they exist when a request for "control"
        /// is made as the key prefix.
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public ICollection GetImplicitResourceKeys(string keyPrefix)
        {
            List<ImplicitResourceKey> keys = new List<ImplicitResourceKey>();

            IDictionaryEnumerator Enumerator = this.ResourceReader.GetEnumerator();
            if (Enumerator == null)
                return keys; // Cannot return null!

            foreach (DictionaryEntry dictentry in this.ResourceReader)
            {
                string key = (string)dictentry.Key;

                if (key.StartsWith(keyPrefix + ".", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    string keyproperty = String.Empty;
                    if (key.Length > (keyPrefix.Length + 1))
                    {
                        int pos = key.IndexOf('.');
                        if ((pos > 0) && (pos == keyPrefix.Length))
                        {
                            keyproperty = key.Substring(pos + 1);
                            if (String.IsNullOrEmpty(keyproperty) == false)
                            {
                                //Debug.WriteLine("Adding Implicit Key: " + keyPrefix + " - " + keyproperty);
                                ImplicitResourceKey implicitkey = new ImplicitResourceKey(String.Empty, keyPrefix, keyproperty);
                                keys.Add(implicitkey);
                            }
                        }
                    }
                }
            }
            return keys;
        }


        /// <summary>
        /// Returns an Implicit key value from the ResourceSet.
        /// Note this method is called only if a ResourceKey was found in the
        /// ResourceSet at load time. If a resource cannot be located this
        /// method is never called to retrieve it. IOW, GetImplicitResourceKeys
        /// determines which keys are actually retrievable.
        ///
        /// This method simply parses the Implicit key and then retrieves
        /// the value using standard GetObject logic for the ResourceID.
        /// </summary>
        /// <param name="implicitKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object GetObject(ImplicitResourceKey implicitKey, CultureInfo culture)
        {
            string ResourceKey = ConstructFullKey(implicitKey);

            string CultureName = null;
            if (culture != null)
                CultureName = culture.Name;
            else
                CultureName = CultureInfo.CurrentUICulture.Name;

            return this.GetObjectInternal(ResourceKey, CultureName);
        }


        /// <summary>
        /// Routine that generates a full resource key string from
        /// an Implicit Resource Key value
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static string ConstructFullKey(ImplicitResourceKey entry)
        {
            string text = entry.KeyPrefix + "." + entry.Property;
            if (entry.Filter.Length > 0)
            {
                text = entry.Filter + ":" + text;
            }
            return text;
        }
        #endregion
    }
}
