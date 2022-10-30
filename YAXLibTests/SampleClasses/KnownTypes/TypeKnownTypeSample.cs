// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLibTests.SampleClasses.KnownTypes;

public class TypeKnownTypeSample
{
    public Type? TheType { get; set; }

    public static TypeKnownTypeSample GetSampleInstance()
    {
        return new TypeKnownTypeSample { TheType = typeof(KnownTypeTests) };
    }
}