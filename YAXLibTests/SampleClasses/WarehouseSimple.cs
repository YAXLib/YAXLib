// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example is our basic hypothetical warehouse")]
public class WarehouseSimple
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public double Area { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static WarehouseSimple GetSampleInstance()
    {
        var w = new WarehouseSimple {
            Name = "Foo Warehousing Ltd.",
            Address = "No. 10, Some Ave., Some City, Some Country",
            Area = 120000.50 // square meters
        };

        return w;
    }
}