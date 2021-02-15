// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;
using NUnit.Framework;
using YAXLib;

namespace YAXLibTests
{
    /// <summary>
    ///     Summary description for XMLUtilsTest
    /// </summary>
    [TestFixture]
    public class XMLUtilsTest
    {
        [Test]
        public void CanCreateLocationTest()
        {
            var elem = new XElement("Base", null);

            Assert.That(XMLUtils.CanCreateLocation(elem, "level1/level2"), Is.True);
            var created = XMLUtils.CreateLocation(elem, "level1/level2");
            Assert.That(created.Name.ToString(), Is.EqualTo("level2"));
            Assert.That(XMLUtils.LocationExists(elem, "level1/level2"), Is.True);
            created = XMLUtils.CreateLocation(elem, "level1/level3");
            Assert.That(created.Name.ToString(), Is.EqualTo("level3"));
            Assert.That(XMLUtils.LocationExists(elem, "level1/level3"), Is.True);
        }
    }
}