using NUnit.Framework;
using YAXLib.Pooling;
using YAXLib.Pooling.YAXLibPools;

namespace YAXLibTests.Pooling;

#nullable enable

[TestFixture]
public class SerializerPoolTests
{
    private static SerializerPool GetSerializerPool()
    {
        SerializerPool.Instance.Clear();
        var sp = SerializerPool.Instance;

        return sp;
    }

    [Test]
    public void Create_New_Instance()
    {
        var sbp = GetSerializerPool();

        Assert.Multiple(() =>
        {
            Assert.That(() => sbp.Get(), Throws.Nothing);
            Assert.That(sbp.Pool.CountActive, Is.EqualTo(1));
            Assert.That(sbp.Pool.CountInactive, Is.EqualTo(0));
            Assert.That(sbp.Pool.CountAll, Is.EqualTo(1));
        });
    }

    [Test]
    public void Get_Pooled_Instance()
    {
        var sp = GetSerializerPool();


        var serializer = sp.Get();

        // Returning an item should clear the StringBuilder
        Assert.That(() => sp.Return(serializer), Throws.Nothing);

        Assert.Multiple(() =>
        {
            Assert.That(sp.Pool.CountActive, Is.EqualTo(0));
            Assert.That(sp.Pool.CountInactive, Is.EqualTo(1));
            Assert.That(sp.Pool.CountAll, Is.EqualTo(1));
        });
    }

    [Test]
    public void Get_PooledObject()
    {
        var sp = GetSerializerPool();
        var serializer = sp.Get();
        sp.Return(serializer);
        sp.Get(out var sb2);
        Assert.That(sb2, Is.SameAs(serializer));
    }

    [Test]
    public void Reset_Pool()
    {
        var sp = GetSerializerPool();

        var savedObjectPoolType = sp.Pool.GetType();

        var serializer = sp.Get();
        sp.Return(serializer);
        sp.Reset();

        Assert.Multiple(() =>
        {
            Assert.That(sp.Pool.CountActive, Is.EqualTo(0));
            Assert.That(sp.Pool.CountInactive, Is.EqualTo(0));
            Assert.That(sp.Pool.CountAll, Is.EqualTo(0));
        });
    }

    [Test]
    public void Dispose_Pool()
    {
        _ = SerializerPool.Instance; // ensure it is in the registry
        var sp = PoolRegistry.Get<SerializerPool>();
        var serializer = sp?.Get();
        sp?.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(serializer, Is.Not.Null, "Serializer instance");
            Assert.That(sp?.Pool.CountAll ?? -1, Is.EqualTo(0), "CountAll");
        });
    }
}
