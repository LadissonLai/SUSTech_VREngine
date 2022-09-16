// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Doozy.Engine.Nody.Models;
using UnityEngine;

// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        [NonSerialized] private static Type[] s_nodeTypes;

        /// <summary> All available node types </summary>
        private static Type[] NodeTypes { get { return s_nodeTypes ?? (s_nodeTypes = GetNodeTypes()); } }

        private static Type[] GetNodeTypes()
        {
            //Get all classes deriving from Node via reflection
            return GetDerivedTypes(typeof(Node));
        }

        /// <summary> Get all classes deriving from baseType via reflection </summary>
        private static Type[] GetDerivedTypes(Type baseType)
        {
            var types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) types.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());
            return types.ToArray();
        }

        public static object ObjectFromType(Type type) { return Activator.CreateInstance(type); }

        public static object ObjectFromFieldName(object obj, string fieldName)
        {
            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName);
            return fieldInfo.GetValue(obj);
        }

        public static KeyValuePair<ContextMenu, MethodInfo>[] GetContextMenuMethods(object obj)
        {
            Type type = obj.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var kvp = new List<KeyValuePair<ContextMenu, MethodInfo>>();
            foreach (MethodInfo method in methods)
            {
                ContextMenu[] attributes = method.GetCustomAttributes(typeof(ContextMenu), true).Select(x => x as ContextMenu).ToArray();
                if (attributes.Length == 0) continue;
                if (method.GetParameters().Length != 0)
                {
                    if (method.DeclaringType != null) Debug.LogWarning("Method " + method.DeclaringType.Name + "." + method.Name + " has parameters and cannot be used for context menu commands.");
                    continue;
                }

                if (method.IsStatic)
                {
                    if (method.DeclaringType != null) Debug.LogWarning("Method " + method.DeclaringType.Name + "." + method.Name + " is static and cannot be used for context menu commands.");
                    continue;
                }

                kvp.AddRange(attributes.Select(t => new KeyValuePair<ContextMenu, MethodInfo>(t, method)));
            }

            //Sort menu items
            kvp.Sort((x, y) => x.Key.priority.CompareTo(y.Key.priority));
            return kvp.ToArray();
        }
    }
}