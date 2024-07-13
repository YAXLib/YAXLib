// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text;

namespace YAXLibTests.SampleClasses;

public class IEnumerableHolderClass
{
    public IEnumerableHolderClass()
    {
        ListOfStrings = new List<Item>();
    }

    public IEnumerable<Item> ListOfStrings { get; set; }

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

    public class Item
    {
        public string Name { get; set; } = string.Empty;

        public static implicit operator Item(string name) { return new Item { Name = name }; }

        public static implicit operator string(Item item) { return item.Name; }
    }
}

