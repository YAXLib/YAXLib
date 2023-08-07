// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[YAXSerializableType(Options = YAXSerializationOptions.DontSerializeNullObjects)]
public class ClassWithOptionsSet
{
    public string StrNotNull { get; set; } = string.Empty;

    // the default value should not be used && no warning or errors should be reported
    [YAXErrorIfMissed(YAXExceptionTypes.Warning, DefaultValue = "Salam")]
    public string? StrNull { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Warning, DefaultValue = 123)]
    public int SomeValueType { get; set; }
}

[YAXSerializableType(Options = YAXSerializationOptions.SerializeNullObjects)]
public class AnotherClassWithOptionsSet
{
    public string StrNotNull { get; set; } = string.Empty;
    public string? StrNull { get; set; }
}

public class ClassWithoutOptionsSet
{
    public string StrNotNull { get; set; } = string.Empty;
    public string? StrNull { get; set; }
}

[ShowInDemoApplication]
public class SerializationOptionsSample
{
    public SerializationOptionsSample()
    {
        ObjectWithOptionsSet = new ClassWithOptionsSet {
            StrNull = null,
            StrNotNull = "SomeString",
            SomeValueType = default
        };

        AnotherObjectWithOptionsSet = new AnotherClassWithOptionsSet {
            StrNull = null,
            StrNotNull = "Some other string"
        };

        ObjectWithoutOptionsSet = new ClassWithoutOptionsSet {
            StrNull = null,
            StrNotNull = "Another string"
        };
    }

    [YAXComment("""
        Str2Null must NOT be serialized when it is null, even 
        if the serialization options of the serializer is changed
        """)]
    public ClassWithOptionsSet ObjectWithOptionsSet { get; set; }

    [YAXComment("""
        Str2Null must be serialized when it is null, even 
        if the serialization options of the serializer is changed
        """)]
    public AnotherClassWithOptionsSet AnotherObjectWithOptionsSet { get; set; }

    [YAXComment("""
        serialization of Str2Null must obey the options set 
        in the serializer itself
        """)]
    public ClassWithoutOptionsSet ObjectWithoutOptionsSet { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static SerializationOptionsSample GetSampleInstance()
    {
        return new SerializationOptionsSample();
    }

    [YAXSerializeAs("my_customer")]
    public class MissingElementsSample1
    {
        [YAXSerializeAs("cust_id")] public int Id { get; set; }
        [YAXSerializeAs("cust_name")] public string? Name { get; set; }
        [YAXSerializeAs("option")] public int? Optional { get; set; }
    }

    [YAXSerializeAs("my_customer")]
    [YAXSerializableType(Options = YAXSerializationOptions.DontSerializeNullObjects)]
    public class MissingElementsSample2
    {
        [YAXSerializeAs("cust_id")] public int Id { get; set; }
        [YAXSerializeAs("cust_name")] public string? Name { get; set; }
        [YAXSerializeAs("option")] public int? Optional { get; set; }
    }

    [YAXSerializeAs("my_record")]
    [YAXSerializableType(Options = YAXSerializationOptions.DoNotSerializeDefaultValues)]
    public class MissingElementsSample3
    {
        [YAXSerializeAs("rec_id")] public int Id { get; set; }
        [YAXSerializeAs("rec_cnt")] public int Count { get; set; }
        [YAXSerializeAs("rec_cnt2")] public int? Count2 { get; set; }
    }
}
