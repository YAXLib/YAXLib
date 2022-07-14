﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Exceptions;
using YAXLib.Options;

namespace YAXLib
{
    /// <summary>
    ///     An XML serialization class which lets developers design the XML file structure and select the exception handling
    ///     policy.
    ///     This class also supports serializing most of the collection classes such as the Dictionary generic class.
    /// </summary>
    public class YAXSerializer : IYAXSerializer<object>, IRecursionCounter
    {
        #region Public constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        public YAXSerializer(Type type)
            : this(type, new SerializerOptions())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="t">The type of the object being serialized/de-serialized.</param>
        /// <param name="options">The <see cref="SerializerOptions"/> settings to influence the process of serialization or de-serialization</param>
        public YAXSerializer(Type t, SerializerOptions options)
        {
            Type = t;
            Options = options;

            // this must be the last call
            ParsingErrors = new YAXParsingErrors();
            XmlNamespaceManager = new XmlNamespaceManager();
            UdtWrapper = TypeWrappersPool.Pool.GetTypeWrapper(Type, this);
            if (UdtWrapper.HasNamespace)
                TypeNamespace = UdtWrapper.Namespace;

            Serialization = new Serialization(this);
            Deserialization = new Deserialization(this);
        }

        #endregion

        #region Public serialization methods

        /// <summary>
        ///     Serializes the specified object and returns a string containing the XML.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A <code>System.String</code> containing the XML</returns>
        public string Serialize(object obj)
        {
            return Serialization.SerializeXDocument(obj).ToString();
        }

        /// <summary>
        ///     Serializes the specified object into a <c>TextWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
        public void Serialize(object obj, TextWriter textWriter)
        {
            textWriter.Write(Serialize(obj));
        }

        /// <summary>
        ///     Serializes the specified object into a <c>XmlWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="xmlWriter">The <c>XmlWriter</c> instance.</param>
        public void Serialize(object obj, XmlWriter xmlWriter)
        {
            Serialization.SerializeXDocument(obj).WriteTo(xmlWriter);
        }

        /// <summary>
        ///     Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
        public XDocument SerializeToXDocument(object obj)
        {
            return Serialization.SerializeXDocument(obj);
        }

