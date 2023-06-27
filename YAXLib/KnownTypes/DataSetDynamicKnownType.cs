// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

internal class DataSetDynamicKnownType : DynamicKnownTypeBase
{
    /// <inheritdoc />
    public override string TypeName => "System.Data.DataSet";

    /// <inheritdoc />
    public override void Serialize(object? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        if (obj == null) throw new ArgumentException("Object must not be null", nameof(obj));

        using var xw = elem.CreateWriter();
        ReflectionUtils.InvokeMethod(obj, "WriteXml", xw);
    }

    /// <inheritdoc />
    public override object? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var child = elem.Elements().FirstOrDefault();
        if (child == null)
            return null;

        using var xr = child.CreateReader();
        var dsType = ReflectionUtils.GetTypeByName("System.Data.DataSet") ?? throw new InvalidOperationException($"Type for 'System.Data.DataSet' not found");
        var ds = Activator.CreateInstance(dsType) ?? throw new InvalidOperationException($"Can't create instance of type '{dsType.Name}'");
        ReflectionUtils.InvokeMethod(ds, "ReadXml", xr);
        return ds;
    }
}
