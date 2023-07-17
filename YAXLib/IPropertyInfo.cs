// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib;

public interface IPropertyInfo : IMemberInfo
{
    bool CanRead { get; }
    bool CanWrite { get; }

    object? GetValue(object? obj, object[]? index);
    void SetValue(object? obj, object? value , object[]? index);
}
