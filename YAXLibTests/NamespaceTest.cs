// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using YAXLibTests.SampleClasses;
using YAXLibTests.SampleClasses.Namespace;

namespace YAXLibTests
{
    [TestFixture]
    public class NamespaceTest
    {
        [Test]
        public void SingleNamespaceSerializationTest()
        {
            const string result = @"<!-- This example shows usage of a custom default namespace -->
" + "<SingleNamespaceSample xmlns=\"http://namespaces.org/default\">" + @"
  <StringItem>This is a test string</StringItem>
  <IntItem>10</IntItem>
</SingleNamespaceSample>";

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
                @"<!-- This example shows usage of a number of custom namespaces -->
<ns1:MultipleNamespaceSample xmlns:ns1=""http://namespaces.org/ns1"" xmlns:ns2=""http://namespaces.org/ns2"" xmlns:ns3=""http://namespaces.org/ns3"">
  <ns1:BoolItem>true</ns1:BoolItem>
  <ns2:StringItem>This is a test string</ns2:StringItem>
  <ns3:IntItem>10</ns3:IntItem>
</ns1:MultipleNamespaceSample>";

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
                @"<AttributeNamespaceSample xmlns:ns=""http://namespaces.org/ns"" xmlns=""http://namespaces.org/default"">
  <Attribs attrib=""value"" ns:attrib2=""value2"" />
</AttributeNamespaceSample>";

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
                @"<CellPhone_MemberAndClassDifferentNamespaces xmlns:x1=""http://namespace.org/x1"" xmlns=""http://namespace.org/nsmain"">
  <x1:TheName>HTC</x1:TheName>
  <OS>Windows Phone 8</OS>
</CellPhone_MemberAndClassDifferentNamespaces>";

            var serializer = new YAXSerializer<CellPhone_MemberAndClassDifferentNamespaces>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespaces.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void MemberAndClassDifferentNamespacePrefixesSerializationTest()
        {
            const string result =
                @"<xmain:CellPhone_MemberAndClassDifferentNamespacePrefixes xmlns:xmain=""http://namespace.org/nsmain"" xmlns:x1=""http://namespace.org/x1"">
  <x1:TheName>HTC</x1:TheName>
  <xmain:OS>Windows Phone 8</xmain:OS>
</xmain:CellPhone_MemberAndClassDifferentNamespacePrefixes>";

            var serializer = new YAXSerializer<CellPhone_MemberAndClassDifferentNamespacePrefixes>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespacePrefixes.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void MultiLevelMemberAndClassDifferentNamespacesSerializationTest()
        {
            const string result =
                @"<CellPhone_MultiLevelMemberAndClassDifferentNamespaces xmlns:x1=""http://namespace.org/x1"" xmlns=""http://namespace.org/nsmain"">
  <Level1>
    <Level2>
      <x1:TheName>HTC</x1:TheName>
    </Level2>
  </Level1>
  <OS>Windows Phone 8</OS>
</CellPhone_MultiLevelMemberAndClassDifferentNamespaces>";

            var serializer = new YAXSerializer<CellPhone_MultiLevelMemberAndClassDifferentNamespaces>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_MultiLevelMemberAndClassDifferentNamespaces.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void DictionaryNamespaceSerializationTest()
        {
            const string result =
                @"<CellPhone_DictionaryNamespace xmlns:x1=""http://namespace.org/x1"" xmlns:p1=""namespace/for/prices/only"" xmlns=""http://namespace.org/nsmain"">
  <x1:TheName>HTC</x1:TheName>
  <OS>Windows Phone 8</OS>
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
</CellPhone_DictionaryNamespace>";
            var serializer = new YAXSerializer<CellPhone_DictionaryNamespace>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_DictionaryNamespace.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void DictionaryNamespaceForAllItemsSerializationTest()
        {
            const string result =
                @"<CellPhone_DictionaryNamespaceForAllItems xmlns:p1=""http://namespace.org/brand"" xmlns:p2=""http://namespace.org/prices"" xmlns:p3=""http://namespace.org/pricepair"" xmlns:p4=""http://namespace.org/color"" xmlns:p5=""http://namespace.org/pricevalue"">
  <p1:Brand>Samsung Galaxy Nexus</p1:Brand>
  <OS>Android</OS>
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
</CellPhone_DictionaryNamespaceForAllItems>";
            var serializer = new YAXSerializer<CellPhone_DictionaryNamespaceForAllItems>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_DictionaryNamespaceForAllItems.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void CollectionNamespaceGoesThruRecursiveNoContainingElementSerializationTest()
        {
            const string result =
                @"<MobilePhone xmlns:app=""http://namespace.org/apps"">
  <DeviceBrand>Samsung Galaxy Nexus</DeviceBrand>
  <OS>Android</OS>
  <app:String>Google Map</app:String>
  <app:String>Google+</app:String>
  <app:String>Google Play</app:String>
</MobilePhone>";
            var serializer = new YAXSerializer<CellPhone_CollectionNamespaceGoesThruRecursiveNoContainingElement>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_CollectionNamespaceGoesThruRecursiveNoContainingElement
                .GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void CollectionNamespaceForAllItemsSerializationTest()
        {
            const string result =
                @"<MobilePhone xmlns:app=""http://namespace.org/apps"" xmlns:cls=""http://namespace.org/colorCol"" xmlns:mdls=""http://namespace.org/modelCol"" xmlns:p1=""http://namespace.org/appName"" xmlns:p2=""http://namespace.org/color"">
  <DeviceBrand>Samsung Galaxy Nexus</DeviceBrand>
  <OS>Android</OS>
  <p1:AppName>Google Map</p1:AppName>
  <p1:AppName>Google+</p1:AppName>
  <p1:AppName>Google Play</p1:AppName>
  <cls:AvailableColors>
    <p2:TheColor>red</p2:TheColor>
    <p2:TheColor>black</p2:TheColor>
    <p2:TheColor>white</p2:TheColor>
  </cls:AvailableColors>
  <mdls:AvailableModels>S1,MII,SXi,NoneSense</mdls:AvailableModels>
</MobilePhone>";
            var serializer = new YAXSerializer<CellPhone_CollectionNamespaceForAllItems>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_CollectionNamespaceForAllItems.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void YAXNamespaceOverridesImplicitNamespaceSerializationTest()
        {
            const string result =
                @"<CellPhone_YAXNamespaceOverridesImplicitNamespace xmlns:p1=""http://namespace.org/explicitBrand"" xmlns:p2=""http://namespace.org/os"">
  <p1:Brand>Samsung Galaxy S II</p1:Brand>
  <p2:OperatingSystem>Android 2</p2:OperatingSystem>
</CellPhone_YAXNamespaceOverridesImplicitNamespace>";

            var serializer = new YAXSerializer<CellPhone_YAXNamespaceOverridesImplicitNamespace>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_YAXNamespaceOverridesImplicitNamespace.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void MultilevelObjectsWithNamespacesSerializationTest()
        {
            const string result =
                @"<MultilevelObjectsWithNamespaces xmlns:ch1=""http://namespace.org/ch1"" xmlns:ch2=""http://namespace.org/ch2"" xmlns=""http://namespace.org/default"">
  <Parent1>
    <ch1:Child1 ch2:Value3=""Value3"">
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
</MultilevelObjectsWithNamespaces>";

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
                @"<Warehouse_Dictionary xmlns=""http://www.mywarehouse.com/warehouse/def/v3"">
  <ItemInfo Item=""Item1"" Count=""10"" />
  <ItemInfo Item=""Item4"" Count=""30"" />
  <ItemInfo Item=""Item2"" Count=""20"" />
</Warehouse_Dictionary>";
            var serializer = new YAXSerializer<Warehouse_Dictionary>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(Warehouse_Dictionary.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void AttributeWithDefaultNamespaceSerializationTest()
        {
            const string result =
                @"<w:font w:name=""Arial"" xmlns:w=""http://example.com/namespace"" />";

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
                @"<AttributeWithNamespaceAsMember xmlns:w=""http://example.com/namespace"">
  <w:Member w:name=""Arial"" />
</AttributeWithNamespaceAsMember>";

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
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
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
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
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
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void MemberAndClassDifferentNamespacesSerializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_MemberAndClassDifferentNamespaces>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespaces.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void MemberAndClassDifferentNamespacePrefixesDeserializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_MemberAndClassDifferentNamespacePrefixes>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespacePrefixes.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void MultiLevelMemberAndClassDifferentNamespacesDeserializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_MultiLevelMemberAndClassDifferentNamespaces>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_MultiLevelMemberAndClassDifferentNamespaces.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void DictionaryNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_DictionaryNamespaceForAllItems>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_DictionaryNamespaceForAllItems.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void DictionaryNamespaceForAllItemsDeserializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_DictionaryNamespace>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_DictionaryNamespace.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void CollectionNamespaceGoesThruRecursiveNoContainingElementDeserializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_CollectionNamespaceGoesThruRecursiveNoContainingElement>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_CollectionNamespaceGoesThruRecursiveNoContainingElement
                .GetSampleInstance());
            var deserialized =
                serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void CollectionNamespaceForAllItemsDeserializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_CollectionNamespaceForAllItems>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_CollectionNamespaceForAllItems.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void YAXNamespaceOverridesImplicitNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer<CellPhone_YAXNamespaceOverridesImplicitNamespace>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(CellPhone_YAXNamespaceOverridesImplicitNamespace.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
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
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void DictionaryWithParentNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer<Warehouse_Dictionary>(new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });
            var got = serializer.Serialize(Warehouse_Dictionary.GetSampleInstance());
            var deserialized = serializer.Deserialize(got);
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
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
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
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
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(serializer.ParsingErrors, Has.Count.EqualTo(0));
        }

        [Test]
        public void CSProjParsingTest()
        {
            var csprojContent =
                @"<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
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
    <Reference Include=""$generatedproject$.EFDAL.Interfaces"">
      <HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
      <SpecificVersion>false</SpecificVersion>
    </Reference>
    <Reference Include=""System"">
      <SpecificVersion>false</SpecificVersion>
    </Reference>
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
      <SpecificVersion>false</SpecificVersion>
    </Reference>
    <Reference Include=""nHydrate.EFCore, Version=0.0.0.0, Culture=neutral, processorArchitecture=MSIL"">
      <HintPath>..\bin\nHydrate.EFCore.dll</HintPath>
      <SpecificVersion>false</SpecificVersion>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Reference Include=""$generatedproject$.EFDAL.Interfaces"">
      <HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
      <SpecificVersion>false</SpecificVersion>
    </Reference>
    <Reference Include=""System"">
      <SpecificVersion>false</SpecificVersion>
    </Reference>
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
      <SpecificVersion>false</SpecificVersion>
    </Reference>
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";

            var project = CsprojParser.Parse(csprojContent);
            var xml2 = CsprojParser.ParseAndRegenerateXml(csprojContent);
            Assert.That(xml2, Is.EqualTo(csprojContent));
        }
    }
}