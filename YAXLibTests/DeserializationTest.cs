// Copyright 2009 - 2014 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using NUnit.Framework;

using YAXLib;
using YAXLibTests.SampleClasses;
using YAXLibTests.SampleClasses.PolymorphicSerialization;
using YAXLibTests.SampleClasses.SelfReferencingObjects;

namespace YAXLibTests
{
    [TestFixture]
    public class DeserializationTest
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private void PerformTest(object obj)
        {
            PerformTestAndReturn(obj);
        }

        /// <summary>
        /// Perform test by using .Equals. Equals should be well implemented for <paramref name="obj"/>
        /// </summary>
        /// <param name="obj"></param>
        private void PerformTestWithEquals(object obj)
        {
            object gottonObject;
            var serializer = SerializeDeserialize(obj, out gottonObject);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
            Assert.AreEqual(obj, gottonObject);
        }

        private object GetTheTwoStringsAndReturn(object obj, out string originalString, out string gottonString, out int errorCounts)
        {
            originalString = GeneralToStringProvider.GeneralToString(obj);
            object gottonObject;
            var serializer = SerializeDeserialize(obj, out gottonObject);
            errorCounts = serializer.ParsingErrors.Count;
            gottonString = GeneralToStringProvider.GeneralToString(gottonObject);
            return gottonObject;
        }

