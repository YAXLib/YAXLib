// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.IO;
using System.Xml;
using System.Xml.Linq;
using YAXLib;
using YAXLib.Options;

namespace YAXLibTests.TestHelpers;

internal class GenericSerializerWrapper<T> : IYAXSerializer<object>, IRecursionCounter
{
    private readonly IYAXSerializer<T> _serializer;

    public GenericSerializerWrapper(IYAXSerializer<T> serializer)
    {
        _serializer = serializer;
    }

    /// <inheritdoc />
    public SerializerOptions Options => _serializer.Options;

    /// <inheritdoc />
    public YAXParsingErrors ParsingErrors => _serializer.ParsingErrors;

    public string Serialize(object? obj)
    {
        return _serializer.Serialize((T?) obj);
    }

    public void Serialize(object? obj, TextWriter textWriter)
    {
        _serializer.Serialize((T?) obj, textWriter);
    }

    public void Serialize(object? obj, XmlWriter xmlWriter)
    {
        _serializer.Serialize((T?) obj, xmlWriter);
    }

    public XDocument SerializeToXDocument(object? obj)
    {
        return _serializer.SerializeToXDocument((T?) obj);
    }

    public void SerializeToFile(object? obj, string fileName)
    {
        _serializer.SerializeToFile((T?) obj, fileName);
    }

    public object? Deserialize(string input)
    {
        return _serializer.Deserialize(input);
    }

    public object? Deserialize(XmlReader xmlReader)
    {
        return _serializer.Deserialize(xmlReader);
    }

    public object? Deserialize(TextReader textReader)
    {
        return _serializer.Deserialize(textReader);
    }

    public object? Deserialize(XElement element)
    {
        return _serializer.Deserialize(element);
    }

    public object? DeserializeFromFile(string fileName)
    {
        return _serializer.DeserializeFromFile(fileName);
    }

    public void SetDeserializationBaseObject(object? obj)
    {
        _serializer.SetDeserializationBaseObject((T?) obj);
    }

    #region Implementation of IRecursionCounter

    /// <inheritdoc />
    public int RecursionCount
    {
        get => _serializer.GetRecursionCount();
        set => _serializer.SetRecursionCount(value);
    }

    #endregion
}