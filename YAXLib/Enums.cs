﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Enumerates different kinds of exception handling policies as used by YAX Library.
    /// </summary>
    public enum YAXExceptionHandlingPolicies
    {
        /// <summary>
        ///     Throws Both Warnings And Errors
        /// </summary>
        ThrowWarningsAndErrors,

        /// <summary>
        ///     Throws Errors only (default)
        /// </summary>
        ThrowErrorsOnly,

        /// <summary>
        ///     Does not throw exceptions, the errors can be accessed via the YAXParsingErrors instance
        /// </summary>
        DoNotThrow
    }

    /// <summary>
    ///     Enumerates different possible behaviours of the library toward exceptions
    /// </summary>
    public enum YAXExceptionTypes
    {
        /// <summary>
        ///     Ignore non-fatal exceptions; neither throw them, nor log them.
        /// </summary>
        Ignore,

        /// <summary>
        ///     Treat exception as a warning
        /// </summary>
        Warning,

        /// <summary>
        ///     Treat exception as an error
        /// </summary>
        Error
    }

    /// <summary>
    ///     Enumerates different serialization options which could be set at construction time.
    /// </summary>
    [Flags]
    public enum YAXSerializationOptions
    {
        /// <summary>
        ///     Serializes null objects also (the default)
        /// </summary>
        SerializeNullObjects = 0,

        /// <summary>
        ///     Prevents serialization of null objects.
        /// </summary>
        DontSerializeNullObjects = 1,

        /// <summary>
        ///     TODO: update the comment
        ///     Prevents serialization of child objects that refer to a parent object which is already serialized, and hene causing
        ///     a cycle or infinite loop
        /// </summary>
        ThrowUponSerializingCyclingReferences = 2,

        /// <summary>
        ///     Prevents serailization of properties with no setter
        /// </summary>
        DontSerializePropertiesWithNoSetter = 4,

        /// <summary>
        ///     Never add YAXLib metadata attributes (e.g., 'yaxlib:realtype') to the serialized XML (even when they would be
        ///     required for deserialization.)
        ///     Useful when generating XML intended for another system's consumption.
        /// </summary>
        SuppressMetadataAttributes = 8,

        /// <summary>
        ///     Provides line number and position (where available) in deserialization exceptions.
        ///     Enabling this may have a performance impact
        /// </summary>
        DisplayLineInfoInExceptions = 16
    }

    /// <summary>
    ///     Enumerates the possible ways of serializing collection classes
    /// </summary>
    public enum YAXCollectionSerializationTypes
    {
        /// <summary>
        ///     Serializes each member of the collection, as a separate element, all enclosed in an element regarding the
        ///     collection itself
        /// </summary>
        Recursive,

        /// <summary>
        ///     Serializes each member of the collection, as a separate element, with no enclosing element for the collection
        /// </summary>
        RecursiveWithNoContainingElement,

        /// <summary>
        ///     Serializes all members of the collection in one element separated by some delimiter, if possible.
        /// </summary>
        Serially
    }

    /// <summary>
    ///     Enumerates possible XML node types upon which a property can be serialized.
    /// </summary>
    public enum YAXNodeTypes
    {
        /// <summary>
        ///     Serialize data as an attribute for the base element
        /// </summary>
        Attribute,

        /// <summary>
        ///     Serialize data as an element
        /// </summary>
        Element,

        /// <summary>
        ///     Serialize data as content of the element
        /// </summary>
        Content
    }

    /// <summary>
    ///     Enumerates possible options for a serializable type
    /// </summary>
    public enum YAXSerializationFields
    {
        /// <summary>
        ///     Serializes only the public properties (the default behaviour)
        /// </summary>
        PublicPropertiesOnly,

        /// <summary>
        ///     Serializes all fields (properties or member variables) which are public, or non-public.
        /// </summary>
        AllFields,

        /// <summary>
        ///     Serializes all fields (properties or member variables) which are public, or non-public, if attributed as
        ///     <c>YAXSerializableField</c>
        /// </summary>
        AttributedFieldsOnly
    }
}