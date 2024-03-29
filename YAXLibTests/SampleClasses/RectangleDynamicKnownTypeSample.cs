﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Drawing;

namespace YAXLibTests.SampleClasses;

public class RectangleDynamicKnownTypeSample
{
    public Rectangle Rect { get; set; }

    public static RectangleDynamicKnownTypeSample GetSampleInstance()
    {
        return new RectangleDynamicKnownTypeSample { Rect = new Rectangle(10, 20, 30, 40) };
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}