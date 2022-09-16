// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Doozy.Editor
{
    public static class SystemExtensions
    {
        /// <summary> Returns true if this can be casted to <see cref="Type" /></summary>
        public static bool IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from)) return true;
            IEnumerable<MethodInfo> methods = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                  .Where(
                                                      m => m.ReturnType == to &&
                                                           (m.Name == "op_Implicit" ||
                                                            m.Name == "op_Explicit")
                                                  );
            return methods.Any();
        }
        
        /// <summary> Return a prettiefied type name. </summary>
        public static string PrettyName(this Type type)
        {
            if (type == null) return "null";
            if (type == typeof(object)) return "object";
            if (type == typeof(float)) return "float";

            if (type == typeof(int)) return "int";

            if (type == typeof(long)) return "long";

            if (type == typeof(double)) return "double";

            if (type == typeof(string)) return "string";

            if (type == typeof(bool)) return "bool";

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                string s = genericType == typeof(List<>) ? "List" : type.GetGenericTypeDefinition().ToString();

                Type[] types = type.GetGenericArguments();
                var stypes = new string[types.Length];
                for (int i = 0; i < types.Length; i++) stypes[i] = PrettyName(types[i]);
                return s + "<" + string.Join(", ", stypes) + ">";
            }

            if (!type.IsArray) return type.ToString();
            {
                string rank = "";
                for (int i = 1; i < type.GetArrayRank(); i++) rank += ",";
                Type elementType = type.GetElementType();
                if (elementType != null && !elementType.IsArray) return PrettyName(elementType) + "[" + rank + "]";

                {
                    string s = PrettyName(elementType);
                    int i = s.IndexOf('[');
                    return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
                }
            }
        }
    }
}