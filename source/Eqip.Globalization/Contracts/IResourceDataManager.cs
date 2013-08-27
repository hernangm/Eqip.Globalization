using System;

namespace Eqip.Globalization
{
    public interface IResourceDataManager
    {
        string GetResourcesAsJavascriptObject(string javaScriptVarName, string ResourceSet, string LocaleId);
        System.Collections.IDictionary GetResourceSet(string CultureName, string ResourceSet);
        System.Collections.Generic.Dictionary<string, object> GetResourceSetNormalizedForLocaleId(string CultureName, string ResourceSet);
        bool IsValidCulture(string IetfTag);
        void AddResource(string ResourceKey, string ResourceValue, string CultureName, string ResourceSet);
    }
}
