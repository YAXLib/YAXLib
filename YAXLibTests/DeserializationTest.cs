// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
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
using System.Threading;

using NUnit.Framework;

using YAXLib;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    [TestFixture]
    public class DeserializationTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private void GetTheTwoStrings(object obj, out string originalString, out string gottonString, out int errorCounts)
        {
            originalString = GeneralToStringProvider.GeneralToString(obj);
            var serializer = new YAXSerializer(obj.GetType(), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            object gottonObject = serializer.Deserialize(serializer.Serialize(obj));
            errorCounts = serializer.ParsingErrors.Count;
            gottonString = GeneralToStringProvider.GeneralToString(gottonObject);
        }

        private void PerformTest(object obj)
        {
            string originalString, gottonString;
            int errorCounts;
            GetTheTwoStrings(obj, out originalString, out gottonString, out errorCounts);
            Assert.That(originalString, Is.Not.Null);
            Assert.That(gottonString, Is.Not.Null);
            Assert.That(gottonString, Is.EqualTo(originalString));
            Assert.That(errorCounts, Is.EqualTo(0));
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

            object obj = GUIDTest.GetSampleInstance(g1,g2,g3,g4);
            PerformTest(obj);
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
            PerformTest(obj);
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
            Assert.IsNull(gottonObject.ObjectWithOptionsSet.StrNull);
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

    }
}
