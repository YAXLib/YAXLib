// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Linq;
using System.Xml.Linq;

namespace YAXLib
{
    internal class XAttributeKnownType : KnownType<XAttribute>
    {
        public override void Serialize(XAttribute obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null) elem.Add(obj);
        }

        public override XAttribute Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return elem.Attributes().FirstOrDefault();
        }
    }
}