﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib;

namespace YAXLibTests.SampleClasses.Namespace
{
    public class CellPhone_YAXNamespaceOverridesImplicitNamespace
    {
        [YAXNamespace("http://namespace.org/explicitBrand")]
        [YAXSerializeAs("{http://namespace.org/implicitBrand}Brand")]
        public string DeviceBrand { get; set; }

        [YAXSerializeAs("{http://namespace.org/os}OperatingSystem")]
        public string OS { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone_YAXNamespaceOverridesImplicitNamespace GetSampleInstance()
        {
            return new CellPhone_YAXNamespaceOverridesImplicitNamespace
            {
                DeviceBrand = "Samsung Galaxy S II",
                OS = "Android 2"
            };
        }
    }
}