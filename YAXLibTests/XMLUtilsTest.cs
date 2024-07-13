// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using YAXLib;

namespace YAXLibTests;

/// <summary>
/// Summary description for XMLUtilsTest
/// </summary>
[TestFixture]
public class XmlUtilsTest
{
    [Test]
    public void CanCreateLocationTest()
    {
        var elem = new XElement("Base", string.Empty);

        Assert.That(XMLUtils.CanCreateLocation(elem, "level1/level2"), Is.True);
        var created = XMLUtils.CreateLocation(elem, "level1/level2");
        Assert.Multiple(() =>
        {
            Assert.That(created.Name.ToString(), Is.EqualTo("level2"));
            Assert.That(XMLUtils.LocationExists(elem, "level1/level2"), Is.True);
        });
        created = XMLUtils.CreateLocation(elem, "level1/level3");
        Assert.Multiple(() =>
        {
            Assert.That(created.Name.ToString(), Is.EqualTo("level3"));
            Assert.That(XMLUtils.LocationExists(elem, "level1/level3"), Is.True);
        });
    }

    [Test]
    public void ConvertObjectToXmlValue()
    {
        Assert.Multiple(() =>
        {
            Assert.That(default(object).ToXmlValue(CultureInfo.InvariantCulture), Is.EqualTo(string.Empty));
            Assert.That(true.ToXmlValue(CultureInfo.InvariantCulture), Is.EqualTo("true"));
            Assert.That(1.1234567d.ToXmlValue(CultureInfo.InvariantCulture), Is.EqualTo("1.1234567"));
            Assert.That(1.123f.ToXmlValue(CultureInfo.InvariantCulture), Is.EqualTo("1.123"));
            Assert.That(new BigInteger(1234567890).ToXmlValue(CultureInfo.InvariantCulture), Is.EqualTo("1234567890"));
            Assert.That(new StringBuilder("123.456").ToXmlValue(CultureInfo.InvariantCulture), Is.EqualTo("123.456"));
        });
    }
}