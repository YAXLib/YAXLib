﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    public class FieldLevelSample
    {
        public string Id;

        public string Title;

        [YAXCustomSerializer(typeof(FieldLevelSerializer))]
        public string Body;

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }
}
