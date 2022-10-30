// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

#pragma warning disable CS0414 // Any values are read using reflection in unit tests

[YAXSerializableType(IncludePrivateMembersFromBaseTypes = true, FieldsToSerialize = YAXSerializationFields.AllFields)]
public class ClassFlaggedToIncludePrivateBaseTypeFields : BaseLevel1
{
    public int PublicPropertyFromLevel0 { get; set; } = 1;

    private int _privateFieldFromLevel0 = 2;
}

[YAXSerializableType(IncludePrivateMembersFromBaseTypes = false, FieldsToSerialize = YAXSerializationFields.AllFields)]
public class ClassFlaggedToExcludePrivateBaseTypeFields : BaseLevel1
{
    public int PublicPropertyFromLevel0 { get; set; } = 1;

    private int _privateFieldFromLevel0 = 2;
}

public class BaseLevel1 : BaseLevel2
{
    protected int ProtectedPropertyFromBaseLevel1 { get; set; } = 11;

    private int _privateFieldFromBaseLevel1 = 12;

    private int PrivatePropertyFromBaseLevel1 { get; set; } = 13;
}

public class BaseLevel2
{
    internal int InternalPropertyFromBaseLevel2 { get; set; } = 21;

    private int _privateFieldFromBaseLevel2 = 22;

    private int PrivatePropertyFromBaseLevel2 { get; set; } = 23;
}
#pragma warning restore CS0414
