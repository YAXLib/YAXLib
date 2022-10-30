// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication(SortKey = "001")]
[YAXComment("This example demonstrates serializing a very simple class")]
public class Book : IEquatable<Book>
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int PublishYear { get; set; }
    public double Price { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static Book GetSampleInstance()
    {
        return new Book {
            Title = "Inside C#",
            Author = "Tom Archer & Andrew Whitechapel",
            PublishYear = 2002,
            Price = 30.5
        };
    }

    public bool Equals(Book? other)
    {
        if (other == null)
            return false;

        if (Title == other.Title && Price == other.Price && Author == other.Author &&
            PublishYear == other.PublishYear)
            return true;
        else
            return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        var personObj = obj as Book;
        return personObj != null && Equals(personObj);
    }

    public override int GetHashCode()
    {
        return (Author, Price, PublishYear, Title).GetHashCode();
    }

    public static bool operator ==(Book? book1, Book? book2)
    {
        if (((object?) book1) == null || ((object?) book2) == null)
            return Equals(book1, book2);

        return book1.Equals(book2);
    }

    public static bool operator !=(Book? book1, Book? book2)
    {
        if (((object?) book1) == null || ((object?) book2) == null)
            return !Equals(book1, book2);

        return !(book1.Equals(book2));
    }
}