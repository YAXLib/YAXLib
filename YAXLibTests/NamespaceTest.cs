// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using YAXLibTests.SampleClasses;
using YAXLibTests.SampleClasses.Namespace;

namespace YAXLibTests;

[TestFixture]
public class NamespaceTest
{
    [Test]
    public void SingleNamespaceSerializationTest()
    {
        const string result = """
            <!-- This example shows usage of a custom default namespace -->

            """ + "<SingleNamespaceSample xmlns=\"http://namespaces.org/default\">" + """
                
                  <StringItem>This is a test string</StringItem>
                  <IntItem>10</IntItem>
                </SingleNamespaceSample>
                """;

        var serializer = new YAXSerializer<SingleNamespaceSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(SingleNamespaceSample.GetInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void MultipleNamespaceSerializationTest()
    {
        const string result =
            """
                <!-- This example shows usage of a number of custom namespaces -->
                <ns1:MultipleNamespaceSample xmlns:ns1="http://namespaces.org/ns1" xmlns:ns2="http://namespaces.org/ns2" xmlns:ns3="http://namespaces.org/ns3">
                  <ns1:BoolItem>true</ns1:BoolItem>
                  <ns2:StringItem>This is a test string</ns2:StringItem>
                  <ns3:IntItem>10</ns3:IntItem>
                </ns1:MultipleNamespaceSample>
                """;

        var serializer = new YAXSerializer<MultipleNamespaceSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(MultipleNamespaceSample.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void AttributeNamespaceSerializationTest()
    {
        const string result =
            """
                <AttributeNamespaceSample xmlns:ns="http://namespaces.org/ns" xmlns="http://namespaces.org/default">
                  <Attribs Attrib="value" ns:Attrib2="value2" />
                </AttributeNamespaceSample>
                """;

        var serializer = new YAXSerializer<AttributeNamespaceSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(AttributeNamespaceSample.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void MemberAndClassDifferentNamespacesDeserializationTest()
    {
        const string result =
            """
                <CellPhoneMemberAndClassDifferentNamespaces xmlns:x1="http://namespace.org/x1" xmlns="http://namespace.org/nsmain">
                  <x1:TheName>HTC</x1:TheName>
                  <Os>Windows Phone 8</Os>
                </CellPhoneMemberAndClassDifferentNamespaces>
                """;

        var serializer = new YAXSerializer<CellPhoneMemberAndClassDifferentNamespaces>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneMemberAndClassDifferentNamespaces.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void MemberAndClassDifferentNamespacePrefixesSerializationTest()
    {
        const string result =
            """
                <xmain:CellPhoneMemberAndClassDifferentNamespacePrefixes xmlns:xmain="http://namespace.org/nsmain" xmlns:x1="http://namespace.org/x1">
                  <x1:TheName>HTC</x1:TheName>
                  <xmain:Os>Windows Phone 8</xmain:Os>
                </xmain:CellPhoneMemberAndClassDifferentNamespacePrefixes>
                """;

        var serializer = new YAXSerializer<CellPhoneMemberAndClassDifferentNamespacePrefixes>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneMemberAndClassDifferentNamespacePrefixes.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void MultiLevelMemberAndClassDifferentNamespacesSerializationTest()
    {
        const string result =
            """
                <CellPhoneMultiLevelMemberAndClassDifferentNamespaces xmlns:x1="http://namespace.org/x1" xmlns="http://namespace.org/nsmain">
                  <Level1>
                    <Level2>
                      <x1:TheName>HTC</x1:TheName>
                    </Level2>
                  </Level1>
                  <Os>Windows Phone 8</Os>
                </CellPhoneMultiLevelMemberAndClassDifferentNamespaces>
                """;

        var serializer = new YAXSerializer<CellPhoneMultiLevelMemberAndClassDifferentNamespaces>(
            new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
        var got = serializer.Serialize(CellPhoneMultiLevelMemberAndClassDifferentNamespaces.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void DictionaryNamespaceSerializationTest()
    {
        const string result =
            """
                <CellPhoneDictionaryNamespace xmlns:x1="http://namespace.org/x1" xmlns:p1="namespace/for/prices/only" xmlns="http://namespace.org/nsmain">
                  <x1:TheName>HTC</x1:TheName>
                  <Os>Windows Phone 8</Os>
                  <p1:Prices>
                    <p1:KeyValuePairOfStringDouble>
                      <p1:Key>red</p1:Key>
                      <p1:Value>120</p1:Value>
                    </p1:KeyValuePairOfStringDouble>
                    <p1:KeyValuePairOfStringDouble>
                      <p1:Key>blue</p1:Key>
                      <p1:Value>110</p1:Value>
                    </p1:KeyValuePairOfStringDouble>
                    <p1:KeyValuePairOfStringDouble>
                      <p1:Key>black</p1:Key>
                      <p1:Value>140</p1:Value>
                    </p1:KeyValuePairOfStringDouble>
                  </p1:Prices>
                </CellPhoneDictionaryNamespace>
                """;
        var serializer = new YAXSerializer<CellPhoneDictionaryNamespace>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneDictionaryNamespace.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void DictionaryNamespaceForAllItemsSerializationTest()
    {
        const string result =
            """
                <CellPhoneDictionaryNamespaceForAllItems xmlns:p1="http://namespace.org/brand" xmlns:p2="http://namespace.org/prices" xmlns:p3="http://namespace.org/pricepair" xmlns:p4="http://namespace.org/color" xmlns:p5="http://namespace.org/pricevalue">
                  <p1:Brand>Samsung Galaxy Nexus</p1:Brand>
                  <Os>Android</Os>
                  <p2:ThePrices>
                    <p3:PricePair>
                      <p4:TheColor>red</p4:TheColor>
                      <p5:ThePrice>120</p5:ThePrice>
                    </p3:PricePair>
                    <p3:PricePair>
                      <p4:TheColor>blue</p4:TheColor>
                      <p5:ThePrice>110</p5:ThePrice>
                    </p3:PricePair>
                    <p3:PricePair>
                      <p4:TheColor>black</p4:TheColor>
                      <p5:ThePrice>140</p5:ThePrice>
                    </p3:PricePair>
                  </p2:ThePrices>
                </CellPhoneDictionaryNamespaceForAllItems>
                """;
        var serializer = new YAXSerializer<CellPhoneDictionaryNamespaceForAllItems>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneDictionaryNamespaceForAllItems.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void CollectionNamespaceGoesThruRecursiveNoContainingElementSerializationTest()
    {
        const string result =
            """
                <MobilePhone xmlns:app="http://namespace.org/apps">
                  <DeviceBrand>Samsung Galaxy Nexus</DeviceBrand>
                  <Os>Android</Os>
                  <app:String>Google Map</app:String>
                  <app:String>Google+</app:String>
                  <app:String>Google Play</app:String>
                </MobilePhone>
                """;
        var serializer = new YAXSerializer<CellPhoneCollectionNamespaceGoesThruRecursiveNoContainingElement>(
            new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
        var got = serializer.Serialize(CellPhoneCollectionNamespaceGoesThruRecursiveNoContainingElement
            .GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void CollectionNamespaceForAllItemsSerializationTest()
    {
        const string result =
            """
                <MobilePhone xmlns:app="http://namespace.org/apps" xmlns:cls="http://namespace.org/colorCol" xmlns:mdls="http://namespace.org/modelCol" xmlns:p1="http://namespace.org/appName" xmlns:p2="http://namespace.org/color">
                  <DeviceBrand>Samsung Galaxy Nexus</DeviceBrand>
                  <Os>Android</Os>
                  <p1:AppName>Google Map</p1:AppName>
                  <p1:AppName>Google+</p1:AppName>
                  <p1:AppName>Google Play</p1:AppName>
                  <cls:AvailableColors>
                    <p2:TheColor>red</p2:TheColor>
                    <p2:TheColor>black</p2:TheColor>
                    <p2:TheColor>white</p2:TheColor>
                  </cls:AvailableColors>
                  <mdls:AvailableModels>S1,MII,SXi,NoneSense</mdls:AvailableModels>
                </MobilePhone>
                """;
        var serializer = new YAXSerializer<CellPhoneCollectionNamespaceForAllItems>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneCollectionNamespaceForAllItems.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void YaxNamespaceOverridesImplicitNamespaceSerializationTest()
    {
        const string result =
            """
                <CellPhoneYaxNamespaceOverridesImplicitNamespace xmlns:p1="http://namespace.org/explicitBrand" xmlns:p2="http://namespace.org/os">
                  <p1:Brand>Samsung Galaxy S II</p1:Brand>
                  <p2:OperatingSystem>Android 2</p2:OperatingSystem>
                </CellPhoneYaxNamespaceOverridesImplicitNamespace>
                """;

        var serializer = new YAXSerializer<CellPhoneYaxNamespaceOverridesImplicitNamespace>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneYaxNamespaceOverridesImplicitNamespace.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void MultilevelObjectsWithNamespacesSerializationTest()
    {
        const string result =
            """
                <MultilevelObjectsWithNamespaces xmlns:ch1="http://namespace.org/ch1" xmlns:ch2="http://namespace.org/ch2" xmlns="http://namespace.org/default">
                  <Parent1>
                    <ch1:Child1 ch2:Value3="Value3">
                      <ch1:Field1>Field1</ch1:Field1>
                      <ch1:Field2>Field2</ch1:Field2>
                      <ch2:Value1>Value1</ch2:Value1>
                      <ch2:Value2>Value2</ch2:Value2>
                    </ch1:Child1>
                  </Parent1>
                  <Parent2>
                    <ch2:Child2>
                      <ch2:Value4>Value4</ch2:Value4>
                    </ch2:Child2>
                  </Parent2>
                </MultilevelObjectsWithNamespaces>
                """;

        var serializer = new YAXSerializer<MultilevelObjectsWithNamespaces>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(MultilevelObjectsWithNamespaces.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void DictionaryWithParentNamespaceSerializationTest()
    {
        const string result =
            """
                <WarehouseDictionary xmlns="http://www.mywarehouse.com/warehouse/def/v3">
                  <ItemInfo Item="Item1" Count="10" />
                  <ItemInfo Item="Item4" Count="30" />
                  <ItemInfo Item="Item2" Count="20" />
                </WarehouseDictionary>
                """;
        var serializer = new YAXSerializer<WarehouseDictionary>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(WarehouseDictionary.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void AttributeWithDefaultNamespaceSerializationTest()
    {
        const string result =
            """<w:font w:name="Arial" xmlns:w="http://example.com/namespace" />""";

        var serializer = new YAXSerializer<AttributeWithNamespace>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(AttributeWithNamespace.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void AttributeWithDefaultNamespaceAsMemberSerializationTest()
    {
        const string result =
            """
                <AttributeWithNamespaceAsMember xmlns:w="http://example.com/namespace">
                  <w:Member w:name="Arial" />
                </AttributeWithNamespaceAsMember>
                """;

        var serializer = new YAXSerializer<AttributeWithNamespaceAsMember>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(AttributeWithNamespaceAsMember.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void SingleNamespaceDeserializationTest()
    {
        var serializer = new YAXSerializer<SingleNamespaceSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var serialized = serializer.Serialize(SingleNamespaceSample.GetInstance());
        var deserialized = serializer.Deserialize(serialized);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MultipleNamespaceDeserializationTest()
    {
        var serializer = new YAXSerializer<MultipleNamespaceSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var serialized = serializer.Serialize(MultipleNamespaceSample.GetSampleInstance());
        var deserialized = serializer.Deserialize(serialized);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void AttributeNamespaceDeserializationTest()
    {
        var serializer = new YAXSerializer<AttributeNamespaceSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(AttributeNamespaceSample.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MemberAndClassDifferentNamespacesSerializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneMemberAndClassDifferentNamespaces>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneMemberAndClassDifferentNamespaces.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MemberAndClassDifferentNamespacePrefixesDeserializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneMemberAndClassDifferentNamespacePrefixes>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneMemberAndClassDifferentNamespacePrefixes.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MultiLevelMemberAndClassDifferentNamespacesDeserializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneMultiLevelMemberAndClassDifferentNamespaces>(
            new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
        var got = serializer.Serialize(CellPhoneMultiLevelMemberAndClassDifferentNamespaces.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void DictionaryNamespaceDeserializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneDictionaryNamespaceForAllItems>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneDictionaryNamespaceForAllItems.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void DictionaryNamespaceForAllItemsDeserializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneDictionaryNamespace>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneDictionaryNamespace.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void CollectionNamespaceGoesThruRecursiveNoContainingElementDeserializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneCollectionNamespaceGoesThruRecursiveNoContainingElement>(
            new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
        var got = serializer.Serialize(CellPhoneCollectionNamespaceGoesThruRecursiveNoContainingElement
            .GetSampleInstance());
        var deserialized =
            serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void CollectionNamespaceForAllItemsDeserializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneCollectionNamespaceForAllItems>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneCollectionNamespaceForAllItems.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void YaxNamespaceOverridesImplicitNamespaceDeserializationTest()
    {
        var serializer = new YAXSerializer<CellPhoneYaxNamespaceOverridesImplicitNamespace>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(CellPhoneYaxNamespaceOverridesImplicitNamespace.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MultilevelObjectsWithNamespacesDeserializationTest()
    {
        var serializer = new YAXSerializer<MultilevelObjectsWithNamespaces>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(MultilevelObjectsWithNamespaces.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void DictionaryWithParentNamespaceDeserializationTest()
    {
        var serializer = new YAXSerializer<WarehouseDictionary>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(WarehouseDictionary.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void AttributeWithDefaultNamespaceDeserializationTest()
    {
        var serializer = new YAXSerializer<AttributeWithNamespace>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(AttributeWithNamespace.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void AttributeWithDefaultNamespaceAsMemberDeserializationTest()
    {
        var serializer = new YAXSerializer<AttributeWithNamespaceAsMember>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(AttributeWithNamespaceAsMember.GetSampleInstance());
        var deserialized = serializer.Deserialize(got);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void CsProjParsingTest()
    {
        var csprojContent =
            """
                <Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                  <PropertyGroup>
                    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                    <ProductVersion>9.0.30729</ProductVersion>
                    <SchemaVersion>2.0</SchemaVersion>
                    <ProjectGuid>$guid$</ProjectGuid>
                    <OutputType>Library</OutputType>
                    <AppDesignerFolder>Properties</AppDesignerFolder>
                    <RootNamespace>$safeprojectname$</RootNamespace>
                    <AssemblyName>$safeprojectname$</AssemblyName>
                    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
                    <FileAlignment>512</FileAlignment>
                    <DebugSymbols>false</DebugSymbols>
                    <Optimize>false</Optimize>
                    <WarningLevel>0</WarningLevel>
                  </PropertyGroup>
                  <PropertyGroup>
                    <DebugSymbols>true</DebugSymbols>
                    <DebugType>full</DebugType>
                    <Optimize>false</Optimize>
                    <OutputPath>bin\Debug\</OutputPath>
                    <DefineConstants>DEBUG;TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                    <DocumentationFile>bin\Debug\$safeprojectname$.xml</DocumentationFile>
                  </PropertyGroup>
                  <PropertyGroup>
                    <DebugSymbols>false</DebugSymbols>
                    <DebugType>pdbonly</DebugType>
                    <Optimize>true</Optimize>
                    <OutputPath>bin\Release\</OutputPath>
                    <DefineConstants>TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <ItemGroup>
                    <Reference Include="$generatedproject$.EFDAL.Interfaces">
                      <HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
                      <SpecificVersion>false</SpecificVersion>
                    </Reference>
                    <Reference Include="System">
                      <SpecificVersion>false</SpecificVersion>
                    </Reference>
                    <Reference Include="System.Core">
                      <RequiredTargetFramework>3.5</RequiredTargetFramework>
                      <SpecificVersion>false</SpecificVersion>
                    </Reference>
                    <Reference Include="nHydrate.EFCore, Version=0.0.0.0, Culture=neutral, processorArchitecture=MSIL">
                      <HintPath>..\bin\nHydrate.EFCore.dll</HintPath>
                      <SpecificVersion>false</SpecificVersion>
                    </Reference>
                  </ItemGroup>
                  <ItemGroup>
                    <Reference Include="$generatedproject$.EFDAL.Interfaces">
                      <HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
                      <SpecificVersion>false</SpecificVersion>
                    </Reference>
                    <Reference Include="System">
                      <SpecificVersion>false</SpecificVersion>
                    </Reference>
                    <Reference Include="System.Core">
                      <RequiredTargetFramework>3.5</RequiredTargetFramework>
                      <SpecificVersion>false</SpecificVersion>
                    </Reference>
                  </ItemGroup>
                  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
                </Project>
                """;

        var project = CsprojParser.Parse(csprojContent);
        var xml2 = CsprojParser.ParseAndRegenerateXml(csprojContent);
        Assert.That(xml2, Is.EqualTo(csprojContent));
    }
}