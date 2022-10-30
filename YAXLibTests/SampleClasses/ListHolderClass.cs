// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text;

namespace YAXLibTests.SampleClasses;

public class ListHolderClass
{
    public ListHolderClass()
    {
        ListOfStrings = new List<string>();
    }

    public List<string> ListOfStrings { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var item in ListOfStrings) sb.AppendLine(item);

        return sb.ToString();
    }

    public static ListHolderClass GetSampleInstance()
    {
        var inst = new ListHolderClass();

        inst.ListOfStrings.Add("Hi");
        inst.ListOfStrings.Add("Hello");

        return inst;
    }
}