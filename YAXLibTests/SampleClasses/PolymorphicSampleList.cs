// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[YAXSerializeAs("sample")]
public abstract class PolymorphicSample
{
}

public class PolymorphicOneSample : PolymorphicSample
{
}

public class PolymorphicTwoSample : PolymorphicSample
{
}

[YAXSerializeAs("samples")]
public class PolymorphicSampleList : List<PolymorphicSample>
{
    public static PolymorphicSampleList GetSampleInstance()
    {
        var samples = new PolymorphicSampleList {
            new PolymorphicOneSample(),
            new PolymorphicTwoSample()
        };
        return samples;
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public class PolymorphicSampleListAsMember
{
    public PolymorphicSampleList SampleList { get; set; } = new();

    public static PolymorphicSampleListAsMember GetSampleInstance()
    {
        return new PolymorphicSampleListAsMember {
            SampleList = PolymorphicSampleList.GetSampleInstance()
        };
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}