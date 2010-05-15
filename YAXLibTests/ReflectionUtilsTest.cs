// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using YAXLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;

namespace YAXLibTests
{
    [TestClass()]
    public class ReflectionUtilsTest
    {
        [TestMethod]
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
        
        [TestMethod]
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

        [TestMethod]
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
    }
}
