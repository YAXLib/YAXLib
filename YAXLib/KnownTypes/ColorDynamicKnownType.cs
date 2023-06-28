// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

internal class ColorDynamicKnownType : DynamicKnownTypeBase
{
    /// <inheritdoc />
    public override string TypeName => "System.Drawing.Color";

    /// <inheritdoc />
    public override void Serialize(object? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        if (obj == null) throw new ArgumentException("Object must not be null", nameof(obj));

        var objectType = obj.GetType();
        if (objectType == null || objectType.FullName != TypeName)
            throw new ArgumentException("Object type does not match the provided typename", nameof(obj));

        var isKnownColor = ReflectionUtils.InvokeGetProperty<bool>(obj, "IsKnownColor");
        if (isKnownColor)
        {
            var colorName = ReflectionUtils.InvokeGetProperty<string>(obj, "Name");
            elem.Value = colorName!;
        }
        else
        {
            var a = ReflectionUtils.InvokeGetProperty<byte>(obj, "A");
            var r = ReflectionUtils.InvokeGetProperty<byte>(obj, "R");
            var g = ReflectionUtils.InvokeGetProperty<byte>(obj, "G");
            var b = ReflectionUtils.InvokeGetProperty<byte>(obj, "B");
            elem.Add(
                new XElement(overridingNamespace.GetXName("A"), a),
                new XElement(overridingNamespace.GetXName("R"), r),
                new XElement(overridingNamespace.GetXName("G"), g),
                new XElement(overridingNamespace.GetXName("B"), b));
        }
    }

    /// <inheritdoc />
    public override object? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        var elemR = elem.Element(overridingNamespace.GetXName("R"));
        if (elemR == null)
        {
            var colorName = elem.Value;
            var colorByName = ReflectionUtils.InvokeStaticMethod(Type, "FromName", colorName);
            return colorByName;
        }

        int a = 255, g = 0, b = 0;

        var elemA = elem.Element(overridingNamespace.GetXName("A"));
        if (elemA != null && !int.TryParse(elemA.Value, out a))
            a = 0;

        if (!int.TryParse(elemR.Value, out var r))
            r = 0;

        var elemG = elem.Element(overridingNamespace.GetXName("G"));
        if (elemG != null && !int.TryParse(elemG.Value, out g))
            g = 0;

        var elemB = elem.Element(overridingNamespace.GetXName("B"));
        if (elemB != null && !int.TryParse(elemB.Value, out b))
            b = 0;

        var result = ReflectionUtils.InvokeStaticMethod(Type, "FromArgb", a, r, g, b);
        return result;
    }
}
