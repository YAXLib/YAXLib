# YAXLib: Yet Another XML Serialization Library for the .NET Framework

YAXLib is an XML Serialization library which helps you structure freely the XML result, choose among private and public fields to be serialized, and serialize all known collection classes and arrays (single-dimensional, multi-dimensional, and jagged arrays) in the .NET Framework.

See this article hosted on [CodeProject](http://www.codeproject.com) to see how to use the library. The article is also found in the `Doc` folder:
[http://www.codeproject.com/Articles/34045/Yet-Another-XML-Serialization-Library-for-the-NET](http://www.codeproject.com/Articles/34045/Yet-Another-XML-Serialization-Library-for-the-NET)

## Features

* Allowing the programmer to format the XML result as desired.
* Support for specifying path-like serialization addresses, e.g., `elem1/elem2/elem3`, and `../elem1`, and `./elem1`.
* Serialization and deserialization of all known generic and non-generic collection classes in `System.Collections`, and `System.Collections.Generic`.
* Support for serialization of single-dimensional, multi-dimensional, and jagged arrays.
* Support for recursive serialization of collections (i.e., collection of collections).
* Support for specifying aliases for enum members.
* Support for defining user-defined custom serializer for specific types or specific fields.
* Allowing the programmer to choose the fields to serialize (public, or non-public properties, or member variables).
* Support for serialization and deserialization of objects through a reference to their base-class or interface (also known as polymorphic serialization).
* Support for multi-stage deserialization.
* Allowing the programmer to add comments for the elements in the XML result.
* and more ...

See the accompanied demo application for an example of each functionality. 

## Nuget

To install YAXLib, run the following command in the Package Manager Console:

    PM> Install-Package YAXLib

## Contact

YAXLib is hosted on both [Github](https://github.com/sinairv/YAXLib) and [CodePlex](http://yaxlib.codeplex.com). Feel free to discuss about and fork it on either of these sites that you prefer. 

Sina Iravanian: [sina@sinairv.com](mailto:sina@sinairv.com)

Homepage: [www.sinairv.com](http://www.sinairv.com)

Github: [github.com/sinairv](https://github.com/sinairv)

Twitter: [@sinairv](http://www.twitter.com/sinairv)

