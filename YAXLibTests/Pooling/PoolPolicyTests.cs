using NUnit.Framework;
using YAXLib.Pooling;
using YAXLib.Pooling.ObjectPools;

namespace YAXLibTests.Pooling;

#nullable enable

[TestFixture]
public class PoolPolicyTests
{
    [Test]
    public void Illegal_Pool_Size_Should_Throw()
    {
        Assert.That(() => new PoolPolicy<object> { MaximumPoolSize = 0 },
            Throws.InstanceOf(typeof(PoolingException))
                .And
                .Property(nameof(PoolingException.PoolType))
                .EqualTo(typeof(object)));
    }
}
