// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace YAXLib.Customization;

internal sealed class Locker : IDisposable
{
    private static readonly ConcurrentDictionary<(int, Type), int> LockedTypes = new ConcurrentDictionary<(int, Type), int>();
    private readonly Type _lockedType;

    public Locker(Type typeToLock)
    {
        _lockedType = typeToLock;
        if (!LockedTypes.TryAdd((Thread.CurrentThread.ManagedThreadId, typeToLock), 0))
            throw new ArgumentException("The type is already locked.", nameof(typeToLock));
    }

    public static bool IsLocked(Type typeToTest)
    {
        return LockedTypes.ContainsKey((Thread.CurrentThread.ManagedThreadId, typeToTest));
    }

    public void Dispose()
    {
        LockedTypes.TryRemove((Thread.CurrentThread.ManagedThreadId, _lockedType), out _);
    }
}
