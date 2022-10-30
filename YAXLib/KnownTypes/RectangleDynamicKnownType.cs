// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;
using YAXLib.Exceptions;

namespace YAXLib.KnownTypes;

internal class RectangleDynamicKnownType : DynamicKnownTypeBase
{
    /// <inheritdoc />
    public override string TypeName => "System.Drawing.Rectangle";

    /// <inheritdoc />
    public override void Serialize(object? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var objectType = obj?.GetType();
        if (obj == null || objectType?.FullName != TypeName)
            throw new ArgumentException("Object type does not match the provided typename", nameof(obj));

        var left = ReflectionUtils.InvokeGetProperty<int>(obj, "Left");
        var top = ReflectionUtils.InvokeGetProperty<int>(obj, "Top");
        var width = ReflectionUtils.InvokeGetProperty<int>(obj, "Width");
        var height = ReflectionUtils.InvokeGetProperty<int>(obj, "Height");

        elem.Add(
            new XElement(overridingNamespace.GetXName("Left"), left),
            new XElement(overridingNamespace.GetXName("Top"), top),
            new XElement(overridingNamespace.GetXName("Width"), width),
            new XElement(overridingNamespace.GetXName("Height"), height));
    }

    /// <inheritdoc />
    public override object? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemLeft = elem.Element(overridingNamespace.GetXName("Left"));
        var elemTop = elem.Element(overridingNamespace.GetXName("Top"));
        var elemWidth = elem.Element(overridingNamespace.GetXName("Width"));
        var elemHeight = elem.Element(overridingNamespace.GetXName("Height"));

        if (elemHeight == null || elemWidth == null || elemTop == null || elemLeft == null)
            throw new YAXElementMissingException(elem.Name + ":[Left|Top|Width|Height]", elem);

        return Activator.CreateInstance(Type,
            int.Parse(elemLeft.Value),
            int.Parse(elemTop.Value),
            int.Parse(elemWidth.Value),
            int.Parse(elemHeight.Value));
    }
}