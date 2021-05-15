// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib
{
    /// <summary>
    ///     Interface for predefined serializers and deserializers for some known dot-net types.
    /// </summary>
    /// <typeparam name="T">The underlying known type</typeparam>
    internal abstract class KnownType<T> : IKnownType
    {
        /// <summary>
        ///     Gets the underlying known type.
        /// </summary>
        public Type Type => typeof(T);

        /// <summary>
        ///     Returns <see langword="true"/>, if <see cref="IKnownType.Serialize"/> is implemented.
        /// </summary>
        public abstract bool CanSerialize { get; }

        /// <summary>
        ///     Returns <see langword="true"/>, if <see cref="IKnownType.Deserialize"/> is implemented.
        /// </summary>
        public abstract bool CanDeserialize { get; }

        public SerializerOptions Options { get; set; }

        void IKnownType.Serialize(object obj, XElement elem, XNamespace overridingNamespace)
        {
            Serialize((T) obj, elem, overridingNamespace);
        }
        
        object IKnownType.Deserialize(XElement baseElement, XNamespace overridingNamespace)
        {
            return Deserialize(baseElement, overridingNamespace);
        }

        /// <summary>
        ///     Serializes the specified object int the specified XML element.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="elem">The XML element.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        public abstract void Serialize(T obj, XElement elem, XNamespace overridingNamespace);

        /// <summary>
        ///     Deserializes the specified XML element to the known type.
        /// </summary>
        /// <param name="elem">The XML element to deserialize object from.</param>
        /// <param name="overridingNamespace">The namespace the element belongs to.</param>
        /// <returns>The deserialized object</returns>
        public abstract T Deserialize(XElement elem, XNamespace overridingNamespace);
    }
}