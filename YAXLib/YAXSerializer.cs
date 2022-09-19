// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Caching;
using YAXLib.Enums;
using YAXLib.Exceptions;
using YAXLib.Options;
using YAXLib.Pooling.ObjectPools;
using YAXLib.Pooling.YAXLibPools;

namespace YAXLib;

/// <summary>
/// An XML serialization class which lets developers design the XML file structure and select the exception handling
/// policy.
/// This class also supports serializing most of the collection classes such as the Dictionary generic class.
/// </summary>
public class YAXSerializer : IYAXSerializer<object>, IRecursionCounter
{
    #region Object Pooling

    /// <summary>
    /// Creates a new <see cref="YAXSerializer" /> instance, that is partially initialized.
    /// This CTOR is used for object pooling. After creating the instance,
    /// <see cref="Initialize(System.Type,SerializerOptions)" /> must be called before using it.
    /// The method is called by <see cref="SerializerPool" />.
    /// </summary>
    internal YAXSerializer()
    {
        Options = new SerializerOptions();
        ParsingErrors = new YAXParsingErrors();
        XmlNamespaceManager = new XmlNamespaceManager();
        DocumentDefaultNamespace = XNamespace.None;
        TypeNamespace = XNamespace.None;
        UdtWrapper = UdtWrapperCache.Instance.GetOrAddItem(Type, Options);

        Serialization = new Serialization(this);
        Deserialization = new Deserialization(this);
    }

    /// <summary>
    /// This instance will be (re-) initialized it a way
    /// that it has the same virgin state like an instance that
    /// was created with one of the public CTORs.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="options"></param>
    internal void Initialize(Type t, SerializerOptions options)
    {
        Type = t;
        Options = options;
        DocumentDefaultNamespace = XNamespace.None;

        Deserialization.Initialize();
        Serialization.Initialize();

        UdtWrapper = UdtWrapperCache.Instance.GetOrAddItem(Type, Options);
        if (UdtWrapper.HasNamespace)
            TypeNamespace = UdtWrapper.Namespace;
    }

    /// <summary>
    /// Pre-initializes this instance so that it prepared for calling
    /// <see cref="Initialize(System.Type,SerializerOptions)" /> after it is
    /// taken from the object pool.
    /// The method is called by <see cref="SerializerPool" /> on returning to the pool.
    /// </summary>
    internal void ReturnToPool()
    {
        _recursionCount = 0;
        SerializedStack = new Stack<object>();
        ParsingErrors.ClearErrors();
        TypeNamespace = XNamespace.Get(string.Empty);
        DocumentDefaultNamespace = XNamespace.Get(string.Empty);
        XmlNamespaceManager.Initialize();
        IsSerializing = false;
    }

    #endregion

    #region Public constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXSerializer" /> class.
    /// </summary>
    /// <param name="type">The type of the object being serialized/deserialized.</param>
    public YAXSerializer(Type type)
        : this(type, new SerializerOptions { SerializationOptions = YAXSerializationOptions.SerializeNullObjects })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXSerializer" /> class.
    /// </summary>
    /// <param name="t">The type of the object being serialized/de-serialized.</param>
    /// <param name="options">
    /// The <see cref="SerializerOptions" /> settings to influence the process of serialization or
    /// de-serialization
    /// </param>
    public YAXSerializer(Type t, SerializerOptions options) : this()
    {
        Initialize(t, options);
    }

    #endregion

    #region Public serialization methods

    /// <summary>
    /// Serializes the specified object and returns a string containing the XML.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A <code>System.String</code> containing the XML</returns>
    public string Serialize(object? obj)
    {
        return Serialization.SerializeXDocument(obj).ToString();
    }

    /// <summary>
    /// Serializes the specified object into a <c>TextWriter</c> instance.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
    public void Serialize(object? obj, TextWriter textWriter)
    {
        textWriter.Write(Serialize(obj));
    }

    /// <summary>
    /// Serializes the specified object into a <c>XmlWriter</c> instance.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="xmlWriter">The <c>XmlWriter</c> instance.</param>
    public void Serialize(object? obj, XmlWriter xmlWriter)
    {
        Serialization.SerializeXDocument(obj).WriteTo(xmlWriter);
    }

    /// <summary>
    /// Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
    public XDocument SerializeToXDocument(object? obj)
    {
        return Serialization.SerializeXDocument(obj);
    }

    /// <summary>
    /// Serializes the specified object to file.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="fileName">Path to the file.</param>
    public void SerializeToFile(object? obj, string fileName)
    {
        var ser = string.Format(
            Options.Culture,
            "{0}{1}{2}",
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
            Environment.NewLine,
            Serialize(obj));
        File.WriteAllText(fileName, ser, Encoding.UTF8);
    }

    #endregion

    #region Public deserialization methods

