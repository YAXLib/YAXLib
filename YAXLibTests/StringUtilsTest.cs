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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YAXLib;

namespace YAXLibTests
{
    /// <summary>
    /// Summary description for StringUtilsTest
    /// </summary>
    [TestClass]
    public class StringUtilsTest
    {
        [TestMethod]
        public void RefineElementNameTest()
        {
            Assert.AreEqual(StringUtils.RefineLocationString(".."), "..");
            Assert.AreEqual(StringUtils.RefineLocationString("."), ".");
            Assert.AreEqual(StringUtils.RefineLocationString("      "), ".");
            Assert.AreEqual(StringUtils.RefineLocationString(" /      \\ "), ".");
            Assert.AreEqual(StringUtils.RefineLocationString("ans"), "ans");
            Assert.AreEqual(StringUtils.RefineLocationString("/ans"), "ans");
            Assert.AreEqual(StringUtils.RefineLocationString("/ans/"), "ans");
            Assert.AreEqual(StringUtils.RefineLocationString("ans/"), "ans");
            Assert.AreEqual(StringUtils.RefineLocationString("ans/////"), "ans");
            Assert.AreEqual(StringUtils.RefineLocationString("ans\\\\\\"), "ans");
            Assert.AreEqual(StringUtils.RefineLocationString("..."), "___");
            Assert.AreEqual(StringUtils.RefineLocationString("one / two / three / four "), "one/two/three/four");
            Assert.AreEqual(StringUtils.RefineLocationString("one / two \\ three / four "), "one/two/three/four");
            Assert.AreEqual(StringUtils.RefineLocationString("one / two / three and else / four "), "one/two/three_and_else/four");
            Assert.AreEqual(StringUtils.RefineLocationString("one / two / .. / four "), "one/two/../four");
            Assert.AreEqual(StringUtils.RefineLocationString("one / two / .. / four / "), "one/two/../four");
            Assert.AreEqual(StringUtils.RefineLocationString("one / two / . . / four / "), "one/two/___/four");
            Assert.AreEqual(StringUtils.RefineLocationString("one / two / two:words.are / four "), "one/two/two_words_are/four");
        }

        [TestMethod]
        public void IsLocationAllGenericTest()
        {
            Assert.IsTrue(StringUtils.IsLocationAllGeneric(".."));
            Assert.IsTrue(StringUtils.IsLocationAllGeneric("."));
            Assert.IsTrue(StringUtils.IsLocationAllGeneric("./.."));
            Assert.IsTrue(StringUtils.IsLocationAllGeneric("../.."));

            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one/.."));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one/.."));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one/../two"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one/../two"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one/../two/.."));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one/../two/.."));
        }

        [TestMethod]
        public void DivideLocationOneStepTest()
        {
            bool returnValue = false;
            string location = "..";
            string newLocation = "";
            string newElement = "";

            location = "..";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.AreEqual(newLocation, "..");
            Assert.AreEqual(newElement, null);
            Assert.AreEqual(returnValue, false);

            location = ".";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.AreEqual(newLocation, ".");
            Assert.AreEqual(newElement, null);
            Assert.AreEqual(returnValue, false);

            location = "../..";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.AreEqual(newLocation, "../..");
            Assert.AreEqual(newElement, null);
            Assert.AreEqual(returnValue, false);

            location = "../../folder";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.AreEqual(newLocation, "../..");
            Assert.AreEqual(newElement, "folder");
            Assert.AreEqual(returnValue, true);

            location = "../../folder/..";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.AreEqual(newLocation, "../../folder/..");
            Assert.AreEqual(newElement, null);
            Assert.AreEqual(returnValue, false);

            location = "one/two/three/four";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.AreEqual(newLocation, "one/two/three");
            Assert.AreEqual(newElement, "four");
            Assert.AreEqual(returnValue, true);

            location = "one";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.AreEqual(newLocation, ".");
            Assert.AreEqual(newElement, "one");
            Assert.AreEqual(returnValue, true);

        }
    }
}
