// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using YAXLib;

namespace YAXLibTests
{
    [TestFixture]
    public class ReflectionUtilsTest
    {
        [Test]
        public void IsArrayTest()
        {
            Assert.That(ReflectionUtils.IsArray(typeof(int[])), Is.True);
            Assert.That(ReflectionUtils.IsArray(typeof(int[,])), Is.True);
            Assert.That(ReflectionUtils.IsArray(typeof(Array)), Is.True);
            Assert.That(ReflectionUtils.IsArray(typeof(List<int>)), Is.False);
            Assert.That(ReflectionUtils.IsArray(typeof(List<>)), Is.False);
            Assert.That(ReflectionUtils.IsArray(typeof(Dictionary<,>)), Is.False);
            Assert.That(ReflectionUtils.IsArray(typeof(Dictionary<int, string>)), Is.False);
            Assert.That(ReflectionUtils.IsArray(typeof(string)), Is.False);
        }

        [Test]
        public void IsCollectionTypeTest()
        {
            Assert.That(ReflectionUtils.IsCollectionType(typeof(int[])), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(Array)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(List<int>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(List<>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(Dictionary<,>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(Dictionary<int, string>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(IEnumerable)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(IEnumerable<>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(IEnumerable<int>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(string)), Is.False);
        }

        [Test]
        public void GetCollectionItemTypeTest()
        {
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(IEnumerable<int>)) == typeof(int), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(double[])) == typeof(double), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(float[][])) == typeof(float[]), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(string[,])) == typeof(string), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(List<char>)) == typeof(char), Is.True);
            Assert.That(
                ReflectionUtils.GetCollectionItemType(typeof(Dictionary<int, char>)) == typeof(KeyValuePair<int, char>),
                Is.True);
            Assert.That(
                ReflectionUtils.GetCollectionItemType(typeof(Dictionary<Dictionary<int, double>, char>)) ==
                typeof(KeyValuePair<Dictionary<int, double>, char>), Is.True);

            //Assert.That(ReflectionUtils.GetCollectionItemType(typeof(IEnumerable<>)) == typeof(object), Is.True);
        }

        [Test]
        public void IsTypeEqualOrInheritedFromTypeTest()
        {
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(int), typeof(object)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(string), typeof(object)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Array), typeof(IEnumerable)), Is.True);
            Assert.That(
                ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(Dictionary<,>)),
                Is.True);
            Assert.That(
                ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(ICollection)),
                Is.True);
            Assert.That(
                ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary)),
                Is.True);
            Assert.That(
                ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary<,>)),
                Is.True);
            Assert.That(
                ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>),
                    typeof(IDictionary<string, int>)), Is.True);
            Assert.That(
                ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>),
                    typeof(IDictionary<int, string>)), Is.False);
            Assert.That(
                ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int[]>),
                    typeof(IDictionary<int, Array>)), Is.False);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(ICollection), typeof(IEnumerable)),
                Is.True);
        }

        [Test]
        public void EqualsOrIsNullableOfTest()
        {
            Assert.That(typeof(int).EqualsOrIsNullableOf(typeof(int)), Is.True);
            Assert.That(typeof(int?).EqualsOrIsNullableOf(typeof(int)), Is.True);
            Assert.That(typeof(int).EqualsOrIsNullableOf(typeof(int?)), Is.True);
            Assert.That(typeof(double).EqualsOrIsNullableOf(typeof(double?)), Is.True);
            Assert.That(typeof(double?).EqualsOrIsNullableOf(typeof(Nullable<>)), Is.False);
            Assert.That(typeof(double?).EqualsOrIsNullableOf(typeof(double)), Is.True);
            Assert.That(typeof(char?).EqualsOrIsNullableOf(typeof(char?)), Is.True);
            Assert.That(typeof(char?).EqualsOrIsNullableOf(typeof(char?)), Is.True);
            Assert.That(typeof(int[]).EqualsOrIsNullableOf(typeof(Array)), Is.False);
        }

        [Test]
        public void GetTypeByNameTest()
        {
            var type1 = ReflectionUtils.GetTypeByName(
                "System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]");
            var type2 = ReflectionUtils.GetTypeByName("System.Collections.Generic.List`1[[System.Int32]]");
            var type3 = ReflectionUtils.GetTypeByName(
                "System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral]]");
            Assert.That(type1, Is.Not.Null);
            Assert.That(type2, Is.Not.Null);
            Assert.That(type3, Is.Not.Null);
            Assert.That(type2, Is.EqualTo(type1));
            Assert.That(type3, Is.EqualTo(type2));
        }
    }
}