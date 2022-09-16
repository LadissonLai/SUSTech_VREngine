using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Doozy.Editor.Nody.Utils
{
    public static class ReflectionUtils
    {
        public static bool GetAttribute<T>(Type classType, out T attributeOut) where T : Attribute
        {
            object[] attributes = classType.GetCustomAttributes(typeof(T), false);
            return GetAttribute(attributes, out attributeOut);
        }

        private static bool GetAttribute<T>(IEnumerable<object> attributes, out T attributeOut) where T : Attribute
        {
            foreach (object attribute in attributes)
                if (attribute.GetType() == typeof(T))
                {
                    attributeOut = attribute as T;
                    return true;
                }

            attributeOut = null;
            return false;
        }

        public static bool GetAttribute<T>(Type classType, string fieldName, out T attributeOut) where T : Attribute
        {
            object[] attributes = classType.GetField(fieldName).GetCustomAttributes(typeof(T), false);
            return GetAttribute(attributes, out attributeOut);
        }

        public static bool HasAttribute<T>(IEnumerable<object> attributes) where T : Attribute { return attributes.Any(t => t.GetType() == typeof(T)); }

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

        /// <summary> Return a pretty field type name. </summary>
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
                string s = "";
                Type genericType = type.GetGenericTypeDefinition();
                s = genericType == typeof(List<>) ? "List" : type.GetGenericTypeDefinition().ToString();

                Type[] types = type.GetGenericArguments();
                var stringTypes = new string[types.Length];
                for (int i = 0; i < types.Length; i++) stringTypes[i] = types[i].PrettyName();
                return s + "<" + string.Join(", ", stringTypes) + ">";
            }

            if (!type.IsArray) return type.ToString();
            {
                string rank = "";
                for (int i = 1; i < type.GetArrayRank(); i++) rank += ",";
                Type elementType = type.GetElementType();
                if (elementType != null && !elementType.IsArray) return elementType.PrettyName() + "[" + rank + "]";

                {
                    string s = elementType.PrettyName();
                    int i = s.IndexOf('[');
                    return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
                }
            }

        }
    }
}