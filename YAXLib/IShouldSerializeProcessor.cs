using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YAXLib
{
    /// <summary>
    /// Serialize Processor Interface for selective serialization
    /// </summary>
    public interface IShouldSerializeProcessor
    {
        bool ShouldSerialize(MemberInfo member);
    }
}
