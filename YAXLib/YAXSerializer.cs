﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace YAXLib
{
    /// <summary>
    ///     An XML serialization class which lets developers design the XML file structure and select the exception handling
    ///     policy.
    ///     This class also supports serializing most of the collection classes such as the Dictionary generic class.
    /// </summary>
    public class YAXSerializer
    {
        private const int DefaultMaxRecursion = 300;

        /// <summary>
        ///     The exception error behaviour enumeration to be used by the YAX library.
        /// </summary>
        private readonly YAXExceptionTypes _defaultExceptionType = YAXExceptionTypes.Warning;

        /// <summary>
        ///     The handling policy enumeration to be used by the YAX library.
        /// </summary>
        private readonly YAXExceptionHandlingPolicies _exceptionPolicy = YAXExceptionHandlingPolicies.ThrowErrorsOnly;

        /// <summary>
        ///     a map of namespaces to their prefixes (if any) to be added utlimately to the xml result
        /// </summary>
        private readonly Dictionary<XNamespace, string> _namespaceToPrefix = new Dictionary<XNamespace, string>();

        /// <summary>
        ///     The list of all errors that have occured.
        /// </summary>
        private readonly YAXParsingErrors _parsingErrors = new YAXParsingErrors();

        /// <summary>
        ///     The serialization option enumeration which can be set during initialization.
        /// </summary>
        private readonly YAXSerializationOptions _serializationOption = YAXSerializationOptions.SerializeNullObjects;

        /// <summary>
        ///     a reference to the base xml element used during serialization.
        /// </summary>
        private XElement _baseElement;

        /// <summary>
        ///     reference to a pre assigned deserialization base object
        /// </summary>
        private object _desObject;

        /// <summary>
        ///     the attribute name used to deserialize meta-data for multi-dimensional arrays.
        /// </summary>
        private string _dimsAttrName = "dims";

        /// <summary>
        ///     The main document's default namespace. This is stored so that if an attribute has the default namespace,
        ///     it should be serialized without namespace assigned to it. Storing it here does NOT mean that elements
        ///     and attributes without any namespace must adapt this namespace. It is just for comparison and control
        ///     purposes.
        /// </summary>
        private XNamespace _documentDefaultNamespace;

        /// <summary>
        ///     Specifies whether an exception is occurred during the deserialization of the current member
        /// </summary>
        private bool _exceptionOccurredDuringMemberDeserialization;

        /// <summary>
        ///     <c>true</c> if this instance is busy serializing objects, <c>false</c> otherwise.
        /// </summary>
        private bool _isSerializing;

        /// <summary>
        ///     XML document object which will hold the resulting serialization
        /// </summary>
        private XDocument _mainDocument;

        /// <summary>
        ///     a collection of already serialized objects, kept for the sake of loop detection and preventing stack overflow
        ///     exception
        /// </summary>
        private Stack<object> _serializedStack;

        /// <summary>
        ///     the attribute name used to deserialize meta-data for real types of objects serialized through
        ///     a reference to their base class or interface.
        /// </summary>
        private string _trueTypeAttrName = "realtype";

        /// <summary>
        ///     The class or structure that is to be serialized/deserialized.
        /// </summary>
        private Type _type;

        /// <summary>
        ///     The type wrapper for the underlying type used in the serializer
        /// </summary>
        private UdtWrapper _udtWrapper;

        /// <summary>
        ///     The initials used for the xml namespace
        /// </summary>
        private string _yaxLibNamespacePrefix = "yaxlib";

        /// <summary>
        ///     The URI address which holds the xmlns:yaxlib definition.
        /// </summary>
        private XNamespace _yaxLibNamespaceUri = XNamespace.Get("http://www.sinairv.com/yaxlib/");

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        public YAXSerializer(Type type)
            : this(type, YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error,
                YAXSerializationOptions.SerializeNullObjects)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        /// <param name="seializationOptions">The serialization option flags.</param>
        public YAXSerializer(Type type, YAXSerializationOptions seializationOptions)
            : this(type, YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error,
                seializationOptions)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        /// <param name="exceptionPolicy">The exception handling policy.</param>
        public YAXSerializer(Type type, YAXExceptionHandlingPolicies exceptionPolicy)
            : this(type, exceptionPolicy, YAXExceptionTypes.Error, YAXSerializationOptions.SerializeNullObjects)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        /// <param name="exceptionPolicy">The exception handling policy.</param>
        /// <param name="defaultExType">The exceptions are treated as the value specified, unless otherwise specified.</param>
        public YAXSerializer(Type type, YAXExceptionHandlingPolicies exceptionPolicy, YAXExceptionTypes defaultExType)
            : this(type, exceptionPolicy, defaultExType, YAXSerializationOptions.SerializeNullObjects)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="t">The type of the object being serialized/deserialized.</param>
        /// <param name="exceptionPolicy">The exception handling policy.</param>
        /// <param name="defaultExType">The exceptions are treated as the value specified, unless otherwise specified.</param>
        /// <param name="option">The serialization option.</param>
        public YAXSerializer(Type t, YAXExceptionHandlingPolicies exceptionPolicy, YAXExceptionTypes defaultExType,
            YAXSerializationOptions option)
        {
            _type = t;
            _exceptionPolicy = exceptionPolicy;
            _defaultExceptionType = defaultExType;
            _serializationOption = option;
            // this must be the last call
            _udtWrapper = TypeWrappersPool.Pool.GetTypeWrapper(_type, this);
            if (_udtWrapper.HasNamespace)
                TypeNamespace = _udtWrapper.Namespace;
            MaxRecursion = DefaultMaxRecursion;
        }

        internal XNamespace TypeNamespace { get; set; }

        internal bool HasTypeNamespace => TypeNamespace.IsEmpty();

        /// <summary>
        ///     Gets the default type of the exception.
        /// </summary>
        /// <value>The default type of the exception.</value>
        public YAXExceptionTypes DefaultExceptionType => _defaultExceptionType;

        /// <summary>
        ///     Gets the serialization option.
        /// </summary>
        /// <value>The serialization option.</value>
        public YAXSerializationOptions SerializationOption => _serializationOption;

        /// <summary>
        ///     Gets the exception handling policy.
        /// </summary>
        /// <value>The exception handling policy.</value>
        public YAXExceptionHandlingPolicies ExceptionHandlingPolicy => _exceptionPolicy;

        /// <summary>
        ///     Gets the parsing errors.
        /// </summary>
        /// <value>The parsing errors.</value>
        public YAXParsingErrors ParsingErrors => _parsingErrors;

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is created to deserialize a non collection member of another
        ///     object.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is craeted to deserialize a non collection member of another object; otherwise,
        ///     <c>false</c>.
        /// </value>
        private bool IsCraetedToDeserializeANonCollectionMember { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether XML elements or attributes should be removed after being deserialized
        /// </summary>
        private bool RemoveDeserializedXmlNodes { get; set; }

        /// <summary>
        ///     The URI address which holds the xmlns:yaxlib definition.
        /// </summary>
        public XNamespace YaxLibNamespaceUri
        {
            get { return _yaxLibNamespaceUri; }
            set { _yaxLibNamespaceUri = value; }
        }

        /// <summary>
        ///     The prefix used for the xml namespace
        /// </summary>
        public string YaxLibNamespacePrefix
        {
            get { return _yaxLibNamespacePrefix; }

            set { _yaxLibNamespacePrefix = value; }
        }

        /// <summary>
        ///     The attribute name used to deserialize meta-data for multi-dimensional arrays.
        /// </summary>
        public string DimensionsAttributeName
        {
            get { return _dimsAttrName; }

            set { _dimsAttrName = value; }
        }

        /// <summary>
        ///     the attribute name used to deserialize meta-data for real types of objects serialized through
        ///     a reference to their base class or interface.
        /// </summary>
        public string RealTypeAttributeName
        {
            get { return _trueTypeAttrName; }

            set { _trueTypeAttrName = value; }
        }

        /// <summary>
        ///     Specifies the maximum serialization depth (default 300).
        ///     This roughly equals the maximum element depth of the resulting XML.
        ///     0 means unlimited.
        ///     1 means an empty XML tag with no content.
        /// </summary>
        public int MaxRecursion { get; set; }

        internal void SetNamespaceToOverrideEmptyNamespace(XNamespace otherNamespace)
        {
            // if namespace info is not already set during construction, 
            // then set it from the other YAXSerializer instance
            if (otherNamespace.IsEmpty() && !HasTypeNamespace) TypeNamespace = otherNamespace;
        }

        /// <summary>
        ///     Serializes the specified object and returns a string containing the XML.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A <code>System.String</code> containing the XML</returns>
        public string Serialize(object obj)
        {
            return SerializeXDocument(obj).ToString();
        }

        /// <summary>
        ///     Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
        public XDocument SerializeToXDocument(object obj)
        {
            return SerializeXDocument(obj);
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
            SerializeXDocument(obj).WriteTo(xmlWriter);
        }

        /// <summary>
        ///     Serializes the specified object to file.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="fileName">Path to the file.</param>
        public void SerializeToFile(object obj, string fileName)
        {
            var ser = string.Format(
                CultureInfo.CurrentCulture,
                "{0}{1}{2}",
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                Environment.NewLine,
                Serialize(obj));
            File.WriteAllText(fileName, ser, Encoding.UTF8);
        }

        /// <summary>
        ///     Deserializes the specified string containing the XML serialization and returns an object.
        /// </summary>
        /// <param name="input">The input string containing the XML serialization.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(string input)
        {
            try
            {
                using (TextReader tr = new StringReader(input))
                {
                    var xdoc = XDocument.Load(tr, GetXmlLoadOptions());
                    var baseElement = xdoc.Root;
                    FindDocumentDefaultNamespace();
                    return DeserializeBase(baseElement);
                }
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), _defaultExceptionType);
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
                var xdoc = XDocument.Load(xmlReader, GetXmlLoadOptions());
                var baseElement = xdoc.Root;
                FindDocumentDefaultNamespace();
                return DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), _defaultExceptionType);
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
                var xdoc = XDocument.Load(textReader, GetXmlLoadOptions());
                var baseElement = xdoc.Root;
                FindDocumentDefaultNamespace();
                return DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), _defaultExceptionType);
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
            try
            {
                var xdoc = new XDocument();
                xdoc.Add(element);
                FindDocumentDefaultNamespace();
                return DeserializeBase(element);
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), _defaultExceptionType);
                return null;
            }
        }

        /// <summary>
        ///     Deserializes an object from the specified file which contains the XML serialization of the object.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>The deserialized object.</returns>
        public object DeserializeFromFile(string fileName)
        {
            try
            {
                return Deserialize(File.ReadAllText(fileName));
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex, ex.LineNumber, ex.LinePosition), _defaultExceptionType);
                return null;
            }
        }

        /// <summary>
        ///     Sets the object used as the base object in the next stage of deserialization.
        ///     This method enables multi-stage deserialization for YAXLib.
        /// </summary>
        /// <param name="obj">The object used as the base object in the next stage of deserialization.</param>
        public void SetDeserializationBaseObject(object obj)
        {
            if (obj != null && !_type.IsInstanceOfType(obj)) throw new YAXObjectTypeMismatch(_type, obj.GetType());

            _desObject = obj;
        }

        /// <summary>
        ///     Cleans up auxiliary memory used by YAXLib during different sessions of serialization.
        /// </summary>
        public static void CleanUpAuxiliaryMemory()
        {
            TypeWrappersPool.CleanUp();
        }

        /// <summary>
        ///     Serializes the object into an <c>XDocument</c> object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns></returns>
        private XDocument SerializeXDocument(object obj)
        {
            // This method must be called by any public Serialize method
            _isSerializing = true;
            if (_serializedStack == null)
                _serializedStack = new Stack<object>();
            _mainDocument = new XDocument();
            _mainDocument.Add(SerializeBase(obj));
            return _mainDocument;
        }

        /// <summary>
        ///     One of the base methods that perform the whole job of serialization.
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>
        ///     an instance of <c>XElement</c> which contains the result of
        ///     serialization of the specified object
        /// </returns>
        private XElement SerializeBase(object obj)
        {
            if (obj == null)
                return new XElement(_udtWrapper.Alias);

            if (!_type.IsInstanceOfType(obj))
                throw new YAXObjectTypeMismatch(_type, obj.GetType());

            FindDocumentDefaultNamespace();

            // to serialize stand-alone collection or dictionary objects
            if (_udtWrapper.IsTreatedAsDictionary)
            {
                var elemResult = MakeDictionaryElement(null, _udtWrapper.Alias, obj,
                    _udtWrapper.DictionaryAttributeInstance, _udtWrapper.CollectionAttributeInstance,
                    _udtWrapper.IsNotAllowdNullObjectSerialization);
                if (_udtWrapper.PreservesWhitespace)
                    XMLUtils.AddPreserveSpaceAttribute(elemResult);
                if (elemResult.Parent == null)
                    AddNamespacesToElement(elemResult);
                return elemResult;
            }

            if (_udtWrapper.IsTreatedAsCollection)
            {
                var elemResult = MakeCollectionElement(null, _udtWrapper.Alias, obj, null, null);
                if (_udtWrapper.PreservesWhitespace)
                    XMLUtils.AddPreserveSpaceAttribute(elemResult);
                if (elemResult.Parent == null)
                    AddNamespacesToElement(elemResult);
                return elemResult;
            }

            if (ReflectionUtils.IsBasicType(_udtWrapper.UnderlyingType))
            {
                bool dummyAlreadyAdded;
                var elemResult = MakeBaseElement(null, _udtWrapper.Alias, obj, out dummyAlreadyAdded);
                if (_udtWrapper.PreservesWhitespace)
                    XMLUtils.AddPreserveSpaceAttribute(elemResult);
                if (elemResult.Parent == null)
                    AddNamespacesToElement(elemResult);
                return elemResult;
            }

            if (!_udtWrapper.UnderlyingType.EqualsOrIsNullableOf(obj.GetType()))
            {
                // this block of code runs if the serializer is instantiated with a
                // another base value such as System.Object but is provided with an
                // object of its child
                var ser = NewInternalSerializer(obj.GetType(), TypeNamespace, null);
                var xdoc = ser.SerializeToXDocument(obj);
                var elem = xdoc.Root;

                // do not pop from stack because the new internal serializer was sufficient for the whole serialization 
                // and this instance of serializer did not do anything extra
                FinalizeNewSerializer(ser, true, false);
                elem.Name = _udtWrapper.Alias;

                AddMetadataAttribute(elem, _yaxLibNamespaceUri + _trueTypeAttrName, obj.GetType().FullName,
                    _documentDefaultNamespace);
                AddNamespacesToElement(elem);

                return elem;
            }
            else
            {
                // SerializeBase will add the object to the stack
                var elem = SerializeBase(obj, _udtWrapper.Alias);
                if (!_type.IsValueType)
                    _serializedStack.Pop();
                Debug.Assert(_serializedStack.Count == 0,
                    "Serialization stack is not empty at the end of serialization");
                return elem;
            }
        }

        private void PushObjectToSerializationStack(object obj)
        {
            if (!obj.GetType().IsValueType)
                _serializedStack.Push(obj);
        }

        private void FindDocumentDefaultNamespace()
        {
            if (_udtWrapper.HasNamespace && string.IsNullOrEmpty(_udtWrapper.NamespacePrefix))
                // it has a default namespace defined (one without a prefix)
                _documentDefaultNamespace = _udtWrapper.Namespace; // set the default namespace
        }

        /// <summary>
        ///     Sets the base XML element. This method is used when an <c>XMLSerializer</c>
        ///     instantiates another <c>XMLSerializer</c> to serialize nested objects.
        ///     Through this method the child objects have access to the already serialized elements of
        ///     their parent.
        /// </summary>
        /// <param name="baseElement">The base XML element.</param>
        private void SetBaseElement(XElement baseElement)
        {
            _baseElement = baseElement;
        }

        /// <summary>
        ///     The base method that performs the whole job of serialization.
        ///     Other serialization methods call this method to have their job done.
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="className">Name of the element that contains the serialized object.</param>
        /// <returns>
        ///     an instance of <c>XElement</c> which contains the result of
        ///     serialization of the specified object
        /// </returns>
        private XElement SerializeBase(object obj, XName className)
        {
            _isSerializing =
                true; // this is set once again here since internal serializers may not call public Serialize methods

            if (_baseElement == null)
            {
                _baseElement = CreateElementWithNamespace(_udtWrapper, className);
            }
            else
            {
                var baseElem = new XElement(className, null);
                _baseElement.Add(baseElem);
                _baseElement = baseElem;
            }

            if (MaxRecursion == 1)
            {
                PushObjectToSerializationStack(obj);
                return _baseElement;
            }

            if (!_type.IsValueType)
            {
                var alreadySerializedObject = _serializedStack.FirstOrDefault(x => ReferenceEquals(x, obj));
                if (alreadySerializedObject != null)
                {
                    if (!_udtWrapper.ThrowUponSerializingCyclingReferences)
                    {
                        // although we are not going to serialize anything, push the object to be picked up
                        // by the pop statement right after serialization
                        PushObjectToSerializationStack(obj);
                        return _baseElement;
                    }

                    throw new YAXCannotSerializeSelfReferentialTypes(_type);
                }

                PushObjectToSerializationStack(obj);
            }

            if (_udtWrapper.HasComment && _baseElement.Parent == null && _mainDocument != null)
                foreach (var comment in _udtWrapper.Comment)
                    _mainDocument.Add(new XComment(comment));

            // if the containing element is set to preserve spaces, then emit the 
            // required attribute
            if (_udtWrapper.PreservesWhitespace) XMLUtils.AddPreserveSpaceAttribute(_baseElement);

            // check if the main class/type has defined custom serializers
            if (_udtWrapper.HasCustomSerializer)
            {
                InvokeCustomSerializerToElement(_udtWrapper.CustomSerializerType, obj, _baseElement);
            }
            else if (KnownTypes.IsKnowType(_type))
            {
                KnownTypes.Serialize(obj, _baseElement, TypeNamespace);
            }
            else // if it has no custom serializers
            {
                // a flag that indicates whether the object had any fields to be serialized
                // if an object did not have any fields to serialize, then we should not remove
                // the containing element from the resulting xml!
                var isAnythingFoundToSerialize = false;

                // iterate through public properties
                foreach (var member in GetFieldsToBeSerialized())
                {
                    if (member.HasNamespace) RegisterNamespace(member.Namespace, member.NamespacePrefix);

                    if (!member.CanRead)
                        continue;

                    // ignore this member if it is attributed as dont serialize
                    if (member.IsAttributedAsDontSerialize)
                        continue;

                    var elementValue = member.GetValue(obj);

                    // make this flat true, so that we know that this object was not empty of fields
                    isAnythingFoundToSerialize = true;

                    // ignore this member if it is null and we are not about to serialize null objects
                    if (elementValue == null &&
                        _udtWrapper.IsNotAllowdNullObjectSerialization)
                        continue;

                    if (elementValue == null &&
                        member.IsAttributedAsDontSerializeIfNull)
                        continue;

                    var areOfSameType = true; // are element value and the member declared type the same?
                    var originalValue = member.GetOriginalValue(obj, null);
                    if (elementValue != null && !member.MemberType.EqualsOrIsNullableOf(originalValue.GetType()))
                        areOfSameType = false;

                    var hasCustomSerializer =
                        member.HasCustomSerializer || member.MemberTypeWrapper.HasCustomSerializer;
                    var isCollectionSerially = member.CollectionAttributeInstance != null &&
                                               member.CollectionAttributeInstance.SerializationType ==
                                               YAXCollectionSerializationTypes.Serially;
                    var isKnownType = member.IsKnownType;

                    var serializationLocation = member.SerializationLocation;

                    // it gets true only for basic data types
                    if (member.IsSerializedAsAttribute &&
                        (areOfSameType || hasCustomSerializer || isCollectionSerially || isKnownType))
                    {
                        if (!XMLUtils.AttributeExists(_baseElement, serializationLocation,
                            member.Alias.OverrideNsIfEmpty(TypeNamespace)))
                        {
                            var attrToCreate = XMLUtils.CreateAttribute(_baseElement,
                                serializationLocation, member.Alias.OverrideNsIfEmpty(TypeNamespace),
                                hasCustomSerializer || isCollectionSerially || isKnownType
                                    ? string.Empty
                                    : elementValue,
                                _documentDefaultNamespace);

                            RegisterNamespace(member.Alias.OverrideNsIfEmpty(TypeNamespace).Namespace, null);

                            if (attrToCreate == null) throw new YAXBadLocationException(serializationLocation);

                            if (member.HasCustomSerializer)
                            {
                                InvokeCustomSerializerToAttribute(member.CustomSerializerType, elementValue,
                                    attrToCreate);
                            }
                            else if (member.MemberTypeWrapper.HasCustomSerializer)
                            {
                                InvokeCustomSerializerToAttribute(member.MemberTypeWrapper.CustomSerializerType,
                                    elementValue, attrToCreate);
                            }
                            else if (member.IsKnownType)
                            {
                                // TODO: create a functionality to serialize to XAttributes
                                //KnownTypes.Serialize(attrToCreate, member.MemberType);
                            }
                            else if (isCollectionSerially)
                            {
                                var tempLoc = new XElement("temp");
                                var added = MakeCollectionElement(tempLoc, "name", elementValue,
                                    member.CollectionAttributeInstance, member.Format);
                                attrToCreate.Value = added.Value;
                            }

                            // if member does not have any typewrappers then it has been already populated with the CreateAttribute method
                        }
                        else
                        {
                            throw new YAXAttributeAlreadyExistsException(member.Alias.LocalName);
                        }
                    }
                    else if (member.IsSerializedAsValue &&
                             (areOfSameType || hasCustomSerializer || isCollectionSerially || isKnownType))
                    {
                        // find the parent element from its location
                        var parElem = XMLUtils.FindLocation(_baseElement, serializationLocation);
                        if (parElem == null) // if the parent element does not exist
                        {
                            // see if the location can be created
                            if (!XMLUtils.CanCreateLocation(_baseElement, serializationLocation))
                                throw new YAXBadLocationException(serializationLocation);
                            // try to create the location
                            parElem = XMLUtils.CreateLocation(_baseElement, serializationLocation);
                            if (parElem == null)
                                throw new YAXBadLocationException(serializationLocation);
                        }

                        // if control is moved here, it means that the parent 
                        // element has been found/created successfully

                        string valueToSet;
                        if (member.HasCustomSerializer)
                        {
                            valueToSet = InvokeCustomSerializerToValue(member.CustomSerializerType, elementValue);
                        }
                        else if (member.MemberTypeWrapper.HasCustomSerializer)
                        {
                            valueToSet = InvokeCustomSerializerToValue(member.MemberTypeWrapper.CustomSerializerType,
                                elementValue);
                        }
                        else if (isKnownType)
                        {
                            var tempLoc = new XElement("temp");
                            KnownTypes.Serialize(elementValue, tempLoc, string.Empty);
                            valueToSet = tempLoc.Value;
                        }
                        else if (isCollectionSerially)
                        {
                            var tempLoc = new XElement("temp");
                            var added = MakeCollectionElement(tempLoc, "name", elementValue,
                                member.CollectionAttributeInstance, member.Format);
                            valueToSet = added.Value;
                        }
                        else
                        {
                            valueToSet = elementValue.ToXmlValue();
                        }

                        parElem.Add(new XText(valueToSet));
                        if (member.PreservesWhitespace)
                            XMLUtils.AddPreserveSpaceAttribute(parElem);
                    }
                    else // if the data is going to be serialized as an element
                    {
                        // find the parent element from its location
                        var parElem = XMLUtils.FindLocation(_baseElement, serializationLocation);
                        if (parElem == null) // if the parent element does not exist
                        {
                            // see if the location can be created
                            if (!XMLUtils.CanCreateLocation(_baseElement, serializationLocation))
                                throw new YAXBadLocationException(serializationLocation);
                            // try to create the location
                            parElem = XMLUtils.CreateLocation(_baseElement, serializationLocation);
                            if (parElem == null)
                                throw new YAXBadLocationException(serializationLocation);
                        }

                        // if control is moved here, it means that the parent 
                        // element has been found/created successfully

                        if (member.HasComment)
                            foreach (var comment in member.Comment)
                                parElem.Add(new XComment(comment));

                        if (hasCustomSerializer)
                        {
                            var elemToFill = new XElement(member.Alias.OverrideNsIfEmpty(TypeNamespace));
                            parElem.Add(elemToFill);
                            if (member.HasCustomSerializer)
                                InvokeCustomSerializerToElement(member.CustomSerializerType, elementValue, elemToFill);
                            else if (member.MemberTypeWrapper.HasCustomSerializer)
                                InvokeCustomSerializerToElement(member.MemberTypeWrapper.CustomSerializerType,
                                    elementValue, elemToFill);

                            if (member.PreservesWhitespace)
                                XMLUtils.AddPreserveSpaceAttribute(elemToFill);
                        }
                        else if (isKnownType)
                        {
                            var elemToFill = new XElement(member.Alias.OverrideNsIfEmpty(TypeNamespace));
                            parElem.Add(elemToFill);
                            KnownTypes.Serialize(elementValue, elemToFill,
                                member.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThen(XNamespace.None));
                            if (member.PreservesWhitespace)
                                XMLUtils.AddPreserveSpaceAttribute(elemToFill);
                        }
                        else
                        {
                            // make an element with the provided data
                            bool moveDescOnly;
                            bool alreadyAdded;
                            var elemToAdd = MakeElement(parElem, member, elementValue, out moveDescOnly,
                                out alreadyAdded);
                            if (!areOfSameType)
                            {
                                var realType = elementValue.GetType();

                                // TODO: find other usages 
                                var realTypeDefinition = member.GetRealTypeDefinition(realType);
                                if (realTypeDefinition != null)
                                {
                                    var alias = realTypeDefinition.Alias;
                                    if (string.IsNullOrEmpty(alias))
                                    {
                                        var typeWrapper = TypeWrappersPool.Pool.GetTypeWrapper(realType, this);
                                        alias = typeWrapper.Alias.LocalName;
                                    }

                                    // TODO: see how namespace is handled in other parts of the code and do the same thing
                                    elemToAdd.Name = XName.Get(alias, elemToAdd.Name.Namespace.NamespaceName);
                                }
                                else
                                {
                                    AddMetadataAttribute(elemToAdd, _yaxLibNamespaceUri + _trueTypeAttrName,
                                        realType.FullName, _documentDefaultNamespace);
                                }
                            }

                            if (moveDescOnly
                            ) // if only the descendants of the resulting element are going to be added ...
                            {
                                XMLUtils.MoveDescendants(elemToAdd, parElem);
                                if (elemToAdd.Parent == parElem)
                                    elemToAdd.Remove();
                            }
                            else if (!alreadyAdded)
                            {
                                // see if such element already exists
                                var existingElem = parElem.Element(member.Alias.OverrideNsIfEmpty(TypeNamespace));
                                if (existingElem == null)
                                {
                                    // if not add the new element gracefully
                                    parElem.Add(elemToAdd);
                                }
                                else // if an element with our desired name already exists
                                {
                                    if (ReflectionUtils.IsBasicType(member.MemberType))
                                        existingElem.SetValue(elementValue);
                                    else
                                        XMLUtils.MoveDescendants(elemToAdd, existingElem);
                                }
                            }
                        }
                    } // end of if serialize data as Element
                } // end of foreach var member

                // This if statement is important. It checks if all the members of an element
                // have been serialized somewhere else, leaving the containing member empty, then
                // remove that element by itself. However if the element is empty, because the 
                // corresponding object did not have any fields to serialize (e.g., DBNull, Random)
                // then keep that element
                if (_baseElement.Parent != null &&
                    XMLUtils.IsElementCompletelyEmpty(_baseElement) &&
                    isAnythingFoundToSerialize)
                    _baseElement.Remove();
            } // end of else if it has no custom serializers

            if (_baseElement.Parent == null) AddNamespacesToElement(_baseElement);

            return _baseElement;
        }

        /// <summary>
        ///     Adds the namespace applying to the object type specified in <paramref name="wrapper" />
        ///     to the <paramref name="className" />
        /// </summary>
        /// <param name="wrapper">The wrapper around the object who's namespace should be added</param>
        /// <param name="className">The root node of the document to which the namespace should be written</param>
        private XElement CreateElementWithNamespace(UdtWrapper wrapper, XName className)
        {
            var elemName = className.OverrideNsIfEmpty(wrapper.Namespace);
            if (elemName.Namespace == wrapper.Namespace)
                RegisterNamespace(elemName.Namespace, wrapper.NamespacePrefix);
            else
                RegisterNamespace(elemName.Namespace, null);

            return new XElement(elemName, null);
        }

        /// <summary>
        ///     Registers the namespace to be added to the root element of the serialized document.
        /// </summary>
        /// <param name="ns">The namespace to be added</param>
        /// <param name="prefix">The prefix for the namespace.</param>
        private void RegisterNamespace(XNamespace ns, string prefix)
        {
            if (!ns.IsEmpty())
                return;

            if (_namespaceToPrefix.ContainsKey(ns))
            {
                var existingPrefix = _namespaceToPrefix[ns];
                // override the prefix only if already existing namespace has no prefix assigned
                if (string.IsNullOrEmpty(existingPrefix))
                    _namespaceToPrefix[ns] = prefix;
            }
            else
            {
                _namespaceToPrefix.Add(ns, prefix);
            }
        }

        private void ImportNamespaces(YAXSerializer otherSerializer)
        {
            foreach (var pair in otherSerializer._namespaceToPrefix) RegisterNamespace(pair.Key, pair.Value);
        }

        private void AddNamespacesToElement(XElement rootNode)
        {
            var nsNoPrefix = new List<XNamespace>();
            foreach (var ns in _namespaceToPrefix.Keys)
            {
                var prefix = _namespaceToPrefix[ns];
                if (string.IsNullOrEmpty(prefix))
                {
                    nsNoPrefix.Add(ns);
                }
                else // if it has a prefix assigned
                {
                    // if no namespace with this prefix already exists
                    if (rootNode.GetNamespaceOfPrefix(prefix) == null)
                    {
                        rootNode.AddAttributeNamespaceSafe(XNamespace.Xmlns + prefix, ns, _documentDefaultNamespace);
                    }
                    else // if this prefix is already added
                    {
                        // check the namespace associated with this prefix
                        var existing = rootNode.GetNamespaceOfPrefix(prefix);
                        if (existing != ns)
                            throw new InvalidOperationException(string.Format(
                                "You cannot have two different namespaces with the same prefix." +
                                Environment.NewLine +
                                "Prefix: {0}, Namespaces: \"{1}\", and \"{2}\"",
                                prefix, ns, existing));
                    }
                }
            }

            // if the main type wrapper has a default (no prefix) namespace
            if (_udtWrapper.Namespace.IsEmpty() && string.IsNullOrEmpty(_udtWrapper.NamespacePrefix))
                // it will be added automatically
                nsNoPrefix.Remove(_udtWrapper.Namespace);

            // now generate namespaces for those without prefix
            foreach (var ns in nsNoPrefix)
                rootNode.AddAttributeNamespaceSafe(XNamespace.Xmlns + rootNode.GetRandomPrefix(), ns,
                    _documentDefaultNamespace);
        }

        /// <summary>
        ///     Makes the element corresponding to the member specified.
        /// </summary>
        /// <param name="insertionLocation">The insertion location.</param>
        /// <param name="member">The member to serialize.</param>
        /// <param name="elementValue">The element value.</param>
        /// <param name="moveDescOnly">
        ///     if set to <c>true</c> specifies that only the descendants of the resulting element should be
        ///     added to the parent.
        /// </param>
        /// <param name="alreadyAdded">
        ///     if set to <c>true</c> specifies the element returned is
        ///     already added to the parent element and should not be added once more.
        /// </param>
        /// <returns></returns>
        private XElement MakeElement(XElement insertionLocation, MemberWrapper member, object elementValue,
            out bool moveDescOnly, out bool alreadyAdded)
        {
            moveDescOnly = false;

            RegisterNamespace(member.Namespace, member.NamespacePrefix);

            XElement elemToAdd;
            if (member.IsTreatedAsDictionary)
            {
                elemToAdd = MakeDictionaryElement(insertionLocation, member.Alias.OverrideNsIfEmpty(TypeNamespace),
                    elementValue, member.DictionaryAttributeInstance, member.CollectionAttributeInstance,
                    member.IsAttributedAsDontSerializeIfNull);
                if (member.CollectionAttributeInstance != null &&
                    member.CollectionAttributeInstance.SerializationType ==
                    YAXCollectionSerializationTypes.RecursiveWithNoContainingElement &&
                    !elemToAdd.HasAttributes)
                    moveDescOnly = true;
                alreadyAdded = elemToAdd.Parent == insertionLocation;
            }
            else if (member.IsTreatedAsCollection)
            {
                elemToAdd = MakeCollectionElement(insertionLocation, member.Alias.OverrideNsIfEmpty(TypeNamespace),
                    elementValue, member.CollectionAttributeInstance, member.Format);

                if (member.CollectionAttributeInstance != null &&
                    member.CollectionAttributeInstance.SerializationType ==
                    YAXCollectionSerializationTypes.RecursiveWithNoContainingElement &&
                    !elemToAdd.HasAttributes)
                    moveDescOnly = true;
                alreadyAdded = elemToAdd.Parent == insertionLocation;
            }
            else
            {
                elemToAdd = MakeBaseElement(insertionLocation, member.Alias.OverrideNsIfEmpty(TypeNamespace),
                    elementValue, out alreadyAdded);
            }

            if (member.PreservesWhitespace)
                XMLUtils.AddPreserveSpaceAttribute(elemToAdd);

            return elemToAdd;
        }

        /// <summary>
        ///     Creates a dictionary element according to the specified options, as described
        ///     by the attribute instances.
        /// </summary>
        /// <param name="insertionLocation">The insertion location.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="elementValue">The element value, corresponding to a dictionary object.</param>
        /// <param name="dicAttrInst">reference to the dictionary attribute instance.</param>
        /// <param name="collectionAttrInst">reference to collection attribute instance.</param>
        /// <param name="dontSerializeNull">Don't serialize <c>null</c> values.</param>
        /// <returns>
        ///     an instance of <c>XElement</c> which contains the dictionary object
        ///     serialized properly
        /// </returns>
        private XElement MakeDictionaryElement(XElement insertionLocation, XName elementName, object elementValue,
            YAXDictionaryAttribute dicAttrInst, YAXCollectionAttribute collectionAttrInst, bool dontSerializeNull)
        {
            if (elementValue == null) return new XElement(elementName);

            Type keyType, valueType;
            if (!ReflectionUtils.IsIDictionary(elementValue.GetType(), out keyType, out valueType))
                throw new ArgumentException("elementValue must be a Dictionary");

            // serialize other non-collection members
            var ser = NewInternalSerializer(elementValue.GetType(), elementName.Namespace, insertionLocation);
            var elem = ser.SerializeBase(elementValue, elementName);
            FinalizeNewSerializer(ser, true);

            // now iterate through collection members

            var dicInst = elementValue as IEnumerable;
            var isKeyAttrib = false;
            var isValueAttrib = false;
            var isKeyContent = false;
            var isValueContent = false;
            string keyFormat = null;
            string valueFormat = null;
            var keyAlias = elementName.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone() + "Key";
            var valueAlias = elementName.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone() + "Value";

            XName eachElementName = null;
            if (collectionAttrInst != null && !string.IsNullOrEmpty(collectionAttrInst.EachElementName))
            {
                eachElementName = StringUtils.RefineSingleElement(collectionAttrInst.EachElementName);
                if (eachElementName.Namespace.IsEmpty())
                    RegisterNamespace(eachElementName.Namespace, null);
                eachElementName =
                    eachElementName.OverrideNsIfEmpty(
                        elementName.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone());
            }

            if (dicAttrInst != null)
            {
                if (dicAttrInst.EachPairName != null)
                {
                    eachElementName = StringUtils.RefineSingleElement(dicAttrInst.EachPairName);
                    if (eachElementName.Namespace.IsEmpty())
                        RegisterNamespace(eachElementName.Namespace, null);
                    eachElementName =
                        eachElementName.OverrideNsIfEmpty(elementName.Namespace.IfEmptyThen(TypeNamespace)
                            .IfEmptyThenNone());
                }

                if (dicAttrInst.SerializeKeyAs == YAXNodeTypes.Attribute)
                    isKeyAttrib = ReflectionUtils.IsBasicType(keyType);
                else if (dicAttrInst.SerializeKeyAs == YAXNodeTypes.Content)
                    isKeyContent = ReflectionUtils.IsBasicType(keyType);

                if (dicAttrInst.SerializeValueAs == YAXNodeTypes.Attribute)
                    isValueAttrib = ReflectionUtils.IsBasicType(valueType);
                else if (dicAttrInst.SerializeValueAs == YAXNodeTypes.Content)
                    isValueContent = ReflectionUtils.IsBasicType(valueType);

                keyFormat = dicAttrInst.KeyFormatString;
                valueFormat = dicAttrInst.ValueFormatString;

                keyAlias = StringUtils.RefineSingleElement(dicAttrInst.KeyName ?? "Key");
                if (keyAlias.Namespace.IsEmpty())
                    RegisterNamespace(keyAlias.Namespace, null);
                keyAlias = keyAlias.OverrideNsIfEmpty(
                    elementName.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone());

                valueAlias = StringUtils.RefineSingleElement(dicAttrInst.ValueName ?? "Value");
                if (valueAlias.Namespace.IsEmpty())
                    RegisterNamespace(valueAlias.Namespace, null);
                valueAlias =
                    valueAlias.OverrideNsIfEmpty(elementName.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone());
            }

            foreach (var obj in dicInst)
            {
                var keyObj = obj.GetType().GetProperty("Key").GetValue(obj, null);
                var valueObj = obj.GetType().GetProperty("Value").GetValue(obj, null);

                var areKeyOfSameType = true;
                var areValueOfSameType = true;

                if (keyObj != null && !keyObj.GetType().EqualsOrIsNullableOf(keyType))
                    areKeyOfSameType = false;

                if (valueObj != null && !valueObj.GetType().EqualsOrIsNullableOf(valueType))
                    areValueOfSameType = false;

                if (keyFormat != null) keyObj = ReflectionUtils.TryFormatObject(keyObj, keyFormat);

                if (valueFormat != null) valueObj = ReflectionUtils.TryFormatObject(valueObj, valueFormat);

                if (eachElementName == null)
                {
                    eachElementName =
                        StringUtils.RefineSingleElement(ReflectionUtils.GetTypeFriendlyName(obj.GetType()));
                    eachElementName =
                        eachElementName.OverrideNsIfEmpty(elementName.Namespace.IfEmptyThen(TypeNamespace)
                            .IfEmptyThenNone());
                }

                var elemChild = new XElement(eachElementName, null);

                if (isKeyAttrib && areKeyOfSameType)
                {
                    elemChild.AddAttributeNamespaceSafe(keyAlias, keyObj, _documentDefaultNamespace);
                }
                else if (isKeyContent && areKeyOfSameType)
                {
                    elemChild.AddXmlContent(keyObj);
                }
                else
                {
                    var addedElem = AddObjectToElement(elemChild, keyAlias, keyObj);
                    if (!areKeyOfSameType)
                    {
                        if (addedElem.Parent == null)
                            // sometimes empty elements are removed because its members are serialized in
                            // other elements, therefore we need to make sure to re-add the element.
                            elemChild.Add(addedElem);

                        AddMetadataAttribute(addedElem, _yaxLibNamespaceUri + _trueTypeAttrName,
                            keyObj.GetType().FullName, _documentDefaultNamespace);
                    }
                }

                if (isValueAttrib && areValueOfSameType)
                {
                    elemChild.AddAttributeNamespaceSafe(valueAlias, valueObj, _documentDefaultNamespace);
                }
                else if (isValueContent && areValueOfSameType)
                {
                    elemChild.AddXmlContent(valueObj);
                }
                else if (!(valueObj == null && dontSerializeNull))
                {
                    var addedElem = AddObjectToElement(elemChild, valueAlias, valueObj);
                    if (!areValueOfSameType)
                    {
                        if (addedElem.Parent == null)
                            // sometimes empty elements are removed because its members are serialized in
                            // other elements, therefore we need to make sure to re-add the element.
                            elemChild.Add(addedElem);

                        AddMetadataAttribute(addedElem, _yaxLibNamespaceUri + _trueTypeAttrName,
                            valueObj.GetType().FullName, _documentDefaultNamespace);
                    }
                }

                elem.Add(elemChild);
            }

            return elem;
        }

        /// <summary>
        ///     Adds an element contatining data related to the specified object, to an existing xml element.
        /// </summary>
        /// <param name="elem">The parent element.</param>
        /// <param name="alias">The name for the element to be added.</param>
        /// <param name="obj">
        ///     The object corresponding to which an element is going to be added to
        ///     an existing parent element.
        /// </param>
        /// <returns>the enclosing XML element.</returns>
        private XElement AddObjectToElement(XElement elem, XName alias, object obj)
        {
            UdtWrapper udt = null;
            if (obj != null)
                udt = TypeWrappersPool.Pool.GetTypeWrapper(obj.GetType(), this);

            if (alias == null && udt != null)
                alias = udt.Alias.OverrideNsIfEmpty(TypeNamespace);

            XElement elemToAdd = null;

            if (udt != null && udt.IsTreatedAsDictionary)
            {
                elemToAdd = MakeDictionaryElement(elem, alias, obj, null, null, udt.IsNotAllowdNullObjectSerialization);
                if (elemToAdd.Parent != elem)
                    elem.Add(elemToAdd);
            }
            else if (udt != null && udt.IsTreatedAsCollection)
            {
                elemToAdd = MakeCollectionElement(elem, alias, obj, null, null);
                if (elemToAdd.Parent != elem)
                    elem.Add(elemToAdd);
            }
            else if (udt != null && udt.IsEnum)
            {
                bool alreadyAdded;
                elemToAdd = MakeBaseElement(elem, alias, udt.EnumWrapper.GetAlias(obj), out alreadyAdded);
                if (!alreadyAdded)
                    elem.Add(elemToAdd);
            }
            else
            {
                bool alreadyAdded;
                elemToAdd = MakeBaseElement(elem, alias, obj, out alreadyAdded);
                if (!alreadyAdded)
                    elem.Add(elemToAdd);
            }

            return elemToAdd;
        }

        /// <summary>
        ///     Serializes a collection object.
        /// </summary>
        /// <param name="insertionLocation">The insertion location.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="elementValue">The object to be serailized.</param>
        /// <param name="collectionAttrInst">The collection attribute instance.</param>
        /// <param name="format">formatting string, which is going to be applied to all members of the collection.</param>
        /// <returns>
        ///     an instance of <c>XElement</c> which will contain the serailized collection
        /// </returns>
        private XElement MakeCollectionElement(
            XElement insertionLocation, XName elementName, object elementValue,
            YAXCollectionAttribute collectionAttrInst, string format)
        {
            if (elementValue == null)
                return new XElement(elementName);

            if (!(elementValue is IEnumerable))
                throw new ArgumentException("elementValue must be an IEnumerable");

            // serialize other non-collection members
            var ser = NewInternalSerializer(elementValue.GetType(), elementName.Namespace, insertionLocation);
            var elemToAdd = ser.SerializeBase(elementValue, elementName);
            FinalizeNewSerializer(ser, true);

            // now iterate through collection members

            var collectionInst = elementValue as IEnumerable;
            var serType = YAXCollectionSerializationTypes.Recursive;
            var seperator = string.Empty;
            XName eachElementName = null;

            if (collectionAttrInst != null)
            {
                serType = collectionAttrInst.SerializationType;
                seperator = collectionAttrInst.SeparateBy;
                if (collectionAttrInst.EachElementName != null)
                {
                    eachElementName = StringUtils.RefineSingleElement(collectionAttrInst.EachElementName);
                    if (eachElementName.Namespace.IsEmpty())
                        RegisterNamespace(eachElementName.Namespace, null);
                    eachElementName =
                        eachElementName.OverrideNsIfEmpty(elementName.Namespace.IfEmptyThen(TypeNamespace)
                            .IfEmptyThenNone());
                }
            }

            var colItemType = ReflectionUtils.GetCollectionItemType(elementValue.GetType());
            var colItemsUdt = TypeWrappersPool.Pool.GetTypeWrapper(colItemType, this);

            if (serType == YAXCollectionSerializationTypes.Serially && !ReflectionUtils.IsBasicType(colItemType))
                serType = YAXCollectionSerializationTypes.Recursive;

            if (serType == YAXCollectionSerializationTypes.Serially && elemToAdd.IsEmpty)
            {
                var sb = new StringBuilder();

                var isFirst = true;
                object objToAdd = null;
                foreach (var obj in collectionInst)
                {
                    if (colItemsUdt.IsEnum)
                        objToAdd = colItemsUdt.EnumWrapper.GetAlias(obj);
                    else if (format != null)
                        objToAdd = ReflectionUtils.TryFormatObject(obj, format);
                    else
                        objToAdd = obj;

                    if (isFirst)
                    {
                        sb.Append(objToAdd.ToXmlValue());
                        isFirst = false;
                    }
                    else
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", seperator, objToAdd);
                    }
                }

                var alreadyAdded = false;
                elemToAdd = MakeBaseElement(insertionLocation, elementName, sb.ToString(), out alreadyAdded);
                if (alreadyAdded)
                    elemToAdd = null;
            }
            else
            {
                //var elem = new XElement(elementName, null);
                object objToAdd = null;

                foreach (var obj in collectionInst)
                {
                    objToAdd = format == null ? obj : ReflectionUtils.TryFormatObject(obj, format);
                    var curElemName = eachElementName;

                    if (curElemName == null) curElemName = colItemsUdt.Alias;

                    var itemElem = AddObjectToElement(elemToAdd, curElemName.OverrideNsIfEmpty(elementName.Namespace),
                        objToAdd);
                    if (obj != null && !obj.GetType().EqualsOrIsNullableOf(colItemType))
                    {
                        if (itemElem.Parent == null
                        ) // i.e., it has been removed, e.g., because all its members have been serialized outside the element
                            elemToAdd.Add(itemElem); // return it back, or undelete this item

                        AddMetadataAttribute(itemElem, _yaxLibNamespaceUri + _trueTypeAttrName,
                            obj.GetType().FullName, _documentDefaultNamespace);
                    }
                }
            }

            var arrayDims = ReflectionUtils.GetArrayDimensions(elementValue);
            if (arrayDims != null && arrayDims.Length > 1)
                AddMetadataAttribute(elemToAdd, _yaxLibNamespaceUri + _dimsAttrName,
                    StringUtils.GetArrayDimsString(arrayDims), _documentDefaultNamespace);

            return elemToAdd;
        }

        /// <summary>
        ///     Makes an XML element with the specified name, corresponding to the object specified.
        /// </summary>
        /// <param name="insertionLocation">The insertion location.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="value">The object to be serialized in an XML element.</param>
        /// <param name="alreadyAdded">
        ///     if set to <c>true</c> specifies the element returned is
        ///     already added to the parent element and should not be added once more.
        /// </param>
        /// <returns>
        ///     an instance of <c>XElement</c> which will contain the serialized object,
        ///     or <c>null</c> if the serialized object is already added to the base element
        /// </returns>
        private XElement MakeBaseElement(XElement insertionLocation, XName name, object value, out bool alreadyAdded)
        {
            alreadyAdded = false;
            if (value == null || ReflectionUtils.IsBasicType(value.GetType()))
            {
                if (value != null)
                    value = value.ToXmlValue();

                return new XElement(name, value);
            }

            if (ReflectionUtils.IsStringConvertibleIFormattable(value.GetType()))
            {
                var elementValue = value.GetType().InvokeMethod("ToString", value, new object[0]);
                //object elementValue = value.GetType().InvokeMember("ToString", BindingFlags.InvokeMethod, null, value, new object[0]);
                return new XElement(name, elementValue);
            }

            var ser = NewInternalSerializer(value.GetType(), name.Namespace, insertionLocation);
            var elem = ser.SerializeBase(value, name);
            FinalizeNewSerializer(ser, true);
            alreadyAdded = true;
            return elem;
        }

        /// <summary>
        ///     The basic method which performs the whole job of deserialization.
        /// </summary>
        /// <param name="baseElement">The element to be deserialized.</param>
        /// <returns>object containing the deserialized data</returns>
        private object DeserializeBase(XElement baseElement)
        {
            _isSerializing = false;

            if (baseElement == null) return _desObject;

            if (_udtWrapper.HasCustomSerializer)
                return InvokeCustomDeserializerFromElement(_udtWrapper.CustomSerializerType, baseElement);

            var realTypeAttr = baseElement.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                _documentDefaultNamespace);
            if (realTypeAttr != null)
            {
                var theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
                if (theRealType != null)
                {
                    _type = theRealType;
                    _udtWrapper = TypeWrappersPool.Pool.GetTypeWrapper(_type, this);
                }
            }

            if (_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                return DeserializeKeyValuePair(baseElement);

            if (KnownTypes.IsKnowType(_type)) return KnownTypes.Deserialize(baseElement, _type, TypeNamespace);

            if ((_udtWrapper.IsTreatedAsCollection || _udtWrapper.IsTreatedAsDictionary) &&
                !IsCraetedToDeserializeANonCollectionMember)
            {
                if (_udtWrapper.DictionaryAttributeInstance != null)
                    return DeserializeTaggedDictionaryValue(baseElement, _udtWrapper.Alias, _type,
                        _udtWrapper.CollectionAttributeInstance, _udtWrapper.DictionaryAttributeInstance);
                return DeserializeCollectionValue(_type, baseElement, _udtWrapper.Alias,
                    _udtWrapper.CollectionAttributeInstance);
            }

            if (ReflectionUtils.IsBasicType(_type)) return ReflectionUtils.ConvertBasicType(baseElement.Value, _type);

            object o;
            o = _desObject ?? Activator.CreateInstance(_type, new object[0]);
            // o = m_desObject ?? m_type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);

            foreach (var member in GetFieldsToBeSerialized())
            {
                if (!member.CanWrite)
                    continue;

                if (member.IsAttributedAsDontSerialize)
                    continue;

                // reset handled exceptions status
                _exceptionOccurredDuringMemberDeserialization = false;

                var elemValue = string.Empty; // the element value gathered at the first phase
                XElement xelemValue = null; // the XElement instance gathered at the first phase
                XAttribute xattrValue = null; // the XAttribute instance gathered at the first phase

                // first evaluate elemValue
                var createdFakeElement = false;

                var serializationLocation = member.SerializationLocation;

                if (member.IsSerializedAsAttribute)
                {
                    // find the parent element from its location
                    var attr = XMLUtils.FindAttribute(baseElement, serializationLocation,
                        member.Alias.OverrideNsIfEmpty(TypeNamespace));
                    if (attr == null) // if the parent element does not exist
                    {
                        // loook for an element with the same name AND a yaxlib:realtype attribute
                        var elem = XMLUtils.FindElement(baseElement, serializationLocation,
                            member.Alias.OverrideNsIfEmpty(TypeNamespace));
                        if (elem != null && elem.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                            _documentDefaultNamespace) != null)
                        {
                            elemValue = elem.Value;
                            xelemValue = elem;
                        }
                        else
                        {
                            OnExceptionOccurred(new YAXAttributeMissingException(
                                    StringUtils.CombineLocationAndElementName(serializationLocation, member.Alias),
                                    elem ?? baseElement),
                                !member.MemberType.IsValueType && _udtWrapper.IsNotAllowdNullObjectSerialization
                                    ? YAXExceptionTypes.Ignore
                                    : member.TreatErrorsAs);
                        }
                    }
                    else
                    {
                        elemValue = attr.Value;
                        xattrValue = attr;
                    }
                }
                else if (member.IsSerializedAsValue)
                {
                    var elem = XMLUtils.FindLocation(baseElement, serializationLocation);
                    if (elem == null) // such element is not found
                    {
                        OnExceptionOccurred(new YAXElementMissingException(
                                serializationLocation, baseElement),
                            !member.MemberType.IsValueType && _udtWrapper.IsNotAllowdNullObjectSerialization
                                ? YAXExceptionTypes.Ignore
                                : member.TreatErrorsAs);
                    }
                    else
                    {
                        var values = elem.Nodes().OfType<XText>().ToArray();
                        if (values.Length <= 0)
                        {
                            // loook for an element with the same name AND a yaxlib:realtype attribute
                            var innerelem = XMLUtils.FindElement(baseElement, serializationLocation,
                                member.Alias.OverrideNsIfEmpty(TypeNamespace));
                            if (innerelem != null &&
                                innerelem.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                                    _documentDefaultNamespace) != null)
                            {
                                elemValue = innerelem.Value;
                                xelemValue = innerelem;
                            }
                            else
                            {
                                OnExceptionOccurred(
                                    new YAXElementValueMissingException(serializationLocation,
                                        innerelem ?? baseElement),
                                    !member.MemberType.IsValueType && _udtWrapper.IsNotAllowdNullObjectSerialization
                                        ? YAXExceptionTypes.Ignore
                                        : member.TreatErrorsAs);
                            }
                        }
                        else
                        {
                            elemValue = values[0].Value;
                            values[0].Remove();
                        }
                    }
                }
                else // if member is serialized as an xml element
                {
                    var canContinue = false;
                    var elem = XMLUtils.FindElement(baseElement, serializationLocation,
                        member.Alias.OverrideNsIfEmpty(TypeNamespace));
                    if (elem == null) // such element is not found
                    {
                        if ((member.IsTreatedAsCollection || member.IsTreatedAsDictionary) &&
                            member.CollectionAttributeInstance != null &&
                            member.CollectionAttributeInstance.SerializationType ==
                            YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)
                        {
                            if (AtLeastOneOfCollectionMembersExists(baseElement, member))
                            {
                                elem = baseElement;
                                canContinue = true;
                            }
                            else
                            {
                                member.SetValue(o, member.DefaultValue);
                                continue;
                            }
                        }
                        else if (!ReflectionUtils.IsBasicType(member.MemberType) && !member.IsTreatedAsCollection &&
                                 !member.IsTreatedAsDictionary)
                        {
                            // try to fix this problem by creating a fake element, maybe all its children are placed somewhere else
                            var fakeElem = XMLUtils.CreateElement(baseElement, serializationLocation,
                                member.Alias.OverrideNsIfEmpty(TypeNamespace));
                            if (fakeElem != null)
                            {
                                createdFakeElement = true;
                                if (AtLeastOneOfMembersExists(fakeElem, member.MemberType))
                                {
                                    canContinue = true;
                                    elem = fakeElem;
                                    elemValue = elem.Value;
                                }
                            }
                        }

                        if (!canContinue)
                            OnExceptionOccurred(new YAXElementMissingException(
                                    StringUtils.CombineLocationAndElementName(serializationLocation,
                                        member.Alias.OverrideNsIfEmpty(TypeNamespace)), baseElement),
                                !member.MemberType.IsValueType && _udtWrapper.IsNotAllowdNullObjectSerialization
                                    ? YAXExceptionTypes.Ignore
                                    : member.TreatErrorsAs);
                    }
                    else
                    {
                        elemValue = elem.Value;
                    }

                    xelemValue = elem;
                }

                // Phase2: Now try to retrieve elemValue's value, based on values gathered in xelemValue, xattrValue, and elemValue
                if (_exceptionOccurredDuringMemberDeserialization)
                {
                    if (_desObject == null
                    ) // i.e. if it was NOT resuming deserialization, set default value, otherwise existing value for the member is kept
                    {
                        if (!member.MemberType.IsValueType && _udtWrapper.IsNotAllowdNullObjectSerialization)
                        {
                            try
                            {
                                member.SetValue(o, null);
                            }
                            catch
                            {
                                OnExceptionOccurred(
                                    new YAXDefaultValueCannotBeAssigned(member.Alias.LocalName, member.DefaultValue,
                                        xattrValue ?? xelemValue ?? baseElement as IXmlLineInfo),
                                    _defaultExceptionType);
                            }
                        }
                        else if (member.DefaultValue != null)
                        {
                            try
                            {
                                member.SetValue(o, member.DefaultValue);
                            }
                            catch
                            {
                                OnExceptionOccurred(
                                    new YAXDefaultValueCannotBeAssigned(member.Alias.LocalName, member.DefaultValue,
                                        xattrValue ?? xelemValue ?? baseElement as IXmlLineInfo),
                                    _defaultExceptionType);
                            }
                        }
                        else
                        {
                            if (!member.MemberType.IsValueType)
                                member.SetValue(o, null /*the value to be assigned */);
                        }
                    }
                }
                else if (member.HasCustomSerializer || member.MemberTypeWrapper.HasCustomSerializer)
                {
                    var deserType = member.HasCustomSerializer
                        ? member.CustomSerializerType
                        : member.MemberTypeWrapper.CustomSerializerType;

                    object desObj;
                    if (member.IsSerializedAsAttribute)
                        desObj = InvokeCustomDeserializerFromAttribute(deserType, xattrValue);
                    else if (member.IsSerializedAsElement)
                        desObj = InvokeCustomDeserializerFromElement(deserType, xelemValue);
                    else if (member.IsSerializedAsValue)
                        desObj = InvokeCustomDeserializerFromValue(deserType, elemValue);
                    else
                        throw new Exception("unknown situation");

                    try
                    {
                        member.SetValue(o, desObj);
                    }
                    catch
                    {
                        OnExceptionOccurred(
                            new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName,
                                xattrValue ?? xelemValue ?? baseElement as IXmlLineInfo), _defaultExceptionType);
                    }
                }
                else if (elemValue != null)
                {
                    RetreiveElementValue(o, member, elemValue, xelemValue);
                }

                if (createdFakeElement && xelemValue != null)
                    // remove the fake element
                    xelemValue.Remove();

                if (RemoveDeserializedXmlNodes)
                {
                    if (xattrValue != null) xattrValue.Remove();
                    else if (xelemValue != null) xelemValue.Remove();
                }
            }


            return o;
        }

        /// <summary>
        ///     Checks whether at least one of the collection members of
        ///     the specified collection exists.
        /// </summary>
        /// <param name="elem">The XML element to check its content.</param>
        /// <param name="member">
        ///     The class-member corresponding to the collection for
        ///     which we intend to check existence of its members.
        /// </param>
        /// <returns></returns>
        private bool AtLeastOneOfCollectionMembersExists(XElement elem, MemberWrapper member)
        {
            if (!((member.IsTreatedAsCollection || member.IsTreatedAsDictionary) &&
                  member.CollectionAttributeInstance != null &&
                  member.CollectionAttributeInstance.SerializationType ==
                  YAXCollectionSerializationTypes.RecursiveWithNoContainingElement))
                throw new ArgumentException("member should be a collection serialized without containing element");

            XName eachElementName = null;

            if (member.CollectionAttributeInstance != null)
                eachElementName = StringUtils.RefineSingleElement(member.CollectionAttributeInstance.EachElementName);

            if (member.DictionaryAttributeInstance != null && member.DictionaryAttributeInstance.EachPairName != null)
                eachElementName = StringUtils.RefineSingleElement(member.DictionaryAttributeInstance.EachPairName);

            if (eachElementName == null)
            {
                var colItemType = ReflectionUtils.GetCollectionItemType(member.MemberType);
                eachElementName = StringUtils.RefineSingleElement(ReflectionUtils.GetTypeFriendlyName(colItemType));
            }

            // return if such an element exists
            return elem.Element(
                       eachElementName.OverrideNsIfEmpty(member.Namespace.IfEmptyThen(TypeNamespace)
                           .IfEmptyThenNone())) !=
                   null;
        }

        /// <summary>
        ///     Checks whether at least one of the members (property or field) of
        ///     the specified object exists.
        /// </summary>
        /// <param name="elem">The XML element to check its content.</param>
        /// <param name="type">
        ///     The class-member corresponding to the object for
        ///     which we intend to check existence of its members.
        /// </param>
        /// <returns></returns>
        private bool AtLeastOneOfMembersExists(XElement elem, Type type)
        {
            if (elem == null)
                throw new ArgumentNullException("elem");

            var typeWrapper = TypeWrappersPool.Pool.GetTypeWrapper(type, this);

            foreach (var member in GetFieldsToBeSerialized(typeWrapper))
            {
                if (!member.CanWrite)
                    continue;

                // ignore this member if it is attributed as dont serialize
                if (member.IsAttributedAsDontSerialize)
                    continue;

                if (member.IsSerializedAsAttribute)
                {
                    // find the parent element from its location
                    var attr = XMLUtils.FindAttribute(elem, member.SerializationLocation, member.Alias);
                    if (attr == null)
                    {
                        // maybe it has got a realtype attribute and hence have turned into an element
                        var theElem = XMLUtils.FindElement(elem, member.SerializationLocation, member.Alias);
                        if (theElem != null &&
                            theElem.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                                _documentDefaultNamespace) != null)
                            return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    var xelem = XMLUtils.FindElement(elem, member.SerializationLocation, member.Alias);
                    if (xelem == null)
                    {
                        if (!ReflectionUtils.IsBasicType(member.MemberType)
                            && !member.IsTreatedAsCollection
                            && !member.IsTreatedAsDictionary
                            && member.MemberType != _type
                        ) // searching for same type objects will lead to infinite loops
                        {
                            // try to create a fake element 
                            var fakeElem = XMLUtils.CreateElement(elem, member.SerializationLocation, member.Alias);
                            if (fakeElem != null)
                            {
                                var memberExists = AtLeastOneOfMembersExists(fakeElem, member.MemberType);
                                fakeElem.Remove();
                                if (memberExists)
                                    return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Retreives the value of the element from the specified XML element or attribute.
        /// </summary>
        /// <param name="o">The object to store the retrieved value at.</param>
        /// <param name="member">The member of the specified object whose value we intent to retreive.</param>
        /// <param name="elemValue">The value of the element stored as string.</param>
        /// <param name="xelemValue">
        ///     The XML element value to be retrieved. If the value to be retrieved
        ///     has been stored in an XML attribute, this reference is <c>null</c>.
        /// </param>
        private void RetreiveElementValue(object o, MemberWrapper member, string elemValue, XElement xelemValue)
        {
            var memberType = member.MemberType;

            // when serializing collection with no containing element, then the real type attribute applies to the class
            // containing the collection, not the collection itself. That's because the containing element of collection is not 
            // serialized. In this case the flag `isRealTypeAttributeNotRelevant` is set to true.
            var isRealTypeAttributeNotRelevant = member.CollectionAttributeInstance != null
                                                 && member.CollectionAttributeInstance.SerializationType ==
                                                 YAXCollectionSerializationTypes.RecursiveWithNoContainingElement;

            // try to retrieve the real-type if specified
            if (xelemValue != null && !isRealTypeAttributeNotRelevant)
            {
                var realTypeAttribute = xelemValue.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                    _documentDefaultNamespace);
                if (realTypeAttribute != null)
                {
                    var realType = ReflectionUtils.GetTypeByName(realTypeAttribute.Value);
                    if (realType != null) memberType = realType;
                }
            }

            if (xelemValue != null && XMLUtils.IsElementCompletelyEmpty(xelemValue) &&
                !ReflectionUtils.IsBasicType(memberType) && !member.IsTreatedAsCollection &&
                !member.IsTreatedAsDictionary &&
                !AtLeastOneOfMembersExists(xelemValue, memberType))
            {
                try
                {
                    member.SetValue(o, member.DefaultValue);
                }
                catch
                {
                    OnExceptionOccurred(
                        new YAXDefaultValueCannotBeAssigned(member.Alias.LocalName, member.DefaultValue, xelemValue),
                        member.TreatErrorsAs);
                }
            }
            else if (memberType == typeof(string))
            {
                if (string.IsNullOrEmpty(elemValue) && xelemValue != null)
                    elemValue = xelemValue.IsEmpty ? null : string.Empty;

                try
                {
                    member.SetValue(o, elemValue);
                }
                catch
                {
                    OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xelemValue),
                        _defaultExceptionType);
                }
            }
            else if (ReflectionUtils.IsBasicType(memberType))
            {
                object convertedObj;

                try
                {
                    if (ReflectionUtils.IsNullable(memberType) && string.IsNullOrEmpty(elemValue))
                        convertedObj = member.DefaultValue;
                    else
                        convertedObj = ReflectionUtils.ConvertBasicType(elemValue, memberType);

                    try
                    {
                        member.SetValue(o, convertedObj);
                    }
                    catch
                    {
                        OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xelemValue),
                            _defaultExceptionType);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is YAXException) throw;

                    OnExceptionOccurred(new YAXBadlyFormedInput(member.Alias.LocalName, elemValue, xelemValue),
                        member.TreatErrorsAs);

                    try
                    {
                        member.SetValue(o, member.DefaultValue);
                    }
                    catch
                    {
                        OnExceptionOccurred(
                            new YAXDefaultValueCannotBeAssigned(member.Alias.LocalName, member.DefaultValue,
                                xelemValue), _defaultExceptionType);
                    }
                }
            }
            else if (member.IsTreatedAsDictionary && member.DictionaryAttributeInstance != null)
            {
                DeserializeTaggedDictionaryMember(o, member, xelemValue);
            }
            else if (member.IsTreatedAsCollection)
            {
                DeserializeCollectionMember(o, member, memberType, elemValue, xelemValue);
            }
            else
            {
                var namespaceToOverride = member.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone();
                var ser = NewInternalSerializer(memberType, namespaceToOverride, null);

                ser.IsCraetedToDeserializeANonCollectionMember =
                    !(member.IsTreatedAsDictionary || member.IsTreatedAsCollection);

                if (_desObject != null) // i.e. it is in resuming mode
                    ser.SetDeserializationBaseObject(member.GetValue(o));

                var convertedObj = ser.DeserializeBase(xelemValue);
                FinalizeNewSerializer(ser, false);

                try
                {
                    member.SetValue(o, convertedObj);
                }
                catch
                {
                    OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xelemValue),
                        _defaultExceptionType);
                }
            }
        }

        /// <summary>
        ///     Retreives the collection value.
        /// </summary>
        /// <param name="colType">Type of the collection to be retrieved.</param>
        /// <param name="xelemValue">The value of xml element.</param>
        /// <param name="memberAlias">The member's alias, used only in exception titles.</param>
        /// <param name="colAttrInstance">The collection attribute instance.</param>
        /// <returns></returns>
        private object DeserializeCollectionValue(Type colType, XElement xelemValue, XName memberAlias,
            YAXCollectionAttribute colAttrInstance)
        {
            object containerObj = null;
            if (ReflectionUtils.IsInstantiableCollection(colType))
            {
                var namespaceToOverride = memberAlias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone();

                var containerSer = NewInternalSerializer(colType, namespaceToOverride, null);

                containerSer.IsCraetedToDeserializeANonCollectionMember = true;
                containerSer.RemoveDeserializedXmlNodes = true;

                containerObj = containerSer.DeserializeBase(xelemValue);
                FinalizeNewSerializer(containerSer, false);
            }

            var lst = new List<object>(); // this will hold the actual data items
            var itemType = ReflectionUtils.GetCollectionItemType(colType);

            if (ReflectionUtils.IsBasicType(itemType) && colAttrInstance != null &&
                colAttrInstance.SerializationType == YAXCollectionSerializationTypes.Serially)
            {
                // What if the collection was serialized serially
                var seps = colAttrInstance.SeparateBy.ToCharArray();

                // can white space characters be added to the separators?
                if (colAttrInstance.IsWhiteSpaceSeparator) seps = seps.Union(new[] {' ', '\t', '\r', '\n'}).ToArray();

                var elemValue = xelemValue.Value;
                var items = elemValue.Split(seps, StringSplitOptions.RemoveEmptyEntries);

                foreach (var wordItem in items)
                    try
                    {
                        lst.Add(ReflectionUtils.ConvertBasicType(wordItem, itemType));
                    }
                    catch
                    {
                        OnExceptionOccurred(new YAXBadlyFormedInput(memberAlias.ToString(), elemValue, xelemValue),
                            _defaultExceptionType);
                    }
            }
            else //if the collection was serialized recursively
            {
                var isPrimitive = ReflectionUtils.IsBasicType(itemType);

                XName eachElemName = null;
                if (colAttrInstance != null && colAttrInstance.EachElementName != null)
                {
                    eachElemName = StringUtils.RefineSingleElement(colAttrInstance.EachElementName);
                    eachElemName =
                        eachElemName.OverrideNsIfEmpty(memberAlias.Namespace.IfEmptyThen(TypeNamespace)
                            .IfEmptyThenNone());
                }

                var elemsToSearch = eachElemName == null ? xelemValue.Elements() : xelemValue.Elements(eachElemName);

                foreach (var childElem in elemsToSearch)
                {
                    var curElementType = itemType;
                    var curElementIsPrimitive = isPrimitive;

                    var realTypeAttribute = childElem.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                        _documentDefaultNamespace);
                    if (realTypeAttribute != null)
                    {
                        var theRealType = ReflectionUtils.GetTypeByName(realTypeAttribute.Value);
                        if (theRealType != null)
                        {
                            curElementType = theRealType;
                            curElementIsPrimitive = ReflectionUtils.IsBasicType(curElementType);
                        }
                    }

                    // TODO: check if curElementType is derived or is the same is itemType, for speed concerns perform this check only when elementName is null
                    if (eachElemName == null && (curElementType == typeof(object) ||
                                                 !ReflectionUtils.IsTypeEqualOrInheritedFromType(curElementType,
                                                     itemType)))
                        continue;

                    if (curElementIsPrimitive)
                    {
                        try
                        {
                            lst.Add(ReflectionUtils.ConvertBasicType(childElem.Value, curElementType));
                        }
                        catch
                        {
                            OnExceptionOccurred(
                                new YAXBadlyFormedInput(childElem.Name.ToString(), childElem.Value, childElem),
                                _defaultExceptionType);
                        }
                    }
                    else
                    {
                        var namespaceToOverride = memberAlias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone();
                        var ser = NewInternalSerializer(curElementType, namespaceToOverride, null);
                        lst.Add(ser.DeserializeBase(childElem));
                        FinalizeNewSerializer(ser, false);
                    }
                }
            } // end of else if 

            // Now what should I do with the filled list: lst
            Type dicKeyType, dicValueType;
            if (ReflectionUtils.IsArray(colType))
            {
                var dimsAttr = xelemValue.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _dimsAttrName,
                    _documentDefaultNamespace);
                var dims = new int[0];
                if (dimsAttr != null) dims = StringUtils.ParseArrayDimsString(dimsAttr.Value);

                Array arrayInstance = null;
                if (dims.Length > 0)
                {
                    var lowerBounds = new int[dims.Length]; // an array of zeros
                    arrayInstance = Array.CreateInstance(itemType, dims, lowerBounds); // create the array

                    var count = Math.Min(arrayInstance.Length, lst.Count);
                    // now fill the array
                    for (var i = 0; i < count; i++)
                    {
                        var inds = GetArrayDimentionalIndex(i, dims);
                        try
                        {
                            arrayInstance.SetValue(lst[i], inds);
                        }
                        catch
                        {
                            OnExceptionOccurred(
                                new YAXCannotAddObjectToCollection(memberAlias.ToString(), lst[i], xelemValue),
                                _defaultExceptionType);
                        }
                    }
                }
                else
                {
                    arrayInstance = Array.CreateInstance(itemType, lst.Count); // create the array

                    var count = Math.Min(arrayInstance.Length, lst.Count);
                    // now fill the array
                    for (var i = 0; i < count; i++)
                        try
                        {
                            arrayInstance.SetValue(lst[i], i);
                        }
                        catch
                        {
                            OnExceptionOccurred(
                                new YAXCannotAddObjectToCollection(memberAlias.ToString(), lst[i], xelemValue),
                                _defaultExceptionType);
                        }
                }

                return arrayInstance;
            }

            if (ReflectionUtils.IsIDictionary(colType, out dicKeyType, out dicValueType))
            {
                //The collection is a Dictionary
                var dic = containerObj;

                foreach (var lstItem in lst)
                {
                    var key = itemType.GetProperty("Key").GetValue(lstItem, null);
                    var value = itemType.GetProperty("Value").GetValue(lstItem, null);
                    try
                    {
                        colType.InvokeMethod("Add", dic, new[] {key, value});
                        //colType.InvokeMember("Add", BindingFlags.InvokeMethod, null, dic, new[] { key, value });
                    }
                    catch
                    {
                        OnExceptionOccurred(
                            new YAXCannotAddObjectToCollection(memberAlias.ToString(), lstItem, xelemValue),
                            _defaultExceptionType);
                    }
                }

                return dic;
            }

            if (ReflectionUtils.IsNonGenericIDictionary(colType))
            {
                var col = containerObj;
                foreach (var lstItem in lst)
                {
                    var key = lstItem.GetType().GetProperty("Key", BindingFlags.Instance | BindingFlags.Public)
                        .GetValue(lstItem, null);
                    var value = lstItem.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)
                        .GetValue(lstItem, null);

                    try
                    {
                        colType.InvokeMethod("Add", col, new[] {key, value});
                        //colType.InvokeMember("Add", BindingFlags.InvokeMethod, null, col, new[] { key, value });
                    }
                    catch
                    {
                        OnExceptionOccurred(
                            new YAXCannotAddObjectToCollection(memberAlias.ToString(), lstItem, xelemValue),
                            _defaultExceptionType);
                    }
                }

                return col;
            }

            if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(BitArray)))
            {
                var bArray = new bool[lst.Count];
                for (var i = 0; i < bArray.Length; i++)
                    try
                    {
                        bArray[i] = (bool) lst[i];
                    }
                    catch
                    {
                    }

                var col = Activator.CreateInstance(colType, bArray);
                //object col = colType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { bArray });

                return col;
            }

            if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Stack)) ||
                ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Stack<>)))
            {
                //object col = colType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
                var col = containerObj;

                const string additionMethodName = "Push";

                for (var i = lst.Count - 1; i >= 0; i--) // the loop must be from end to front
                    try
                    {
                        colType.InvokeMethod(additionMethodName, col, new[] {lst[i]});
                        //colType.InvokeMember(additionMethodName, BindingFlags.InvokeMethod, null, col, new[] { lst[i] });
                    }
                    catch
                    {
                        OnExceptionOccurred(
                            new YAXCannotAddObjectToCollection(memberAlias.ToString(), lst[i], xelemValue),
                            _defaultExceptionType);
                    }

                return col;
            }

            if (ReflectionUtils.IsIEnumerable(colType))
            {
                if (containerObj == null)
                    return lst;

                var col = containerObj;

                var additionMethodName = "Add";

                if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Queue)) ||
                    ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Queue<>)))
                    additionMethodName = "Enqueue";
                else if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(LinkedList<>)))
                    additionMethodName = "AddLast";

                foreach (var lstItem in lst)
                    try
                    {
                        colType.InvokeMethod(additionMethodName, col, new[] {lstItem});
                    }
                    catch
                    {
                        OnExceptionOccurred(
                            new YAXCannotAddObjectToCollection(memberAlias.ToString(), lstItem, xelemValue),
                            _defaultExceptionType);
                    }

                return col;
            }

            return null;
        }

        /// <summary>
        ///     Deserializes the collection member.
        /// </summary>
        /// <param name="o">The object to store the retrieved value at.</param>
        /// <param name="member">The member of the specified object whose value we intent to retreive.</param>
        /// <param name="colType">Type of the collection to be retrieved.</param>
        /// <param name="elemValue">The value of the element stored as string.</param>
        /// <param name="xelemValue">
        ///     The XML element value to be retrieved. If the value to be retrieved
        ///     has been stored in an XML attribute, this reference is <c>null</c>.
        /// </param>
        private void DeserializeCollectionMember(object o, MemberWrapper member, Type colType, string elemValue,
            XElement xelemValue)
        {
            object colObject;

            if (member.CollectionAttributeInstance != null && member.CollectionAttributeInstance.SerializationType ==
                YAXCollectionSerializationTypes.Serially &&
                (member.IsSerializedAsAttribute || member.IsSerializedAsValue))
            {
                colObject = DeserializeCollectionValue(colType, new XElement("temp", elemValue), "temp",
                    member.CollectionAttributeInstance);
            }
            else
            {
                var memberAlias = member.Alias.OverrideNsIfEmpty(TypeNamespace);
                colObject = DeserializeCollectionValue(colType, xelemValue, memberAlias,
                    member.CollectionAttributeInstance);
            }

            try
            {
                member.SetValue(o, colObject);
            }
            catch
            {
                OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xelemValue),
                    _defaultExceptionType);
            }
        }

        /// <summary>
        ///     Gets the dimensional index for an element of a multi-dimensional array from a
        ///     linear index specified.
        /// </summary>
        /// <param name="linInd">The linear index.</param>
        /// <param name="dims">The dimensions of the array.</param>
        /// <returns></returns>
        private static int[] GetArrayDimentionalIndex(long linInd, int[] dims)
        {
            var result = new int[dims.Length];

            var d = (int) linInd;

            for (var n = dims.Length - 1; n > 0; n--)
            {
                result[n] = d % dims[n];
                d = (d - result[n]) / dims[n];
            }

            result[0] = d;
            return result;
        }

        private object DeserializeTaggedDictionaryValue(XElement xelemValue, XName alias, Type type,
            YAXCollectionAttribute colAttributeInstance, YAXDictionaryAttribute dicAttrInstance)
        {
            // otherwise the "else if(member.IsTreatedAsCollection)" block solves the problem
            Type keyType, valueType;
            if (!ReflectionUtils.IsIDictionary(type, out keyType, out valueType))
                throw new Exception("elemValue must be a Dictionary");

            // deserialize non-collection fields
            var namespaceToOverride = alias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone();
            var containerSer = NewInternalSerializer(type, namespaceToOverride, null);
            containerSer.IsCraetedToDeserializeANonCollectionMember = true;
            containerSer.RemoveDeserializedXmlNodes = true;
            var dic = containerSer.DeserializeBase(xelemValue);
            FinalizeNewSerializer(containerSer, false);

            // now try to deserialize collection fields
            Type pairType = null;
            ReflectionUtils.IsIEnumerable(type, out pairType);
            XName eachElementName = StringUtils.RefineSingleElement(ReflectionUtils.GetTypeFriendlyName(pairType));
            var isKeyAttrib = false;
            var isValueAttrib = false;
            var isKeyContent = false;
            var isValueContent = false;
            var keyAlias = alias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone() + "Key";
            var valueAlias = alias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone() + "Value";

            if (colAttributeInstance != null && colAttributeInstance.EachElementName != null)
            {
                eachElementName = StringUtils.RefineSingleElement(colAttributeInstance.EachElementName);
                eachElementName =
                    eachElementName.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone());
            }

            if (dicAttrInstance != null)
            {
                if (dicAttrInstance.EachPairName != null)
                {
                    eachElementName = StringUtils.RefineSingleElement(dicAttrInstance.EachPairName);
                    eachElementName =
                        eachElementName.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone());
                }

                if (dicAttrInstance.SerializeKeyAs == YAXNodeTypes.Attribute)
                    isKeyAttrib = ReflectionUtils.IsBasicType(keyType);
                else if (dicAttrInstance.SerializeKeyAs == YAXNodeTypes.Content)
                    isKeyContent = ReflectionUtils.IsBasicType(keyType);

                if (dicAttrInstance.SerializeValueAs == YAXNodeTypes.Attribute)
                    isValueAttrib = ReflectionUtils.IsBasicType(valueType);
                else if (dicAttrInstance.SerializeValueAs == YAXNodeTypes.Content)
                    isValueContent = ReflectionUtils.IsBasicType(valueType);

                if (dicAttrInstance.KeyName != null)
                {
                    keyAlias = StringUtils.RefineSingleElement(dicAttrInstance.KeyName);
                    keyAlias = keyAlias.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone());
                }

                if (dicAttrInstance.ValueName != null)
                {
                    valueAlias = StringUtils.RefineSingleElement(dicAttrInstance.ValueName);
                    valueAlias =
                        valueAlias.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(TypeNamespace).IfEmptyThenNone());
                }
            }

            foreach (var childElem in xelemValue.Elements(eachElementName))
            {
                object key = null, value = null;
                YAXSerializer keySer = null, valueSer = null;

                var isKeyFound = VerifyDictionaryPairElements(ref keyType, ref isKeyAttrib, ref isKeyContent, keyAlias,
                    childElem);
                var isValueFound = VerifyDictionaryPairElements(ref valueType, ref isValueAttrib, ref isValueContent,
                    valueAlias, childElem);

                if (!isKeyFound && !isValueFound)
                    continue;

                if (isKeyFound)
                {
                    if (isKeyAttrib)
                    {
                        key = ReflectionUtils.ConvertBasicType(
                            childElem.Attribute_NamespaceSafe(keyAlias, _documentDefaultNamespace).Value, keyType);
                    }
                    else if (isKeyContent)
                    {
                        key = ReflectionUtils.ConvertBasicType(childElem.GetXmlContent(), keyType);
                    }
                    else if (ReflectionUtils.IsBasicType(keyType))
                    {
                        key = ReflectionUtils.ConvertBasicType(childElem.Element(keyAlias).Value, keyType);
                    }
                    else
                    {
                        if (keySer == null)
                            keySer = NewInternalSerializer(keyType, keyAlias.Namespace, null);

                        key = keySer.DeserializeBase(childElem.Element(keyAlias));
                        FinalizeNewSerializer(keySer, false);
                    }
                }

                if (isValueFound)
                {
                    if (isValueAttrib)
                    {
                        value = ReflectionUtils.ConvertBasicType(
                            childElem.Attribute_NamespaceSafe(valueAlias, _documentDefaultNamespace).Value, valueType);
                    }
                    else if (isValueContent)
                    {
                        value = ReflectionUtils.ConvertBasicType(childElem.GetXmlContent(), valueType);
                    }
                    else if (ReflectionUtils.IsBasicType(valueType))
                    {
                        value = ReflectionUtils.ConvertBasicType(childElem.Element(valueAlias).Value, valueType);
                    }
                    else
                    {
                        if (valueSer == null)
                            valueSer = NewInternalSerializer(valueType, valueAlias.Namespace, null);

                        value = valueSer.DeserializeBase(childElem.Element(valueAlias));
                        FinalizeNewSerializer(valueSer, false);
                    }
                }

                try
                {
                    type.InvokeMethod("Add", dic, new[] {key, value});
                    //type.InvokeMember("Add", BindingFlags.InvokeMethod, null, dic, new object[] { key, value });
                }
                catch
                {
                    OnExceptionOccurred(
                        new YAXCannotAddObjectToCollection(alias.LocalName,
                            new KeyValuePair<object, object>(key, value), childElem),
                        _defaultExceptionType);
                }
            }

            return dic;
        }

        private YAXSerializer NewInternalSerializer(Type type, XNamespace namespaceToOverride,
            XElement insertionLocation)
        {
            var serializer = new YAXSerializer(type, _exceptionPolicy, _defaultExceptionType, _serializationOption);
            serializer.MaxRecursion = MaxRecursion == 0 ? 0 : MaxRecursion - 1;
            serializer._serializedStack = _serializedStack;
            serializer._documentDefaultNamespace = _documentDefaultNamespace;
            if (namespaceToOverride != null)
                serializer.SetNamespaceToOverrideEmptyNamespace(namespaceToOverride);

            if (insertionLocation != null)
                serializer.SetBaseElement(insertionLocation);

            return serializer;
        }

        private void FinalizeNewSerializer(YAXSerializer serializer, bool importNamespaces,
            bool popFromSerializationStack = true)
        {
            if (serializer == null)
                return;

            if (popFromSerializationStack && _isSerializing && serializer._type != null &&
                !serializer._type.IsValueType)
                _serializedStack.Pop();

            if (importNamespaces)
                ImportNamespaces(serializer);
            _parsingErrors.AddRange(serializer.ParsingErrors);
        }

        /// <summary>
        ///     Deserializes a dictionary member which also benefits from a YAXDictionary attribute
        /// </summary>
        /// <param name="o">The object to hold the deserialized value.</param>
        /// <param name="member">The member corresponding to the dictionary member.</param>
        /// <param name="xelemValue">The XML element value.</param>
        private void DeserializeTaggedDictionaryMember(object o, MemberWrapper member, XElement xelemValue)
        {
            var dic = DeserializeTaggedDictionaryValue(xelemValue, member.Alias, member.MemberType,
                member.CollectionAttributeInstance, member.DictionaryAttributeInstance);

            try
            {
                member.SetValue(o, dic);
            }
            catch
            {
                OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xelemValue),
                    _defaultExceptionType);
            }
        }

        /// <summary>
        ///     Verifies the existence of dictionary pair <c>Key</c> and <c>Value</c> elements.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="isKeyAttrib">if set to <c>true</c> means that key has been serialize as an attribute.</param>
        /// <param name="isKeyContent">if set to <c>true</c> means that key has been serialize as an XML content.</param>
        /// <param name="keyAlias">The alias for <c>Key</c>.</param>
        /// <param name="childElem">The child XML elemenet to search <c>Key</c> and <c>Value</c> elements in.</param>
        /// <returns></returns>
        private bool VerifyDictionaryPairElements(ref Type keyType, ref bool isKeyAttrib, ref bool isKeyContent,
            XName keyAlias, XElement childElem)
        {
            var isKeyFound = false;

            if (isKeyAttrib && childElem.Attribute_NamespaceSafe(keyAlias, _documentDefaultNamespace) != null)
            {
                isKeyFound = true;
            }
            else if (isKeyContent && childElem.GetXmlContent() != null)
            {
                isKeyFound = true;
            }
            else if (isKeyAttrib || isKeyContent)
            {
                // loook for an element with the same name AND a yaxlib:realtype attribute
                var elem = childElem.Element(keyAlias);
                if (elem != null)
                {
                    var realTypeAttr = elem.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                        _documentDefaultNamespace);
                    if (realTypeAttr != null)
                    {
                        var theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
                        if (theRealType != null)
                        {
                            keyType = theRealType;
                            isKeyAttrib = false;
                            isKeyContent = false;
                            isKeyFound = true;
                        }
                    }
                }
            }
            else
            {
                var elem = childElem.Element(keyAlias);
                if (elem != null)
                {
                    isKeyFound = true;

                    var realTypeAttr = elem.Attribute_NamespaceSafe(_yaxLibNamespaceUri + _trueTypeAttrName,
                        _documentDefaultNamespace);
                    if (realTypeAttr != null)
                    {
                        var theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
                        if (theRealType != null) keyType = theRealType;
                    }
                }
            }

            return isKeyFound;
        }

        /// <summary>
        ///     Deserializes the XML reperesentation of a key-value pair, as specified, and returns
        ///     a <c>KeyValuePair</c> instance containing the deserialized data.
        /// </summary>
        /// <param name="baseElement">The element contating the XML reperesentation of a key-value pair.</param>
        /// <returns>a <c>KeyValuePair</c> instance containing the deserialized data</returns>
        private object DeserializeKeyValuePair(XElement baseElement)
        {
            var genArgs = _type.GetGenericArguments();
            var keyType = genArgs[0];
            var valueType = genArgs[1];

            var xnameKey = TypeNamespace.IfEmptyThenNone() + "Key";
            var xnameValue = TypeNamespace.IfEmptyThenNone() + "Value";

            object keyValue, valueValue;
            if (ReflectionUtils.IsBasicType(keyType))
            {
                try
                {
                    keyValue = ReflectionUtils.ConvertBasicType(
                        baseElement.Element(xnameKey).Value, keyType);
                }
                catch (NullReferenceException)
                {
                    keyValue = null;
                }
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(keyType))
            {
                keyValue = Activator.CreateInstance(keyType, baseElement.Element(xnameKey).Value);
            }
            else if (ReflectionUtils.IsCollectionType(keyType))
            {
                keyValue = DeserializeCollectionValue(keyType,
                    baseElement.Element(xnameKey), xnameKey, null);
            }
            else
            {
                var ser = NewInternalSerializer(keyType, xnameKey.Namespace.IfEmptyThenNone(), null);
                keyValue = ser.DeserializeBase(baseElement.Element(xnameKey));
                FinalizeNewSerializer(ser, false);
            }

            if (ReflectionUtils.IsBasicType(valueType))
            {
                try
                {
                    valueValue = ReflectionUtils.ConvertBasicType(baseElement.Element(xnameValue).Value, valueType);
                }
                catch (NullReferenceException)
                {
                    valueValue = null;
                }
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(valueType))
            {
                valueValue = Activator.CreateInstance(valueType, baseElement.Element(xnameValue).Value);
            }
            else if (ReflectionUtils.IsCollectionType(valueType))
            {
                valueValue = DeserializeCollectionValue(valueType,
                    baseElement.Element(xnameValue), xnameValue, null);
            }
            else
            {
                var ser = NewInternalSerializer(valueType, xnameValue.Namespace.IfEmptyThenNone(), null);
                valueValue = ser.DeserializeBase(baseElement.Element(xnameValue));
                FinalizeNewSerializer(ser, false);
            }

            var pair = Activator.CreateInstance(_type, keyValue, valueValue);
            return pair;
        }

        private static object InvokeCustomDeserializerFromElement(Type customDeserType, XElement elemToDeser)
        {
            var customDeserializer = Activator.CreateInstance(customDeserType, new object[0]);
            return customDeserType.InvokeMethod("DeserializeFromElement", customDeserializer,
                new object[] {elemToDeser});
        }

        private static object InvokeCustomDeserializerFromAttribute(Type customDeserType, XAttribute attrToDeser)
        {
            var customDeserializer = Activator.CreateInstance(customDeserType, new object[0]);
            return customDeserType.InvokeMethod("DeserializeFromAttribute", customDeserializer,
                new object[] {attrToDeser});
        }

        private static object InvokeCustomDeserializerFromValue(Type customDeserType, string valueToDeser)
        {
            var customDeserializer = Activator.CreateInstance(customDeserType, new object[0]);
            return customDeserType.InvokeMethod("DeserializeFromValue", customDeserializer,
                new object[] {valueToDeser});
        }

        private static void InvokeCustomSerializerToElement(Type customSerType, object objToSerialize,
            XElement elemToFill)
        {
            var customSerializer = Activator.CreateInstance(customSerType, new object[0]);
            customSerType.InvokeMethod("SerializeToElement", customSerializer, new[] {objToSerialize, elemToFill});
        }

        private static void InvokeCustomSerializerToAttribute(Type customSerType, object objToSerialize,
            XAttribute attrToFill)
        {
            var customSerializer = Activator.CreateInstance(customSerType, new object[0]);
            customSerType.InvokeMethod("SerializeToAttribute", customSerializer, new[] {objToSerialize, attrToFill});
        }

        private static string InvokeCustomSerializerToValue(Type customSerType, object objToSerialize)
        {
            var customSerializer = Activator.CreateInstance(customSerType, new object[0]);
            return (string) customSerType.InvokeMethod("SerializeToValue", customSerializer, new[] {objToSerialize});
        }

        /// <summary>
        ///     Gets the sequence of fields to be serialized for the specified type. This sequence is retrieved according to
        ///     the field-types specified by the user.
        /// </summary>
        /// <param name="typeWrapper">
        ///     The type wrapper for the type whose serializable
        ///     fields is going to be retrieved.
        /// </param>
        /// <returns>the sequence of fields to be serialized for the specified type</returns>
        private IEnumerable<MemberWrapper> GetFieldsToBeSerialized(UdtWrapper typeWrapper)
        {
            foreach (var member in typeWrapper.UnderlyingType.GetMembers(BindingFlags.Instance |
                                                                         BindingFlags.NonPublic | BindingFlags.Public))
            {
                var name0 = member.Name[0];
                if ((char.IsLetter(name0) ||
                     name0 == '_'
                    ) && // TODO: this is wrong, .NET supports unicode variable names or those starting with @
                    (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field))
                {
                    var prop = member as PropertyInfo;
                    if (prop != null)
                    {
                        // ignore indexers; if member is an indexer property, do not serialize it
                        if (prop.GetIndexParameters().Length > 0)
                            continue;

                        // don't serialize delegates as well
                        if (ReflectionUtils.IsTypeEqualOrInheritedFromType(prop.PropertyType, typeof(Delegate)))
                            continue;
                    }

                    if (typeWrapper.IsCollectionType || typeWrapper.IsDictionaryType) //&& typeWrapper.IsAttributedAsNotCollection)
                        if (ReflectionUtils.IsPartOfNetFx(member))
                            continue;

                    var memInfo = new MemberWrapper(member, this);
                    if (memInfo.IsAllowedToBeSerialized(typeWrapper.FieldsToSerialize,
                        _udtWrapper.DontSerializePropertiesWithNoSetter)) yield return memInfo;
                }
            }
        }

        /// <summary>
        ///     Gets the sequence of fields to be serialized for the serializer's underlying type.
        ///     This sequence is retrieved according to the field-types specified by the user.
        /// </summary>
        /// <returns>the sequence of fields to be serialized for the serializer's underlying type.</returns>
        private IEnumerable<MemberWrapper> GetFieldsToBeSerialized()
        {
            return GetFieldsToBeSerialized(_udtWrapper).OrderBy(t => t.Order);
        }

        private void AddMetadataAttribute(XElement parent, XName attrName, object attrValue,
            XNamespace documentDefaultNamespace)
        {
            if (!_udtWrapper.SuppressMetadataAttributes)
            {
                parent.AddAttributeNamespaceSafe(attrName, attrValue, documentDefaultNamespace);
                RegisterNamespace(_yaxLibNamespaceUri, _yaxLibNamespacePrefix);
            }
        }

        /// <summary>
        ///     Generates XDocument LoadOptions from SerializationOption
        /// </summary>
        private LoadOptions GetXmlLoadOptions()
        {
            var options = LoadOptions.None;
            if (_serializationOption.HasFlag(YAXSerializationOptions.DisplayLineInfoInExceptions))
                options |= LoadOptions.SetLineInfo;
            return options;
        }


        /// <summary>
        ///     Called when an exception occurs inside the library. It applies the exception handling policies.
        /// </summary>
        /// <param name="ex">The exception that has occurred.</param>
        /// <param name="exceptionType">Type of the exception.</param>
        private void OnExceptionOccurred(YAXException ex, YAXExceptionTypes exceptionType)
        {
            _exceptionOccurredDuringMemberDeserialization = true;
            if (exceptionType == YAXExceptionTypes.Ignore) return;

            _parsingErrors.AddException(ex, exceptionType);
            if (_exceptionPolicy == YAXExceptionHandlingPolicies.ThrowWarningsAndErrors ||
                _exceptionPolicy == YAXExceptionHandlingPolicies.ThrowErrorsOnly &&
                exceptionType == YAXExceptionTypes.Error)
                throw ex;
        }
    }
}