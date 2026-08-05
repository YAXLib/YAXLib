// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib.MarkObjWithId;

internal class SerSession
{
    public readonly DictObjWithId ObjDict = new DictObjWithId();

    public void Reset()
    {
        ObjDict.Clear();
    }
}
