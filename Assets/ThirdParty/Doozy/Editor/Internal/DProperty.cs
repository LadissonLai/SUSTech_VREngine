// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine;
using UnityEditor;

namespace Doozy.Editor.Internal
{
    public class DProperty
    {
        private readonly SerializedObject m_serializedObject;
        private readonly Dictionary<string, SerializedProperty> m_childProperties = new Dictionary<string, SerializedProperty>();

        public DProperty(SerializedObject serializedObject) { m_serializedObject = serializedObject; }

        public SerializedProperty Get(string propertyName)
        {
            if (m_childProperties.ContainsKey(propertyName)) return m_childProperties[propertyName];
            SerializedProperty s = m_serializedObject.FindProperty(propertyName);
            if (s == null)
            {
                DDebug.Log("Property '" + propertyName + "' was not found.");
                return null;
            }

            m_childProperties.Add(propertyName, s);
            return m_childProperties[propertyName];
        }

        public void Clear()
        {
            m_childProperties.Clear();
        }
    }
}