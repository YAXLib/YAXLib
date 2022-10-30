// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.Namespace;

[YAXNamespace("http://namespace.org/nsmain")]
public class CellPhoneMemberAndClassDifferentNamespaces
{
    [YAXSerializeAs("TheName")]
    [YAXNamespace("x1", "http://namespace.org/x1")]
    public string? DeviceBrand { get; set; }

    public string? Os { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CellPhoneMemberAndClassDifferentNamespaces GetSampleInstance()
    {
        return new CellPhoneMemberAndClassDifferentNamespaces {
            DeviceBrand = "HTC",
            Os = "Windows Phone 8"
        };
    }
}