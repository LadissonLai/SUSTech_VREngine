// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.IO;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global

namespace Doozy.Editor
{
    public class EditorColors : ScriptableObject
    {
        private static EditorColors s_instance;

        public static EditorColors Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetDatabase.LoadAssetAtPath<EditorColors>(Path.Combine(DoozyPath.EDITOR_GUI_PATH, "EditorColors.asset"));
                return s_instance;
            }
        }
        
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        private static DColor UnknownColor { get { return new DColor("Unknown Color", Color.magenta, Color.magenta, Color.magenta); } }

        [Header("Base Colors")] public DColor Red = new DColor("Red",
                                                               new Color().ColorFrom256(255, 162, 180),
                                                               new Color().ColorFrom256(255, 23, 68),
                                                               new Color().ColorFrom256(128, 12, 34));

        public DColor Pink = new DColor("Pink",
                                        new Color().ColorFrom256(252, 173, 201),
                                        new Color().ColorFrom256(247, 51, 121),
                                        new Color().ColorFrom256(124, 26, 61));

        public DColor Purple = new DColor("Purple",
                                          new Color().ColorFrom256(238, 153, 253),
                                          new Color().ColorFrom256(213, 0, 249),
                                          new Color().ColorFrom256(107, 0, 125));

        public DColor DeepPurple = new DColor("DeepPurple",
                                              new Color().ColorFrom256(192, 153, 247),
                                              new Color().ColorFrom256(98, 0, 234),
                                              new Color().ColorFrom256(49, 0, 117));

        public DColor Indigo = new DColor("Indigo",
                                          new Color().ColorFrom256(177, 189, 255),
                                          new Color().ColorFrom256(61, 90, 254),
                                          new Color().ColorFrom256(31, 45, 127));

        public DColor Blue = new DColor("Blue",
                                        new Color().ColorFrom256(169, 201, 255),
                                        new Color().ColorFrom256(41, 121, 255),
                                        new Color().ColorFrom256(21, 61, 128));

        public DColor LightBlue = new DColor("LightBlue",
                                             new Color().ColorFrom256(153, 223, 255),
                                             new Color().ColorFrom256(0, 175, 255),
                                             new Color().ColorFrom256(0, 88, 128));

        public DColor Cyan = new DColor("Cyan",
                                        new Color().ColorFrom256(153, 245, 255),
                                        new Color().ColorFrom256(0, 229, 255),
                                        new Color().ColorFrom256(0, 115, 128));

        public DColor Teal = new DColor("Teal",
                                        new Color().ColorFrom256(165, 246, 226),
                                        new Color().ColorFrom256(29, 233, 182),
                                        new Color().ColorFrom256(15, 117, 91));

        public DColor Green = new DColor("Green",
                                         new Color().ColorFrom256(153, 245, 200),
                                         new Color().ColorFrom256(0, 230, 118),
                                         new Color().ColorFrom256(0, 115, 59));

        public DColor LightGreen = new DColor("LightGreen",
                                              new Color().ColorFrom256(200, 255, 154),
                                              new Color().ColorFrom256(118, 255, 3),
                                              new Color().ColorFrom256(59, 128, 2));

        public DColor Lime = new DColor("Lime",
                                        new Color().ColorFrom256(232, 255, 153),
                                        new Color().ColorFrom256(198, 255, 0),
                                        new Color().ColorFrom256(99, 128, 0));

        public DColor Yellow = new DColor("Yellow",
                                          new Color().ColorFrom256(255, 247, 153),
                                          new Color().ColorFrom256(255, 234, 0),
                                          new Color().ColorFrom256(128, 117, 0));

        public DColor Amber = new DColor("Amber",
                                         new Color().ColorFrom256(255, 231, 153),
                                         new Color().ColorFrom256(255, 196, 0),
                                         new Color().ColorFrom256(128, 98, 0));

        public DColor Orange = new DColor("Orange",
                                          new Color().ColorFrom256(255, 211, 153),
                                          new Color().ColorFrom256(255, 145, 0),
                                          new Color().ColorFrom256(128, 73, 0));

        public DColor DeepOrange = new DColor("DeepOrange",
                                              new Color().ColorFrom256(255, 177, 153),
                                              new Color().ColorFrom256(255, 61, 0),
                                              new Color().ColorFrom256(128, 31, 0));

        public DColor UnityLight = new DColor("UnityLight", new Color().FromHex("E0E0E0"), new Color().FromHex("C2C2C2"), new Color().FromHex("A4A4A4"));
        public DColor Gray = new DColor("Gray", new Color().FromHex("9B9B9B"), new Color().FromHex("7D7D7D"), new Color().FromHex("5F5F5F"));
        public DColor UnityDark = new DColor("UnityDark", new Color().FromHex("666666"), new Color().FromHex("484848"), new Color().FromHex("2A2A2A"));

        [Header("Info Colors")] public DColor Info = new DColor("Info", new Color().ColorFrom256(123, 222, 240), new Color().ColorFrom256(34, 200, 230), new Color().ColorFrom256(17, 100, 115));
        public DColor Ok = new DColor("Ok", new Color().ColorFrom256(181, 240, 123), new Color().ColorFrom256(132, 230, 34), new Color().ColorFrom256(66, 115, 17));
        public DColor Warning = new DColor("Warning", new Color().ColorFrom256(240, 198, 116), new Color().ColorFrom256(230, 161, 23), new Color().ColorFrom256(115, 80, 11));
        public DColor Error = new DColor("Error", new Color().ColorFrom256(224, 102, 102), new Color().ColorFrom256(204, 0, 0), new Color().ColorFrom256(102, 0, 0));
        public DColor Help = new DColor("Help", new Color().ColorFrom256(145, 145, 145), new Color().ColorFrom256(72, 72, 72), new Color().ColorFrom256(86, 86, 86));

        [Header("Nody Colors")] public DColor Input = new DColor("Input", new Color().ColorFrom256(255, 187, 51), new Color().ColorFrom256(255, 170, 0), new Color().ColorFrom256(191, 128, 0));
        public DColor Output = new DColor("Output", new Color().ColorFrom256(51, 187, 255), new Color().ColorFrom256(0, 170, 255), new Color().ColorFrom256(0, 128, 191));

        [Header("Custom Colors")] public List<DColor> CustomColors;

        public DColor GetCustomColor(string colorName)
        {
            if (CustomColors == null || CustomColors.Count == 0) return UnknownColor;
            foreach (DColor customColor in CustomColors)
                if (customColor.ColorName.Equals(colorName))
                    return customColor;
            return UnknownColor;
        }

        public void AddCustomColor(DColor dColor)
        {
            if (CustomColors == null) CustomColors = new List<DColor>();
            if (CustomColors.Contains(dColor)) return;
            CustomColors.Add(dColor);
            Save();
        }

        public void RemoveCustomColor(DColor dColor)
        {
            if (CustomColors == null || CustomColors.Count == 0 || !CustomColors.Contains(dColor)) return;
            CustomColors.Remove(dColor);
            Save();
        }

        public void RemoveCustomColor(string colorName)
        {
            if (CustomColors == null || CustomColors.Count == 0) return;
            DColor targetColor = null;
            foreach (DColor dColor in CustomColors)
            {
                if (!dColor.ColorName.Equals(colorName)) continue;
                targetColor = dColor;
                break;
            }

            if (targetColor == null) return;
            CustomColors.Remove(targetColor);
            Save();
        }

        private void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [CustomEditor(typeof(EditorColors))]
    public class EditorColorsEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return ColorName.Gray; } }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderEditorColors));
            DGUI.Property.Draw(GetProperty("Red"));
            DGUI.Property.Draw(GetProperty("Pink"));
            DGUI.Property.Draw(GetProperty("Purple"));
            DGUI.Property.Draw(GetProperty("DeepPurple"));
            DGUI.Property.Draw(GetProperty("Indigo"));
            DGUI.Property.Draw(GetProperty("Blue"));
            DGUI.Property.Draw(GetProperty("LightBlue"));
            DGUI.Property.Draw(GetProperty("Cyan"));
            DGUI.Property.Draw(GetProperty("Teal"));
            DGUI.Property.Draw(GetProperty("Green"));
            DGUI.Property.Draw(GetProperty("LightGreen"));
            DGUI.Property.Draw(GetProperty("Lime"));
            DGUI.Property.Draw(GetProperty("Yellow"));
            DGUI.Property.Draw(GetProperty("Amber"));
            DGUI.Property.Draw(GetProperty("Orange"));
            DGUI.Property.Draw(GetProperty("DeepOrange"));
            GUILayout.Space(8);
            DGUI.Property.Draw(GetProperty("UnityLight"));
            DGUI.Property.Draw(GetProperty("Gray"));
            DGUI.Property.Draw(GetProperty("UnityDark"));
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }
    }
}