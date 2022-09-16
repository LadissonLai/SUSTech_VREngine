// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Editor.Internal
{
    public class DGUIContent
    {
        private readonly Dictionary<string, GUIContent> m_contents = new Dictionary<string, GUIContent>();
        private readonly Dictionary<string, Vector2> m_sizes = new Dictionary<string, Vector2>();

        private static GUIStyle DefaultStyle { get { return DGUI.Label.Style(); } }

        private static string Key(string text, GUIStyle style) { return style.name + ":" + text; }
        private static string Key(string text) { return DefaultStyle.name + ":" + text; }

        public GUIContent Get(string text) { return Get(text, DefaultStyle); }

        public GUIContent Get(string text, GUIStyle style) { return m_contents[Key(text, style)]; }

        public Vector2 GetSize(GUIContent content) { return GetSize(content, DefaultStyle); }

        public Vector2 GetSize(GUIContent content, GUIStyle style)
        {
            string key = Key(content.text, style);
            if (m_sizes.ContainsKey(key)) return m_sizes[key];
            if (m_contents.ContainsKey(key))
                m_sizes.Add(key, style.CalcSize(m_contents[key]));
            else
                Add(content, style);
            return m_sizes[key];
        }

        public Vector2 GetSize(string text) { return GetSize(text, DefaultStyle); }

        public Vector2 GetSize(string text, GUIStyle style)
        {
            string key = Key(text, style);
            if (m_sizes.ContainsKey(key)) return m_sizes[key];
            if (m_contents.ContainsKey(key))
                m_sizes.Add(key, style.CalcSize(m_contents[key]));
            else
                Add(text, style);
            return m_sizes[key];
        }

        public float GetWidth(string text) { return GetWidth(text, DefaultStyle); }
        public float GetWidth(string text, GUIStyle style) { return GetSize(text, style).x; }
        public float GetHeight(string text) { return GetHeight(text, DefaultStyle); }
        public float GetHeight(string text, GUIStyle style) { return GetSize(text, style).y; }

        public GUIContent Add(GUIContent content) { return Add(content, DefaultStyle); }

        public GUIContent Add(GUIContent content, GUIStyle style)
        {
            string key = Key(content.text, style);
            if (m_contents.ContainsKey(key)) return m_contents[key];
            m_contents.Add(key, content);
            m_sizes.Add(key, style.CalcSize(content));
            return m_contents[key];
        }

        public GUIContent Add(string text) { return Add(text, DefaultStyle); }
        public GUIContent Add(string text, GUIStyle style) { return Add(new GUIContent(text), style); }
        public GUIContent Add(string text, string tooltip) { return Add(text, tooltip, DefaultStyle); }
        public GUIContent Add(string text, string tooltip, GUIStyle style) { return Add(new GUIContent(text, tooltip), style); }

        public void Clear()
        {
            m_contents.Clear();
            m_sizes.Clear();
        }
    }
}