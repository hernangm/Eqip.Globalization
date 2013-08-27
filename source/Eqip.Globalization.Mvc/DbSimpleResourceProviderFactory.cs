using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.Mvc;

namespace Eqip.Globalization.Mvc
{
    /// <summary>
    /// Provider factory that instantiates the individual provider. The provider
    /// passes a 'classname' which is the ResourceSet id or how a resource is identified.
    /// For global resources it's the name of hte resource file, for local resources
    /// it's the full Web relative virtual path
    /// </summary>
    public class DbSimpleResourceProviderFactory : ResourceProviderFactory
    {
        public override IResourceProvider CreateGlobalResourceProvider(string classname)
        {
            var dataManager = DependencyResolver.Current.GetService<IResourceDataManager>();
            return new DbSimpleResourceProvider(dataManager, null, classname);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            //// DEPENDENCY HERE: use Configuration class to strip off Virtual path leaving
            ////                      just a page/control relative path for ResourceSet Ids

            //// ASP.NET passes full virtual path: Strip out the virtual path 
            //// leaving us just with app relative page/control path
            //string ResourceSetName = WebUtils.GetAppRelativePath(virtualPath);

            //return new DbSimpleResourceProvider(null, ResourceSetName.ToLower());
            throw new NotImplementedException();
        }
    }
}


