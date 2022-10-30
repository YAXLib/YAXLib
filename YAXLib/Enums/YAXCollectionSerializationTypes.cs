// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.


namespace YAXLib.Enums;

/// <summary>
/// Enumerates the possible ways of serializing collection classes
/// </summary>
public enum YAXCollectionSerializationTypes
{
    /// <summary>
    /// Serializes each member of the collection, as a separate element, all enclosed in an element regarding the
    /// collection itself
    /// </summary>
    Recursive,

    /// <summary>
    /// Serializes each member of the collection, as a separate element, with no enclosing element for the collection
    /// </summary>
    RecursiveWithNoContainingElement,

    /// <summary>
    /// Serializes all members of the collection in one element separated by some delimiter, if possible.
    /// </summary>
    Serially
}