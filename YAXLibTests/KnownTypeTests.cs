// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Threading;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using YAXLib;
using YAXLib.Customization;
using YAXLib.Enums;
using YAXLib.Exceptions;
using YAXLib.KnownTypes;
using YAXLib.Options;
using YAXLibTests.SampleClasses;
using YAXLibTests.SampleClasses.KnownTypes;
using YAXLibTests.SampleClasses.YAXLibTests.SampleClasses;

namespace YAXLibTests;

[TestFixture]
public class KnownTypeTests
{
    [Test]
    public void WellKnownType_Remove()
    {
        var existsBefore = WellKnownTypes.TryGetKnownType(typeof(TimeSpan), out _);
        var removeSuccess = WellKnownTypes.Remove(typeof(TimeSpan));
        var existsAfter = WellKnownTypes.TryGetKnownType(typeof(TimeSpan), out _);

        Assert.Multiple(() =>
        {
            Assert.That(existsBefore, Is.True);
            Assert.That(removeSuccess, Is.True);
            Assert.That(existsAfter, Is.False);
        });
    }

    [Test]
    public void WellKnownType_Add()
    {
        WellKnownTypes.Clear();

        WellKnownTypes.Add(new TimeSpanKnownType());
        WellKnownTypes.Add(new ColorDynamicKnownType());
        WellKnownTypes.Add(new ExceptionKnownBaseType());

        Assert.That(() => WellKnownTypes.TryGetKnownType(typeof(TimeSpan), out _), Is.True);
        Assert.That(() => WellKnownTypes.TryGetKnownType(typeof(Color), out _), Is.True);
        Assert.That(() => WellKnownTypes.TryGetKnownType(typeof(Exception), out _), Is.True);
    }

    [Test]
    public void WellKnownType_RestoreDefault()
    {
        WellKnownTypes.Remove(typeof(TimeSpan));
        WellKnownTypes.RestoreDefault();

        Assert.That(() => WellKnownTypes.TryGetKnownType(typeof(TimeSpan), out _), Is.True);
        Assert.That(() => WellKnownTypes.TryGetKnownType(null, out _), Is.False);
    }

    [Test]
    public void TestWrappers()
    {
        var typeToTest = typeof(TimeSpan);
        var serializer = new YAXSerializer(typeToTest);
        var typeWrapper = new UdtWrapper(typeToTest, serializer.Options);

        Assert.That(typeWrapper.IsKnownType, Is.True);
    }

    [Test]
    public void TypeKnownTypeSerialization()
    {
        const string expectedXml = "<TheType>YAXLibTests.KnownTypeTests</TheType>";

        var typeExample = TypeKnownTypeSample.GetSampleInstance();
        var s = new TypeKnownType();
        var serialized = new XElement(XName.Get(nameof(TypeKnownTypeSample.TheType)));
        // Context is a required argument, but it's not used here
        var discardCtx = new SerializationContext(null,
            new UdtWrapper(typeof(TypeKnownTypeSample), new SerializerOptions()),
            new YAXSerializer(typeof(int)));

        s.Serialize(typeExample.TheType, serialized, XNamespace.None, discardCtx);
        var deserialized = s.Deserialize(serialized, XNamespace.None, discardCtx);

        Assert.Multiple(() =>
        {
            Assert.That(serialized.ToString(), Is.EqualTo(expectedXml));
            Assert.That(deserialized!.ToString(), Is.EqualTo("YAXLibTests.KnownTypeTests"));
        });
    }

    [Test]
    public void TypeKnownTypeSerialization_Using_Serializer()
    {
        var typeExample = TypeKnownTypeSample.GetSampleInstance();
        var serializer = new YAXSerializer(typeof(TypeKnownTypeSample));
        const string expectedXml = """
            <TypeKnownTypeSample>
              <TheType>YAXLibTests.KnownTypeTests</TheType>
            </TypeKnownTypeSample>
            """;

        var serialized = serializer.Serialize(typeExample);
        var deserialized = (TypeKnownTypeSample?) serializer.Deserialize(serialized);

        Assert.Multiple(() =>
        {
            Assert.That(serialized, Is.EqualTo(expectedXml));
            Assert.That(deserialized?.TheType?.UnderlyingSystemType, Is.EqualTo(typeExample.TheType?.UnderlyingSystemType));
        });
    }

