// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using YAXLibTests.SampleClasses;
using YAXLibTests.SampleClasses.PolymorphicSerialization;
using YAXLibTests.SampleClasses.SelfReferencingObjects;
using YAXLibTests.TestHelpers;

namespace YAXLibTests;

public abstract class DeserializationTestBase
{
    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private void PerformTest(object obj, Type? objType = null)
    {
        var result = PerformTestAndReturn(obj, objType);
        if (objType is { } && result is { })
        {
            var runtimeType = result.GetType();
            var wantedType = objType;
            Assert.That(objType.IsAssignableFrom(runtimeType), $"runtime type: {runtimeType} can not be assigned to {wantedType}");
        }
    }

    /// <summary>
    /// Perform test by using .Equals. Equals should be well implemented for <paramref name="obj" />
    /// </summary>
    /// <param name="obj"></param>
    private void PerformTestWithEquals(object obj, Type? objType = null)
    {
        var serializer = SerializeDeserialize(obj, out var gottonObject, objType);
        Assert.Multiple(() =>
        {
            Assert.That(serializer.ParsingErrors.Count, Is.EqualTo(0));
            Assert.That(gottonObject, Is.EqualTo(obj));
        });
    }

    private object? GetTheTwoStringsAndReturn(object obj, out string originalString, out string? gottonString,
        out int errorCounts, Type? objType = null)
    {
        originalString = GeneralToStringProvider.GeneralToString(obj);
        var serializer = SerializeDeserialize(obj, out var gottonObject, objType);
        errorCounts = serializer.ParsingErrors.Count;
        gottonString = GeneralToStringProvider.GeneralToString(gottonObject);
        return gottonObject;
    }

    private YAXSerializer SerializeDeserialize(object obj, out object? gottonObject, Type? objType = null)
    {
        var serializer = CreateSerializer(objType ?? obj.GetType(),
            new SerializerOptions {
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow
            });
        var serResult = serializer.Serialize(obj);
        gottonObject = serializer.Deserialize(serResult);
        return serializer;
    }

    private object? PerformTestAndReturn(object obj, Type? objType = null)
    {
        var result =
            GetTheTwoStringsAndReturn(obj, out var originalString, out var gottonString, out var errorCounts, objType);
        Assert.Multiple(() =>
        {
            Assert.That(originalString, Is.Not.Null);
            Assert.That(gottonString, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(gottonString, Is.EqualTo(originalString));
            Assert.That(errorCounts, Is.EqualTo(0));
        });
        return result;
    }

    [Test]
    public void DeserializeBasicTypesTest()
    {
        PerformTest(666);
    }

    [Test]
    public void DesBookTest()
    {
        PerformTest(Book.GetSampleInstance());
    }

    [Test]
    public void DesBookWithDecimalTest()
    {
        PerformTest(SimpleBookClassWithDecimalPrice.GetSampleInstance());
    }

