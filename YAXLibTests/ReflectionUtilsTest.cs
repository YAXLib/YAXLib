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
            Assert.IsTrue(ReflectionUtils.IsArray(typeof(int[])));
            Assert.IsTrue(ReflectionUtils.IsArray(typeof(int[,])));
            Assert.IsTrue(ReflectionUtils.IsArray(typeof(Array)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(List<int>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(List<>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(Dictionary<,>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(Dictionary<int, string>)));
            Assert.IsFalse(ReflectionUtils.IsArray(typeof(string)));
        }
        
        [Test]
        public void IsCollectionTypeTest()
        {
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(int[])));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(Array)));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(List<int>)));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(List<>)));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(Dictionary<,>)));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(Dictionary<int,string>)));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(IEnumerable)));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(IEnumerable<>)));
            Assert.IsTrue(ReflectionUtils.IsCollectionType(typeof(IEnumerable<int>)));
            Assert.IsFalse(ReflectionUtils.IsCollectionType(typeof(string)));
        }

        [Test]
        public void GetCollectionItemTypeTest()
        {
            Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(IEnumerable<int>)) == typeof(int));
            Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(double[])) == typeof(double));
            Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(float[][])) == typeof(float[]));
            Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(string[,])) == typeof(string));
            Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(List<char>)) == typeof(char));
            Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(Dictionary<int,char>)) == typeof(KeyValuePair<int, char>));
            Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(Dictionary<Dictionary<int, double>, char>)) == typeof(KeyValuePair<Dictionary<int, double>, char>));

            //Assert.IsTrue(ReflectionUtils.GetCollectionItemType(typeof(IEnumerable<>)) == typeof(object));
        }

        [Test]
        public void IsTypeEqualOrInheritedFromTypeTest()
        {
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof (int), typeof (object)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(string), typeof(object)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Array), typeof(IEnumerable)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string,int>), typeof(Dictionary<,>)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(ICollection)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary<,>)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary<string, int>)));
            Assert.IsFalse(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int>), typeof(IDictionary<int, string>)));
            Assert.IsFalse(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(Dictionary<string, int[]>), typeof(IDictionary<int, Array>)));
            Assert.IsTrue(ReflectionUtils.IsTypeEqualOrInheritedFromType(typeof(ICollection), typeof(IEnumerable)));
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
