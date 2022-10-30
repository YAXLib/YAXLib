// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Linq;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

internal class XAttributeKnownType : KnownTypeBase<XAttribute>
{
    /// <inheritdoc />
    public override void Serialize(XAttribute? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        if (obj != null) elem.Add(obj);
    }

    /// <inheritdoc />
    public override XAttribute? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        return elem.Attributes().FirstOrDefault();
    }
}