    /// <summary>
    /// Deserializes the specified string containing the XML serialization and returns an object.
    /// </summary>
    /// <param name="input">The input string containing the XML serialization.</param>
    /// <returns>The deserialized object.</returns>
    public object? Deserialize(string input)
    {
        try
        {
            using TextReader tr = new StringReader(input);
            var xDocument = XDocument.Load(tr, Deserialization.GetXmlLoadOptions());
            var baseElement = xDocument.Root;
            DocumentDefaultNamespace = UdtWrapper.FindDocumentDefaultNamespace();
            return Deserialization.DeserializeBase(baseElement);
        }
        catch (XmlException ex)
        {
            Deserialization.OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition),
                Options.ExceptionBehavior);
            return null;
        }
    }

    /// <summary>
    /// Deserializes an object while reading input from an instance of <c>XmlReader</c>.
    /// </summary>
    /// <param name="xmlReader">The <c>XmlReader</c> instance to read input from.</param>
    /// <returns>The deserialized object.</returns>
    public object? Deserialize(XmlReader xmlReader)
    {
        try
        {
            var xDocument = XDocument.Load(xmlReader, Deserialization.GetXmlLoadOptions());
            var baseElement = xDocument.Root;
            DocumentDefaultNamespace = UdtWrapper.FindDocumentDefaultNamespace();
            return Deserialization.DeserializeBase(baseElement);
        }
        catch (XmlException ex)
        {
            Deserialization.OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition),
                Options.ExceptionBehavior);
            return null;
        }
    }

    /// <summary>
    /// Deserializes an object while reading input from an instance of <c>TextReader</c>.
    /// </summary>
    /// <param name="textReader">The <c>TextReader</c> instance to read input from.</param>
    /// <returns>The deserialized object.</returns>
    public object? Deserialize(TextReader textReader)
    {
        try
        {
            var xDocument = XDocument.Load(textReader, Deserialization.GetXmlLoadOptions());
            var baseElement = xDocument.Root;
            DocumentDefaultNamespace = UdtWrapper.FindDocumentDefaultNamespace();
            return Deserialization.DeserializeBase(baseElement);
        }
        catch (XmlException ex)
        {
            Deserialization.OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition),
                Options.ExceptionBehavior);
            return null;
        }
    }

    /// <summary>
    /// Deserializes an object while reading from an instance of <c>XElement</c>
    /// </summary>
    /// <param name="element">The <c>XElement</c> instance to read from.</param>
    /// <returns>The deserialized object</returns>
    public object? Deserialize(XElement element)
    {
        // impossible to throw YAXBadlyFormedXML
        var xDocument = new XDocument();
        xDocument.Add(element);
        DocumentDefaultNamespace = UdtWrapper.FindDocumentDefaultNamespace();
        return Deserialization.DeserializeBase(element);
    }

    /// <summary>
    /// Deserializes an object from the specified file which contains the XML serialization of the object.
    /// </summary>
    /// <param name="fileName">Path to the file.</param>
    /// <returns>The deserialized object.</returns>
    public object? DeserializeFromFile(string fileName)
    {
        return Deserialize(File.ReadAllText(fileName));
    }

    /// <summary>
    /// Sets the object used as the base object in the next stage of deserialization.
    /// This method enables multi-stage deserialization for YAXLib.
    /// </summary>
    /// <param name="obj">The object used as the base object in the next stage of deserialization.</param>
    public void SetDeserializationBaseObject(object? obj)
    {
        Deserialization.SetDeserializationBaseObject(obj);
    }

    #endregion

    #region Other public methods

    /// <summary>
    /// Clears the static caches used across all <see cref="YAXSerializer" /> instances.
    /// </summary>
    public static void ClearCache()
    {
        UdtWrapperCache.Instance.Clear();
        MemberWrapperCache.Instance.Clear();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the <see cref="SerializerOptions" /> settings
    /// to influence the process of serialization or de-serialization of <see cref="YAXSerializer" />s.
    /// </summary>
    public SerializerOptions Options { get; private set; }

    /// <summary>
    /// Gets the parsing errors.
    /// </summary>
    /// <value>The parsing errors.</value>
    public YAXParsingErrors ParsingErrors { get; }

    #endregion

    #region Get and return a child serializer from/to object pool

    /// <summary>
    /// Creates a new internal child <see cref="YAXSerializer" /> for recursive de/serialization.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="namespaceToOverride"></param>
    /// <param name="insertionLocation"></param>
    /// <param name="serializer">The initialized child serializer.</param>
    /// <returns>
    /// An <see cref="IDisposable" /> <see cref="PooledObject{T}" /> that will return
    /// the serializer to the <see cref="SerializerPool" /> upon auto-disposal when out of scope.
    /// Important: Add the <see langword="using" /> declaration to the local variable.
    /// </returns>
    /// <example>
    /// using var serializerPooledObject
    /// = GetChildSerializer(type, namespaceToOverride, insertionLocation, out var serializer);
    /// </example>
    internal PooledObject<YAXSerializer> GetChildSerializer(Type type, XNamespace? namespaceToOverride,
        XElement? insertionLocation, out YAXSerializer serializer)
    {
        _recursionCount++;

        // Get a standard serializer from the pool
        var serializerPoolObject = SerializerPool.Instance.Get(out serializer);
        serializer.Initialize(type, Options);
        // Make it a child serializer
        InitializeAsChildSerializer(serializer, namespaceToOverride, insertionLocation);

        return serializerPoolObject;
    }

    /// <summary>
    /// Initialize the standard serializer as child serializer
    /// </summary>
    /// <param name="serializer"></param>
    /// <param name="namespaceToOverride"></param>
    /// <param name="insertionLocation"></param>
    private void InitializeAsChildSerializer(YAXSerializer serializer, XNamespace? namespaceToOverride,
        XElement? insertionLocation)
    {
        serializer.SerializedStack = SerializedStack;
        serializer.DocumentDefaultNamespace = DocumentDefaultNamespace;
        ((IRecursionCounter) serializer).RecursionCount = _recursionCount;

        if (namespaceToOverride != null)
            serializer.SetNamespaceToOverrideEmptyNamespace(namespaceToOverride);

        if (insertionLocation != null)
            serializer.Serialization.SetBaseElement(insertionLocation);
    }

    /// <summary>
    /// Perform some house-keeping after the child serializer has completed.
    /// <para>
    /// Note: <b>It is not guaranteed that this method always gets called</b>,
    /// e.g. when de/serializing self-referencing classes (class members having
    /// the same type as the class), or when an exception is thrown.
    /// </para>
    /// <para>
    /// However, <see cref="GetChildSerializer" /> will return an object that is auto-disposed
    /// when out of scope.
    /// </para>
    /// </summary>
    /// <param name="serializer"></param>
    /// <param name="importNamespaces"></param>
    /// <param name="popFromSerializationStack"></param>
    internal void FinalizeChildSerializer(YAXSerializer serializer, bool importNamespaces,
        bool popFromSerializationStack = true)
    {
        if (serializer == null)
            return;

        if (_recursionCount > 0) _recursionCount--;

        if (popFromSerializationStack && IsSerializing && serializer.Type is { IsValueType: false })
            SerializedStack.Pop();

        if (importNamespaces)
            XmlNamespaceManager.ImportNamespaces(serializer);

        ParsingErrors.AddRange(serializer.ParsingErrors);
    }

    #endregion

    #region Internal properties

    /// <summary>
    /// The main document's default namespace. This is stored so that if an attribute has the default namespace,
    /// it should be serialized without namespace assigned to it. Storing it here does NOT mean that elements
    /// and attributes without any namespace must adapt this namespace. It is just for comparison and control
    /// purposes.
    /// </summary>
    internal XNamespace DocumentDefaultNamespace { get; set; }

    /// <summary>
    /// Get an instance of the class used for <see cref="YAXLib.Serialization" />.
    /// </summary>
    internal Serialization Serialization { get; }

    /// <summary>
    /// Get an instance of the class used for <see cref="YAXLib.Deserialization" />.
    /// </summary>
    internal Deserialization Deserialization { get; }

    /// <summary>
    /// <see langword="true" /> if this instance is busy serializing objects, <see langword="false" /> otherwise.
    /// </summary>
    internal bool IsSerializing { get; set; }

    /// <summary>
    /// A collection of already serialized objects, kept for the sake of loop detection and preventing stack overflow
    /// exception
    /// </summary>
    internal Stack<object> SerializedStack { get; set; } = new();

    /// <summary>
    /// The class or structure that is to be serialized/deserialized.
    /// </summary>
    internal Type Type { get; set; } = typeof(object);

    /// <summary>
    /// The type wrapper for the underlying type used in the serializer.
    /// </summary>
    internal UdtWrapper UdtWrapper { get; set; }

    /// <inheritdoc />
    int IRecursionCounter.RecursionCount
    {
        get => _recursionCount;
        set => _recursionCount = value;
    }

    private int _recursionCount;

    /// <summary>
    /// A manager that keeps a map of namespaces to their prefixes (if any) to be added ultimately to the xml result
    /// </summary>
    internal XmlNamespaceManager XmlNamespaceManager { get; private set; }

    internal XNamespace TypeNamespace { get; set; }

    #endregion

    #region Private methods

    private void SetNamespaceToOverrideEmptyNamespace(XNamespace otherNamespace)
    {
        // If namespace info is not already set during construction,
        // then set it from the other YAXSerializer instance
        if (otherNamespace.IsEmpty() && !TypeNamespace.IsEmpty()) TypeNamespace = otherNamespace;
    }

    #endregion
}