        private static YAXSerializer SerializeDeserialize(object obj, out object gottonObject)
        {
            var serializer = new YAXSerializer(obj.GetType(), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            var serResult = serializer.Serialize(obj);
            gottonObject = serializer.Deserialize(serResult);
            return serializer;
        }

        private object PerformTestAndReturn(object obj)
        {
            string originalString, gottonString;
            int errorCounts;
            var result = GetTheTwoStringsAndReturn(obj, out originalString, out gottonString, out errorCounts);
            Assert.That(originalString, Is.Not.Null);
            Assert.That(gottonString, Is.Not.Null);
            Assert.That(gottonString, Is.EqualTo(originalString));
            Assert.That(errorCounts, Is.EqualTo(0));
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
            Guid g1 = Guid.NewGuid();
            Guid g2 = Guid.NewGuid();
            Guid g3 = Guid.NewGuid();
            Guid g4 = Guid.NewGuid();

            object obj = GUIDTest.GetSampleInstance(g1, g2, g3, g4);
            PerformTest(obj);
        }

        [Test]
        public void DesEmptyNullableTest()
        {
            const string xml = @"<NullableSample2 />";
            YAXSerializer serializer = new YAXSerializer(typeof(NullableSample2), YAXExceptionHandlingPolicies.DoNotThrow);
            NullableSample2 got = (NullableSample2)serializer.Deserialize(xml);

            Assert.That(got, Is.Not.Null);
            Assert.That(got.Boolean, Is.Null);
            Assert.That(got.DateTime, Is.Null);
            Assert.That(got.Decimal, Is.Null);
            Assert.That(got.Enum, Is.Null);
            Assert.That(got.Number, Is.Null);
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
            string result =
@"<MoreComplexBook2 Author_s_Name=""Tom Archer"">
  <Title>Inside C#</Title>
  <PublishYear>2002</PublishYear>
  <Price>30.5</Price>
</MoreComplexBook2>";
            MoreComplexBook2 book = new MoreComplexBook2();
            book.Author = new Author()
            {
                Name = null,
                Age = 40
            };

            string initialToString = book.ToString();

            YAXSerializer serializer = new YAXSerializer(typeof(MoreComplexBook2), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            serializer.SetDeserializationBaseObject(book);
            MoreComplexBook2 bookResult = (MoreComplexBook2)serializer.Deserialize(result);
            Assert.AreNotEqual(bookResult.ToString(), initialToString);
        }

        [Test]
        public void DesSerializationOptionsSampleTest()
        {
            object obj = SerializationOptionsSample.GetSampleInstance();
            PerformTest(obj);

            string input1 =
@"<SerializationOptionsSample>
  <!-- Str2Null must NOT be serialized when it is null, even -->
  <!-- if the serialization options of the serializer is changed -->
  <ObjectWithOptionsSet>
    <StrNotNull>SomeString</StrNotNull>
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
</SerializationOptionsSample>";

            var serializer = new YAXSerializer(typeof(SerializationOptionsSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.DontSerializeNullObjects);
            var gottonObject = serializer.Deserialize(input1) as SerializationOptionsSample;

            Assert.That(123, Is.EqualTo(gottonObject.ObjectWithOptionsSet.SomeValueType));
            Assert.That(gottonObject.ObjectWithOptionsSet.StrNull, Is.Null);
            Assert.That(1, Is.EqualTo(serializer.ParsingErrors.Count));
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
            var obj = YAXLibTests.SampleClasses.RectangleDynamicKnownTypeSample.GetSampleInstance();
            PerformTest(obj);
        }
        
        [Test]
        public void DesDataSetAndDataTableDynamicKnownTypes()
        {
            var obj = DataSetAndDataTableKnownTypeSample.GetSampleInstance();

            var serializer = new YAXSerializer(obj.GetType(), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            object gottonObject = serializer.Deserialize(serializer.Serialize(obj));
            Assert.That(obj.ToString(), Is.EqualTo(gottonObject.ToString()));
        }

        [Test]
        public void AttributeForKeyInDictionaryPropertyTest()
        {
            var container = DictionaryContainerSample.GetSampleInstance();

            var ser = new YAXSerializer(typeof(DictionaryContainerSample));

            string input = ser.Serialize(container);

            var deserializedContainer = (DictionaryContainerSample)ser.Deserialize(input);

            Assert.IsNotNull(deserializedContainer.Items);
            Assert.IsTrue(deserializedContainer.Items.Count == container.Items.Count,
                          "Expected Count: {0}. Actual Count: {1}",
                          container.Items.Count,
                          deserializedContainer.Items.Count);
        }

        [Test]
        public void DeserializingADictionaryDerivedInstance()
        {
            var inst = DictionarySample.GetSampleInstance();

            var ser = new YAXSerializer(typeof(DictionarySample));

            string input = ser.Serialize(inst);

            var deserializedInstance = (DictionarySample)ser.Deserialize(input);

            Assert.That(deserializedInstance, Is.Not.Null);
            Assert.IsTrue(deserializedInstance.Count == inst.Count,
                          "Expected Count: {0}. Actual Count: {1}",
                          inst.Count,
                          deserializedInstance.Count);
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

            var ser = new YAXSerializer(typeof(IndirectSelfReferringObject),
                YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Error);

            string input = ser.Serialize(inst);

            var deserializedInstance = (IndirectSelfReferringObject)ser.Deserialize(input);

            Assert.That(deserializedInstance, Is.Not.Null);
            Assert.IsNull(deserializedInstance.Child.Parent);
        }

        [Test]
        public void DeserializeDirectSelfReferringObjectWhenDontThrowUponSerializingCyclingReferencesIsNotSet()
        {
            var inst = DirectSelfReferringObject.GetSampleInstanceWithCycle();

            var ser = new YAXSerializer(typeof(DirectSelfReferringObject),
                YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Error);

            string input = ser.Serialize(inst);

            var deserializedInstance = (DirectSelfReferringObject)ser.Deserialize(input);

            Assert.That(deserializedInstance, Is.Not.Null);
            Assert.IsNull(deserializedInstance.Next.Next);
        }

        [Test]
        public void DeserializeDirectSelfReferringObjectWithSelfCycleWhenThrowUponSerializingCyclingReferencesIsNotSet()
        {
            var inst = DirectSelfReferringObject.GetSampleInstanceWithSelfCycle();

            var ser = new YAXSerializer(typeof(DirectSelfReferringObject),
                YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Error);

            string input = ser.Serialize(inst);

            var deserializedInstance = (DirectSelfReferringObject)ser.Deserialize(input);

            Assert.That(deserializedInstance, Is.Not.Null);
            Assert.IsNull(deserializedInstance.Next);
        }

        [Test]
        public void InfiniteLoopCausedBySerializingCalculatedPropertiesCanBePreventedBySettingDontSerializePropertiesWithNoSetter()
        {
            var ser = new YAXSerializer(typeof(CalculatedPropertiesCanCauseInfiniteLoop), YAXSerializationOptions.DontSerializePropertiesWithNoSetter);
            string result = ser.Serialize(CalculatedPropertiesCanCauseInfiniteLoop.GetSampleInstance());

            var deserialzedInstance = ser.Deserialize(result) as CalculatedPropertiesCanCauseInfiniteLoop;
            Assert.IsNotNull(deserialzedInstance);
        }

        [Test]
        public void MaxRecursionPreventsInfiniteLoop()
        {
            var ser = new YAXSerializer(typeof(CalculatedPropertiesCanCauseInfiniteLoop));
            ser.MaxRecursion = 10;
            string result = ser.Serialize(CalculatedPropertiesCanCauseInfiniteLoop.GetSampleInstance());
            var deserialzedInstance = ser.Deserialize(result) as CalculatedPropertiesCanCauseInfiniteLoop;
            Assert.IsNotNull(deserialzedInstance);
            Assert.AreEqual(2.0M, deserialzedInstance.Data);
        }

        [Test]
        public void OrderedDeserialization()
        {
            var obj = BookClassWithOrdering.GetSampleInstance();
            obj = (BookClassWithOrdering)PerformTestAndReturn(obj);
            var first = "";
            var second = "";
            var third = "";
            var fourth = "";
            var fifth = "";
            var sixth = "";
            var seventh = "";
            obj.DecentralizationOrder.TryGetValue(0, out first);
            obj.DecentralizationOrder.TryGetValue(1, out second);
            obj.DecentralizationOrder.TryGetValue(2, out third);
            obj.DecentralizationOrder.TryGetValue(3, out fourth);
            obj.DecentralizationOrder.TryGetValue(4, out fifth);
            obj.DecentralizationOrder.TryGetValue(5, out sixth);
            obj.DecentralizationOrder.TryGetValue(6, out seventh);
            Assert.AreEqual(first, "Author");
            Assert.AreEqual(second, "Title");
            Assert.AreEqual(third, "PublishYear");
            Assert.AreEqual(fourth, "Price");
            Assert.AreEqual(fifth, "Review");
            Assert.AreEqual(sixth, "Publisher");
            Assert.AreEqual(seventh, "Editor");
        }

        [Test]
        public void DeserializingPolymorphicCollectionWithNoContainingElement()
        {
            var ser = new YAXSerializer(typeof(BaseContainer));
            var container = new DerivedContainer
            {
                Items = new BaseItem[]
                {
                    new BaseItem { Data = "Some Data" }
                }
            };
            string result = ser.Serialize(container);
            var deserialzedInstance = ser.Deserialize(result) as BaseContainer;

            Assert.That(deserialzedInstance.Items[0].Data, Is.EqualTo("Some Data"));
            Assert.That(deserialzedInstance.Items.Length, Is.EqualTo(1));
        }

        [Test]
        public void DeserializingPolymorphicCollectionWithPolymorphicItems()
        {
            var ser = new YAXSerializer(typeof(BaseContainer));
            var container = new BaseContainer
            {
                Items = new BaseItem[]
                {
                    new DerivedItem { Data = "Some Data" }
                }
            };
            string result = ser.Serialize(container); // This works correct
            var deserialzedInstance = ser.Deserialize(result) as BaseContainer;

            Assert.That(deserialzedInstance.Items[0], Is.InstanceOf<DerivedItem>());
            Assert.That(deserialzedInstance.Items[0].Data, Is.EqualTo("Some Data"));
            Assert.That(deserialzedInstance.Items.Length, Is.EqualTo(1));
        }

    }
}
