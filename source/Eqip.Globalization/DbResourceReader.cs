using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Collections;

namespace Eqip.Globalization
{
    public class DbResourceReader : IResourceReader
    {
        private IDictionary _resources;

        public DbResourceReader(IDictionary resources)
        {
            _resources = resources;
        }
        IDictionaryEnumerator IResourceReader.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }
        void IResourceReader.Close()
        {
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }
        void IDisposable.Dispose()
        {
        }
    }
}
