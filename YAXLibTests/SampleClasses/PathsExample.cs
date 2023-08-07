// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("""
    This example demonstrates how not to use 
    white spaces as separators while serializing 
    collection classes serially
    """)]
public class PathsExample
{
    [YAXCollection(YAXCollectionSerializationTypes.Serially, SeparateBy = ";", IsWhiteSpaceSeparator = false)]
    public List<string> Paths { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static PathsExample GetSampleInstance()
    {
        var paths = new List<string>();
        paths.Add("""C:\SomeFile.txt""");
        paths.Add("""C:\SomeFolder\SomeFile.txt""");
        paths.Add("""C:\Some Folder With Space Such As\Program Files""");

        return new PathsExample {
            Paths = paths
        };
    }
}
