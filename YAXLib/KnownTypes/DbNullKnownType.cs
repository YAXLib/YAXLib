// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

internal class DbNullKnownType : KnownTypeBase<DBNull>
{
    /// <inheritdoc />
    public override void Serialize(DBNull? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        if (obj != null)
            elem.Value = "DBNull";
    }

    /// <inheritdoc />
    public override DBNull? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        if (string.IsNullOrEmpty(elem.Value))
            return null;
        return DBNull.Value;
    }
}