// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using System.Linq;
using System.Xml.Linq;

namespace YAXLib.KnownTypes
{
    internal class XElementKnownType : KnownTypeAbstract<XElement>
    {
        /// <inheritdoc />
        public override void Serialize(XElement? obj, XElement elem, XNamespace overridingNamespace, ISerializationContext serializationContext)
        {
            if (obj != null) elem.Add(obj);
        }

        /// <inheritdoc />
        public override XElement? Deserialize(XElement elem, XNamespace overridingNamespace, ISerializationContext serializationContext)
        {
            return elem.Elements().FirstOrDefault();
        }
    }
}
