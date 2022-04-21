// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
    internal class TypeKnownType : KnownType<Type>
    {
        public override void Serialize(Type obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null)
                elem.Value = obj.FullName ?? string.Empty;
        }

        public override Type Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            return ReflectionUtils.GetTypeByName(elem.Value);
        }
    }
}