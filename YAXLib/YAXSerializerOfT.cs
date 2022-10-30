// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.IO;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib;

/// <summary>
/// An XML serialization class which lets developers design the XML file structure and select the exception handling
/// policy.
/// This class also supports serializing most of the collection classes such as the Dictionary generic class.
/// </summary>
public class YAXSerializer<T> : IYAXSerializer<T>, IRecursionCounter
{
    private readonly YAXSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXSerializer" /> class.
    /// </summary>
    public YAXSerializer()
    {
        _serializer = new YAXSerializer(typeof(T));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXSerializer" /> class.
    /// </summary>
    /// <param name="options">
    /// The <see cref="SerializerOptions" /> settings to influence the process of serialization or
    /// de-serialization
    /// </param>
    public YAXSerializer(SerializerOptions options)
    {
        _serializer = new YAXSerializer(typeof(T), options);
    }

    /// <summary>
    /// Gets the <see cref="SerializerOptions" /> settings
    /// to influence the process of serialization or de-serialization of <see cref="YAXSerializer" />s.
    /// </summary>
    public SerializerOptions Options => _serializer.Options;

    /// <summary>
    /// Gets the parsing errors.
    /// </summary>
    /// <value>The parsing errors.</value>
    public YAXParsingErrors ParsingErrors => _serializer.ParsingErrors;

    int IRecursionCounter.RecursionCount
    {
        get
        {
            IRecursionCounter serializer = _serializer;
            return serializer.RecursionCount;
        }

        set
        {
            IRecursionCounter serializer = _serializer;
            serializer.RecursionCount = value;
        }
    }

    /// <summary>
    /// Serializes the specified object and returns a string containing the XML.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A <code>System.String</code> containing the XML</returns>
    public string Serialize(T? obj)
    {
        return _serializer.Serialize(obj);
    }

    /// <summary>
    /// Serializes the specified object into a <c>TextWriter</c> instance.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
    public void Serialize(T? obj, TextWriter textWriter)
    {
        _serializer.Serialize(obj, textWriter);
    }

    /// <summary>
    /// Serializes the specified object into a <c>XmlWriter</c> instance.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="xmlWriter">The <c>XmlWriter</c> instance.</param>
    public void Serialize(T? obj, XmlWriter xmlWriter)
    {
        _serializer.Serialize(obj, xmlWriter);
    }

    /// <summary>
    /// Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
    public XDocument SerializeToXDocument(T? obj)
    {
        return _serializer.SerializeToXDocument(obj);
    }

    /// <summary>
    /// Serializes the specified object to file.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="fileName">Path to the file.</param>
    public void SerializeToFile(T? obj, string fileName)
    {
        _serializer.SerializeToFile(obj, fileName);
    }

    /// <summary>
    /// Deserializes the specified string containing the XML serialization and returns an object.
    /// </summary>
    /// <param name="input">The input string containing the XML serialization.</param>
    /// <returns>The deserialized object.</returns>
    public T? Deserialize(string input)
    {
        return (T?) _serializer.Deserialize(input);
    }

    /// <summary>
    /// Deserializes an object while reading input from an instance of <c>XmlReader</c>.
    /// </summary>
    /// <param name="xmlReader">The <c>XmlReader</c> instance to read input from.</param>
    /// <returns>The deserialized object.</returns>
    public T? Deserialize(XmlReader xmlReader)
    {
        return (T?) _serializer.Deserialize(xmlReader);
    }

    /// <summary>
    /// Deserializes an object while reading input from an instance of <c>TextReader</c>.
    /// </summary>
    /// <param name="textReader">The <c>TextReader</c> instance to read input from.</param>
    /// <returns>The deserialized object.</returns>
    public T? Deserialize(TextReader textReader)
    {
        return (T?) _serializer.Deserialize(textReader);
    }

    /// <summary>
    /// Deserializes an object while reading from an instance of <c>XElement</c>
    /// </summary>
    /// <param name="element">The <c>XElement</c> instance to read from.</param>
    /// <returns>The deserialized object</returns>
    public T? Deserialize(XElement element)
    {
        return (T?) _serializer.Deserialize(element);
    }

    /// <summary>
    /// Deserializes an object from the specified file which contains the XML serialization of the object.
    /// </summary>
    /// <param name="fileName">Path to the file.</param>
    /// <returns>The deserialized object.</returns>
    public T? DeserializeFromFile(string fileName)
    {
        return (T?) _serializer.DeserializeFromFile(fileName);
    }

    /// <summary>
    /// Sets the object used as the base object in the next stage of de-serialization.
    /// This method enables multi-stage de-serialization for YAXLib.
    /// </summary>
    /// <param name="obj">The object used as the base object in the next stage of de-serialization.</param>
    public void SetDeserializationBaseObject(T? obj)
    {
        _serializer.SetDeserializationBaseObject(obj);
    }
}