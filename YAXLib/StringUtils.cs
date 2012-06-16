// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace YAXLib
{
    /// <summary>
    /// Provides string utility methods
    /// </summary>
    internal class StringUtils
    {
        /// <summary>
        /// Refines the location string. Trims it, and replaces invlalid characters with underscore.
        /// </summary>
        /// <param name="elemAddr">The element address to refine.</param>
        /// <returns>the refined location string</returns>
        public static string RefineLocationString(string elemAddr)
        {
            elemAddr = elemAddr.Trim(' ', '\t', '\r', '\n', '\v', '/', '\\');
            if (String.IsNullOrEmpty(elemAddr))
                return ".";

            // replace all back-slaches to slash
            elemAddr = elemAddr.Replace('\\', '/');

            var sb = new StringBuilder();
            string[] steps = elemAddr.Split(new [] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < steps.Length; i++)
            {
                if (i == 0)
                    sb.Append(RefineSingleElement(steps[i]));
                else
                    sb.AppendFormat("/" + RefineSingleElement(steps[i]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Heuristically determines if the supplied name conforms to the "expanded XML name" form supported by the System.Xml.Linq.XName class.
        /// </summary>
        /// <param name="name">The name to be examined.</param>
        /// <returns><c>true</c> if the supplied name appears to be in expanded form, otherwise <c>false</c>.</returns>
        public static bool LooksLikeExpandedXName(string name)
        {
            // XName permits strings of the form '{namespace}localname'. Detecting such cases allows
            // YAXLib to support explicit namespace use.
            // http://msdn.microsoft.com/en-us/library/system.xml.linq.xname.aspx

            name = name.Trim();

            // Needs at least 3 chars ('{}a' is, in theory, valid), must start with '{', must have a following closing '}' which must not be last char.
            if (name.Length >= 3)
            {
                if (name[ 0 ] == '{')
                {
                    int closingBrace = name.IndexOf('}', 1);
                    return closingBrace != -1 && closingBrace < (name.Length - 1);
                }
            }
            return false;
        }

        
        /// <summary>
        /// Refines a single element name. Refines the location string. Trims it, and replaces invlalid characters with underscore.
        /// </summary>
        /// <param name="elemName">Name of the element.</param>
        /// <returns>the refined element name</returns>
        public static string RefineSingleElement(string elemName)
        {
            elemName = elemName.Trim(' ', '\t', '\r', '\n', '\v', '/', '\\');
            if (IsSingleLocationGeneric(elemName))
            {
                return elemName;
            }
            else if (LooksLikeExpandedXName(elemName))
            {
                // thanks go to CodePlex user: tg73 (http://www.codeplex.com/site/users/view/tg73)
                // for providing the code for expanded xml name support

                // XName permits strings of the form '{namespace}localname'. Detecting such cases allows
                // YAXLib to support explicit namespace use.
                // http://msdn.microsoft.com/en-us/library/system.xml.linq.xname.aspx

                // Leave namespace part alone, refine localname part.
                int closingBrace = elemName.IndexOf( '}' );
                string refinedLocalname = RefineSingleElement( elemName.Substring( closingBrace + 1 ) );
                return elemName.Substring( 0, closingBrace + 1 ) + refinedLocalname;
            }
            else
            {
                var sb = new StringBuilder();

                // invalid chars are all punctunations except underline
                foreach (char c in elemName)
                {
                    if (Char.IsLetterOrDigit(c) || c == '_')
                        sb.Append(c);
                    else
                        sb.Append('_');
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Exttracts the path and alias from location string.
        /// A pure path location string: level1/level2
        /// A location string augmented with alias: level1/level2#somename
        /// Here path is "level1/level2" and alias is "somename".
        /// </summary>
        /// <param name="locationString">The location string.</param>
        /// <param name="path">The path to be extracted.</param>
        /// <param name="alias">The alias to be extracted.</param>
        public static void ExttractPathAndAliasFromLocationString(string locationString, out string path, out string alias)
        {
            int poundIndex = locationString.IndexOf('#');
            if(poundIndex >= 0)
            {
                if (poundIndex == 0)
                    path = "";
                else
                    path = locationString.Substring(0, poundIndex).Trim();

                if (poundIndex == locationString.Length - 1)
                    alias = "";
                else
                    alias = locationString.Substring(poundIndex + 1).Trim();
            }
            else
            {
                path = locationString;
                alias = "";
            }
        }


        /// <summary>
        /// Combines a location string and an element name to form a bigger location string.
        /// </summary>
        /// <param name="location">The location string.</param>
        /// <param name="elemName">Name of the element.</param>
        /// <returns>a bigger location string formed by combining a location string and an element name.</returns>
        public static string CombineLocationAndElementName(string location, string elemName)
        {
            return String.Format("{0}/{1}", location, elemName);
        }

        /// <summary>
        /// Divides the location string one step, to form a shorter location string.
        /// </summary>
        /// <param name="location">The location string to divide.</param>
        /// <param name="newLocation">The new location string which is one level shorter.</param>
        /// <param name="newElem">The element name removed from the end of location string.</param>
        /// <returns></returns>
        public static bool DivideLocationOneStep(string location, out string newLocation, out string newElem)
        {
            newLocation = location;
            newElem = null;
            
            int slashIdx = location.LastIndexOf('/');
            if (slashIdx < 0) // no slashes found
            {
                if (IsLocationAllGeneric(location))
                {
                    return false;
                }
                else
                {
                    newElem = location;
                    newLocation = ".";
                    return true;
                }
            }
            else // some slashes found
            {
                string preSlash = location.Substring(0, slashIdx);
                string postSlash = location.Substring(slashIdx + 1);

                if (IsLocationAllGeneric(postSlash))
                {
                    return false;
                }
                else
                {
                    newLocation = preSlash;
                    newElem = postSlash;
                    return true;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified location is composed of levels 
        /// which are themselves either "." or "..".
        /// </summary>
        /// <param name="location">The location string to check.</param>
        /// <returns>
        /// <c>true</c> if the specified location string is all composed of "." or ".." levels; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLocationAllGeneric(string location)
        {
            string[] locSteps = location.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string loc in locSteps)
            {
                if (!IsSingleLocationGeneric(loc))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified location string is either "." or "..".
        /// </summary>
        /// <param name="location">The location string to check.</param>
        /// <returns>
        /// <c>true</c> if the specified location string is either "." or ".."; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSingleLocationGeneric(string location)
        {
            return location == "." || location == "..";
        }

        /// <summary>
        /// Gets the string corresponidng to the given array dimensions.
        /// </summary>
        /// <param name="dims">The array dimensions.</param>
        /// <returns>the string corresponidng to the given array dimensions</returns>
        public static string GetArrayDimsString(int[] dims)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dims.Length; i++)
            {
                if (i != 0)
                    sb.Append(",");
                sb.Append(dims[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses the array dimensions string, and returns the corresponding dimensions array.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>the dimensions array corresponiding to the given string</returns>
        public static int[] ParseArrayDimsString(string str)
        {
            string[] strDims = str.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> lst = new List<int>();
            foreach (string strDim in strDims)
            {
                int dim;
                if (Int32.TryParse(strDim, out dim))
                    lst.Add(dim);
            }

            return lst.ToArray();
        }

    }
}
