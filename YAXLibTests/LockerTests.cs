// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib.Customization;

namespace YAXLibTests;

[TestFixture]
public class LockerTests
{
    [Test]
    public void Locking_A_Type_More_Than_Once_Should_Throw()
    {
        using var l1 = new Locker(typeof(long));

        Assert.That(code: () => _ = new Locker(typeof(long)), Throws.ArgumentException);
    }

    [Test]
    public void Dispose_Should_Remove_Type_From_LockingList()
    {
        var locker = new Locker(typeof(int));
        locker.Dispose();

        Assert.That(code: () => _ = new Locker(typeof(int)), Throws.Nothing);
    }
}
