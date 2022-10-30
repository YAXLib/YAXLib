// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

public class AttributeInheritanceWithPropertyOverride : AttributeInheritance
{
    public override string Gender => "Female"; // should inherit the base's YAXSerializeAs attribute

    [YAXSerializeAs("CurrentAge")] // should override the base's YAXSerializeAs attribute
    public override double Age
    {
        get { return base.Age; }
        set { base.Age = value; }
    }

    public new static AttributeInheritanceWithPropertyOverride GetSampleInstance()
    {
        return new AttributeInheritanceWithPropertyOverride {
            Name = "Sally",
            Age = 38.7
        };
    }
}

[YAXSerializeAs("Child")]
public class AttributeInheritance : AttributeInheritanceBase
{
    [YAXSerializeAs("TheAge")] public virtual double Age { get; set; }

    public static AttributeInheritance GetSampleInstance()
    {
        return new AttributeInheritance {
            Name = "John",
            Age = 30.2
        };
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

[YAXSerializeAs("Base")]
public class AttributeInheritanceBase
{
    [YAXSerializeAs("TheName")] public string? Name { get; set; }

    [YAXSerializeAs("TheGender")] public virtual string Gender => "Unknown";
}