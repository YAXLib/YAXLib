// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Xml.Linq;

namespace YAXLib
{
    internal class DataTableDynamicKnownType : DynamicKnownType
    {
        public override bool CanSerialize => true;
        public override bool CanDeserialize => true;
        public override string TypeName => "System.Data.DataTable";

        public override void Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            using (var xw = elem.CreateWriter())
            {
                var dsType = ReflectionUtils.GetTypeByName("System.Data.DataSet");
                var ds = Activator.CreateInstance(dsType);
                var dsTables = ReflectionUtils.InvokeGetProperty<object>(ds, "Tables");
                var dtCopy = ReflectionUtils.InvokeMethod(obj, "Copy");
                ReflectionUtils.InvokeMethod(dsTables, "Add", dtCopy);
                ReflectionUtils.InvokeMethod(ds, "WriteXml", xw);
            }
        }

        public override object Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            var dsElem = elem.Elements().FirstOrDefault(x => x.Name.LocalName == "NewDataSet");
            if (dsElem == null)
                return null;

            using (var xr = dsElem.CreateReader())
            {
                var dsType = ReflectionUtils.GetTypeByName("System.Data.DataSet");
                var ds = Activator.CreateInstance(dsType);
                ReflectionUtils.InvokeMethod(ds, "ReadXml", xr);
                var dsTables = ReflectionUtils.InvokeGetProperty<object>(ds, "Tables");
                var dsTablesCount = ReflectionUtils.InvokeGetProperty<int>(dsTables, "Count");
                if (dsTablesCount > 0)
                {
                    var dsTablesZero = ReflectionUtils.InvokeIntIndexer<object>(dsTables, "Index", 0);
                    var copyDt = ReflectionUtils.InvokeMethod(dsTablesZero, "Copy");
                    return copyDt;
                }

                return null;
            }
        }
    }
}