    [Test]
    public void DesBookStructTest()
    {
        object obj = BookStruct.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesWarehouseSimpleTest()
    {
        object obj = WarehouseSimple.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesWarehouseStructuredTest()
    {
        object obj = WarehouseStructured.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesWarehouseWithArrayTest()
    {
        object obj = WarehouseWithArray.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesWarehouseWithDictionaryTest()
    {
        object obj = WarehouseWithDictionary.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesWarehouseNestedObjectTest()
    {
        object obj = WarehouseNestedObjectExample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesProgrammingLanguageTest()
    {
        object obj = ProgrammingLanguage.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesColorExampleTest()
    {
        object obj = ColorExample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesMultiLevelClassTest()
    {
        object obj = MultilevelClass.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesPathsExampleTest()
    {
        object obj = PathsExample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesMoreComplexExampleTest()
    {
        object obj = MoreComplexExample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesNestedDicSampleTest()
    {
        object obj = NestedDicSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesGUIDTestTest()
    {
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();
        var g3 = Guid.NewGuid();
        var g4 = Guid.NewGuid();

        object obj = GUIDTest.GetSampleInstance(g1, g2, g3, g4);
        PerformTest(obj);
    }

    [Test]
    public void DesEmptyNullableTest()
    {
        const string xml = """<NullableSample2 />""";
        var serializer = CreateSerializer(typeof(NullableSample2),
            new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects
            });
        var got = (NullableSample2?) serializer.Deserialize(xml);

        Assert.Multiple(() =>
        {
            Assert.That(got, Is.Not.Null);
            Assert.That(got?.Boolean, Is.Null);
            Assert.That(got?.DateTime, Is.Null);
            Assert.That(got?.Decimal, Is.Null);
            Assert.That(got?.Enum, Is.Null);
            Assert.That(got?.Number, Is.Null);
        });
    }

    [Test]
    public void DesNullableTest()
    {
        object obj = NullableClass.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesNullableSample2Test()
    {
        object obj = NullableSample2.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesListHolderClassTest()
    {
        object obj = ListHolderClass.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesIEnumerableHolderClassTest()
    {
        object obj = IEnumerableHolderClass.GetSampleInstance();
        PerformTest(obj);
    }


    [Test]
    public void DesIEnumerableListClassTest()
    {
        var list = new System.Collections.Generic.List<IEnumerableHolderClass.Item>();
        list.Add("Hallo");
        list.Add("Welt");
        IEnumerable<IEnumerableHolderClass.Item> enumerable = list;

        //do not use the runtime type!
        PerformTest(enumerable, typeof(IEnumerable<IEnumerableHolderClass.Item>));
    }

    [Test]
    public void DesStandaloneListTest()
    {
        object obj = ListHolderClass.GetSampleInstance().ListOfStrings;
        PerformTest(obj);
    }

    [Test]
    public void DesNamesExampleTest()
    {
        object obj = NamesExample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesRequestTest()
    {
        object obj = Request.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesAudioSampleTest()
    {
        object obj = AudioSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesTimeSpanTest()
    {
        object obj = TimeSpanSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesMoreComplexBookTest()
    {
        object obj = MoreComplexBook.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesMoreComplexBookTwoTest()
    {
        object obj = MoreComplexBook2.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesMoreComplexBookThreeTest()
    {
        object obj = MoreComplexBook3.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesWarehouseWithDictionaryNoContainerTest()
    {
        object obj = WarehouseWithDictionaryNoContainer.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesWarehouseWithCommentsTest()
    {
        object obj = WarehouseWithComments.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesEnumsSampleTest()
    {
        object obj = EnumsSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesMultiDimArraySampleTest()
    {
        object obj = MultiDimArraySample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesAnotherArraySampleTest()
    {
        object obj = AnotherArraySample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesCollectionOfInterfacesSampleTest()
    {
        object obj = CollectionOfInterfacesSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesInterfaceMatchingSampleTest()
    {
        object obj = InterfaceMatchingSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesNonGenericCollectionsSampleTest()
    {
        object obj = NonGenericCollectionsSample.GetSampleInstance();
        PerformTestWithEquals(obj);
    }

    [Test]
    public void DesGenericCollectionsSampleTest()
    {
        object obj = GenericCollectionsSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void MoreComplexBookTwoResumedDeserializationTest()
    {
        var result =
            """
                <MoreComplexBook2 Author_s_Name="Tom Archer">
                  <Title>Inside C#</Title>
                  <PublishYear>2002</PublishYear>
                  <Price>30.5</Price>
                </MoreComplexBook2>
                """;
        var book = new MoreComplexBook2();
        book.Author = new Author {
            Name = null,
            Age = 40
        };

        var initialToString = book.ToString();

        var serializer = CreateSerializer<MoreComplexBook2>(new SerializerOptions
            { ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow });
        serializer.SetDeserializationBaseObject(book);
        var bookResult = (MoreComplexBook2?) serializer.Deserialize(result);
        Assert.That(bookResult?.ToString(), Is.Not.EqualTo(initialToString));
    }

    [Test]
    public void DesSerializationOptionsSampleTest()
    {
        object obj = SerializationOptionsSample.GetSampleInstance();
        PerformTest(obj); // uses YAXSerializationOptions.SerializeNullObjects

        var input1 =
            """
                <SerializationOptionsSample>
                  <!-- Str2Null must NOT be serialized when it is null, even -->
                  <!-- if the serialization options of the serializer is changed -->
                  <ObjectWithOptionsSet>
                    <StrNotNull>SomeString</StrNotNull>
                    <!-- StrNull : no element -->
                    <!-- SomeValueType : no element -->
                  </ObjectWithOptionsSet>
                  <!-- Str2Null must be serialized when it is null, even -->
                  <!-- if the serialization options of the serializer is changed -->
                  <AnotherObjectWithOptionsSet>
                    <StrNotNull>Some other string</StrNotNull>
                    <StrNull />
                  </AnotherObjectWithOptionsSet>
                  <!-- serialization of Str2Null must obey the options set -->
                  <!-- in the serializer itself -->
                  <ObjectWithoutOptionsSet>
                    <StrNotNull>Another string</StrNotNull>
                    <StrNull />
                  </ObjectWithoutOptionsSet>
                </SerializationOptionsSample>
                """;

        var serializer = CreateSerializer<SerializationOptionsSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects
        });
        var gottonObject = (SerializationOptionsSample?) serializer.Deserialize(input1);

        Assert.Multiple(() =>
        {
            Assert.That(gottonObject?.ObjectWithOptionsSet.SomeValueType, Is.EqualTo(123),
                    "Missing element: DefaultValue from attribute should be used");
            Assert.That(gottonObject?.ObjectWithOptionsSet.StrNull, Is.Null, "Empty element: Deserializes as null");
            Assert.That(serializer.ParsingErrors.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void DesPathAndAliasAssignmentSampleTest()
    {
        object obj = PathAndAliasAssignmentSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesCollectionSeriallyAsAttributeTest()
    {
        object obj = CollectionSeriallyAsAttribute.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesDictionaryKeyValueAsInterfaceTest()
    {
        object obj = DictionaryKeyValueAsInterface.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesPreserveWhitespaceOnFieldsTest()
    {
        object obj = PreserveWhitespaceOnFieldsSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesPreserveWhitespaceOnClassTest()
    {
        object obj = PreserveWhitespaceOnClassSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesGuidAsBasicTypeTest()
    {
        object obj = GuidAsBasicType.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesDictionaryKeyValueAsContent()
    {
        object obj = DictionaryKeyValueAsContent.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesDictionaryWithExtraProperties()
    {
        object obj = DictionaryWithExtraProperties.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesDictionaryWithExtraPropertiesAttributedAsNotCollection()
    {
        var obj = DictionaryWithExtraPropertiesAttributedAsNotCollection.GetSampleInstance();
        // it is going to ignore the collection members which are not explicitly exposed
        obj.Clear();
        PerformTest(obj);
    }

    [Test]
    public void DesCollectionWithExtraProperties()
    {
        object obj = CollectionWithExtraProperties.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesCollectionWithExtraPropertiesAttributedAsNotCollection()
    {
        var obj = CollectionWithExtraPropertiesAttributedAsNotCollection.GetSampleInstance();
        // it is going to ignore the collection members which are not explicitly exposed
        obj.Clear();
        PerformTest(obj);
    }

    [Test]
    public void DesRectangleDynamicKnownTypeSample()
    {
        var obj = RectangleDynamicKnownTypeSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DesDataSetAndDataTableDynamicKnownTypes()
    {
        var obj = DataSetAndDataTableKnownTypeSample.GetSampleInstance();

        var serializer = CreateSerializer<DataSetAndDataTableKnownTypeSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var gottonObject = serializer.Deserialize(serializer.Serialize(obj));
        Assert.That(obj.ToString(), Is.EqualTo(gottonObject?.ToString()));
    }

    [Test]
    public void AttributeForKeyInDictionaryPropertyTest()
    {
        var container = DictionaryContainerSample.GetSampleInstance();

        var ser = CreateSerializer<DictionaryContainerSample>();

        var input = ser.Serialize(container);

        var deserializedContainer = (DictionaryContainerSample?) ser.Deserialize(input);

        Assert.That(deserializedContainer?.Items, Is.Not.Null);
        Assert.That(deserializedContainer?.Items.Count, Is.EqualTo(container.Items.Count), $"Expected Count: {container.Items.Count}. Actual Count: {deserializedContainer?.Items.Count}");
    }

    [Test]
    public void DeserializingADictionaryDerivedInstance()
    {
        var inst = DictionarySample.GetSampleInstance();

        var ser = CreateSerializer<DictionarySample>();

        var input = ser.Serialize(inst);

        var deserializedInstance = (DictionarySample?) ser.Deserialize(input);

        Assert.That(deserializedInstance, Is.Not.Null);
        Assert.That(deserializedInstance?.Count, Is.EqualTo(inst.Count), $"Expected Count: {inst.Count}. Actual Count: {deserializedInstance?.Count}");
    }

    [Test]
    public void DeserializingOneLetterAliases()
    {
        object obj = OneLetterAlias.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void TestIndexerPropertiesAreNotDeserialized()
    {
        object obj = IndexerSample.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void SingleLetterPropertiesAreSerializedProperly()
    {
        object obj = SingleLetterPropertyNames.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DelegatesAndFunctionPointersMustBeIgnored()
    {
        object obj = DelegateInstances.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void RepetitiveReferencesAreNotLoop()
    {
        object obj = RepetitiveReferenceIsNotLoop.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void SelfReferringTypeIsNotNecessarilyASelfReferringObject()
    {
        object obj = DirectSelfReferringObject.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void IndirectSelfReferringTypeIsNotNecessarilyASelfReferringObject()
    {
        object obj = IndirectSelfReferringObject.GetSampleInstance();
        PerformTest(obj);
    }

    [Test]
    public void DeserializeIndirectSelfReferringObjectWhenThrowUponSerializingCyclingReferencesIsNotSet()
    {
        var inst = IndirectSelfReferringObject.GetSampleInstanceWithLoop();

        var ser = CreateSerializer<IndirectSelfReferringObject>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Error
        });

        var input = ser.Serialize(inst);

        var deserializedInstance = (IndirectSelfReferringObject?) ser.Deserialize(input);

        Assert.That(deserializedInstance, Is.Not.Null);
        Assert.That(deserializedInstance?.Child?.Parent, Is.Null);
    }

    [Test]
    public void DeserializeDirectSelfReferringObjectWhenDontThrowUponSerializingCyclingReferencesIsNotSet()
    {
        var inst = DirectSelfReferringObject.GetSampleInstanceWithCycle();

        var ser = CreateSerializer<DirectSelfReferringObject>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Error
        });

        var input = ser.Serialize(inst);

        var deserializedInstance = (DirectSelfReferringObject?) ser.Deserialize(input);

        Assert.That(deserializedInstance, Is.Not.Null);
        Assert.That(deserializedInstance?.Next?.Next, Is.Null);
    }

    [Test]
    public void DeserializeDirectSelfReferringObjectWithSelfCycleWhenThrowUponSerializingCyclingReferencesIsNotSet()
    {
        var inst = DirectSelfReferringObject.GetSampleInstanceWithSelfCycle();

        var ser = CreateSerializer<DirectSelfReferringObject>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Error
        });

        var input = ser.Serialize(inst);

        var deserializedInstance = (DirectSelfReferringObject?) ser.Deserialize(input);

        Assert.That(deserializedInstance, Is.Not.Null);
        Assert.That(deserializedInstance?.Next, Is.Null);
    }

    [Test]
    public void
        InfiniteLoopCausedBySerializingCalculatedPropertiesCanBePreventedBySettingDontSerializePropertiesWithNoSetter()
    {
        var ser = CreateSerializer<CalculatedPropertiesCanCauseInfiniteLoop>(new SerializerOptions {
            ExceptionBehavior = YAXExceptionTypes.Error,
            SerializationOptions = YAXSerializationOptions.DontSerializePropertiesWithNoSetter
        });
        var result = ser.Serialize(CalculatedPropertiesCanCauseInfiniteLoop.GetSampleInstance());

        var deserializedInstance = ser.Deserialize(result);
        Assert.That(deserializedInstance, Is.Not.Null);
    }

    [Test]
    public void MaxRecursionPreventsInfiniteLoop()
    {
        var options = new SerializerOptions { MaxRecursion = 10 };
        var ser = CreateSerializer<CalculatedPropertiesCanCauseInfiniteLoop>(options);
        var result = ser.Serialize(CalculatedPropertiesCanCauseInfiniteLoop.GetSampleInstance());
        var deserializedInstance = ser.Deserialize(result) as CalculatedPropertiesCanCauseInfiniteLoop;
        Assert.Multiple(() =>
        {
            Assert.That(deserializedInstance, Is.Not.Null);
            Assert.That(ser.Options.MaxRecursion, Is.EqualTo(10));
        });
        Assert.Multiple(() =>
        {
            Assert.That(deserializedInstance?.Data, Is.EqualTo(2.0M));
            Assert.That(ser.GetRecursionCount(), Is.EqualTo(0));
        });
    }

    [Test]
    public void OrderedDeserialization()
    {
        var obj = BookClassWithOrdering.GetSampleInstance();
        obj = (BookClassWithOrdering) PerformTestAndReturn(obj)!;

        obj.DecentralizationOrder.TryGetValue(0, out var first);
        obj.DecentralizationOrder.TryGetValue(1, out var second);
        obj.DecentralizationOrder.TryGetValue(2, out var third);
        obj.DecentralizationOrder.TryGetValue(3, out var fourth);
        obj.DecentralizationOrder.TryGetValue(4, out var fifth);
        obj.DecentralizationOrder.TryGetValue(5, out var sixth);
        obj.DecentralizationOrder.TryGetValue(6, out var seventh);
        Assert.Multiple(() =>
        {
            Assert.That(first, Is.EqualTo("Author"));
            Assert.That(second, Is.EqualTo("Title"));
            Assert.That(third, Is.EqualTo("PublishYear"));
            Assert.That(fourth, Is.EqualTo("Price"));
            Assert.That(fifth, Is.EqualTo("Review"));
            Assert.That(sixth, Is.EqualTo("Publisher"));
            Assert.That(seventh, Is.EqualTo("Editor"));
        });
    }

    [Test]
    public void DeserializingPolymorphicCollectionWithNoContainingElement()
    {
        var ser = CreateSerializer<BaseContainer>();
        var container = new DerivedContainer {
            Items = new[] {
                new BaseItem { Data = "Some Data" }
            }
        };
        var result = ser.Serialize(container);
        var deserializedInstance = (BaseContainer?) ser.Deserialize(result);

        Assert.Multiple(() =>
        {
            Assert.That(deserializedInstance?.Items?[0].Data, Is.EqualTo("Some Data"));
            Assert.That(deserializedInstance?.Items?.Length, Is.EqualTo(1));
        });
    }

    [Test]
    public void DeserializingPolymorphicCollectionWithPolymorphicItems()
    {
        var ser = CreateSerializer<BaseContainer>();
        var container = new BaseContainer {
            Items = new BaseItem[] {
                new DerivedItem { Data = "Some Data" }
            }
        };
        var result = ser.Serialize(container); // This works correct
        var deserializedInstance = (BaseContainer?) ser.Deserialize(result);

        Assert.That(deserializedInstance?.Items?[0], Is.InstanceOf<DerivedItem>());
        Assert.Multiple(() =>
        {
            Assert.That(deserializedInstance?.Items?[0].Data, Is.EqualTo("Some Data"));
            Assert.That(deserializedInstance?.Items?.Length, Is.EqualTo(1));
        });
    }

    [Test]
    public void Global_Option_DontSerializeNullObjects_Should_Serialize_And_Deserialize()
    {
        var serializer = CreateSerializer<SerializationOptionsSample.MissingElementsSample1>(new SerializerOptions
            { SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects });

        var customer = new SerializationOptionsSample.MissingElementsSample1
            { Id = 1234 }; // leave both nullable properties null
        var xml = serializer.Serialize(customer);
        var deserializedCustomer = serializer.Deserialize(xml) as SerializationOptionsSample.MissingElementsSample1;

        xml.Should().NotContain("cust_name", "null string? should not be serialized");
        xml.Should().NotContain("option", "null int? should not be serialized");
        deserializedCustomer.Should()
            .BeEquivalentTo(customer, "Missing elements should deserialize with default values");
    }

    protected abstract IYAXSerializer<object> CreateSerializer<T>(SerializerOptions? options = null);
    protected abstract YAXSerializer CreateSerializer(Type type, SerializerOptions? options = null);

    [Test]
    public void Attribute_Option_DontSerializeNullObjects_Should_Serialize_And_Deserialize()
    {
        var serializer =
            CreateSerializer<SerializationOptionsSample.MissingElementsSample2>(
                new SerializerOptions {
                    // will be overridden by YAXSerializableType attribute in this test
                    SerializationOptions = YAXSerializationOptions.SerializeNullObjects
                });

        var customer = new SerializationOptionsSample.MissingElementsSample2
            { Id = 1234 }; // leave both nullable properties null
        var xml = serializer.Serialize(customer);
        var deserializedCustomer = serializer.Deserialize(xml);

        xml.Should().NotContain("cust_name", "null string? should not be serialized");
        xml.Should().NotContain("option", "null int? should not be serialized");
        deserializedCustomer.Should()
            .BeEquivalentTo(customer, "Missing elements should deserialize with default values");
    }

    [Test]
    public void Attribute_Option_DontSerializeDefaultValues_Should_Serialize_And_Deserialize()
    {
        var serializer =
            CreateSerializer<SerializationOptionsSample.MissingElementsSample3>();

        var record = new SerializationOptionsSample.MissingElementsSample3
        { Id = 1234 }; // leave count property zero and count2 property null
        var xml = serializer.Serialize(record);
        var deserializedRecord = serializer.Deserialize(xml);

        xml.Should().NotContain("rec_cnt", "0 integer should not be serialized");
        xml.Should().NotContain("rec_cnt2", "null int? should not be serialized");
        deserializedRecord.Should()
            .BeEquivalentTo(record, "Missing elements should deserialize with default values");
    }

    [Test]
    public void DeserializeFromFile()
    {
        var book = Book.GetSampleInstance();
        var file = Path.GetTempFileName();

        var serializer = CreateSerializer<Book>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });

        serializer.SerializeToFile(book, file);

        var deserializedBook = (Book?) serializer.DeserializeFromFile(file);
        deserializedBook.Should().BeEquivalentTo(book);
    }

    [Test]
    public void DeserializeFromXElement()
    {
        var book = Book.GetSampleInstance();
        var serializer = CreateSerializer<Book>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var xDocument = serializer.SerializeToXDocument(book);

        var deserializedBook = serializer.Deserialize(xDocument.Root!);
        deserializedBook.Should().BeEquivalentTo(book);
    }

    [Test]
    public void DeserializeFromTextReader()
    {
        var book = Book.GetSampleInstance();
        var serializer = CreateSerializer<Book>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });

        using var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream);
        using var streamReader = new StreamReader(stream);
        serializer.Serialize(Book.GetSampleInstance(), streamWriter);
        streamWriter.Flush();
        stream.Position = 0;

        var deserializedBook = serializer.Deserialize(streamReader);
        deserializedBook.Should().BeEquivalentTo(book);
    }

    [Test]
    public void DeserializeFromXmlReader()
    {
        var book = Book.GetSampleInstance();
        var serializer = CreateSerializer<Book>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });

        using var stream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(stream);
        serializer.Serialize(Book.GetSampleInstance(), xmlWriter);
        xmlWriter.Flush();
        stream.Position = 0;

        var xmlReader = XmlReader.Create(stream);
        var deserializedBook = serializer.Deserialize(xmlReader);
        deserializedBook.Should().BeEquivalentTo(book);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SetDeserializationBaseObject(bool isGeneric)
    {
        var serializer = isGeneric ? CreateSerializer<Book>() : CreateSerializer(typeof(Book));

        Assert.That(code: () => serializer.SetDeserializationBaseObject(Book.GetSampleInstance()), Throws.Nothing);
        Assert.That(code: () => serializer.SetDeserializationBaseObject(null), Throws.Nothing);
        Assert.That(code: () => serializer.SetDeserializationBaseObject(""),
            Throws.InstanceOf<YAXLib.Exceptions.YAXObjectTypeMismatch>().Or.InstanceOf<InvalidCastException>());
    }
}
