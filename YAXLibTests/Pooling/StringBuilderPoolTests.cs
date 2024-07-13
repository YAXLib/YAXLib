using NUnit.Framework;
using YAXLib.Pooling;
using YAXLib.Pooling.SpecializedPools;

namespace YAXLibTests.Pooling;

#nullable enable

[TestFixture]
public class StringBuilderPoolTests
{
    private static StringBuilderPool GetStringBuilderPool()
    {
        StringBuilderPool.Instance.Clear();
        var sbp = StringBuilderPool.Instance;

        return sbp;
    }

    [Test]
    public void Create_New_Instance()
    {
        var sbp = GetStringBuilderPool();

        Assert.Multiple(() =>
        {
            Assert.That(() => sbp.Get(), Throws.Nothing);
            Assert.That(sbp.Pool.CountActive, Is.EqualTo(1));
            Assert.That(sbp.Pool.CountInactive, Is.EqualTo(0));
            Assert.That(sbp.Pool.CountAll, Is.EqualTo(1));
        });
    }

    [Test]
    public void Get_and_Reset_Pooled_Instance()
    {
        var sbp = GetStringBuilderPool();
        sbp.DefaultStringBuilderCapacity = 1234;

        var sb = sbp.Get();

        Assert.Multiple(() =>
        {
            Assert.That(sbp.Pool.CountActive, Is.EqualTo(1));
            Assert.That(sb.Capacity, Is.EqualTo(sbp.DefaultStringBuilderCapacity));
        });

        sb.Append(new string('x', sbp.DefaultStringBuilderCapacity * 2)); // Exceed the default capacity

        // Returning an item should clear the StringBuilder
        Assert.That(() => sbp.Return(sb), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(sb.Length, Is.EqualTo(0));

            Assert.That(sbp.Pool.CountActive, Is.EqualTo(0));
            Assert.That(sbp.Pool.CountInactive, Is.EqualTo(1));
            Assert.That(sbp.Pool.CountAll, Is.EqualTo(1));
        });

        sbp.Reset();

        Assert.Multiple(() =>
        {
            Assert.That(sbp.Pool.CountActive, Is.EqualTo(0));
            Assert.That(sbp.Pool.CountInactive, Is.EqualTo(0));
            Assert.That(sbp.Pool.CountAll, Is.EqualTo(0));
        });
    }

    [Test]
    public void Get_PooledObject()
    {
        var sbp = GetStringBuilderPool();
        var sb = sbp.Get();
        sbp.Return(sb);
        sbp.Get(out var sb2);
        Assert.That(sb, Is.EqualTo(sb2));
    }

    [Test]
    public void Dispose_Pool()
    {
        _ = StringBuilderPool.Instance; // ensure it is in the registry
        var sbp = PoolRegistry.Get<StringBuilderPool>();
        var stringBuilder = sbp?.Get();
        sbp?.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(stringBuilder, Is.Not.Null, "StringBuilder instance");
            Assert.That(sbp?.Pool.CountAll ?? -1, Is.EqualTo(0), "CountAll");
        });
    }
}
