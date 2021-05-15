// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Linq;
using System.Xml.Linq;

namespace YAXLib
{
    internal class XElementKnownType : KnownType<XElement>
    {
        public override bool CanSerialize => true;
        public override bool CanDeserialize => true;

        public override void Serialize(XElement obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null) elem.Add(obj);
        }

        public override XElement Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return elem.Elements().FirstOrDefault();
        }
    }
}