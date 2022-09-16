// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine;
using UnityEditor;

namespace Doozy.Editor.Internal
{
    public class DPropertyRelative
    {
        private readonly Dictionary<string, SerializedProperty> m_childProperties = new Dictionary<string, SerializedProperty>();

        public SerializedProperty Add(PropertyName propertyName, SerializedProperty parentProperty) { return Add(propertyName.ToString(), parentProperty);}
        public SerializedProperty Add(string propertyName, SerializedProperty parentProperty)
        {
            string key = parentProperty.propertyPath + "." + propertyName;
            if (m_childProperties.ContainsKey(key))  return m_childProperties[key];
            SerializedProperty s = parentProperty.FindPropertyRelative(propertyName);
            if (s == null)
            {
                DDebug.Log("Property '" + key + "' was not found.");
                return null;
            }
            m_childProperties.Add(key, s);
            return m_childProperties[key];
        }


        public SerializedProperty Get(PropertyName propertyName, SerializedProperty parentProperty) { return Get(propertyName.ToString(), parentProperty);}
        public SerializedProperty Get(string propertyName, SerializedProperty parentProperty)
        {
            string key = parentProperty.propertyPath + "." + propertyName;
            return m_childProperties[key];
        }
    }
}