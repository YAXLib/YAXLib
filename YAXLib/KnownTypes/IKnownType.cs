// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib
{
    internal interface IKnownType
    {
        /// <summary>
        ///     Gets the underlying known type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Returns <see langword="true"/>, if <see cref="IKnownType.Serialize"/> is implemented.
        /// </summary>
        bool CanSerialize { get; }

        /// <summary>
        ///     Returns <see langword="true"/>, if <see cref="IKnownType.Deserialize"/> is implemented.
        /// </summary>
        bool CanDeserialize { get; }

        /// <summary>
        ///    Gets or sets the <see cref="SerializerOptions"/>.
        /// </summary>
        SerializerOptions Options { get; set; }

        /// <summary>
        ///     Serializes the specified object int the specified XML element.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="elem">The XML element.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        void Serialize(object obj, XElement elem, XNamespace overridingNamespace);

        /// <summary>
        ///     Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        /// <returns>The deserialized object</returns>
        object Deserialize(XElement elem, XNamespace overridingNamespace);
    }
}