        /// <summary>
        ///     Serializes the specified object to file.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="fileName">Path to the file.</param>
        public void SerializeToFile(object obj, string fileName)
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
        ///     Deserializes the specified string containing the XML serialization and returns an object.
        /// </summary>
        /// <param name="input">The input string containing the XML serialization.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(string input)
        {
            try
            {
                using TextReader tr = new StringReader(input);
                var xDocument = XDocument.Load(tr, Deserialization.GetXmlLoadOptions());
                var baseElement = xDocument.Root;
                FindDocumentDefaultNamespace();
                return Deserialization.DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                Deserialization.OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), Options.ExceptionBehavior);
                return null;
            }
        }

        /// <summary>
        ///     Deserializes an object while reading input from an instance of <c>XmlReader</c>.
        /// </summary>
        /// <param name="xmlReader">The <c>XmlReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(XmlReader xmlReader)
        {
            try
            {
                var xDocument = XDocument.Load(xmlReader, Deserialization.GetXmlLoadOptions());
                var baseElement = xDocument.Root;
                FindDocumentDefaultNamespace();
                return Deserialization.DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                Deserialization.OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), Options.ExceptionBehavior);
                return null;
            }
        }

        /// <summary>
        ///     Deserializes an object while reading input from an instance of <c>TextReader</c>.
        /// </summary>
        /// <param name="textReader">The <c>TextReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(TextReader textReader)
        {
            try
            {
                var xDocument = XDocument.Load(textReader, Deserialization.GetXmlLoadOptions());
                var baseElement = xDocument.Root;
                FindDocumentDefaultNamespace();
                return Deserialization.DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                Deserialization.OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), Options.ExceptionBehavior);
                return null;
            }
        }

        /// <summary>
        ///     Deserializes an object while reading from an instance of <c>XElement</c>
        /// </summary>
        /// <param name="element">The <c>XElement</c> instance to read from.</param>
        /// <returns>The deserialized object</returns>
        public object Deserialize(XElement element)
        {
            // impossible to throw YAXBadlyFormedXML
            var xDocument = new XDocument();
            xDocument.Add(element);
            FindDocumentDefaultNamespace();
            return Deserialization.DeserializeBase(element);
        }

        /// <summary>
        ///     Deserializes an object from the specified file which contains the XML serialization of the object.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>The deserialized object.</returns>
        public object DeserializeFromFile(string fileName)
        {
            return Deserialize(File.ReadAllText(fileName));
        }

        /// <summary>
        ///     Sets the object used as the base object in the next stage of de-serialization.
        ///     This method enables multi-stage deserialization for YAXLib.
        /// </summary>
        /// <param name="obj">The object used as the base object in the next stage of deserialization.</param>
        public void SetDeserializationBaseObject(object obj)
        {
            Deserialization.SetDeserializationBaseObject(obj);
        }

        #endregion

        #region Other public methods

        /// <summary>
        ///     Cleans up auxiliary memory used by YAXLib during different sessions of serialization.
        /// </summary>
        public static void CleanUpAuxiliaryMemory()
        {
            TypeWrappersPool.CleanUp();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="SerializerOptions"/> settings
        ///     to influence the process of serialization or de-serialization of <see cref="YAXSerializer"/>s.
        /// </summary>
        public SerializerOptions Options { get; }

        /// <summary>
        ///     Gets the parsing errors.
        /// </summary>
        /// <value>The parsing errors.</value>
        public YAXParsingErrors ParsingErrors { get; }

        #endregion

        #region Internal methods

        internal YAXSerializer NewInternalSerializer(Type type, XNamespace namespaceToOverride,
            XElement insertionLocation)
        {
            _recursionCount = Options.MaxRecursion == 0 ? 0 : _recursionCount + 1;
            var serializer = new YAXSerializer(type, Options) {
                SerializedStack = SerializedStack,
                DocumentDefaultNamespace = DocumentDefaultNamespace
            };
            ((IRecursionCounter) serializer).RecursionCount = _recursionCount;

            if (namespaceToOverride != null)
                serializer.SetNamespaceToOverrideEmptyNamespace(namespaceToOverride);

            if (insertionLocation != null)
                serializer.Serialization.SetBaseElement(insertionLocation);

            return serializer;
        }

        internal void FinalizeNewSerializer(YAXSerializer serializer, bool importNamespaces,
            bool popFromSerializationStack = true)
        {
            if (serializer == null)
                return;

            if (_recursionCount > 0) _recursionCount--;

            if (popFromSerializationStack && IsSerializing && serializer.Type != null &&
                !serializer.Type.IsValueType)
                SerializedStack.Pop();

            if (importNamespaces)
                XmlNamespaceManager.ImportNamespaces(serializer);

            ParsingErrors.AddRange(serializer.ParsingErrors);
        }

        /// <summary>
        ///     Gets the sequence of fields to be serialized or to be deserialized for the serializer's underlying type.
        ///     This sequence is retrieved according to the field-types specified by the user.
        /// </summary>
        /// <returns>The sequence of fields to be de/serialized for the serializer's underlying type.</returns>
        internal IEnumerable<MemberWrapper> GetFieldsToBeSerialized()
        {
            return GetFieldsToBeSerialized(UdtWrapper).OrderBy(t => t.Order);
        }

        /// <summary>
        ///     Gets the sequence of fields to be serialized or to be deserialized for the specified type.
        ///     This sequence is retrieved according to the field-types specified by the user.
        /// </summary>
        /// <param name="typeWrapper">
        ///     The type wrapper for the type whose serializable
        ///     fields is going to be retrieved.
        /// </param>
        /// <returns>the sequence of fields to be de/serialized for the specified type</returns>
        internal IEnumerable<MemberWrapper> GetFieldsToBeSerialized(UdtWrapper typeWrapper)
        {
#pragma warning disable S3011 // disable sonar accessibility bypass warning
            foreach (var member in typeWrapper.UnderlyingType.GetMembers(
                         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                         typeWrapper.IncludePrivateMembersFromBaseTypes))
#pragma warning restore S3011 // enable sonar accessibility bypass warning
            {
                if (!IsValidPropertyOrField(member)) continue;
                if (member is PropertyInfo prop && !CanSerializeProperty(prop)) continue;

                if ((typeWrapper.IsCollectionType || typeWrapper.IsDictionaryType)
                    && ReflectionUtils.IsPartOfNetFx(member))
                    continue;

                var memInfo = new MemberWrapper(member, this);
                if (memInfo.IsAllowedToBeSerialized(typeWrapper.FieldsToSerialize,
                        UdtWrapper.DoNotSerializePropertiesWithNoSetter)) yield return memInfo;
            }
        }

        internal void FindDocumentDefaultNamespace()
        {
            if (UdtWrapper.HasNamespace && string.IsNullOrEmpty(UdtWrapper.NamespacePrefix))
                // it has a default namespace defined (one without a prefix)
                DocumentDefaultNamespace = UdtWrapper.Namespace; // set the default namespace
        }

        #endregion

        #region Internal properties

        /// <summary>
        ///     The main document's default namespace. This is stored so that if an attribute has the default namespace,
        ///     it should be serialized without namespace assigned to it. Storing it here does NOT mean that elements
        ///     and attributes without any namespace must adapt this namespace. It is just for comparison and control
        ///     purposes.
        /// </summary>
        /// <remarks>
        ///     Is set by method <see cref="YAXSerializer.FindDocumentDefaultNamespace"/>
        /// </remarks>
        internal XNamespace DocumentDefaultNamespace { get; set; }

        /// <summary>
        ///     Get an instance of the class used for <see cref="YAXLib.Serialization"/>.
        /// </summary>
        internal Serialization Serialization { get; }

        /// <summary>
        ///     Get an instance of the class used for <see cref="YAXLib.Deserialization"/>.
        /// </summary>
        internal Deserialization Deserialization { get; }

        /// <summary>
        ///     <see langword="true" /> if this instance is busy serializing objects, <see langword="false" /> otherwise.
        /// </summary>
        internal bool IsSerializing { get; set; }

        /// <summary>
        ///     A collection of already serialized objects, kept for the sake of loop detection and preventing stack overflow
        ///     exception
        /// </summary>
        internal Stack<object> SerializedStack { get; set; }

        /// <summary>
        ///     The class or structure that is to be serialized/deserialized.
        /// </summary>
        internal Type Type { get; set; }

        /// <summary>
        ///     The type wrapper for the underlying type used in the serializer
        /// </summary>
        internal UdtWrapper UdtWrapper { get; set; }

        /// <summary>
        ///     Gets or sets the number of recursions (number of total created <see cref="YAXSerializer"/> instances).
        /// </summary>
        int IRecursionCounter.RecursionCount
        {
            get => _recursionCount;
            set => _recursionCount = value;
        }

        private int _recursionCount;

        /// <summary>
        ///     A manager that keeps a map of namespaces to their prefixes (if any) to be added ultimately to the xml result
        /// </summary>
        internal XmlNamespaceManager XmlNamespaceManager { get; }

        internal XNamespace TypeNamespace { get; set; }

        #endregion

        #region Private methods

        private void SetNamespaceToOverrideEmptyNamespace(XNamespace otherNamespace)
        {
            // if namespace info is not already set during construction,
            // then set it from the other YAXSerializer instance
            if (otherNamespace.IsEmpty() && !TypeNamespace.IsEmpty()) TypeNamespace = otherNamespace;
        }

        private static bool IsValidPropertyOrField(MemberInfo member)
        {
            var name0 = member.Name[0];
            return (char.IsLetter(name0) || name0 == '_') &&
                   (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field);
        }

        private static bool CanSerializeProperty(PropertyInfo prop)
        {
            // ignore indexers; if member is an indexer property, do not serialize it
            if (prop.GetIndexParameters().Length > 0)
                return false;

            // don't serialize delegates as well
            if (ReflectionUtils.IsTypeEqualOrInheritedFromType(prop.PropertyType, typeof(Delegate)))
                return false;

            return true;
        }

        #endregion
    }
}
