[2.16.0] Feb 14, 2021
   * Now supporting netstandard2.0 netstandard2.1 net461 net462 net472 net48 net5.0
   * Dropped support of netstandard1.6
   * Extended unit tests for netstandard
   * Enabled SourceLink
   * SemVer versioning

[2.15] Feb 18, 2017
   * Added support for .NET Core. Huge thanks go to [axuno gGmbH](https://github.com/axunonb) for developing the support for .NET Core.
   * YAXLib has a logo now! Huge thanks again to [axuno gGmbH](https://github.com/axunonb) for designing the logo.
   * Option added to suppress meta data attributes. Turn on `SuppressMetadataAttributes` flag when
     constructing the serializer. Note that this should only be used when not deserializing using YAXLib. 
     Thanks go to GitHub user [superoctave2](https://github.com/superoctave2) for developing this feature.

[2.14] Apr 30, 2016
   * Fixed a bug with deserializing Double/Sinble/BigInteger Min and Max values. 
     Thanks go to CodePlex user [vincentbl](https://www.codeplex.com/site/users/view/vincentbl)
     For reporting and suggesting the fix for the issue.
   * YAXLib will serialize `Type` and `RuntimeType` instances using their full name, avoiding 
     the need to traverse too much objects with cycling references in memory
   * Renamed constructor option `DontSerializeCyclingReferences` to `ThrowUponSerializingCyclingReferences`
     and inverted its intention at the same time. This change will disable serializing cycling
     references by default.
   * Added `YAXElementOrder` attribute to specify order upon which peroperties/fields are serialized/deserialized
     Thanks go to GitHub user [ACV](https://github.com/acvanzant) for developing this feature.
   * Added `YAXDontSerializeIfNullAttribute` attribute to prevent serialization of a specific field
     or property when their value is null.
     Thanks go to GitHub user [ACV](https://github.com/acvanzant) for developing this feature.
   * Fixed bugs about `null` collection and dictionary values
   * Fixed bugs about deserializing polymorphic collections when serialized with no containing element
     
[2.13] May 18, 2014
   * Fixed a bug and added unit tests related to serializing path like aliases with one 
     letter (e.g., './B'). Thanks go to CodeProject user B.O.B. for reporting this bug.
   * Added `Bin/*.dll.mdb` to `.gitignore` as requested by [@silwol](https://github.com/silwol).
   * Fixed the issue with Indexer properties. Indexers must not be serialized/deserialized.
     Thanks go to CodePlex user [iBeta](http://www.codeplex.com/site/users/view/iBeta) for
     reporting this bug.
   * YAXLib will ignore delegate (callback/function pointer) properties, so that the exception
     upon serializing them is prevented. Thanks go to CodePlex user [zack2656](https://www.codeplex.com/site/users/view/zack2656)
     for reporting this issue at CodePlex under [issue #17684](https://yaxlib.codeplex.com/workitem/17684).
   * Significant improve in cycling object reference detection
   * Self Referring types are allowed as long as references do not create a loop
   * Added an option to ignore self referring objects instead of throwing exceptions
   * Added an option not to serialize properties with no setter (will help in loop prevention, 
     see `CalculatedPropertiesCanCauseInfiniteLoop` sample class for an example).
   * Added the `MaxRecursion` property to the serializer with a default value of 300. This will prevent loops.
   * Minor bug fix on default constructor detection for collection classes.
   
[2.12] February 19, 2013
   * Added support for serialization of non-collection fields
     in objects derived from collection types/interfaces
   * Fixed a bug with namespace data not serialized for collections 
     passed directly to the serializer
   * Fixed a bug where attributes with namespace same as the default 
     namespace are not serialized properly. Thanks go to [@silwol](https://github.com/silwol)
     for reporting this bug.
   * Added DynamicKnownTypes to remove dependency to System.Drawing (and potential other dlls)
     This will make YAXLib more portable.
   * Added support for serializing DataTable and DataSet objects without making
     YAXLib dependent to System.Data.dll

[2.11] October 14, 2012
   * XML entity names now accept W3C standards.
   * Nullable types are not serialized with a `yaxlib:realtype`
     attribute any more.
   * Fixed some namespace related bugs.
   * Fixed an issue with deserializing date-time values that expose
     date time kind of UTC during serialization.
   * Classes can accept [YAXDictionary] and [YAXCollection] 
     attributes. The value to these attributes can then be 
     overridden by members instantiating them.
   * Unit tests all migrated to NUnit.
   * Lots of thanks go to Asbjørn Ulsberg for migrating the unit-tests
     NUnit, reporting the above bugs, and providing failing unit-tests
     regarding the above bugs.
   * It is now allowed to change the constant values used by the library to
     serialize meta-data. For this modify the following properties of the 
     `YAXSerializer` instance: 
     `YaxLibNamespaceUri`, `YaxLibNamespacePrefix`, 
     `DimentionsAttributeName`, `RealTypeAttributeName`
     Please see `YAXLibTests` > `OverridingYAXLibMetadataTests.cs` for a an
     example of how to achieve this.

[2.10.1] October 4, 2012
   * Allowing dashes to appear in name aliases
     Thanks go to Asbjørn Ulsberg for fixing the issue and providing
     unit tests.

[2.10] October 1, 2012
   * Full support for XML Namespaces. 
   * Defining XML namespaces through [YAXNamespace] attribute which 
     allows for specifying prefixes also.
   * Implicit XML namespace definition wherever there's an option
     to define an alias for an xml-attribute or xml-element, like an
     expanded name, as in "{path/to/namespace}alias". This support have
     been added into YAXSerializeAs attribute, EachElementName property
     of the YAXCollection attribute; and KeyName, and ValueName properties
     of the YAXDictionary attribute.
   * The initial support for XML-namespaces into YAXLib had been brought
     by kind community contribution. Special thanks go to Benjamin Pannell, 
     Tom Glastonbury, and CodePlex user "sttereo3" for starting this.
     However full namespace support and vast and various bug fixes 
     have been performed in current version.
   * Added a Visual Studio 2012 solution file.

[2.9] July 22, 2012
   * Lots of thanks go to Benjamin Pannell for bringing the support
     for XML namespaces into the library. This is brought through the
     [YAXNamespace] attribute.
   * Support for serializing Guids as XML attributes. Thanks go to 
     CodePlex user "jtrahan", for reporting this issue. 
   * Polymorphic serialization now works when creating a serializer 
     of a base type and passing an object. Thanks go to 
     Gumar Hermannsson for reporting this issue.

[2.8.3] June 18, 2012
   * Fixed the bug of removing elements corresponding to objects with
     no properties (such as DBNull, and Random). Thanks go to Robert Baron
     for reporting this bug.
   * Added support for [YAXPreserveWhitespace] attribute.
   * Added PreserveWhitespaceOnClassSample and PreserveWhitespaceOnFieldsSample
     sample classes that demonstrate how this attribute works. Whenever this
     attribute is added to a field or class, the xml:space="preserve"
     XML attribute is added to the corresponding XML element produced to keep
     whitespace entries upon deserialization. Thanks go to Robert Baron for
     reporting this problem and suggesting the solution.

[2.8.2] June 13, 2012
   * YAXLib is now a strongly-named assembly (thanks go to Jonas Fischer
     for suggesting and providing the patch for doing this)
   * Added a key to help build the project
   * Moved the sample classes from Demo-Application to the Test project
   * Added the script to build the library with another key, different 
     from that included in the project higherarchy, to sign the library 
     with another key, specific to the developer of the library.

[2.8.1] May 8, 2012
   * Migrating from SVN to Git
   * Fixing the issue with null known-types references, thanks go to 
     Tom Glastonbury, for reporting this issue (http://yaxlib.codeplex.com/workitem/17676)
     and providing the patch.
   * Added the XElementKnownType and XAttributeKnownType to the list of
     known-types as suggested and provided by Tom Glastonbury.
   * Added support for expanded XML names in the form of 
     "{namespace}elementname", which was reported and patched in
     this issue (http://yaxlib.codeplex.com/workitem/17661)
     by Tom Glastonbury.

[2.8] January 26, 2012
   * Switching to Visual Studio 2010
   * Adding support for specifying alias and path-like serialization addresses 
     together, the alias is seperated from path with a # character:
     [YAXAttributeFor("level1/level2#alias")]
     Here you don't need to use an extra [YAXSerializeAs("alias")].
   * Merging custom deserialization logic with custom serialization. Therefore,
     in order to make use of custom serialization capability one needs only to
     implement ICustomSerializer<>, and use the attribute YAXCustomSerializer on
     the desired field or type. The deserialization logic is also included in them.
   * Fixing the bug in XMLUtils.CreateLocation method, which did not create locations
     for which a portion a location already existed.
   * Adding support that any field that uses YAXCustomSerializer attribute
     can be serialized as an XML-attribute or value for another element. The 
     type that implements the custom serializer is responsible for creating the
     required attribute or value.
   * Adding support for collections that are serialized serially, to be also
     serialized as an attribute or value for another element.
   * Fixed a bug resulting from serializing a collection element through 
     a reference whose fields are serialized in other elements.
     Thanks go to CodePlex user: GraywizardX (http://www.codeplex.com/site/users/view/GraywizardX)
     Related discussion: http://yaxlib.codeplex.com/discussions/287166

[2.7] June 22, 2011
   * Fixing the bug that the primitive or basic types are not serialized 
     if directly passed as an object to the serializer.
     Thanks go to Code Project user, FCBCoder, for reporting this bug.

[2.6] May 25, 2011
   * Making YAXLib to serialize collection of objects which are stored through
     a reference to their base class, to be serialized with their class name or
     user-defined alias, instead of the base class name as the xml-element name.
     Thanks go to Seer Tenedos and Lars Udengaard for suggesting this.
     
[2.5] May 10, 2011
   * Fixing the bug that serialization and deserialization of decimal 
     values were not supported. Thanks go to Janko D, for submitting reporting
     the bug and submitting the patches (#9334 and #9335).
   * Forcing YAXLib to serialize values in the invariant culture. This neutralizes
     the changes made in version 2.4, and fixes the issue #14150 in another way.

[2.4] February 13, 2011
   * Fixing issue #14150. Now the serialized data follow the active culture 
     of the serializing thread. The limitation is that the culture of the 
     deserializer's thread must be the same, otherwise it is possible that 
     some parsing exception occur.

[2.3] February 13, 2011
   * Fixing a threading bug by making the type-wrappers pool thread safe.

[2.2] July 7, 2010
   * Adding support for definition of custom serializer and deserializers. 
     This is done through the YAXCustomSerializerAttribute, and 
     YAXCustomDeserializerAttribute.
     See SampleClasses.CustomSerializationTests for an example.
   
   * Changing back retreived element values to regular strings, not including
     representation of their child elements.
   

[2.1] July 5, 2010
   * Adding support for srializing fields as a value for another element. This
     is done through YAXValueFor attribute.
   * Added YAXElementValueAlreadyExistsException, and YAXElementValueMissingException
     exception classes to handle related exceptions.
   * Added BookClassTesgingSerializeAsValue sample class to demonstrate the new feature.

[2.0] April 15, 2010
   * YAXLib
   * Adding support for serialization and deserialization for all known generic
     and non-generic collection classes in System.Collections, and 
     System.Collections.Generic.
   * Adding support for recursive serialization of collections without serializing
     an element for their enclosing collection.
   * Adding support for specifying path-like serialization addresses, 
     e.g., elem1/elem2/elem3, and ../elem1, and ./elem1
   * Adding support for specifying aliases for enum members.
   * Adding support for choosing the fields to serialize (public, or non-public 
     properties, or member varialbes).
   * Adding support for serialization and deserialization of objects through 
     a reference to their base-class or interface.
   * Adding support for multi-stage deserialization.
   * Adding support for serialization of single-dimensional, multi-dimensional, 
     and jagged arrays.
   * Adding unit-tests for several parts of the library.

[1.1.5] Feb 22, 2010
   * YAXLib
   * Adding support to deserialize some known built-in .NET Types. Starting from TimeSpan.
   * Thanks go to James South; for reporting this problem for type TimeSpan

[1.1.4] Feb 22, 2010
   * YAXLib
   * Changing the style of the whole library, so that it matches StyleCop guidelines.
   * A lot of thanks go to James South; for his kind efforts for cleaning up the code, 
     and leaving documentation of 12 methods only to me.

[1.1.3] Jan 28, 2010
   * YAXLib/YAXSerializer.cs:
     Fixing the bug of reading value from an attribute pertaining to an absent parent element.
     Thanks go to Donovan Solms for reporting this bug.

[1.1.2] May 15, 2009
   * YAXLib/YAXSerializer.cs:
     Adding support for serializing property-less objects, that expose the 
     IFormattable interface (e.g., the Guid struct).
     Thanks go to Wayne Hiller for reporting this problem.

[1.1.1] May 6, 2009
   * YAXLib/YAXSerializer.cs:
     Fixing some bug related to nested collection classes.
     Thanks go to Jiri Steuer for reporting this bug.

[1.1] April 13, 2009
    * Updating the article
    * Fixing some minor bugs
    * Enhancing the demo application

[1.0.10] April 12, 2009
   * YAXLib/YAXSerializer.cs, YAXLib/YAXExceptionHandlingPolicies.cs:
     Added an Ignore item to the YAXExceptionTypes enum.
     Added YAXSerializationOptions enum, and a corresponding argument to the 
      YAXSerializer constructor, which currently lets the developer choose 
      whether to serialize null properties or not.
     If the developer chooses DontSerializeNullObjects as an option, then
      the serialization and deserialization processes keep the null properties
      status intact. Also the developer may select the error type of those properties
      to Ignore (via the ErrorIfMissed attribute or at construction time), so that 
      their absence is not reported as a problem.

[1.0.9] April 11, 2009
   * YAXLib/YAXAttributes.cs, YAXLib/YAXSerializer.cs:
     Added the support for inserting comments through the [YAXComment] attribute.
     The comments are only applicable to classes.

[1.0.8] April 11, 2009
   * DemoApplication:
     Added a GeneralToString provider which convert an object to string based upon its
     public properties via Reflection. This method is careful about null references, therefore
     we will not observe NullReferenceException in ToString methods.

[1.0.7] April 10, 2009
   * YAXLib/YAXAttributes.cs, YAXLib/YAXSerializer.cs:
     added the [YAXNotCollection] attribute. If some class implements the IEnumerable 
     interface, it is known by YAXLib as a collection class. This attribute prevents
     YAXLib to just iterate through the items provided through the IEnumerable interface;
     but to serialize the actual properties of the class, just as the way it is done with
     other classes.

[1.0.6] April 10, 2009
   * YAXLib/YAXAttributes.cs, YAXLib/YAXSerializer.cs:
     added support for controlling the serialization behaviour of generic dictionary 
     classes via YAXDictionary attribute. With this attribute one can specify: 
     - aliases for key and value parts
     - whether key/value should be serialized as XML attribute or XML element.
     - the format strings for serializing key/value parts.
  
[1.0.5] April 09, 2009
   * YAXLib/YAXAttributes.cs, YAXLib/YAXExceptions, YAXLib/YAXSerializer.cs: 
     adding support for format strings which could be used while serializing 
     objects with the given formats. Added YAXFormatAttribute, and 
     YAXInvalidFormatProvided exception class. 


[1.0.4] April 05, 2009
   * YAXLib/YAXAttributes.cs, YAXLib/YAXSerializer.cs: 
     adding support for ignoring whitespace characters, while deserializing 
     serially serialized collection classes, through IsWhiteSpaceSeparator 
     property of YAXCollectionAttribute class.

[1.0.3] March 26, 2009
   * YAXLib/YAXSerializer.cs: 
     fixing the bug, that the library did not ignore static properties.

[1.0.2] March 25, 2009
   * YAXLib/YAXSerializer.cs: 
     fixing the bug on deserializing null nested objects.
     Thanks go to Anton Levshunov for reporting this bug.

[1.0.1] March 24, 2009
   * YAXLib/YAXSerializer.cs: 
     fixing the bug that deserializing collection classes 
     attributed as [YAXDontSerialize] lead to a NullReferenceException. 
     Thanks go to Peter Zacho for reporting this bug.

[1.0.0] March 13, 2009
   * First version
