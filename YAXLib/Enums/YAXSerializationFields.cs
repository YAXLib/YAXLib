// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib.Enums
{
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