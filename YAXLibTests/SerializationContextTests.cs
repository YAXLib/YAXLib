// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses.CustomSerialization;

namespace YAXLibTests
{
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
            var memberInfo = udtWrapper.UnderlyingType.GetMember(memberName)[0];
            var memberWrapper = new MemberWrapper(memberInfo, serializer.Options);
            var sc = new SerializationContext(memberWrapper, udtWrapper, serializer);

            Assert.That(sc.SerializerOptions, Is.EqualTo(serializer.Options));
            Assert.That(sc.ClassType!.Name, Is.EqualTo(sampleType.Name));
            Assert.That(sc.MemberType!.UnderlyingSystemType.Name, Is.EqualTo(nameof(String)));
            Assert.That(sc.MemberInfo!.Name, Is.EqualTo(memberName));
            Assert.That(sc.PropertyInfo != null ? sc.PropertyInfo!.Name : sc.FieldInfo!.Name, Is.EqualTo(memberName));
        }

        [Test]
        public void SerializationContext_All_Set_For_ClassLevel()
        {
            var sampleType = typeof(ClassLevelSample);
            var serializer = new YAXSerializer(sampleType);
            var udtWrapper = serializer.UdtWrapper;
            var sc = new SerializationContext(null, udtWrapper, serializer);

            Assert.That(sc.SerializerOptions, Is.EqualTo(serializer.Options));
            Assert.That(sc.ClassType!.Name, Is.EqualTo(sampleType.Name));
        }
    }
}
