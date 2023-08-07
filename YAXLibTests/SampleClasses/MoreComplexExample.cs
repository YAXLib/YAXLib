// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("""
   This example tries to show almost all features of YAXLib which were not shown before.
   FamousPoints - shows a dictionary with a non-primitive value member.
   IntEnumerable - shows serializing properties of type IEnumerable<>
   Students - shows the usage of YAXNotCollection attribute
   """)]
public class MoreComplexExample
{
    private List<int> _lst = new List<int>();

    [YAXDictionary(EachPairName = "PointInfo", KeyName = "PName",
        ValueName = "ThePoint", SerializeKeyAs = YAXNodeTypes.Attribute,
        SerializeValueAs = YAXNodeTypes.Attribute)]
    public Dictionary<string, MyPoint> FamousPoints { get; set; } = new();

    public IEnumerable<int> IntEnumerable
    {
        get { return _lst; }

        set { _lst = value.ToList(); }
    }

    [YAXNotCollection] public Students Students { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static MoreComplexExample GetSampleInstance()
    {
        var famPoints = new Dictionary<string, MyPoint>();
        famPoints.Add("Center", new MyPoint { X = 0, Y = 0 });
        famPoints.Add("Q1", new MyPoint { X = 1, Y = 1 });
        famPoints.Add("Q2", new MyPoint { X = -1, Y = 1 });

        int[] nArray = { 1, 3, 5, 7 };

        return new MoreComplexExample {
            FamousPoints = famPoints,
            Students = Students.GetSampleInstance(),
            IntEnumerable = nArray
        };
    }
}

public class MyPoint
{
    [YAXAttributeForClass] public int X { get; set; }

    [YAXAttributeForClass] public int Y { get; set; }

    public override string ToString()
    {
        return string.Format("({0}, {1})", X, Y);
    }
}

public class Students : IEnumerable<string>
{
    public int Count { get; set; }

    public string[]? Names { get; set; }

    public string[]? Families { get; set; }

    #region IEnumerable<string> Members

    public IEnumerator<string> GetEnumerator()
    {
        return new StudentsEnumerator(this);
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new StudentsEnumerator(this);
    }

    #endregion

    public string GetAt(int i)
    {
        return string.Format("{0}, {1}", Families?[i], Names?[i]);
    }

    public override string ToString()
    {
        if (Names == null || Families == null) return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine(string.Format("Count = {0}", Count));
        sb.AppendLine("Names: ");
        foreach (var str in Names)
            sb.Append(str + "  ");

        sb.AppendLine("Families: ");
        foreach (var str in Families)
            sb.Append(str + "  ");

        return sb.ToString();
    }

    public static Students GetSampleInstance()
    {
        return new Students {
            Count = 3,
            Names = new[] { "Ali", "Dave", "John" },
            Families = new[] { "Alavi", "Black", "Doe" }
        };
    }
}

public class StudentsEnumerator : IEnumerator<string>
{
    private readonly Students _students;
    private int _counter = -1;

    public StudentsEnumerator(Students studentsInstance)
    {
        _students = studentsInstance;
        _counter = -1;
    }

    #region IEnumerator<string> Members

    public string Current => _students.GetAt(_counter);

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion

    #region IEnumerator Members

    object IEnumerator.Current => _students.GetAt(_counter);

    public bool MoveNext()
    {
        _counter++;
        if (_counter >= _students.Count)
            return false;
        return true;
    }

    public void Reset()
    {
        _counter = -1;
    }

    #endregion
}
