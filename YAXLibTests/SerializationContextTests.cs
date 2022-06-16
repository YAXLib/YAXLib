// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using NUnit.Framework;
using YAXLib;
using YAXLib.Options;
using YAXLibTests.SampleClasses.CustomSerialization;

namespace YAXLibTests
{
    [TestFixture]
    public class SerializationContextTests
    {
        [TestCase(typeof(PropertyLevelSample))]
        [TestCase(typeof(FieldLevelSample))]
        public void SerializationContext_All_Set_For_Class_And_Members(Type sampleType)
        {
            const string memberName = "Title";
            var udtWrapper = new UdtWrapper(sampleType, null);
            var memberInfo = udtWrapper.UnderlyingType.GetMember(memberName)[0];
            var memberWrapper = new MemberWrapper(memberInfo, null);
            var sc = new SerializationContext(memberWrapper, udtWrapper, new SerializerOptions());

            Assert.That(sc.SerializerOptions, Is.Not.Null);
            Assert.That(sc.ClassType!.Name, Is.EqualTo(sampleType.Name));
            Assert.That(sc.MemberType!.UnderlyingSystemType.Name, Is.EqualTo(nameof(String)));
            Assert.That(sc.MemberInfo!.Name, Is.EqualTo(memberName));
            Assert.That(sc.PropertyInfo != null ? sc.PropertyInfo!.Name : sc.FieldInfo!.Name, Is.EqualTo(memberName));
        }
    }
}
