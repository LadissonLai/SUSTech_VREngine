// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Doozy.Editor.Nody.NodeGUI
{
    /// <summary>
    ///     Handles caching of custom editor classes and their target types.
    ///     <para />
    ///     Accessible with GetEditor(UIConnectionTrigger type)
    /// </summary>
    /// <typeparam name="T"> NodeGUIBase </typeparam>
    /// <typeparam name="A"> Attribute </typeparam>
    /// <typeparam name="S"> ScriptableObject </typeparam>
    public class NodeGUIBase<T, A, S> where A : Attribute, NodeGUIBase<T, A, S>.INodeGUI where T : NodeGUIBase<T, A, S> where S : ScriptableObject
    {
        private static Dictionary<Type, Type> customEditorTypes;
        private static Dictionary<S, T> editors = new Dictionary<S, T>();
        public S target;
        public SerializedObject serializedObject;

        public static T GetEditor(S target)
        {
            if (target == null) return null;
            if (!editors.ContainsKey(target))
            {
                Type type = target.GetType();
                Type editorType = GetEditorType(type);
                editors.Add(target, Activator.CreateInstance(editorType) as T);
                editors[target].target = target;
                editors[target].serializedObject = new SerializedObject(target);
            }

            T editor = editors[target];
            if (editor.target == null) editor.target = target;
            if (editor.serializedObject == null) editor.serializedObject = new SerializedObject(target);
            return editor;
        }

        private static Type GetEditorType(Type type)
        {
            while (true)
            {
                if (type == null) return null;
                if (customEditorTypes == null) CacheCustomEditors(GetDerivedTypes(typeof(T)));
                if (customEditorTypes != null && customEditorTypes.ContainsKey(type)) return customEditorTypes[type];
                type = type.BaseType; //If type isn't found, try base type
            }
        }

        private static void CacheCustomEditors(ICollection<Type> customEditors) //get all classes deriving from Types via reflection
        {
            if (customEditors.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", "customEditors");
            customEditorTypes = new Dictionary<Type, Type>();
            foreach (Type editor in customEditors)
            {
                if (editor.IsAbstract) continue;
                object[] attributes = editor.GetCustomAttributes(typeof(A), false);
                if (attributes.Length == 0) continue;
                var attribute = attributes[0] as A;
                if (attribute != null) customEditorTypes.Add(attribute.GetInspectedType(), editor);
            }
        }

        /// <summary> Get all classes deriving from baseType via reflection </summary>
        public static Type[] GetDerivedTypes(Type baseType)
        {
            var types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
                if (assembly != null)
                    types.AddRange(assembly.GetTypes().Where(t => baseType != null && (!t.IsAbstract && baseType.IsAssignableFrom(t))).ToArray());
            return types.ToArray();
        }

        public interface INodeGUI
        {
            Type GetInspectedType();
        }
    }
}