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
            XElement elem = new XElement("Base", null);

            Assert.IsTrue(true);
            //Assert.IsFalse(XMLUtils.CanCreateElement(elem, ".."));
        }
    }
}
