// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace YAXLib.MarkObjWithId;
internal class DictObjWithId
{
    public const string ATTR_FLAG_OBJID = "_ObjId";



    public Dictionary<int, object> _dict = new();

    internal int NextObjIdx = 1000;
    public int GetOrNewIdx(object objTarget)
    {
        Debug.Assert(objTarget != null);
        var kv = _dict.SingleOrDefault(x => ReferenceEquals(x.Value, objTarget));
        if (kv.Value != null)
            return kv.Key;
        var newidx = NextObjIdx++;
        _dict.Add(newidx, objTarget);
        return newidx;
    }
    public int? GetObjIdx(object objTarget)
    {
        Debug.Assert(objTarget != null);
        var kv = _dict.SingleOrDefault(x => ReferenceEquals(x.Value, objTarget));
        if (kv.Value != null)
            return kv.Key;
        return null;
    }

    internal void Clear()
    {
        _dict.Clear();
    }

    internal bool TryGetObj(int objId, out object resultObject) => _dict.TryGetValue(objId, out resultObject);

    internal void SaveObj(int objId, object resultObject)
    {
        _dict.Add(objId, resultObject);
    }

}
