// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Linq;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[YAXComment("This class contains fields that are vulnerable to culture changes!")]
public class CultureSample
{
    public double Number1 { get; set; }

    [YAXAttributeForClass] public double Number2 { get; set; }

    public double Number3 { get; set; }

    public double[]? Numbers { get; set; }

    public decimal Dec1 { get; set; }

    [YAXAttributeForClass] public decimal Dec2 { get; set; }

    public DateTime Date1 { get; set; }

    [YAXAttributeForClass] public DateTime Date2 { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CultureSample) obj);
    }

    protected bool Equals(CultureSample? other)
    {
        if (Numbers!.Where((t, i) => !t.Equals(other?.Numbers?[i])).Any()) return false;
        return Number1.Equals(other?.Number1) && Number2.Equals(other.Number2) && Number3.Equals(other.Number3) &&
               Dec1 == other.Dec1 && Dec2 == other.Dec2 && Date1.Equals(other.Date1) && Date2.Equals(other.Date2);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Number1.GetHashCode();
            hashCode = (hashCode * 397) ^ Number2.GetHashCode();
            hashCode = (hashCode * 397) ^ Number3.GetHashCode();
            hashCode = (hashCode * 397) ^ (Numbers?.GetHashCode() ?? 0);
            hashCode = (hashCode * 397) ^ Dec1.GetHashCode();
            hashCode = (hashCode * 397) ^ Dec2.GetHashCode();
            hashCode = (hashCode * 397) ^ Date1.GetHashCode();
            hashCode = (hashCode * 397) ^ Date2.GetHashCode();
            return hashCode;
        }
    }

    public static CultureSample GetSampleInstance()
    {
        return new CultureSample {
            Date1 = new DateTime(2010, 10, 11, 18, 20, 30),
            Date2 = new DateTime(2011, 9, 20, 4, 10, 30),
            Dec1 = 192389183919123.18232131m,
            Dec2 = 19232389.18391912318232131m,
            Number1 = 123123.1233f,
            Number2 = 32243.67676f,
            Number3 = 21313.123123f,
            Numbers = new[] { 23213.2132d, 123.213d, 123.23e32d }
        };
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}