    [Test]
    public void KnownType_UsingSerializationContext()
    {
        const string expectedXml = """
            <UsingSerializationContextSample>
              <Text>Sample Text KnownType</Text>
            </UsingSerializationContextSample>
            """;
        WellKnownTypes.Add(new UsingSerializationContextKnownType());
        var data = new UsingSerializationContextSample { Text = "Sample Text" };
        var serializer = new YAXSerializer(typeof(UsingSerializationContextSample));

        // Known type adds/removes " KnownType" to/from Text property
        // to make sure it is actually invoked
        var serialized = serializer.Serialize(data);
        var deserialized = (UsingSerializationContextSample?) serializer.Deserialize(serialized);

        Assert.That(serialized, Is.EqualTo(expectedXml));
        deserialized.Should().BeEquivalentTo(data);

        WellKnownTypes.Remove(typeof(UsingSerializationContextKnownType));
    }

    [Test]
    public void RuntimeTypeDynamicKnownTypeSerialization()
    {
        const string expectedXml = "<TheType>YAXLibTests.KnownTypeTests</TheType>";

        var typeExample = TypeKnownTypeSample.GetSampleInstance();
        var s = new RuntimeTypeDynamicKnownType();
        var serialized = new XElement(XName.Get(nameof(TypeKnownTypeSample.TheType)));
        // Context is a required argument, but it's not used here
        var discardCtx = new SerializationContext(null, new UdtWrapper(typeof(XElement), new SerializerOptions()),
            new YAXSerializer(typeof(int)));

        s.Serialize(typeExample.TheType, serialized, XNamespace.None, discardCtx);
        var deserialized = (Type?) s.Deserialize(serialized, XNamespace.None, discardCtx);

        Assert.Multiple(() =>
        {
            Assert.That(serialized.ToString(), Is.EqualTo(expectedXml));
            Assert.That(deserialized?.ToString(), Is.EqualTo("YAXLibTests.KnownTypeTests"));
        });
    }

    [Test]
    public void RuntimeTypeDynamicKnownTypeSerialization_Using_Serializer()
    {
        const string expectedXml =
            """<Object yaxlib:realtype="System.RuntimeType" xmlns:yaxlib="http://www.sinairv.com/yaxlib/">YAXLibTests.KnownTypeTests</Object>""";
        object t = GetType(); // this test class
        var serializer = new YAXSerializer(typeof(object));

        var serialized = serializer.Serialize(t);
        var deserialized = serializer.Deserialize(serialized);

        Assert.Multiple(() =>
        {
            Assert.That(serialized, Is.EqualTo(expectedXml));
            Assert.That(deserialized?.ToString(), Is.EqualTo("YAXLibTests.KnownTypeTests"));
        });
    }

    [Test]
    public void DateOnlyKnownTypeSerialization()
    {
        const string expectedXml = """
            <DateOnly>
              <DayNumber>738884</DayNumber>
            </DateOnly>
            """;
        var date = new DateOnly(2023, 12, 31);
        var serializer = new YAXSerializer(typeof(DateOnly));
        var serialized = serializer.Serialize(date);

        Assert.That(serialized, Is.EqualTo(expectedXml));
    }

    [Test]
    public void DateOnlyKnownTypeDeserialization()
    {
        var date = new DateOnly(2023, 12, 31);
        var xml = $"""
            <DateOnly>
              <DayNumber>{date.DayNumber}</DayNumber>
            </DateOnly>
            """;
        
        var serializer = new YAXSerializer(typeof(DateOnly));
        var deserialized = serializer.Deserialize(xml);

        Assert.That(deserialized, Is.EqualTo(date));
    }

