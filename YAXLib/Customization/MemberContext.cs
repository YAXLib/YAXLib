// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Reflection;
using YAXLib.Caching;

namespace YAXLib.Customization;

/// <summary>
/// The member context provides information about the attributes of a member and member metadata.
/// </summary>
public class MemberContext : IMemberContext
{
    private readonly MemberWrapper _memberWrapper;

    /// <summary>
    /// Creates a new member context instance.
    /// </summary>
    /// <param name="memberWrapper"></param>
    /// <param name="serializer"></param>
    internal MemberContext(MemberWrapper memberWrapper, YAXSerializer serializer)
    {
        _memberWrapper = memberWrapper;

        MemberInfo = memberWrapper.MemberInfo;
        FieldInfo = memberWrapper.FieldInfo;
        PropertyInfo = memberWrapper.PropertyInfo;
        TypeContext =
            new TypeContext(UdtWrapperCache.Instance.GetOrAddItem(memberWrapper.MemberType, serializer.Options),
                serializer);
    }

    /// <inheritdoc />
    public MemberInfo MemberInfo { get; }

    /// <inheritdoc />
    public FieldInfo? FieldInfo { get; }

    /// <summary>
    /// The member's <see cref="PropertyInfo" /> for property serialization, else <see langword="null" />.
    /// </summary>
    public PropertyInfo? PropertyInfo { get; }

    /// <inheritdoc />
    public TypeContext TypeContext { get; }

    /// <inheritdoc />
    public object? GetValue(object? obj, object[]? index = null)
    {
        return _memberWrapper.GetOriginalValue(obj, index);
    }
}
