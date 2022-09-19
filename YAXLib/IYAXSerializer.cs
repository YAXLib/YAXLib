// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.IO;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib;

/// <summary>
/// Serialize or deserialize <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IYAXSerializer<T>
{
    /// <summary>
    /// Gets the <see cref="SerializerOptions" /> settings
    /// to influence the process of serialization or de-serialization of <see cref="YAXSerializer" />s.
    /// </summary>
    SerializerOptions Options { get; }

    /// <summary>
    /// Gets the parsing errors.
    /// </summary>
    /// <value>The parsing errors.</value>
    YAXParsingErrors ParsingErrors { get; }

    /// <summary>
    /// Serializes the specified object and returns a string containing the XML.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A <code>System.String</code> containing the XML</returns>
    string Serialize(T? obj);

    /// <summary>
    /// Serializes the specified object into a <c>TextWriter</c> instance.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
    void Serialize(T? obj, TextWriter textWriter);

    /// <summary>
    /// Serializes the specified object into a <c>XmlWriter</c> instance.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="xmlWriter">The <c>XmlWriter</c> instance.</param>
    void Serialize(T? obj, XmlWriter xmlWriter);

    /// <summary>
    /// Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
    XDocument SerializeToXDocument(T? obj);

    /// <summary>
    /// Serializes the specified object to file.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="fileName">Path to the file.</param>
    void SerializeToFile(T? obj, string fileName);

    /// <summary>
    /// Deserializes the specified string containing the XML serialization and returns an object.
    /// </summary>
    /// <param name="input">The input string containing the XML serialization.</param>
    /// <returns>The deserialized object.</returns>
    T? Deserialize(string input);

    /// <summary>
    /// Deserializes an object while reading input from an instance of <c>XmlReader</c>.
    /// </summary>
    /// <param name="xmlReader">The <c>XmlReader</c> instance to read input from.</param>
    /// <returns>The deserialized object.</returns>
    T? Deserialize(XmlReader xmlReader);

    /// <summary>
    /// Deserializes an object while reading input from an instance of <c>TextReader</c>.
    /// </summary>
    /// <param name="textReader">The <c>TextReader</c> instance to read input from.</param>
    /// <returns>The deserialized object.</returns>
    T? Deserialize(TextReader textReader);

    /// <summary>
    /// Deserializes an object while reading from an instance of <c>XElement</c>
    /// </summary>
    /// <param name="element">The <c>XElement</c> instance to read from.</param>
    /// <returns>The deserialized object</returns>
    T? Deserialize(XElement element);

    /// <summary>
    /// Deserializes an object from the specified file which contains the XML serialization of the object.
    /// </summary>
    /// <param name="fileName">Path to the file.</param>
    /// <returns>The deserialized object.</returns>
    T? DeserializeFromFile(string fileName);

    /// <summary>
    /// Sets the object used as the base object in the next stage of de-serialization.
    /// This method enables multi-stage de-serialization for YAXLib.
    /// </summary>
    /// <param name="obj">The object used as the base object in the next stage of de-serialization.</param>
    void SetDeserializationBaseObject(T? obj);
}