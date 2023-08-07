// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("""
    This example shows a multi-level class, which helps to test 
    the null references identity problem. 
    Thanks go to Anton Levshunov for proposing this example,
    and a discussion on this matter.
    """)]
public class MultilevelClass
{
    public List<FirstLevelClass> Items { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static MultilevelClass GetSampleInstance()
    {
        var obj = new MultilevelClass {
            Items = new List<FirstLevelClass> {
                new FirstLevelClass(),
                new FirstLevelClass()
            }
        };

        obj.Items[0].Second = new SecondLevelClass();
        obj.Items[0].Id = "1";
        obj.Items[0].Second!.SecondId = "1-2";

        obj.Items[1].Id = "2";
        obj.Items[1].Second = null;
        return obj;
    }
}

public class FirstLevelClass
{
    public string? Id { get; set; }

    public SecondLevelClass? Second { get; set; }
}

public class SecondLevelClass
{
    public string? SecondId { get; set; }
}
