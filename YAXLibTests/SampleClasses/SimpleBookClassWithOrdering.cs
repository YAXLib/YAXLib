// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication(SortKey = "003")]
[YAXComment("This example demonstrates serializing a very simple class, but with partial priority ordering.")]
public class BookClassWithOrdering
{
    private string? _author;
    private string? _editor;
    private double _price;
    private string? _publisher;
    private int _publishYear;

    private string? _review;
    private string? _title;
    private int _currentElement;

    [YAXDontSerialize] public Dictionary<int, string> DecentralizationOrder = new Dictionary<int, string>();

    [YAXElementOrder(1)]
    public string? Title
    {
        get { return _title; }
        set
        {
            _title = value;
            DecentralizationOrder.Add(_currentElement++, "Title");
        }
    }

    [YAXElementOrder(0)]
    public string? Author
    {
        get { return _author; }
        set
        {
            _author = value;
            DecentralizationOrder.Add(_currentElement++, "Author");
        }
    }

    public int PublishYear
    {
        get { return _publishYear; }
        set
        {
            _publishYear = value;
            DecentralizationOrder.Add(_currentElement++, "PublishYear");
        }
    }

    public double Price
    {
        get { return _price; }
        set
        {
            _price = value;
            DecentralizationOrder.Add(_currentElement++, "Price");
        }
    }

    public string? Review
    {
        get { return _review; }
        set
        {
            _review = value;
            DecentralizationOrder.Add(_currentElement++, "Review");
        }
    }

    public string? Publisher
    {
        get { return _publisher; }
        set
        {
            _publisher = value;
            DecentralizationOrder.Add(_currentElement++, "Publisher");
        }
    }

    public string? Editor
    {
        get { return _editor; }
        set
        {
            _editor = value;
            DecentralizationOrder.Add(_currentElement++, "Editor");
        }
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static BookClassWithOrdering GetSampleInstance()
    {
        return new BookClassWithOrdering {
            Title = "Reinforcement Learning an Introduction",
            Author = "R. S. Sutton & A. G. Barto",
            PublishYear = 1998,
            Price = 38.75,
            Publisher = "MIT Press",
            Review = "This book is very good at being a book.",
            Editor = "MIT Productions"
        };
    }
}