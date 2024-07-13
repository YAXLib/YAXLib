// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Linq;
using NUnit.Framework;
using YAXLib;
using YAXLib.Customization;
using YAXLib.Enums;
using YAXLib.Options;
using YAXLibTests.SampleClasses.CustomSerialization;

namespace YAXLibTests;

[TestFixture]
public class SerializationContextTests
{
    [TestCase(typeof(PropertyLevelSample))]
    [TestCase(typeof(FieldLevelSample))]
    public void SerializationContext_All_Set_For_Fields_And_Properties(Type sampleType)
    {
        const string memberName = "Title";
        var serializer = new YAXSerializer(sampleType);
        var udtWrapper = serializer.UdtWrapper;
        var memberInfo = udtWrapper.UnderlyingType.GetMember(memberName)[0].Wrap();
        var memberWrapper = new MemberWrapper(memberInfo, serializer.Options);
        var sc = new SerializationContext(memberWrapper, udtWrapper, serializer);

        Assert.Multiple(() =>
        {
            Assert.That(sc.SerializerOptions, Is.EqualTo(serializer.Options));
            Assert.That(sc.TypeContext.Type!.Name, Is.EqualTo(sampleType.Name));
            Assert.That(sc.MemberContext!.TypeContext!.Type.Name, Is.EqualTo(nameof(String)));
            Assert.That(sc.MemberContext!.MemberDescriptor!.Name, Is.EqualTo(memberName));
        });
    }

    [Test]
    public void SerializationContext_All_Set_For_ClassLevel()
    {
        var sampleType = typeof(ClassLevelCtxSample);
        var serializer = new YAXSerializer(sampleType);
        var udtWrapper = serializer.UdtWrapper;
        var sc = new SerializationContext(null, udtWrapper, serializer);

        Assert.Multiple(() =>
        {
            Assert.That(sc.SerializerOptions, Is.EqualTo(serializer.Options));
            Assert.That(sc.TypeContext.Type!.Name, Is.EqualTo(sampleType.Name));
        });
    }

    [Test]
    public void Fields_For_DeSerialization()
    {
        // FieldsToSerialize = YAXSerializationFields.AllFields
        var serializer = new YAXSerializer(typeof(FieldLevelSample),
            new SerializerOptions { SerializationOptions = YAXSerializationOptions.SerializeNullObjects });

        var udtWrapper = serializer.UdtWrapper;
        var sc = new SerializationContext(null, udtWrapper, serializer);

        // Get the member context for the "Title" field
        var titleCtx = sc.TypeContext.GetFieldsForSerialization()
            .FirstOrDefault(f => f.MemberDescriptor.Name == nameof(FieldLevelSample.Title));

        // Get the member context for the "Length" property of the "Title" field
        var lengthCtx = titleCtx!.TypeContext.GetFieldsForSerialization()
            .FirstOrDefault(p => p.MemberDescriptor.Name == nameof(string.Length));

        Assert.Multiple(() =>
        {
            Assert.That(sc.TypeContext.GetFieldsForSerialization().Count(), Is.EqualTo(3));
            Assert.That(sc.TypeContext.GetFieldsForDeserialization().Count(), Is.EqualTo(3));
            Assert.That(lengthCtx!.TypeContext.Type, Is.EqualTo(typeof(int)));
        });
    }

    [Test]
    public void Properties_For_DeSerialization()
    {
        var serializer = new YAXSerializer(typeof(PropertyLevelSample),
            new SerializerOptions { SerializationOptions = YAXSerializationOptions.SerializeNullObjects });
        var udtWrapper = serializer.UdtWrapper;
        var sc = new SerializationContext(null, udtWrapper, serializer);

        Assert.Multiple(() =>
        {
            Assert.That(sc.TypeContext.GetFieldsForSerialization().Count(), Is.EqualTo(3));
            Assert.That(sc.TypeContext.GetFieldsForDeserialization().Count(), Is.EqualTo(3));
        });
    }

    [Test]
    public void Get_Member_Value()
    {
        const string memberName = nameof(FieldLevelSample.Title);
        var sampleType = typeof(ClassLevelSample);
        var serializer = new YAXSerializer(sampleType);
        var udtWrapper = serializer.UdtWrapper;
        var memberInfo = udtWrapper.UnderlyingType.GetMember(memberName)[0].Wrap();
        var memberWrapper = new MemberWrapper(memberInfo, serializer.Options);
        var data = new ClassLevelSample { Title = "The Title" };
        var sc = new SerializationContext(memberWrapper, udtWrapper, serializer);

        Assert.That(sc.MemberContext!.GetValue(data), Is.EqualTo(data.Title));
    }
}
