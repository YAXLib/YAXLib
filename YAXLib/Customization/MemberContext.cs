// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
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
        MemberDescriptor = memberWrapper.MemberDescriptor;

        // todo remove this code block along with obsolete properties on next major release
        // todo and do PropertyWrapper.WrappedProperty and FieldWrapper.WrappedField private
#pragma warning disable CS0618
        switch (MemberDescriptor)
        {
            case PropertyWrapper prop:

                MemberInfo = PropertyInfo = prop.WrappedProperty;
                break;
            case FieldWrapper field:
                MemberInfo = FieldInfo = field.WrappedField;
                break;
        }
#pragma warning restore CS0618

        var udtWrapper = UdtWrapperCache.Instance.GetOrAddItem(memberWrapper.MemberType, serializer.Options);
        TypeContext = new TypeContext(udtWrapper, serializer);
    }

    /// <inheritdoc />
    [Obsolete("Will be removed in a future version, please use the MemberDescriptor property instead.")]
    public MemberInfo MemberInfo { get; } = null!;

    /// <inheritdoc />
    [Obsolete("Will be removed in a future version, please use the MemberDescriptor property instead.")]
    public FieldInfo? FieldInfo { get; }

    /// <inheritdoc />
    [Obsolete("Will be removed in a future version, please use the MemberDescriptor property instead.")]
    public PropertyInfo? PropertyInfo { get; }

    /// <inheritdoc />
    public IMemberDescriptor MemberDescriptor { get; }

    /// <inheritdoc />
    public TypeContext TypeContext { get; }

    /// <inheritdoc />
    public object? GetValue(object? obj, object[]? index = null)
    {
        return _memberWrapper.GetOriginalValue(obj, index);
    }
}
