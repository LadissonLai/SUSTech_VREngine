// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody
{
    [Serializable]
    public class NodyStylesBuilder : ScriptableObject
    {
        public GUISkin DarkSkin;
        public GUISkin LightSkin;
    }

    [CustomEditor(typeof(NodyStylesBuilder))]
    public class NodyStylesBuilderEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return ColorName.Teal; } }
        private NodyStylesBuilder Builder { get { return (NodyStylesBuilder) target; } }

        private const string ACCENT = "Accent";
        private const string AREA = "Area";
        private const string BODY = "Body";
        private const string BUTTON = "Button";
        private const string BUTTON_DELETE = "ButtonDelete";
        private const string COMPONENT = "Component";
        private const string CONNECTION_POINT = "ConnectionPoint";
        private const string DOT = "Dot";
        private const string FOOTER = "Footer";
        private const string GLOW = "Glow";
        private const string HEADER = "Header";
        private const string HORIZONTAL_DIVIDER = "HorizontalDivider";
        private const string ICON = "Icon";
        private const string NODE = "Node";
        private const string OUTLINE = "Outline";
        private const string SELECTED = "Selected";
        private const string SPLASH_SCREEN = "SplashScreen";
        private const string TOOLBAR = "Toolbar";
        private const string WINDOW = "Window";

        private enum ConnectionPointType
        {
            Minus,
            MultipleConnected,
            MultipleEmpty,
            OverrideConnected,
            OverrideEmpty
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.ComponentHeaderStylesBuilder));
            GUILayout.Space(DGUI.Properties.Space(2));

            SerializedProperty darkSkin = GetProperty("DarkSkin");
            SerializedProperty lightSkin = GetProperty("LightSkin");
            DGUI.Property.Draw(darkSkin, "Dark Skin", ComponentColorName, darkSkin.objectReferenceValue == null);
            GUILayout.Space(DGUI.Properties.Space(2));
            DGUI.Property.Draw(lightSkin, "Light Skin", ComponentColorName, lightSkin.objectReferenceValue == null);
            GUILayout.Space(DGUI.Properties.Space(4));
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (DGUI.Button.Dynamic.DrawIconButton(Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconFaRepeatAlt),
                                                       "Regenerate All Styles",
                                                       Size.M, TextAlign.Left,
                                                       ComponentColorName,
                                                       ComponentColorName,
                                                       DGUI.Properties.SingleLineHeight * 2,
                                                       false))
                    RegenerateAllStyles();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }

        private static Texture2D GetTexture(string fileName)
        {
            string filePath = Path.Combine(DoozyPath.EDITOR_NODY_IMAGES_PATH, fileName + ".png");
            var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
            if (texture2D == null) UnityEngine.Debug.Log("[StylesBuilder] Null texture! '" + filePath + "' was not found.");

            return texture2D;
        }

        #region Regenerate Styles

        private static List<GUIStyle> RegenerateStyles(Skin skin)
        {
            var styles = new List<GUIStyle>
            {
                NodeArea(),
                NodeBody(skin),
                NodeButtonDelete(skin),
                NodeDot(skin),
                NodeFooter(skin),
                NodeGlow(skin),
                NodeHeader(skin),
                NodeHeaderAccent(skin),
                NodeHorizontalDivider(skin),
                NodeOutline(skin),
                SplashScreen(skin)
            };

            styles.AddRange(ComponentHeaders(skin));
            styles.AddRange(ConnectionPoints(skin));
            styles.AddRange(Icons());
            styles.AddRange(NodeIcons());
            styles.AddRange(WindowToolbarButtons(skin));
            return styles;
        }

        private void RegenerateAllStyles()
        {
            List<GUIStyle> darkStyles = RegenerateStyles(Skin.Dark);
            List<GUIStyle> lightStyles = RegenerateStyles(Skin.Light);
            Builder.DarkSkin.customStyles = darkStyles.ToArray().OrderBy(style => style.name).ToArray();
            Builder.LightSkin.customStyles = lightStyles.ToArray().OrderBy(style => style.name).ToArray();
            EditorUtility.SetDirty(Builder.DarkSkin);
            EditorUtility.SetDirty(Builder.LightSkin);
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region Node Area

        private static GUIStyle NodeArea()
        {
            const string styleName = NODE + AREA;
            return new GUIStyle
            {
                name = styleName,
                overflow = new RectOffset(6, 6, 6, 6)
            };
        }

        #endregion

        #region Node Body

        private static GUIStyle NodeBody(Skin skin)
        {
            const string styleName = NODE + BODY;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(8, 8, 8, 8),
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion

        #region Node Button Delete

        private static GUIStyle NodeButtonDelete(Skin skin)
        {
            const string styleName = NODE + BUTTON_DELETE;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                hover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion

        #region Node Dot

        private static GUIStyle NodeDot(Skin skin)
        {
            const string styleName = NODE + DOT;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                imagePosition = ImagePosition.ImageOnly
            };
        }

        #endregion

        #region Node Footer

        private static GUIStyle NodeFooter(Skin skin)
        {
            const string styleName = NODE + FOOTER;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(8, 8, 0, 0),
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion

        #region Node Glow

        private static GUIStyle NodeGlow(Skin skin)
        {
            const string styleName = NODE + GLOW;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(12, 12, 12, 12),
                overflow = new RectOffset(-2, -2, -2, -2),
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion

        #region Node Header

        private static GUIStyle NodeHeader(Skin skin)
        {
            const string styleName = NODE + HEADER;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(8, 8, 8, 8),
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion

        #region Node Header Accent

        private static GUIStyle NodeHeaderAccent(Skin skin)
        {
            const string styleName = NODE + HEADER + ACCENT;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(1, 1, 0, 0),
                stretchWidth = true,
            };
        }

        #endregion

        #region Node Horizontal Divider

        private static GUIStyle NodeHorizontalDivider(Skin skin)
        {
            const string styleName = NODE + HORIZONTAL_DIVIDER;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(1, 1, 0, 0),
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion

        #region Node Outline

        private static GUIStyle NodeOutline(Skin skin)
        {
            const string styleName = NODE + OUTLINE;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(12, 12, 12, 12),
                overflow = new RectOffset(-4, -4, -4, -4),
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion

        #region Splash Screen

        private static GUIStyle SplashScreen(Skin skin)
        {
            const string styleName = SPLASH_SCREEN;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                imagePosition = ImagePosition.ImageOnly,
                stretchWidth = true,
                stretchHeight = true
            };
        }

        #endregion


        #region Connection Points

        private static IEnumerable<GUIStyle> ConnectionPoints(Skin skin)
        {
            var styles = new List<GUIStyle>();
            foreach (ConnectionPointType pointType in Enum.GetValues(typeof(ConnectionPointType)))
                styles.Add(ConnectionPoint(pointType, skin));
            return styles;
        }

        private static GUIStyle ConnectionPoint(ConnectionPointType type, Skin skin)
        {
            string styleName = CONNECTION_POINT + type;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                hover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                imagePosition = ImagePosition.ImageOnly
            };
        }

        #endregion

        #region Icons
        private static GUIStyle Icon(string iconName)
        {
            return new GUIStyle
            {
                name = ICON + iconName,
                normal =
                {
                    background = GetTexture(ICON + iconName)
                },
                imagePosition = ImagePosition.ImageOnly
            };
        }

         private static List<GUIStyle> Icons()
        {
            return new List<GUIStyle>
            {
                Icon("Graph"),
                Icon("SubGraph"),
                Icon("GraphController"),
            };
        }
        #endregion
        
        #region Node Icon

        private static GUIStyle NodeIcon(string iconName)
        {
            string styleName = NODE + ICON + iconName;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName)},
                imagePosition = ImagePosition.ImageOnly
            };
        }

        private static IEnumerable<GUIStyle> NodeIcons()
        {
            return new List<GUIStyle>
            {
                NodeIcon("EnterNode"),
                NodeIcon("ExitNode"),
                NodeIcon("StartNode"),
                NodeIcon("SubGraphNode"),
                NodeIcon("SwitchBackNode")
            };
        }

        #endregion

        #region ComponentHeaders

        public static GUIStyle ComponentHeader(string componentName, Skin skin)
        {
            string styleName = COMPONENT + HEADER + componentName;
            return new GUIStyle
            {
                name = styleName,
                normal = {background = GetTexture(styleName + skin)},
                border = new RectOffset(292, 28, 0, 0),
                imagePosition = ImagePosition.ImageOnly,
                fixedHeight = 48
            };
        }


        private static List<GUIStyle> ComponentHeaders(Skin skin)
        {
            var styles = new List<GUIStyle>
            {
                ComponentHeader("EnterNode", skin),
                ComponentHeader("ExitNode", skin),
                ComponentHeader("Graph", skin),
                ComponentHeader("GraphController", skin),
                ComponentHeader("StartNode", skin),
                ComponentHeader("SubGraph", skin),
                ComponentHeader("SubGraphNode", skin),
                ComponentHeader("SwitchBackNode", skin),
            };

            return styles;
        }

        #endregion
        
        #region WindowToolbarButtons
         private static List<GUIStyle> WindowToolbarButtons(Skin skin)
        {
            var styles = new List<GUIStyle>();
            styles.AddRange(WindowToolbarButton("Close", skin));
            styles.AddRange(WindowToolbarButton("Load", skin));
            styles.AddRange(WindowToolbarButton("New", skin));
            styles.AddRange(WindowToolbarButton("NewGraph", skin));
            styles.AddRange(WindowToolbarButton("NewSubGraph", skin));
            styles.AddRange(WindowToolbarButton("Save", skin));
            styles.AddRange(WindowToolbarButton("SaveAs", skin));
            return styles;
        }

        private static List<GUIStyle> WindowToolbarButton(string buttonName, Skin skin)
        {
            const TextAnchor alignment = TextAnchor.MiddleLeft;
            var border = new RectOffset(30, 1, 2, 2);
            var padding = new RectOffset(34, 4, 2, 2);
            const int fontSize = 14;
            Font font = DGUI.Fonts.SansationRegular;

            string styleName = WINDOW + TOOLBAR + BUTTON + buttonName;
            string styleNameSelected = styleName + SELECTED;
            return new List<GUIStyle>
            {
                new GUIStyle
                {
                    name = styleName,
                    normal = {background = GetTexture(styleName + StyleState.Normal + skin), textColor = skin == Skin.Dark ? EditorColors.Instance.UnityLight.Normal : EditorColors.Instance.UnityLight.Dark},
                    hover = {background = GetTexture(styleName + StyleState.Hover + skin), textColor = skin == Skin.Dark ? new Color().ColorFrom256(229, 229, 229) : EditorColors.Instance.Gray.Normal},
                    active = {background = GetTexture(styleName + StyleState.Active + skin), textColor = skin == Skin.Dark ? EditorColors.Instance.Gray.Normal : EditorColors.Instance.Gray.Dark},
                    onNormal = {background = GetTexture(styleName + StyleState.Normal + skin), textColor = skin == Skin.Dark ? EditorColors.Instance.UnityLight.Normal : EditorColors.Instance.UnityLight.Dark},
                    onHover = {background = GetTexture(styleName + StyleState.Hover + skin), textColor = skin == Skin.Dark ? new Color().ColorFrom256(229, 229, 229) : EditorColors.Instance.Gray.Dark},
                    onActive = {background = GetTexture(styleName + StyleState.Active + skin), textColor = EditorColors.Instance.Gray.Normal},
                    alignment = alignment,
                    border = border,
                    padding = padding,
                    fontSize = fontSize,
                    font = font
                },

                new GUIStyle
                {
                    name = styleNameSelected,
                    normal = {background = GetTexture(styleNameSelected + skin), textColor = skin == Skin.Dark ? Color.white : EditorColors.Instance.Gray.Normal},
                    onNormal = {background = GetTexture(styleNameSelected + skin), textColor = skin == Skin.Dark ? Color.white : EditorColors.Instance.Gray.Normal},
                    alignment = alignment,
                    border = border,
                    padding = padding,
                    fontSize = fontSize,
                    font = font
                }
            };
        }
        #endregion
    }
}