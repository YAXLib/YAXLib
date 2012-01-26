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
using System.Xml.Linq;

namespace YAXLibTests
{
    /// <summary>
    /// Summary description for XMLUtilsTest
    /// </summary>
    [TestClass]
    public class XMLUtilsTest
    {
        [TestMethod]
        public void CanCreateLocationTest()
        {
            var elem = new XElement("Base", null);

            Assert.IsTrue(XMLUtils.CanCreateLocation(elem, "level1/level2"));
            var created = XMLUtils.CreateLocation(elem, "level1/level2");
            Assert.AreEqual(created.Name.ToString(), "level2");
            Assert.IsTrue(XMLUtils.LocationExists(elem, "level1/level2"));
            created = XMLUtils.CreateLocation(elem, "level1/level3");
            Assert.AreEqual(created.Name.ToString(), "level3");
            Assert.IsTrue(XMLUtils.LocationExists(elem, "level1/level3"));
        }
    }
}
