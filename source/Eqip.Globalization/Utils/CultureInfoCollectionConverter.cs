using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Specialized;

namespace Eqip.Globalization.Utils
{
    public class CultureInfoCollectionConverter : ConfigurationConverterBase
    {
        //public override bool CanConvertFrom(ITypeDescriptorContext context, Type source)
        //{
        //    if (source.Equals(typeof(string)))
        //        return true;

        //    return base.CanConvertFrom(context, source);

        //}

        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //{
        //    if (destinationType.Equals(typeof(string)))
        //        return true;

        //    return base.CanConvertTo(context, destinationType);

        //}

        public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
        {
            var list = new StringCollection();
            var l = (List<CultureInfo>)value;
            foreach (var item in l)
            {
                list.Add(((CultureInfo)item).Name);
            }
            return list;
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            var list = new List<CultureInfo>();
            foreach (var item in data.ToString().Split(','))
            {
                list.Add(new CultureInfo(item));
            }
            return list;
        }

    }

}
