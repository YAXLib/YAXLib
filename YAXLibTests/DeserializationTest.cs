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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YAXLib;
using DemoApplication.SampleClasses;
using DemoApplication;

namespace YAXLibTests
{
    [TestClass]
    public class DeserializationTest
    {
        private void GetTheTwoStrings(object obj, out string originalString, out string gottonString, out int errorCounts)
        {
            originalString = GeneralToStringProvider.GeneralToString(obj);
            YAXSerializer serializer = new YAXSerializer(obj.GetType(), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            object gottonObject = serializer.Deserialize(serializer.Serialize(obj));
            errorCounts = serializer.ParsingErrors.Count;
            gottonString = GeneralToStringProvider.GeneralToString(gottonObject);
        }

        private void PerformTest(object obj)
        {
            string originalString, gottonString;
            int errorCounts;
            GetTheTwoStrings(obj, out originalString, out gottonString, out errorCounts);
            Assert.IsNotNull(originalString);
            Assert.IsNotNull(gottonString);
            Assert.AreEqual(originalString, gottonString);
            Assert.AreEqual(errorCounts, 0);
        }

        [TestMethod]
        public void DesBookTest()
        {
            PerformTest(Book.GetSampleInstance());
        }

        [TestMethod]
        public void DesBookStructTest()
        {
            object obj = BookStruct.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesWarehouseSimpleTest()
        {
            object obj = WarehouseSimple.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesWarehouseStructuredTest()
        {
            object obj = WarehouseStructured.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesWarehouseWithArrayTest()
        {
            object obj = DemoApplication.SampleClasses.WarehouseWithArray.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesWarehouseWithDictionaryTest()
        {
            object obj = DemoApplication.SampleClasses.WarehouseWithDictionary.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesWarehouseNestedObjectTest()
        {
            object obj = DemoApplication.SampleClasses.WarehouseNestedObjectExample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesProgrammingLanguageTest()
        {
            object obj = DemoApplication.SampleClasses.ProgrammingLanguage.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesColorExampleTest()
        {
            object obj = DemoApplication.SampleClasses.ColorExample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesMultiLevelClassTest()
        {
            object obj = DemoApplication.SampleClasses.MultilevelClass.GetSampleInstance();
            PerformTest(obj);
        }


        [TestMethod]
        public void DesPathsExampleTest()
        {
            object obj = DemoApplication.SampleClasses.PathsExample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesMoreComplexExampleTest()
        {
            object obj = DemoApplication.SampleClasses.MoreComplexExample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesNestedDicSampleTest()
        {
            object obj = DemoApplication.SampleClasses.NestedDicSample.GetSampleInstance();
            PerformTest(obj);

        }

        [TestMethod]
        public void DesGUIDTestTest()
        {
            Guid g1 = Guid.NewGuid();
            Guid g2 = Guid.NewGuid();
            Guid g3 = Guid.NewGuid();
            Guid g4 = Guid.NewGuid();

            object obj = DemoApplication.SampleClasses.GUIDTest.GetSampleInstance(g1,g2,g3,g4);
            PerformTest(obj);
        }

        [TestMethod]
        public void DesNullableTest()
        {
            object obj = DemoApplication.SampleClasses.NullableClass.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesNullableSample2Test()
        {
            object obj = DemoApplication.SampleClasses.NullableSample2.GetSampleInstance();
            PerformTest(obj);
        }


        [TestMethod]
        public void DesListHolderClassTest()
        {
            object obj = DemoApplication.SampleClasses.ListHolderClass.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesStandaloneListTest()
        {
            object obj = ListHolderClass.GetSampleInstance().ListOfStrings;
            PerformTest(obj);
        }

        [TestMethod]
        public void DesNamesExampleTest()
        {
            object obj = DemoApplication.SampleClasses.NamesExample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesRequestTest()
        {
            object obj = DemoApplication.SampleClasses.Request.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesAudioSampleTest()
        {
            object obj = DemoApplication.SampleClasses.AudioSample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesTimeSpanTest()
        {
            object obj = DemoApplication.SampleClasses.TimeSpanSample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesMoreComplexBookTest()
        {
            object obj = DemoApplication.SampleClasses.MoreComplexBook.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesMoreComplexBookTwoTest()
        {
            object obj = DemoApplication.SampleClasses.MoreComplexBook2.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesMoreComplexBookThreeTest()
        {
            object obj = DemoApplication.SampleClasses.MoreComplexBook3.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesWarehouseWithDictionaryNoContainerTest()
        {
            object obj = DemoApplication.SampleClasses.WarehouseWithDictionaryNoContainer.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesWarehouseWithCommentsTest()
        {
            object obj = DemoApplication.SampleClasses.WarehouseWithComments.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesEnumsSampleTest()
        {
            object obj = DemoApplication.SampleClasses.EnumsSample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesMultiDimArraySampleTest()
        {
            object obj = DemoApplication.SampleClasses.MultiDimArraySample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesAnotherArraySampleTest()
        {
            object obj = DemoApplication.SampleClasses.AnotherArraySample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesCollectionOfInterfacesSampleTest()
        {
            object obj = DemoApplication.SampleClasses.CollectionOfInterfacesSample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesInterfaceMatchingSampleTest()
        {
            object obj = DemoApplication.SampleClasses.InterfaceMatchingSample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesNonGenericCollectionsSampleTest()
        {
            object obj = DemoApplication.SampleClasses.NonGenericCollectionsSample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
        public void DesGenericCollectionsSampleTest()
        {
            object obj = DemoApplication.SampleClasses.GenericCollectionsSample.GetSampleInstance();
            PerformTest(obj);
        }

        [TestMethod]
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

        [TestMethod]
        public void DesSerializationOptionsSampleTest()
        {
            object obj = DemoApplication.SampleClasses.SerializationOptionsSample.GetSampleInstance();
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

            YAXSerializer serializer = new YAXSerializer(typeof(SerializationOptionsSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.DontSerializeNullObjects);
            SerializationOptionsSample gottonObject = serializer.Deserialize(input1) as SerializationOptionsSample;

            Assert.AreEqual(gottonObject.ObjectWithOptionsSet.SomeValueType, 123);
            Assert.IsNull(gottonObject.ObjectWithOptionsSet.StrNull);
            Assert.AreEqual(serializer.ParsingErrors.Count, 1);
        }

    }
}
