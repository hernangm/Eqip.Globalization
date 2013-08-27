using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eqip.Globalization
{
    public interface IResource
    {
        string ResourceSet { get; }
        string ResourceKey { get;  }
        string ResourceValue { get;}
        string ResourceType { get;  }
        System.Data.Linq.Binary BinFile { get;  }
        string TextFile { get;  }
    }
}
