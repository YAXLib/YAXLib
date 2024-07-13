using NUnit.Framework;
using YAXLib.Pooling.SpecializedPools;

namespace YAXLibTests.Pooling;

#nullable enable

[TestFixture]
public class HashSetPoolTests
{
    private static HashSetPool<int> GetHashSetPool()
    {
        ListPool<int>.Instance.Clear();
        var hsp = HashSetPool<int>.Instance;

        return hsp;
    }

    [Test]
    public void Create_New_Instance()
    {
        var hsp = GetHashSetPool();

        Assert.Multiple(() =>
        {
            Assert.That(() => hsp.Get(), Throws.Nothing);
            Assert.That(hsp.Pool.CountActive, Is.EqualTo(1));
            Assert.That(hsp.Pool.CountInactive, Is.EqualTo(0));
            Assert.That(hsp.Pool.CountAll, Is.EqualTo(1));
        });
    }
}
