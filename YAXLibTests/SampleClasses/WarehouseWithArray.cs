﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib;

namespace YAXLibTests.SampleClasses
{
    public enum PossibleItems
    {
        Item1,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6,
        Item7,
        Item8,
        Item9,
        Item10,
        Item11,
        Item12
    }

    [ShowInDemoApplication]
    [YAXComment("This example shows the serialization of arrays")]
    public class WarehouseWithArray
    {
        [YAXAttributeForClass] public string Name { get; set; }

        [YAXSerializeAs("address")]
        [YAXAttributeFor("SiteInfo")]
        public string Address { get; set; }

        [YAXSerializeAs("SurfaceArea")]
        [YAXElementFor("SiteInfo")]
        public double Area { get; set; }

        [YAXCollection(YAXCollectionSerializationTypes.Serially, SeparateBy = ", ")]
        [YAXSerializeAs("StoreableItems")]
        public PossibleItems[] Items { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static WarehouseWithArray GetSampleInstance()
        {
            var w = new WarehouseWithArray
            {
                Name = "Foo Warehousing Ltd.",
                Address = "No. 10, Some Ave., Some City, Some Country",
                Area = 120000.50, // square meters
                Items = new[] {PossibleItems.Item3, PossibleItems.Item6, PossibleItems.Item9, PossibleItems.Item12}
            };

            return w;
        }
    }
}