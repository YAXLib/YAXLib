// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[YAXSerializeAs("Pricing")]
public class Request
{
    [YAXAttributeForClass] public string? Id { get; set; }

    [YAXAttributeFor("version")] public string? Major { get; set; }

    [YAXAttributeFor("version")] public string? Minor { get; set; }

    [YAXSerializeAs("value_date")]
    [YAXElementFor("input")]
    public string? ValueDate { get; set; }

    [YAXSerializeAs("storage_date")]
    [YAXElementFor("input")]
    public string? StorageDate { get; set; }

    [YAXSerializeAs("user")]
    [YAXElementFor("input")]
    public string? User { get; set; }

    [YAXElementFor("input")]
    [YAXSerializeAs("skylab_config")]
    public SkyLabConfig? Config { get; set; }

    internal static Request GetSampleInstance()
    {
        return new Request {
            Id = "123",
            Major = "1",
            Minor = "0",
            ValueDate = "2010-10-5",
            StorageDate = "2010-10-5",
            User = "me",
            Config = new SkyLabConfig { Config = "someconf", Job = "test" }
        };
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public class SkyLabConfig
{
    [YAXSerializeAs("SomeString")] public string? Config { get; set; }

    [YAXSerializeAs("job")] public string? Job { get; set; }
}