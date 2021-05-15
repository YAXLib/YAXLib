// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Xml.Linq;

namespace YAXLib
{
    internal class DataSetDynamicKnownType : DynamicKnownType
    {
        public override bool CanSerialize => true;
        public override bool CanDeserialize => true;
        public override string TypeName => "System.Data.DataSet";

        public override void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            using (var xw = elem.CreateWriter())
            {
                ReflectionUtils.InvokeMethod(obj, "WriteXml", xw);
            }
        }

        public override object Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var child = elem.Elements().FirstOrDefault();
            if (child == null)
                return null;
            using (var xr = child.CreateReader())
            {
                var dsType = ReflectionUtils.GetTypeByName("System.Data.DataSet");
                var ds = Activator.CreateInstance(dsType);
                ReflectionUtils.InvokeMethod(ds, "ReadXml", xr);
                return ds;
            }
        }
    }
}