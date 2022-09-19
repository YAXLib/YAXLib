// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.CustomSerialization;

[YAXCustomSerializer(typeof(ClassLevelCtxSerializer))]
public class ClassLevelCtxSample
{
    public string? MessageBody { get; set; }

    public string? Title { get; set; }
}

public class ClassLevelCtxSampleAsElement
{
    public ClassLevelCtxSample ClassLevelCtxSample { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public class ClassLevelCtxSampleAsAttribute
{
    [YAXAttributeForClass] public ClassLevelCtxSample ClassLevelCtxSample { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public class ClassLevelCtxSampleAsValue
{
    [YAXValueForClass] public ClassLevelCtxSample ClassLevelCtxSample { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}