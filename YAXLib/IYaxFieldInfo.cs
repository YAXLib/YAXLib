// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib;

public interface IYaxFieldInfo : IYaxMemberInfo
{
    object? GetValue(object? obj);
    void SetValue(object? obj, object? value);
}
