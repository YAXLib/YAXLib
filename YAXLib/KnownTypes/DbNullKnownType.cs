// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
    internal class DbNullKnownType : KnownType<DBNull>
    {
        public override bool CanSerialize => true;
        public override bool CanDeserialize => true;

        public override void Serialize(DBNull obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj != null)
                elem.Value = "DBNull";
        }

        public override DBNull Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            if (string.IsNullOrEmpty(elem.Value))
                return null;
            return DBNull.Value;
        }
    }
}