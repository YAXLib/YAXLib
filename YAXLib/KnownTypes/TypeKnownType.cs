// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

internal class TypeKnownType : KnownTypeBase<Type>
{
    /// <inheritdoc />
    public override void Serialize(Type? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        if (obj != null)
            elem.Value = obj.FullName ?? string.Empty;
    }

    /// <inheritdoc />
    public override Type? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        return ReflectionUtils.GetTypeByName(elem.Value);
    }
}