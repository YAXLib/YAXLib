// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example shows our hypothetical warehouse, a little bit structured")]
public class WarehouseStructured
{
    [YAXAttributeForClass] public string? Name { get; set; }

    [YAXSerializeAs("address")]
    [YAXAttributeFor("SiteInfo")]
    public string? Address { get; set; }

    [YAXSerializeAs("SurfaceArea")]
    [YAXElementFor("SiteInfo")]
    public double Area { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static WarehouseStructured GetSampleInstance()
    {
        var w = new WarehouseStructured {
            Name = "Foo Warehousing Ltd.",
            Address = "No. 10, Some Ave., Some City, Some Country",
            Area = 120000.50 // square meters
        };

        return w;
    }
}