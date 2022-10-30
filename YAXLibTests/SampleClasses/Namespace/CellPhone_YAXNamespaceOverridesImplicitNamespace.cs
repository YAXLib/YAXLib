// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.Namespace;

public class CellPhoneYaxNamespaceOverridesImplicitNamespace
{
    [YAXNamespace("http://namespace.org/explicitBrand")]
    [YAXSerializeAs("{http://namespace.org/implicitBrand}Brand")]
    public string? DeviceBrand { get; set; }

    [YAXSerializeAs("{http://namespace.org/os}OperatingSystem")]
    public string? Os { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CellPhoneYaxNamespaceOverridesImplicitNamespace GetSampleInstance()
    {
        return new CellPhoneYaxNamespaceOverridesImplicitNamespace {
            DeviceBrand = "Samsung Galaxy S II",
            Os = "Android 2"
        };
    }
}