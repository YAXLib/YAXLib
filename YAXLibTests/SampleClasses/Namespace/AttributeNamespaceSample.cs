// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.Namespace;

[YAXNamespace("http://namespaces.org/default")]
public class AttributeNamespaceSample
{
    [YAXAttributeFor("Attribs")] public string? Attrib { get; private set; }

    [YAXAttributeFor("Attribs")]
    [YAXNamespace("ns", "http://namespaces.org/ns")]
    public string? Attrib2 { get; private set; }

    public static AttributeNamespaceSample GetSampleInstance()
    {
        return new AttributeNamespaceSample {
            Attrib = "value",
            Attrib2 = "value2"
        };
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}