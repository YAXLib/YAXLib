// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLibTests.SampleClasses;

public class NamesExample
{
    public string? FirstName { get; set; }

    public PersonInfo[]? Persons { get; set; }

    public static NamesExample GetSampleInstance()
    {
        var info1 = new PersonInfo { FirstName = "Li" };
        var info2 = new PersonInfo { FirstName = "Hu", LastName = "Hu" };
        var w = new NamesExample {
            FirstName = "Li",
            Persons = new[] { info1, info2 }
        };

        return w;
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

public class PersonInfo
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}