// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;

namespace YAXLib
{
    internal static class KnownTypeExtensions
    {
        internal static XName GetXName(this IKnownType self, string name, XNamespace overridingNamespace)
        {
            if (overridingNamespace.IsEmpty())
                return XName.Get(name, overridingNamespace.NamespaceName);
            return XName.Get(name);
        }
    }
}