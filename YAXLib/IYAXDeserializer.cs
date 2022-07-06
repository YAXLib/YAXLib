// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.IO;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib
{
    /// <summary>
    /// deserialize <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IYAXDeserializer<out T>
    {
        /// <summary>
        /// Gets the <see cref="SerializerOptions" /> settings
        /// to influence the process of serialization or de-serialization of <see cref="YAXSerializer" />s.
        /// </summary>
        SerializerOptions Options { get; }

        /// <summary>
        /// Deserializes the specified string containing the XML serialization and returns an object.
        /// </summary>
        /// <param name="input">The input string containing the XML serialization.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize(string input);

        /// <summary>
        /// Deserializes an object while reading input from an instance of <c>XmlReader</c>.
        /// </summary>
        /// <param name="xmlReader">The <c>XmlReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize(XmlReader xmlReader);

        /// <summary>
        /// Deserializes an object while reading input from an instance of <c>TextReader</c>.
        /// </summary>
        /// <param name="textReader">The <c>TextReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize(TextReader textReader);

        /// <summary>
        /// Deserializes an object while reading from an instance of <c>XElement</c>
        /// </summary>
        /// <param name="element">The <c>XElement</c> instance to read from.</param>
        /// <returns>The deserialized object</returns>
        T Deserialize(XElement element);

        /// <summary>
        /// Deserializes an object from the specified file which contains the XML serialization of the object.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>The deserialized object.</returns>
        T DeserializeFromFile(string fileName);
    }
}