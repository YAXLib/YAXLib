﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using NUnit.Framework;
using YAXLib;
using YAXLib.Caching;
using YAXLib.Options;
using YAXLibTests.SampleClasses;

namespace YAXLibTests.Caching;

[TestFixture]
public class MemberWrapperCacheTests
{
    [Test]
    public void SerializingShouldFillTheCache()
    {
        MemberWrapperCache.Instance.Clear();
        var countAfterClear = MemberWrapperCache.Instance.CacheDictionary.Count;
        var s = new YAXSerializer<Book>();
        _ = s.Serialize(Book.GetSampleInstance());

        Assert.That(countAfterClear, Is.EqualTo(0));
        Assert.That(MemberWrapperCache.Instance.CacheDictionary.Count, Is.GreaterThan(0));
        Assert.That(MemberWrapperCache.Instance.CacheDictionary[(typeof(Book), s.Options)].Count, Is.GreaterThan(0));
    }

    [Test]
    public void DeserializingShouldFillTheCache()
    {
        // Get some xml to deserialize to a Book instance
        var s = new YAXSerializer<Book>();
        var xml = s.Serialize(Book.GetSampleInstance());

        MemberWrapperCache.Instance.Clear();
        var countAfterClear = MemberWrapperCache.Instance.CacheDictionary.Count;
        s = new YAXSerializer<Book>();
        _ = s.Deserialize(xml);

        Assert.That(countAfterClear, Is.EqualTo(0));
        Assert.That(MemberWrapperCache.Instance.CacheDictionary.Count, Is.GreaterThan(0));
        Assert.That(MemberWrapperCache.Instance.CacheDictionary[(typeof(Book), s.Options)].Count, Is.GreaterThan(0));
    }

    [Test]
    public void MaxCacheSizeCannotBecomeNegative()
    {
        MemberWrapperCache.Instance.MaxCacheSize = -1;
        Assert.That(MemberWrapperCache.Instance.MaxCacheSize, Is.EqualTo(0));

        MemberWrapperCache.Instance.MaxCacheSize = MemberWrapperCache.DefaultCacheSize;
    }

    [Test]
    public void CacheCannotExceedMaximumSize()
    {
        MemberWrapperCache.Instance.Clear();
        MemberWrapperCache.Instance.MaxCacheSize = 5;
        var so = new SerializerOptions();
        MemberWrapperCache.Instance.Add((typeof(string), so), new List<MemberWrapper>());
        var dupeAdded = MemberWrapperCache.Instance.TryAdd((typeof(string), so), new List<MemberWrapper>());
        MemberWrapperCache.Instance.Add((typeof(int), so), new List<MemberWrapper>());
        MemberWrapperCache.Instance.Add((typeof(uint), so), new List<MemberWrapper>());
        MemberWrapperCache.Instance.Add((typeof(long), so), new List<MemberWrapper>());
        MemberWrapperCache.Instance.Add((typeof(ulong), so), new List<MemberWrapper>());
        MemberWrapperCache.Instance.Add((typeof(char), so), new List<MemberWrapper>());

        Assert.That(dupeAdded, Is.False);
        Assert.That(MemberWrapperCache.Instance.CacheDictionary.Count, Is.EqualTo(5));
        Assert.That(MemberWrapperCache.Instance.CacheDictionary.ContainsKey((typeof(string), so)), Is.False); // FIFO

        MemberWrapperCache.Instance.MaxCacheSize = MemberWrapperCache.DefaultCacheSize;
    }
}
