// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace DemoApplication;

public class ClassInfoListItem
{
    public Type ClassType { get; set; }
    public object SampleObject { get; set; }

    public ClassInfoListItem(Type classType, object sampleObject)
    {
        ClassType = classType;
        SampleObject = sampleObject;
    }

    public override string ToString()
    {
        return ClassType.Name;
    }
}