    [Test]
    public void DateOnlyKnownTypeDeserializationFallback()
    {
        var date = new DateOnly(2023, 12, 31);
        var xml = $"""
            <DateOnly>
            {date.DayNumber}
            </DateOnly>
            """;
        
        var serializer = new YAXSerializer(typeof(DateOnly));
        var deserialized = serializer.Deserialize(xml);

        Assert.That(deserialized, Is.EqualTo(date));
    }

    [Test]
    public void DateOnly_Bad_Format_Should_Throw()
    {
        var xml1 = "<DateOnly>not-an-int</DateOnly>";
        var xml2 = "<DateOnly><Ticks>not-an-int</Ticks></DateOnly>";
        var serializer = new YAXSerializer<DateOnly>();

        Assert.That(code: () => serializer.Deserialize(xml1), Throws.TypeOf<YAXBadlyFormedInput>());
        Assert.That(code: () => serializer.Deserialize(xml2), Throws.TypeOf<YAXBadlyFormedInput>());
    }

    [Test]
    public void TimeOnlyKnownTypeSerialization()
    {
        const string expectedXml = """
            <TimeOnly>
              <Ticks>183670890000</Ticks>
            </TimeOnly>
            """;
        var time = new TimeOnly(5,6, 7, 89);
        var serializer = new YAXSerializer(typeof(TimeOnly));
        var serialized = serializer.Serialize(time);

        Assert.That(serialized, Is.EqualTo(expectedXml));
    }

    [Test]
    public void TimeOnlyKnownTypeDeserialization()
    {
        var time = new TimeOnly(5,6, 7, 89);
        var xml = $"""
            <TimeOnly>
              <Ticks>{time.Ticks}</Ticks>
            </TimeOnly>
            """;
        
        var serializer = new YAXSerializer(typeof(TimeOnly));
        var deserialized = serializer.Deserialize(xml);

        Assert.That(deserialized, Is.EqualTo(time));
    }

    [Test]
    public void TimeOnlyKnownTypeDeserializationFallback()
    {
        var time = new TimeOnly(5,6, 7, 89);
        var xml = $"""
            <TimeOnly>
            {time.Ticks}
            </TimeOnly>
            """;
        
        var serializer = new YAXSerializer(typeof(TimeOnly));
        var deserialized = serializer.Deserialize(xml);

        Assert.That(deserialized, Is.EqualTo(time));
    }

    [Test]
    public void TimeOnly_Bad_Format_Should_Throw()
    {
        var xml1 = "<TimeOnly>not-a-long</TimeOnly>";
        var xml2 = "<TimeOnly><Ticks>not-a-long</Ticks></TimeOnly>";
        var serializer = new YAXSerializer<TimeOnly>();

        Assert.That(code: () => serializer.Deserialize(xml1), Throws.TypeOf<YAXBadlyFormedInput>());
        Assert.That(code: () => serializer.Deserialize(xml2), Throws.TypeOf<YAXBadlyFormedInput>());
    }

    [Test]
    public void DbNullKnownTypeSerialization()
    {
        const string expectedXml = "<dbNullExample>DBNull</dbNullExample>";

        var dbNullExample = DBNull.Value;
        var s = new DbNullKnownType();
        var serialized = new XElement(XName.Get(nameof(dbNullExample)));
        // Context is a required argument, but it's not used here
        var discardCtx = new SerializationContext(null, new UdtWrapper(typeof(DBNull), new SerializerOptions()),
            new YAXSerializer(typeof(int)));

        s.Serialize(dbNullExample, serialized, XNamespace.None, discardCtx);
        var deserialized = s.Deserialize(serialized, XNamespace.None, discardCtx);
        var deserializedAsNull =
            s.Deserialize(new XElement(XName.Get(nameof(dbNullExample))), XNamespace.None, discardCtx);

        Assert.Multiple(() =>
        {
            Assert.That(serialized.ToString(), Is.EqualTo(expectedXml));
            Assert.That(deserialized, Is.TypeOf<DBNull>());
            Assert.That(deserializedAsNull, Is.Null);
        });
    }

