// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses.Namespace
{
    [YAXSerializeAs("MobilePhone")]
    public class CellPhone_CollectionNamespaceGoesThruRecursiveNoContainingElement
    {
        public string DeviceBrand { get; set; }
        public string OS { get; set; }

        [YAXNamespace("app", "http://namespace.org/apps")]
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
        public List<string> IntalledApps { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone_CollectionNamespaceGoesThruRecursiveNoContainingElement GetSampleInstance()
        {
            return new CellPhone_CollectionNamespaceGoesThruRecursiveNoContainingElement
            {
                DeviceBrand = "Samsung Galaxy Nexus",
                OS = "Android",
                IntalledApps = new List<string> {"Google Map", "Google+", "Google Play"}
            };
        }
    }
}