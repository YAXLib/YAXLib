// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
public class YaxLibMetadataOverriding
{
    public int[,]? IntArray { get; set; }

    public object? Obj { get; set; }


    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static YaxLibMetadataOverriding GetSampleInstance()
    {
        var instance = new YaxLibMetadataOverriding();
        instance.SetSampleData();
        return instance;
    }

    protected void SetSampleData()
    {
        IntArray = new int[2, 3];

        for (var i = 0; i < 2; i++)
        for (var j = 0; j < 3; j++)
            IntArray[i, j] = i + j + 1;

        Obj = "Hello, World!";
    }
}

[ShowInDemoApplication]
[YAXNamespace("http://namespace.org/sample")]
public class YaxLibMetadataOverridingWithNamespace : YaxLibMetadataOverriding
{
    public new static YaxLibMetadataOverridingWithNamespace GetSampleInstance()
    {
        var instance = new YaxLibMetadataOverridingWithNamespace();
        instance.SetSampleData();
        return instance;
    }
}