    [Test]
    public void TestSingleKnownTypeSerialization()
    {
        var typeToTest = typeof(Color);
        var serializer = new YAXSerializer(typeToTest);

        var col1 = Color.FromArgb(145, 123, 123);
        var colStr1 = serializer.Serialize(col1);

        const string expectedCol1 = """
            <Color>
              <A>255</A>
              <R>145</R>
              <G>123</G>
              <B>123</B>
            </Color>
            """;

        Assert.That(colStr1, Is.EqualTo(expectedCol1));

        var col2 = SystemColors.ButtonFace;
        var colStr2 = serializer.Serialize(col2);
        const string expectedCol2 = """<Color>ButtonFace</Color>""";

        Assert.That(colStr2, Is.EqualTo(expectedCol2));
    }

    [Test]
    public void SerializingXKnownTypesAsNull()
    {
        var inst = ClassContainingXElement.GetSampleInstance();
        inst.TheElement = null;
        inst.TheAttribute = null;

        var ser = new YAXSerializer<ClassContainingXElement>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowErrorsOnly,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });

        var xml = ser.Serialize(inst);
        var deserializedInstance = ser.Deserialize(xml);
        Assert.That(deserializedInstance?.ToString(), Is.EqualTo(inst.ToString()));
    }

    [Test]
    public void RectangleSerializationTest()
    {
        const string result =
            """
                <RectangleDynamicKnownTypeSample>
                  <Rect>
                    <Left>10</Left>
                    <Top>20</Top>
                    <Width>30</Width>
                    <Height>40</Height>
                  </Rect>
                </RectangleDynamicKnownTypeSample>
                """;

        var serializer = new YAXSerializer<RectangleDynamicKnownTypeSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(RectangleDynamicKnownTypeSample.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void DataSetAndDataTableSerializationTest()
    {
        const string result =
            """
                <DataSetAndDataTableKnownTypeSample>
                  <TheDataTable>
                    <NewDataSet>
                      <TableName xmlns="http://tableNs/">
                        <Col1>1</Col1>
                        <Col2>2</Col2>
                        <Col3>3</Col3>
                      </TableName>
                      <TableName xmlns="http://tableNs/">
                        <Col1>y</Col1>
                        <Col2>4</Col2>
                        <Col3>n</Col3>
                      </TableName>
                    </NewDataSet>
                  </TheDataTable>
                  <TheDataSet>
                    <MyDataSet>
                      <Table1>
                        <Cl1>num1</Cl1>
                        <Cl2>34</Cl2>
                      </Table1>
                      <Table1>
                        <Cl1>num2</Cl1>
                        <Cl2>54</Cl2>
                      </Table1>
                      <Table2>
                        <C1>one</C1>
                        <C2>1</C2>
                        <C3>1.5</C3>
                      </Table2>
                      <Table2>
                        <C1>two</C1>
                        <C2>2</C2>
                        <C3>2.5</C3>
                      </Table2>
                    </MyDataSet>
                  </TheDataSet>
                </DataSetAndDataTableKnownTypeSample>
                """;

        var serializer = new YAXSerializer<DataSetAndDataTableKnownTypeSample>(new SerializerOptions {
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
            ExceptionBehavior = YAXExceptionTypes.Warning,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var got = serializer.Serialize(DataSetAndDataTableKnownTypeSample.GetSampleInstance());
        Assert.That(got, Is.EqualTo(result));
    }

    [Test]
    public void TestExtensionMethod()
    {
        var colorKnownType = new ColorDynamicKnownType();
        var t1 = colorKnownType.Type;
        IKnownType kt = new ColorDynamicKnownType();

        Assert.That(kt.Type, Is.EqualTo(t1));
    }

    [Test]
    public void TestColorNames()
    {
        var colorKnownType = new ColorDynamicKnownType();
        // Context is not required for this test
        var dummySerializationContext =
            new SerializationContext(null, new UdtWrapper(typeof(Color), new SerializerOptions()),
                new YAXSerializer(typeof(ColorDynamicKnownType)));

        var elem = new XElement("TheColor", "Red");
        var desCl = (Color?) colorKnownType.Deserialize(elem, string.Empty, dummySerializationContext);
        Assert.That(desCl?.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

        var serElem = new XElement("TheColor");
        colorKnownType.Serialize(Color.Red, serElem, "", dummySerializationContext);
        Assert.That(serElem.ToString(), Is.EqualTo(elem.ToString()));

        var elemRgbForRed = new XElement("TheColor",
            new XElement("A", 255),
            new XElement("R", 255),
            new XElement("G", 0),
            new XElement("B", 0));
        var desCl2 = (Color?) colorKnownType.Deserialize(elemRgbForRed, "", dummySerializationContext);
        Assert.That(desCl2?.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

        var elemRgbAndValueForRed = new XElement("TheColor",
            "Blue",
            new XElement("R", 255),
            new XElement("G", 0),
            new XElement("B", 0));
        var desCl3 = (Color?) colorKnownType.Deserialize(elemRgbAndValueForRed, "", dummySerializationContext);
        Assert.That(desCl3?.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
    }

    [Test]
    public void ExceptionKnownType_Serialize_CustomException()
    {
        try
        {
            ExceptionTestSamples.ThrowCustomException();
        }
        catch (Exception ex)
        {
            // Use serializer with default MaxRecursion and without SuppressMetadataAttributes
            var ser = new YAXSerializer(typeof(Exception), new SerializerOptions {
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            var exceptionSerialized = ser.SerializeToXDocument(ex);
            var outerElement = exceptionSerialized.Root;

            Assert.Multiple(() =>
            {
                Assert.That(outerElement?.Name.LocalName, Is.EqualTo("Exception"), "Element name should be 'Exception'");
                Assert.That(outerElement?.Attribute("{http://www.sinairv.com/yaxlib/}realtype")?.Value,
                    Is.EqualTo("YAXLibTests.SampleClasses.YAXLibTests.SampleClasses.CustomException"),
                    "Exception 'realtype' attribute should exist");
                Assert.That(outerElement?.Element("Message")?.Value, Does.Contain(ex.Message),
                    "Exception message should exist");
            });

            var innerElement = exceptionSerialized.Root?.Element("InnerException");

            Assert.Multiple(() =>
            {
                Assert.That(innerElement?.Attribute("{http://www.sinairv.com/yaxlib/}realtype")?.Value,
                            Is.EqualTo("System.DivideByZeroException"), "'InnerException 'realtype' attribute should exist");
                Assert.That(innerElement?.Element("Message")?.Value, Does.Contain(ex.InnerException!.Message),
                    "InnerException message should exist");
            });
        }
    }

    [TestCase(3)] // minimum to serialize outer exception
    [TestCase(10)] // serialize including inner exceptions
    public void ExceptionKnownType_Serialize_InvalidOperationException(int maxRecursion)
    {
        try
        {
            ExceptionTestSamples.ThrowInvalidOperationException();
        }
        catch (Exception ex)
        {
            // Use serializer with custom MaxRecursion and without SuppressMetadataAttributes
            var ser = new YAXSerializer<Exception>(new SerializerOptions {
                MaxRecursion = maxRecursion,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            var exceptionSerialized = ser.SerializeToXDocument(ex);

            var outerElement = exceptionSerialized.Root;

            Assert.Multiple(() =>
            {
                Assert.That(outerElement?.Name.LocalName, Is.EqualTo("Exception"), "Element name should be 'Exception'");
                Assert.That(outerElement?.Attribute("{http://www.sinairv.com/yaxlib/}realtype")?.Value,
                    Is.EqualTo("System.InvalidOperationException"), "Exception 'realtype' attribute should exist");
                Assert.That(outerElement?.Element("Message")?.Value, Does.Contain(ex.Message),
                    "Exception message should exist");
            });

            if (maxRecursion > 3)
            {
                var innerElement = exceptionSerialized.Root?.Element(XName.Get("InnerException"));
                Assert.Multiple(() =>
                {
                    Assert.That(innerElement?.Attribute("{http://www.sinairv.com/yaxlib/}realtype")?.Value,
                                    Is.EqualTo("System.ArgumentException"), "'InnerException 'realtype' attribute should exist");
                    Assert.That(innerElement?.Element("Message")?.Value, Does.Contain(ex.InnerException!.Message),
                        "InnerException message should exist");
                });
            }
        }
    }

    [TestCase(1)] // minimum to deserialize outer exception
    [TestCase(10)] // deserialize including inner exceptions
    public void ExceptionKnownType_Deserialize_CustomException(int maxRecursion)
    {
        var toDeserialize =
            """
                <Exception yaxlib:realtype="YAXLibTests.SampleClasses.YAXLibTests.SampleClasses.CustomException" xmlns:yaxlib="http://www.sinairv.com/yaxlib/">
                  <Info />
                  <TargetSite>Void ThrowCustomException()</TargetSite>
                  <Message>This is a custom exception</Message>
                  <Data>
                    <IDictionary xmlns:yaxlib="http://www.sinairv.com/yaxlib/">
                      <Object yaxlib:realtype="System.Collections.DictionaryEntry">
                        <Key yaxlib:realtype="System.String">TheKey</Key>
                        <Value yaxlib:realtype="System.String">TheValue</Value>
                      </Object>
                    </IDictionary>
                  </Data>
                  <InnerException yaxlib:realtype="System.DivideByZeroException" xmlns:yaxlib="http://www.sinairv.com/yaxlib/">
                    <TargetSite />
                    <Message>InnerException of CustomException</Message>
                    <Data />
                    <InnerException yaxlib:realtype="System.Threading.AbandonedMutexException" xmlns:yaxlib="http://www.sinairv.com/yaxlib/">
                      <Mutex />
                      <MutexIndex>-1</MutexIndex>
                      <TargetSite />
                      <Message>CustomException / DivideByZero / InnerException</Message>
                      <Data />
                      <InnerException />
                      <HelpLink />
                      <Source />
                      <HResult>-2146233043</HResult>
                      <StackTrace />
                    </InnerException>
                    <HelpLink />
                    <Source />
                    <HResult>-2147352558</HResult>
                    <StackTrace />
                  </InnerException>
                  <HelpLink />
                  <Source>YAXLibTests</Source>
                  <HResult>-2146233088</HResult>
                  <StackTrace>   at YAXLibTests.SampleClasses.YAXLibTests.SampleClasses.ExceptionTestSamples.ThrowCustomException() in X:\YAXLib\YAXLibTests\SampleClasses\ExceptionTestSample.cs:line 37
                   at YAXLibTests.KnownTypeTests.ExceptionKnownType_Serialize_CustomException(Int32 maxRecursion) in X:\YAXLib\YAXLibTests\KnownTypeTests.cs:line 220</StackTrace>
                </Exception>
                """;

        // Use serializer with custom MaxRecursion and without SuppressMetadataAttributes
        var ser = new YAXSerializer<Exception>(new SerializerOptions {
            MaxRecursion = maxRecursion,
            SerializationOptions = YAXSerializationOptions.SerializeNullObjects
        });
        var deserialized = ser.Deserialize(toDeserialize);

        Assert.That(deserialized?.Message, Is.EqualTo("This is a custom exception"));
        if (maxRecursion > 1)
        {
            Assert.That(deserialized?.InnerException, Is.TypeOf<DivideByZeroException>());
            Assert.Multiple(() =>
            {
                Assert.That(deserialized?.InnerException?.Message, Is.EqualTo("InnerException of CustomException"));
                Assert.That(deserialized?.InnerException?.InnerException, Is.TypeOf<AbandonedMutexException>());
            });
        }
    }

    [Test]
    public void ExceptionKnownType_Deserialize_InvalidOperationException()
    {
        var toDeserialize =
            """
                <Exception yaxlib:realtype="System.InvalidOperationException" xmlns:yaxlib="http://www.sinairv.com/yaxlib/">
                  <TargetSite>Void ThrowException(System.String)</TargetSite>
                  <Message>System exception unit test</Message>
                  <Data />
                  <InnerException yaxlib:realtype="System.ArgumentException" xmlns:yaxlib="http://www.sinairv.com/yaxlib/">
                    <Message>Inner exception unit test (Parameter 'arg')</Message>
                    <ParamName>arg</ParamName>
                    <TargetSite />
                    <Data />
                    <HelpLink />
                    <Source />
                    <HResult>-2147024809</HResult>
                    <StackTrace />
                  </InnerException>
                  <HelpLink />
                  <Source>YAXLibTests</Source>
                  <HResult>-2146233079</HResult>
                  <StackTrace>   at YAXLibTests.KnownTypeTests.ThrowException(String arg) in D:\Projects\Source\Repos\YAXLib\YAXLibTests\KnownTypeTests.cs:line 238
                   at YAXLibTests.KnownTypeTests.Serialize_SystemException() in X:YAXLib\YAXLibTests\KnownTypeTests.cs:line 247</StackTrace>
                </Exception>
                """;

        // Use serializer with default MaxRecursion and without SuppressMetadataAttributes
        var ser = new YAXSerializer<Exception>(new SerializerOptions
            { SerializationOptions = YAXSerializationOptions.SerializeNullObjects });
        var deserialized = ser.Deserialize(toDeserialize);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized?.Message, Is.EqualTo("System exception unit test"));
            Assert.That(deserialized?.InnerException, Is.TypeOf<ArgumentException>());
        });
        Assert.That(deserialized?.InnerException?.Message, Is.EqualTo("Inner exception unit test (Parameter 'arg')"));
    }

    [Test]
    public void ExceptionKnownType_Deserialize_InvalidOperationException_Without_Child_Elements()
    {
        var toDeserialize =
            """
                <Exception yaxlib:realtype="System.InvalidOperationException" xmlns:yaxlib="http://www.sinairv.com/yaxlib/">
                </Exception>
                """;

        var ser = new YAXSerializer<Exception>(new SerializerOptions
            { SerializationOptions = YAXSerializationOptions.SerializeNullObjects });
        var deserialized = ser.Deserialize(toDeserialize);

        Assert.That(deserialized, Is.InstanceOf<InvalidOperationException>());
        Assert.Multiple(() =>
        {
            Assert.That(deserialized?.Message, Is.Empty);
            Assert.That(deserialized?.InnerException, Is.Null);
        });
    }

    [Test]
    public void ExceptionKnownType_Serialize_Null_ExceptionInstance()
    {
        var xElem = new XElement(XName.Get("Exception"));
        new ExceptionKnownBaseType().Serialize(null, xElem, XNamespace.None,
            new SerializationContext(null, new UdtWrapper(typeof(Exception), new SerializerOptions()),
                new YAXSerializer(typeof(Exception))));

        Assert.That(xElem.HasElements, Is.False);
    }

    [Test]
    public void ExceptionKnownType_Deserialize_Not_An_ExceptionElement()
    {
        var xElem = new XElement(XName.Get("NotAnExceptionElement"));
        var result = new ExceptionKnownBaseType().Deserialize(xElem, XNamespace.None,
            new SerializationContext(null, new UdtWrapper(typeof(Exception), new SerializerOptions()),
                new YAXSerializer(typeof(Exception))));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ExceptionKnownType_Deserialize_NonExisting_ExceptionType()
    {
        var xElem = XElement.Parse(
            """<Exception yaxlib:realtype="System.DateTime" xmlns:yaxlib="http://www.sinairv.com/yaxlib/"></Exception>""");

        var result = new ExceptionKnownBaseType().Deserialize(xElem, XNamespace.None,
            new SerializationContext(null, new UdtWrapper(typeof(Exception), new SerializerOptions()),
                new YAXSerializer(typeof(Exception))));

        Assert.That(result, Is.Null);
    }
}
