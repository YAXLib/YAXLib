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
using System.Globalization;

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

            StringBuilder sb = new StringBuilder();
            string[] steps = elemAddr.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
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
            else
            {
                StringBuilder sb = new StringBuilder();

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
