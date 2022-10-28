// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;

namespace YAXLib.KnownTypes;

internal static class XNamespaceExtensions
{
    internal static XName GetXName(this XNamespace ns, string name)
    {
        return ns.IsEmpty()
            ? XName.Get(name, ns.NamespaceName)
            : XName.Get(name);
    }
}