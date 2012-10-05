// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using NUnit.Framework;

using YAXLib;

using System;
using System.Collections.Generic;
using System.Collections;

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
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(List<int>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(List<>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(Dictionary<,>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(Dictionary<int, string>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(string)));
        }
        
        [Test]
        public void IsCollectionTypeTest()
        {
            Assert.That(ReflectionUtils.IsCollectionType(typeof(int[])), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(Array)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(List<int>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(List<>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(Dictionary<,>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(Dictionary<int,string>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(IEnumerable)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(IEnumerable<>)), Is.True);
            Assert.That(ReflectionUtils.IsCollectionType(typeof(IEnumerable<int>)), Is.True);
            Assert.IsFalse(ReflectionUtils.IsCollectionType(typeof(string)));
        }

        [Test]
        public void GetCollectionItemTypeTest()
        {
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(IEnumerable<int>)) == typeof(int), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(double[])) == typeof(double), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(float[][])) == typeof(float[]), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(string[,])) == typeof(string), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(List<char>)) == typeof(char), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(Dictionary<int,char>)) == typeof(KeyValuePair<int, char>), Is.True);
            Assert.That(ReflectionUtils.GetCollectionItemType(typeof(Dictionary<Dictionary<int, double>, char>)) == typeof(KeyValuePair<Dictionary<int, double>, char>), Is.True);

            //Assert.That(ReflectionUtils.GetCollectionItemType(typeof(IEnumerable<>)) == typeof(object), Is.True);
        }

        [Test]
        public void IsTypeEqualOrInheritedFromTypeTest()
        {
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof (int), typeof (object)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(string), typeof(object)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Array), typeof(IEnumerable)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string,int>), typeof(Dictionary<,>)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(ICollection)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary<,>)), Is.True);
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary<string, int>)), Is.True);
            Assert.IsFalse(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary<int, string>)));
            Assert.IsFalse(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int[]>), typeof(IDictionary<int, Array>)));
            Assert.That(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(ICollection), typeof(IEnumerable)), Is.True);
        }

        [Test]
        public void GetTypeByNameTest()
        {
            var type1 = ReflectionUtils.GetTypeByName("System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]");
            var type2 = ReflectionUtils.GetTypeByName("System.Collections.Generic.List`1[[System.Int32]]");
            var type3 = ReflectionUtils.GetTypeByName("System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral]]");
            Assert.IsNotNull(type1);
            Assert.IsNotNull(type2);
            Assert.IsNotNull(type3);
            Assert.That(type2, Is.EqualTo(type1));
            Assert.That(type3, Is.EqualTo(type2));
        }
    }
}
