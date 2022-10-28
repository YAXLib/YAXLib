// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.Namespace;

[YAXComment("This example shows usage of a custom default namespace")]
[YAXNamespace("http://namespaces.org/default")]
public class SingleNamespaceSample
{
    public string? StringItem { get; set; }

    public int IntItem { get; set; }

    public static SingleNamespaceSample GetInstance()
    {
        return new SingleNamespaceSample {
            StringItem = "This is a test string",
            IntItem = 10
        };
    }
}