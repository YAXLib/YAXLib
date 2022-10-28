// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLibTests;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ShowInDemoApplicationAttribute : Attribute
{
    public ShowInDemoApplicationAttribute()
    {
        SortKey = null;
        GetSampleInstanceMethodName = "GetSampleInstance";
    }

    public string? SortKey { get; set; }

    public string GetSampleInstanceMethodName { get; set; }
}