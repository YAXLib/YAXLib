// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

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
            Assert.AreEqual(StringUtils.RefineLocationString("one-two-three-four"), "one-two-three-four");
        }

        [TestMethod]
        public void ExtractPathAndAliasTest()
        {
            TestPathAndAlias("one/two#name", "one/two", "name");
            TestPathAndAlias("one / two # name", "one / two", "name");
            TestPathAndAlias("one / two # name1 name2", "one / two", "name1 name2");
            TestPathAndAlias(" one / two # name1 name2", "one / two", "name1 name2");
            TestPathAndAlias(" one / two name1 name2 ", " one / two name1 name2 ", "");
            TestPathAndAlias(" one / two # name1 # name2 ", "one / two", "name1 # name2");
            TestPathAndAlias(" one / two # ", "one / two", "");
            TestPathAndAlias(" one / two #", "one / two", "");
            TestPathAndAlias("# one / two ", "", "one / two");
        }

        private static void TestPathAndAlias(string locationString, string expPath, string expAlias)
        {
            string path, alias;
            StringUtils.ExttractPathAndAliasFromLocationString(locationString, out path, out alias);
            Assert.AreEqual(path, expPath);
            Assert.AreEqual(alias, expAlias);
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
            string newLocation;
            string newElement;

            string location = "..";
            bool returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
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

        [TestMethod]
        public void LooksLikeExpandedNameTest()
        {
            var falseCases = new[] {"", "    ", "{}", "{a", "{} ", " {}", " {} ", " {a} ", "{a}", "{a}    ", "something"};
            var trueCases = new[] {"{a}b", " {a}b ", " {a}b"};

            foreach (var falseCase in falseCases)
            {
                Assert.IsFalse(StringUtils.LooksLikeExpandedXName(falseCase));
            }

            foreach (var trueCase in trueCases)
            {
                Assert.IsTrue(StringUtils.LooksLikeExpandedXName(trueCase));
            }
        } 
    }
}
