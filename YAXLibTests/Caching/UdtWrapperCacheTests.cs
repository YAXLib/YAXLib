// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;
using YAXLib.Caching;
using YAXLib.Options;
using YAXLibTests.SampleClasses;

namespace YAXLibTests.Caching;

[TestFixture]
public class UdtWrapperCacheTests
{
    [Test]
    public void SerializingShouldFillTheCache()
    {
        UdtWrapperCache.Instance.Clear();
        var countAfterClear = UdtWrapperCache.Instance.CacheDictionary.Count;
        var s = new YAXSerializer<Book>();
        _ = s.Serialize(Book.GetSampleInstance());

        Assert.Multiple(() =>
        {
            Assert.That(countAfterClear, Is.EqualTo(0));
            Assert.That(UdtWrapperCache.Instance.CacheDictionary, Contains.Key((typeof(Book), s.Options)));
        });
    }

    [Test]
    public void DeserializingShouldFillTheCache()
    {
        // Get some xml to deserialize to a Book instance
        var s = new YAXSerializer<Book>();
        var xml = s.Serialize(Book.GetSampleInstance());

        UdtWrapperCache.Instance.Clear();
        var countAfterClear = UdtWrapperCache.Instance.CacheDictionary.Count;
        s = new YAXSerializer<Book>();
        _ = s.Deserialize(xml);

        Assert.Multiple(() =>
        {
            Assert.That(countAfterClear, Is.EqualTo(0));
            Assert.That(UdtWrapperCache.Instance.CacheDictionary, Contains.Key((typeof(Book), s.Options)));
        });
    }

    [Test]
    public void MaxCacheSizeCannotBecomeNegative()
    {
        UdtWrapperCache.Instance.MaxCacheSize = -1;
        Assert.That(UdtWrapperCache.Instance.MaxCacheSize, Is.EqualTo(0));

        UdtWrapperCache.Instance.MaxCacheSize = UdtWrapperCache.DefaultCacheSize;
    }

    [Test]
    public void CacheCannotExceedMaximumSize()
    {
        // Creating the serializer adds the first type to the cache
        var serializerOptions = new SerializerOptions();
        UdtWrapperCache.Instance.Clear();
        UdtWrapperCache.Instance.MaxCacheSize = 5;

        UdtWrapperCache.Instance.GetOrAddItem(typeof(string), serializerOptions);
        UdtWrapperCache.Instance.GetOrAddItem(typeof(int), serializerOptions);
        UdtWrapperCache.Instance.GetOrAddItem(typeof(uint), serializerOptions);
        UdtWrapperCache.Instance.GetOrAddItem(typeof(long), serializerOptions);
        UdtWrapperCache.Instance.GetOrAddItem(typeof(ulong), serializerOptions);
        UdtWrapperCache.Instance.GetOrAddItem(typeof(char), serializerOptions);

        Assert.That(UdtWrapperCache.Instance.CacheDictionary, Has.Count.EqualTo(5));
        Assert.That(UdtWrapperCache.Instance.CacheDictionary.ContainsKey((typeof(string), serializerOptions)), Is.False); // FIFO

        UdtWrapperCache.Instance.MaxCacheSize = UdtWrapperCache.DefaultCacheSize;
    }
}
