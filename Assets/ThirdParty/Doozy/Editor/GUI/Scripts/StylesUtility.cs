// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    [Serializable]
    public class StylesUtility : ScriptableObject
    {
        public string Namespace = "Doozy";
        public string ClassName = "Styles";
        public string EnumName = "StyleName";
        public GUISkin DarkSkin;
        public GUISkin LightSkin;
    }

    [CustomEditor(typeof(StylesUtility))]
    public class StylesUtilityEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return ColorName.Teal; } }
        private StylesUtility Utility { get { return (StylesUtility) target; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderStylesUtility));
            GUILayout.Space(DGUI.Properties.Space(2));
            SerializedProperty namespaceProperty = GetProperty("Namespace");
            SerializedProperty classNameProperty = GetProperty("ClassName");
            SerializedProperty enumNameProperty = GetProperty("EnumName");
            SerializedProperty darkSkinProperty = GetProperty("DarkSkin");
            SerializedProperty lightSkinProperty = GetProperty("LightSkin");
            DGUI.Property.Draw(namespaceProperty, "Namespace", ComponentColorName, string.IsNullOrEmpty(namespaceProperty.stringValue));
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(classNameProperty, "Class Name", ComponentColorName, string.IsNullOrEmpty(classNameProperty.stringValue));
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(enumNameProperty, "Enum Name", ComponentColorName, string.IsNullOrEmpty(enumNameProperty.stringValue));
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(darkSkinProperty, "Dark Skin", ComponentColorName, darkSkinProperty.objectReferenceValue == null);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(lightSkinProperty, "Light Skin", ComponentColorName, lightSkinProperty.objectReferenceValue == null);
            GUILayout.Space(DGUI.Properties.Space(4));
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaCode),
                                                       "Generate Enum from Styles",
                                                       Size.M, TextAlign.Left,
                                                       ComponentColorName,
                                                       ComponentColorName,
                                                       DGUI.Properties.SingleLineHeight * 2,
                                                       false))
                    GenerateEnumFromStyles();
                GUILayout.Space(DGUI.Properties.Space(2));
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaSortAlphaDown),
                                                       "Sort Styles",
                                                       Size.M, TextAlign.Left,
                                                       ComponentColorName,
                                                       ComponentColorName,
                                                       DGUI.Properties.SingleLineHeight * 2,
                                                       false))
                    SortStyles();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }

        public void SortStyles()
        {
            bool lightSkinUpdated = false;
            if (Utility.LightSkin != null)
            {
                Utility.LightSkin.customStyles = Utility.LightSkin.customStyles.OrderBy(style => style.name).ToArray();
                EditorUtility.SetDirty(Utility.LightSkin);
                lightSkinUpdated = true;
            }

            bool darkSkinUpdated = false;
            if (Utility.DarkSkin != null)
            {
                Utility.DarkSkin.customStyles = Utility.DarkSkin.customStyles.OrderBy(style => style.name).ToArray();
                EditorUtility.SetDirty(Utility.DarkSkin);
                darkSkinUpdated = true;
            }

            if (!lightSkinUpdated && !darkSkinUpdated) return;
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private void GenerateEnumFromStyles()
        {
            List<string> styleNames = Utility.DarkSkin.customStyles.Select(style => style.name).ToList();
            string assetPath = AssetDatabase.GetAssetPath(Utility);
            string folderPath = assetPath.Replace(Utility.name + ".asset", "");
            string newFilePath = folderPath + Utility.EnumName + ".cs";
            Debug.Log(newFilePath);
            using (var streamWriter = new StreamWriter(newFilePath))
            {
                streamWriter.WriteLine(DGUI.Copyright.LINE_ONE);
                streamWriter.WriteLine(DGUI.Copyright.LINE_TWO);
                streamWriter.WriteLine(DGUI.Copyright.LINE_THREE);
                streamWriter.WriteLine("");
                streamWriter.WriteLine("// --- Generated Enum ---");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("namespace " + Utility.Namespace);
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("    public partial class " + Utility.ClassName);
                streamWriter.WriteLine("    {");
                streamWriter.WriteLine("        public enum " + Utility.EnumName);
                streamWriter.WriteLine("        {");
                for (int i = 0; i < styleNames.Count; i++)
                {
                    if (string.IsNullOrEmpty(styleNames[i]))
                    {
                        Debug.Log("[EditorTexture] While generating the " + Utility.EnumName + " enum, an empty entry was found at index:" + i);
                        continue;
                    }

                    streamWriter.WriteLine("            " + styleNames[i] + ",");
                }

                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
    }
}