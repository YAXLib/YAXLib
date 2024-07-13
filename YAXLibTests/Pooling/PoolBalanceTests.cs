using System;
using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using YAXLib.Pooling.ObjectPools;
using YAXLibTests.TestHelpers;

namespace YAXLibTests.Pooling;

#nullable enable

[TestFixture]
public class PoolBalanceTests
{
    private static void DoSomeSerializingWork()
    {
        // Do some work
        for (var i = 0; i < 4; i++)
        {
            var serializer = new YAXSerializer(typeof(int), new SerializerOptions {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            _ = serializer.SerializeToXDocument(i);
        }
    }

    [Test]
    public void No_Active_Pool_Items_After_Smart_Format()
    {
        var pools = PoolingHelpers.GetAllPoolsCleared();

        DoSomeSerializingWork();

        foreach (var p in pools)
        {
            Console.WriteLine(p.Type + """:""");
            Console.WriteLine("""{0}: {1}""", nameof(IPoolCounters.CountActive), p.Counters?.CountActive);
            Console.WriteLine("""{0}: {1}""", nameof(IPoolCounters.CountInactive), p.Counters?.CountInactive);

            if (p.Counters!.CountAll <= 0) continue;

            Console.WriteLine();
            Assert.Multiple(() =>
            {
                Assert.That(p.Counters.CountActive, Is.EqualTo(0),
                            string.Join(" ", nameof(IPoolCounters.CountActive), p.Type?.ToString()));
                Assert.That(p.Counters.CountInactive, Is.GreaterThan(0),
                    string.Join(" ", nameof(IPoolCounters.CountInactive), p.Type?.ToString()));
            });
        }
    }
}
