// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This sample demonstrates serialization of non-generic collection classes")]
public class NonGenericCollectionsSample
{
    public List<object>? ObjList { get; set; }

    public ArrayList? TheArrayList { get; set; }

    public Hashtable? TheHashtable { get; set; }

    public Queue? TheQueue { get; set; }

    public Stack? TheStack { get; set; }

    public SortedList? TheSortedList { get; set; }

    public BitArray? TheBitArray { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static NonGenericCollectionsSample GetSampleInstance()
    {
        var lst = new List<object> {
            1,
            3.0,
            "Hello",
            new DateTime(2010, 3, 4),
            new Author { Name = "Charles", Age = 50 }
        };

        var arLst = new ArrayList {
            2,
            8.5,
            "Hi",
            new Author { Name = "Steve", Age = 30 }
        };

        var table = new Hashtable {
            { 1.0, "Tim" },
            { "Tom", "Sam" },
            { new DateTime(2009, 2, 1), 7 }
        };

        var bitArray = new BitArray(10) {
            [1] = true,
            [6] = true
        };

        var queue = new Queue();
        queue.Enqueue(10);
        queue.Enqueue(20);
        queue.Enqueue(30);

        var stack = new Stack();
        stack.Push(100);
        stack.Push(200);
        stack.Push(300);


        var sortedList = new SortedList {
            { 1, 2 },
            { 5, 7 },
            { 8, 2 }
        };

        return new NonGenericCollectionsSample {
            ObjList = lst,
            TheArrayList = arLst,
            TheHashtable = table,
            TheBitArray = bitArray,
            TheQueue = queue,
            TheStack = stack,
            TheSortedList = sortedList
        };
    }

    #region Equality members

    protected bool Equals(NonGenericCollectionsSample? other)
    {
        return EqualsHelpers.CollectionEquals(ObjList, other?.ObjList)
               && EqualsHelpers.CollectionEquals(TheArrayList, other?.TheArrayList)
               && EqualsHelpers.DictionaryEquals(TheHashtable, other?.TheHashtable)
               && EqualsHelpers.CollectionEquals(TheQueue, other?.TheQueue)
               && EqualsHelpers.CollectionEquals(TheStack, other?.TheStack)
               && EqualsHelpers.DictionaryEquals(TheSortedList, other?.TheSortedList)
               && EqualsHelpers.CollectionEquals(TheBitArray, other?.TheBitArray);
    }


    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
    /// <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    /// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />;
    /// otherwise, false.
    /// </returns>
    /// <param name="obj">The object to compare with the current object. </param>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((NonGenericCollectionsSample) obj);
    }

    /// <summary>Serves as a hash function for a particular type. </summary>
    /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ObjList != null ? ObjList.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (TheArrayList != null ? TheArrayList.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TheHashtable != null ? TheHashtable.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TheQueue != null ? TheQueue.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TheStack != null ? TheStack.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TheSortedList != null ? TheSortedList.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TheBitArray != null ? TheBitArray.GetHashCode() : 0);
            return hashCode;
        }
    }

    #endregion
}