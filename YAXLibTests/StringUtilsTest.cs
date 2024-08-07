﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;

namespace YAXLibTests;

/// <summary>
/// Summary description for StringUtilsTest
/// </summary>
[TestFixture]
public class StringUtilsTest
{
    [Test]
    public void RefineElementNameTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(StringUtils.RefineLocationString(".."), Is.EqualTo(".."));
            Assert.That(StringUtils.RefineLocationString("."), Is.EqualTo("."));
            Assert.That(StringUtils.RefineLocationString("      "), Is.EqualTo("."));
            Assert.That(StringUtils.RefineLocationString(" /      \\ "), Is.EqualTo("."));
            Assert.That(StringUtils.RefineLocationString("ans"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("/ans"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("/ans/"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("ans/"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("ans/////"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("ans\\\\\\"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("..."), Is.EqualTo("_.."));
            Assert.That(StringUtils.RefineLocationString("one / two / three / four "),
                Is.EqualTo("one/two/three/four"));
            Assert.That(StringUtils.RefineLocationString("one / two \\ three / four "),
                Is.EqualTo("one/two/three/four"));
            Assert.That(StringUtils.RefineLocationString("one / two / three and else / four "),
                Is.EqualTo("one/two/three_and_else/four"));
            Assert.That(StringUtils.RefineLocationString("one / two / .. / four "), Is.EqualTo("one/two/../four"));
            Assert.That(StringUtils.RefineLocationString("one / two / .. / four / "), Is.EqualTo("one/two/../four"));
            Assert.That(StringUtils.RefineLocationString("one / two / . . / four / "), Is.EqualTo("one/two/__./four"));
            Assert.That(StringUtils.RefineLocationString("one / two / two:words.are / four "),
                Is.EqualTo("one/two/two_words.are/four"));
            Assert.That(StringUtils.RefineLocationString("one-two-three-four"), Is.EqualTo("one-two-three-four"));
            Assert.That(StringUtils.RefineLocationString("one.two.three.four"), Is.EqualTo("one.two.three.four"));
            Assert.That(StringUtils.RefineLocationString(".one"), Is.EqualTo("_one"));
            Assert.That(StringUtils.RefineLocationString("-one"), Is.EqualTo("_one"));
            Assert.That(StringUtils.RefineLocationString("one."), Is.EqualTo("one."));
            Assert.That(StringUtils.RefineLocationString("one-"), Is.EqualTo("one-"));
        });
    }

    [Test]
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

    private static void TestPathAndAlias(string locationString, string expectedPath, string expectedAlias)
    {
        string path, alias;
        StringUtils.ExtractPathAndAliasFromLocationString(locationString, out path, out alias);
        Assert.Multiple(() =>
        {
            Assert.That(path, Is.EqualTo(expectedPath));
            Assert.That(alias, Is.EqualTo(expectedAlias));
        });
    }

    [Test]
    public void IsLocationAllGenericTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(StringUtils.IsLocationAllGeneric(".."), Is.True);
            Assert.That(StringUtils.IsLocationAllGeneric("."), Is.True);
            Assert.That(StringUtils.IsLocationAllGeneric("./.."), Is.True);
            Assert.That(StringUtils.IsLocationAllGeneric("../.."), Is.True);

            Assert.That(StringUtils.IsLocationAllGeneric("../one/.."), Is.False);
            Assert.That(StringUtils.IsLocationAllGeneric("../one"), Is.False);
            Assert.That(StringUtils.IsLocationAllGeneric("one/.."), Is.False);
            Assert.That(StringUtils.IsLocationAllGeneric("one"), Is.False);
            Assert.That(StringUtils.IsLocationAllGeneric("one/../two"), Is.False);
            Assert.That(StringUtils.IsLocationAllGeneric("../one/../two"), Is.False);
            Assert.That(StringUtils.IsLocationAllGeneric("../one/../two/.."), Is.False);
            Assert.That(StringUtils.IsLocationAllGeneric("one/../two/.."), Is.False);
        });
    }

    [Test]
    public void DivideLocationOneStepTest()
    {
        string newLocation;
        string newElement;

        var location = "..";
        var returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
        Assert.Multiple(() =>
        {
            Assert.That(newLocation, Is.EqualTo(".."));
            Assert.That(newElement, Is.Empty);
            Assert.That(returnValue, Is.False);
        });

        location = ".";
        returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
        Assert.Multiple(() =>
        {
            Assert.That(newLocation, Is.EqualTo("."));
            Assert.That(newElement, Is.Empty);
            Assert.That(returnValue, Is.False);
        });

        location = "../..";
        returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
        Assert.Multiple(() =>
        {
            Assert.That(newLocation, Is.EqualTo("../.."));
            Assert.That(newElement, Is.Empty);
            Assert.That(returnValue, Is.False);
        });

        location = "../../folder";
        returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
        Assert.Multiple(() =>
        {
            Assert.That(newLocation, Is.EqualTo("../.."));
            Assert.That(newElement, Is.EqualTo("folder"));
            Assert.That(returnValue, Is.True);
        });

        location = "../../folder/..";
        returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
        Assert.Multiple(() =>
        {
            Assert.That(newLocation, Is.EqualTo("../../folder/.."));
            Assert.That(newElement, Is.Empty);
            Assert.That(returnValue, Is.False);
        });

        location = "one/two/three/four";
        returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
        Assert.Multiple(() =>
        {
            Assert.That(newLocation, Is.EqualTo("one/two/three"));
            Assert.That(newElement, Is.EqualTo("four"));
            Assert.That(returnValue, Is.True);
        });

        location = "one";
        returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
        Assert.Multiple(() =>
        {
            Assert.That(newLocation, Is.EqualTo("."));
            Assert.That(newElement, Is.EqualTo("one"));
            Assert.That(returnValue, Is.True);
        });
    }

    [Test]
    public void LooksLikeExpandedNameTest()
    {
        var falseCases = new[]
            { "", "    ", "{}", "{a", "{} ", " {}", " {} ", " {a} ", "{a}", "{a}    ", "something" };
        var trueCases = new[] { "{a}b", " {a}b ", " {a}b" };

        foreach (var falseCase in falseCases) Assert.That(StringUtils.LooksLikeExpandedXName(falseCase), Is.False);

        foreach (var trueCase in trueCases) Assert.That(StringUtils.LooksLikeExpandedXName(trueCase), Is.True);
    }
}