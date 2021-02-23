﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib;

namespace YAXLibTests.SampleClasses.Namespace
{
    [YAXNamespace("http://namespace.org/nsmain")]
    public class CellPhone_MemberAndClassDifferentNamespaces
    {
        [YAXSerializeAs("TheName")]
        [YAXNamespace("x1", "http://namespace.org/x1")]
        public string DeviceBrand { get; set; }

        public string OS { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone_MemberAndClassDifferentNamespaces GetSampleInstance()
        {
            return new CellPhone_MemberAndClassDifferentNamespaces
            {
                DeviceBrand = "HTC",
                OS = "Windows Phone 8"
            };
        }
    }
}