// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("""
    This class provides an example of successful serialization/deserialization 
    of collection objects in "System.Collections.Generic" namespaces
    """)]
public class GenericCollectionsSample
{
    public Stack<int> TheStack { get; set; } = new();

    [YAXDictionary(EachPairName = "Item", SerializeKeyAs = YAXNodeTypes.Attribute,
        SerializeValueAs = YAXNodeTypes.Attribute)]
    public SortedList<double, string> TheSortedList { get; set; } = new();

    [YAXDictionary(EachPairName = "Item", SerializeKeyAs = YAXNodeTypes.Attribute,
        SerializeValueAs = YAXNodeTypes.Attribute)]
    public SortedDictionary<int, double> TheSortedDictionary { get; set; } = new();

    public Queue<string> TheQueue { get; set; } = new();
    public HashSet<int> TheHashSet { get; set; } = new();
    public LinkedList<double> TheLinkedList { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static GenericCollectionsSample GetSampleInstance()
    {
        var stack = new Stack<int>();
        stack.Push(7);
        stack.Push(1);
        stack.Push(79);

        var sortedList = new SortedList<double, string>();
        sortedList.Add(1.0, "Hi");
        sortedList.Add(0.5, "Hello");
        sortedList.Add(5.0, "How are you?");

        var sortedDic = new SortedDictionary<int, double>();
        sortedDic.Add(5, 2.0);
        sortedDic.Add(10, 1.0);
        sortedDic.Add(1, 30.0);

        var q = new Queue<string>();
        q.Enqueue("Hi");
        q.Enqueue("Hello");
        q.Enqueue("How are you?");

        var hashSet = new HashSet<int>();
        hashSet.Add(1);
        hashSet.Add(2);
        hashSet.Add(4);
        hashSet.Add(6);

        var lnkList = new LinkedList<double>();
        lnkList.AddLast(1.0);
        lnkList.AddLast(5.0);
        lnkList.AddLast(61.0);

        return new GenericCollectionsSample {
            TheStack = stack,
            TheSortedList = sortedList,
            TheSortedDictionary = sortedDic,
            TheQueue = q,
            TheHashSet = hashSet,
            TheLinkedList = lnkList
        };
    }
}
