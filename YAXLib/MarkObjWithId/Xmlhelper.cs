// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace YAXLib.MarkObjWithId;

internal static class Xmlhelper
{


    public static string ATTR_FLAG_OBJID => DictObjWithId.ATTR_FLAG_OBJID;
    /// <summary>
    /// Only the repeated ObjId is keept
    /// </summary>
    /// <param name="mainDocument"></param>
    /// <returns></returns>
    internal static XDocument ClearUnNecessaryObjId(this XDocument mainDocument)
    {
        var root = mainDocument.Root;
        var ItemArr = CollectItems(root, new List<ObjIdItem>());
        var group = ItemArr.GroupBy(x => x.ObjId, (idx, parents) => (ID: idx, CNT: parents.Count())).ToList();
        var IdToDelArr = group.Where(x => x.CNT == 1).Select(x => x.ID).ToList();
        ItemArr.Where(item => IdToDelArr.Exists(id => id == item.ObjId))
            .ToList()
            .ForEach(item => item.Parent.Attribute(ATTR_FLAG_OBJID).Remove());

        return mainDocument;


        List<ObjIdItem> CollectItems(XElement xelement, List<ObjIdItem> ItemArr)
        {
            var attr = xelement.Attribute(ATTR_FLAG_OBJID);
            if (attr != null)
                ItemArr.Add(new ObjIdItem { Parent = xelement, ObjId = int.Parse(attr.Value) });
            foreach (var xe in xelement.Elements())
                ItemArr = CollectItems(xe, ItemArr);
            return ItemArr;
        }
    }

    class ObjIdItem
    {
        public int ObjId { get; set; }
        public XElement Parent { get; set; }

    }


    static public bool ShouldMarkObjId(this MemberWrapper member)
    {
        var memtype = member.MemberType;
        if (memtype.IsValueType)
            return false;
        if (!member.IsSerializedAsElement)
            return false;

        if (memtype.FullName == "System.String")
            return false;
        return true;

    }
    static public bool ShouldMarkObjId(this Type memtype)
    { 
        if (memtype.IsValueType)
            return false; 

        if (memtype.FullName == "System.String")
            return false;
        return true;

    }

    public static int? GetObjIdFromAttribute(this XElement? xe)
    {
        var attr = xe?.Attribute(DictObjWithId.ATTR_FLAG_OBJID);
        if (attr == null)
        {
            return null;
        }
        var rst = Int32.Parse(attr.Value);
        return rst;
    }

    public static bool IsNullXmlNode(this XElement xe)
    {
        var attributCnt = xe.Attributes().Select(x => x.Name.LocalName)
            .Except(new List<string> { ATTR_FLAG_OBJID }).Count();
        var isEmpty = xe.IsEmpty;
        var HasElements = xe.HasElements;

        return attributCnt == 0 && isEmpty && !HasElements;
    }
}

