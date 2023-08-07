// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

public class Author
{
    [YAXSerializeAs("Author's Name")]
    [YAXAttributeFor("..")]
    public string? Name { get; set; }

    [YAXSerializeAs("Author's Age")]
    [YAXElementFor("../Something/Or/Another")]
    public int Age { get; set; }

    #region Equality members

    protected bool Equals(Author other)
    {
        return string.Equals(Name, other.Name) && Age == other.Age;
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
        return Equals((Author) obj);
    }

    /// <summary>Serves as a hash function for a particular type. </summary>
    /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
        }
    }

    #endregion
}

[ShowInDemoApplication]
[YAXComment("""
    This class shows how members of nested objects 
    can be serialized in their parents using serialization 
    addresses including ".."
    """)]
public class MoreComplexBook2
{
    public string? Title { get; set; }

    public Author? Author { get; set; }

    public int PublishYear { get; set; }
    public double Price { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static MoreComplexBook2 GetSampleInstance()
    {
        var auth = new Author { Age = 30, Name = "Tom Archer" };
        return new MoreComplexBook2 {
            Title = "Inside C#",
            Author = auth,
            PublishYear = 2002,
            Price = 30.5
        };
    }
}
