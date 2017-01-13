using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YAXLib
{
    /// <summary>
    /// Before every property is serialized if the attribute preprocessor callback is populated in the YAXSerializer,
    /// The preprocessor could change the result Attributes, making possible to add, change, or delete attributes dynamically.
    /// </summary>
    public interface IAttributesPreprocessor
    {

        /// <summary>
        /// MemberInfo of the Property
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        Attribute[] PreProcessAttributes(MemberInfo member);
    }
}
