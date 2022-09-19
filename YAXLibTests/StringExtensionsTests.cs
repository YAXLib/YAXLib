// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using System.Text;
using NUnit.Framework;
using YAXLib;

namespace YAXLibTests;

[TestFixture]
internal class StringExtensionsTests
{
    [Test]
    public void Base64_DefaultEncoding_Roundtrip_Should_Succeed()
    {
        var toEncode = "\u0000abc\u0001def\u0000";
        var encoded = toEncode.ToBase64();
        Assert.That(encoded.FromBase64(), Is.EqualTo(toEncode));
    }

    [Test]
    public void Base64_CustomEncoding_Roundtrip_Should_Succeed()
    {
        var toEncode = "\u0000abc\u0001def\u0000";
        var encoded = toEncode.ToBase64(Encoding.ASCII);
        Assert.That(encoded.FromBase64(Encoding.ASCII), Is.EqualTo(toEncode));
    }

    [Test]
    public void Base64_Encode_Null_Should_Be_Null()
    {
        Assert.That(default(string?).ToBase64(), Is.Null);
    }

    [Test]
    public void Base64_Decode_Null_Should_Be_Null()
    {
        Assert.That(default(string?).FromBase64(), Is.Null);
    }
}