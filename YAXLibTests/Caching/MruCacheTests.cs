// 
// Copyright (c) 2004-2021 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 

using NUnit.Framework;
using YAXLib.Caching;

namespace YAXLibTests.Caching
{
    [TestFixture]
    public class MruCacheTests
    {
        [Test]
        public void SimpleCacheAddAndLookupTest()
        {
            var mruCache = new MruCache<int, string>(100);
            for (var i = 0; i < 100; ++i)
                mruCache.TryAddValue(i, i.ToString());

            for (var i = 0; i < 100; ++i)
            {
                Assert.True(mruCache.TryGetValue(i, out var value));
                Assert.That(value, Is.EqualTo(i.ToString()));
            }

            Assert.False(mruCache.TryGetValue(101, out _));
        }

        [Test]
        public void OverflowCacheAndLookupTest()
        {
            var mruCache = new MruCache<int, string>(100);
            for (var i = 0; i < 200; ++i)
                mruCache.TryAddValue(i, i.ToString());

            string value;
            for (var i = 0; i < 100; ++i)
                Assert.False(mruCache.TryGetValue(i, out _));

            for (var i = 140; i < 200; ++i)
            {
                Assert.True(mruCache.TryGetValue(i, out value));
                Assert.That(value, Is.EqualTo(i.ToString()));
            }
        }

        [Test]
        public void OverflowVersionCacheAndLookupTest()
        {
            string value;
            var mruCache = new MruCache<int, string>(100);
            for (var i = 0; i < 200; ++i)
            {
                mruCache.TryAddValue(i, i.ToString());
                Assert.True(mruCache.TryGetValue(i, out value));    // No longer a virgin
                Assert.That(value, Is.EqualTo(i.ToString()));
            }

            for (var i = 0; i < 90; ++i)
                Assert.False(mruCache.TryGetValue(i, out _));

            for (var i = 140; i < 200; ++i)
            {
                Assert.True(mruCache.TryGetValue(i, out value));
                Assert.That(value, Is.EqualTo(i.ToString()));
            }
        }

        [Test]
        public void OverflowFreshCacheAndLookupTest()
        {
            string value;
            var mruCache = new MruCache<int, string>(100);
            for (var i = 0; i < 200; ++i)
            {
                mruCache.TryAddValue(i, i.ToString());
                Assert.True(mruCache.TryGetValue(i, out value));    // No longer a virgin
                Assert.That(value, Is.EqualTo(i.ToString()));
            }

            for (var j = 0; j < 2; ++j)
                for (var i = 110; i < 200; ++i)
                    if (!mruCache.TryGetValue(i, out _))
                    {
                        mruCache.TryAddValue(i, i.ToString());
                        Assert.True(mruCache.TryGetValue(i, out _));
                    }

            for (var i = 300; i < 310; ++i)
                mruCache.TryAddValue(i, i.ToString());

            var cacheCount = 0;
            for (var i = 110; i < 200; ++i)
                if (mruCache.TryGetValue(i, out _))
                    ++cacheCount;

            Assert.True(cacheCount > 60);   // See that old cache was not killed
        }

        [Test]
        public void RecentlyUsedLookupTest()
        {
            string value;

            var mruCache = new MruCache<int, string>(100);
            for (var i = 0; i < 200; ++i)
            {
                mruCache.TryAddValue(i, i.ToString());
                for (var j = 0; j < i; j += 10)
                {
                    Assert.True(mruCache.TryGetValue(j, out value));
                    Assert.That(value, Is.EqualTo(j.ToString()));
                }
            }

            for (var j = 0; j < 100; j += 10)
            {
                Assert.True(mruCache.TryGetValue(j, out value));
                Assert.That(value, Is.EqualTo(j.ToString()));
            }

            for (var i = 170; i < 200; ++i)
            {
                Assert.True(mruCache.TryGetValue(i, out value));
                Assert.That(value, Is.EqualTo(i.ToString()));
            }
        }
    }
}
