// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    [Serializable]
    public class StylesBuilder : ScriptableObject
    {
        public GUISkin DarkSkin;
        public GUISkin LightSkin;
    }

    [CustomEditor(typeof(StylesBuilder))]
    public class StylesBuilderEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return ColorName.Teal; } }
        private StylesBuilder Builder { get { return (StylesBuilder) target; } }

        private const string ARROW = "Arrow";
        private const string BACKGROUND = "Background";
        private const string BAR = "Bar";
        private const string BUTTON = "Button";
        private const string CHECKBOX = "CheckBox";
        private const string COLOR = "Color";
        private const string COMPONENT = "Component";
        private const string DIVIDER = "Divider";
        private const string DOOZY = "Doozy";
        private const string HEADER = "Header";
        private const string ICON = "Icon";
        private const string INFO_MESSAGE = "InfoMessage";
        private const string LABEL = "Label";
        private const string MESSAGE = "Message";
        private const string NODE = "Node";
        private const string RADIO = "Radio";
        private const string ROUNDED = "Rounded";
        private const string SECONDARY = "Secondary";
        private const string SELECTED = "Selected";
        private const string SWITCH = "Switch";
        private const string VIEW = "View";
        private const string TAB = "Tab";
        private const string TITLE = "Title";
        private const string TOOLBAR = "Toolbar";
        private const string WHITE_GRADIENT = "WhiteGradient";
        private const string WINDOW = "Window";

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderStylesBuilder));
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
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaRepeatAlt),
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
            string filePath = Path.Combine(DoozyPath.EDITOR_IMAGES_PATH, fileName + ".png");
            var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
            if (texture2D == null) Debug.Log("[StylesBuilder] Null texture! '" + filePath + "' was not found.");

            return texture2D;
        }

        #region Regenerate Styles

        private static List<GUIStyle> RegenerateStyles(Skin skin)
        {
            var styles = new List<GUIStyle>();
            styles.AddRange(ArrowButtons(skin));
            styles.AddRange(Backgrounds(skin));

            styles.AddRange(Buttons(skin));
            styles.AddRange(Bars(skin));

            styles.AddRange(CheckBoxes(skin));
            
            styles.AddRange(ColorButtons());

            styles.AddRange(ComponentHeaders(skin));

            styles.AddRange(Dividers(skin));

            styles.AddRange(Icons(skin));
            styles.AddRange(IconButtons());

            styles.AddRange(InfoMessage());

            styles.AddRange(Labels(skin));

            styles.AddRange(NodeIcons());

            styles.Add(RadioButton(ToggleState.Disabled, skin));
            styles.Add(RadioButton(ToggleState.Enabled, skin));

            styles.AddRange(Switches(skin));

            styles.AddRange(Tabs(skin));

            styles.Add(TitleBackground(skin));

            styles.AddRange(WhiteGradients());

            styles.Add(DoozyWindowIconBackground(skin));
            styles.AddRange(DoozyWindowToolbarButtons(skin));
            styles.AddRange(DoozyWindowViewHeader(skin));
            styles.AddRange(WindowToolbarElements());
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

        #region ArrowButtons

        private static List<GUIStyle> ArrowButtons(Skin skin)
        {
            var styles = new List<GUIStyle>();
            foreach (ShortDirection direction in Enum.GetValues(typeof(ShortDirection)))
                styles.Add(ArrowButton(direction, skin));
            return styles;
        }

        private static GUIStyle ArrowButton(ShortDirection direction, Skin skin)
        {
            string styleName = ARROW + BUTTON + direction;
            var style = new GUIStyle
                        {
                            name = styleName,
                            normal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                            hover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                            active = {background = GetTexture(styleName + StyleState.Active + skin)},
                            onNormal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                            onHover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                            onActive = {background = GetTexture(styleName + StyleState.Active + skin)},
                            imagePosition = ImagePosition.ImageOnly
                        };

            switch (direction)
            {
                case ShortDirection.ToRight:
                    style.border = new RectOffset(30, 0, 0, 0);
                    break;
                case ShortDirection.ToBottom:
                    style.border = new RectOffset(0, 0, 30, 0);
                    break;
                case ShortDirection.ToLeft:
                    style.border = new RectOffset(0, 30, 0, 0);
                    break;
                case ShortDirection.ToTop:
                    style.border = new RectOffset(0, 0, 0, 30);
                    break;
                default: throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            return style;
        }

        #endregion

        #region Backgrounds

        private static List<GUIStyle> Backgrounds(Skin skin)
        {
            var styles = new List<GUIStyle>();
            foreach (CornerType cornerType in Enum.GetValues(typeof(CornerType)))
                styles.Add(Background(cornerType, skin));
            return styles;
        }

        private static GUIStyle Background(CornerType cornerType, Skin skin)
        {
            string styleName = BACKGROUND + cornerType;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName + skin)},
                       border = new RectOffset(4, 4, 4, 4),
                       imagePosition = ImagePosition.ImageOnly,
                       stretchWidth = true,
                       stretchHeight = true
                   };
        }

        #endregion

        #region UIButton

        private static TextAnchor GetTextAnchor(TextAlign textAlign)
        {
            switch (textAlign)
            {
                case TextAlign.Left:   return TextAnchor.MiddleLeft;
                case TextAlign.Center: return TextAnchor.MiddleCenter;
                case TextAlign.Right:  return TextAnchor.MiddleRight;
                default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
            }
        }

        private static List<GUIStyle> Buttons(Skin skin)
        {
            var styles = new List<GUIStyle>();

            //BUTTON LABELS
            foreach (TextAlign textAlign in Enum.GetValues(typeof(TextAlign)))
            foreach (Size size in Enum.GetValues(typeof(Size)))
                styles.Add(ButtonLabel(textAlign, size, skin));

            //BUTTONS
            foreach (State state in Enum.GetValues(typeof(State)))
                styles.Add(Button(state, skin));

            //BUTTONS LEFT MIDDLE RIGHT
            foreach (State state in Enum.GetValues(typeof(State)))
            foreach (TabPosition tabPosition in Enum.GetValues(typeof(TabPosition)))
                styles.Add(Button(state, tabPosition, skin));

            return styles;
        }

        private static GUIStyle ButtonLabel(TextAlign textAlign, Size size, Skin skin)
        {
            string styleName = BUTTON + LABEL + size + textAlign;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},
                       onNormal = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},
                       hover = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                       onHover = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                       active = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Active, skin)},
                       onActive = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Active, skin)},
                       focused = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                       onFocused = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                       padding = new RectOffset(4, 4, 2, 2),
                       fontSize = DGUI.Sizes.ButtonFontSize(size),
                       alignment = GetTextAnchor(textAlign),
                       clipping = TextClipping.Clip,
                       stretchWidth = true
                   };
        }

        private static GUIStyle Button(State state, Skin skin)
        {
            string styleName = BUTTON + state;
            return new GUIStyle(ButtonLabel(TextAlign.Center, Size.M, skin))
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                       onNormal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                       hover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       onHover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       active = {background = GetTexture(styleName + StyleState.Active + skin)},
                       onActive = {background = GetTexture(styleName + StyleState.Active + skin)},
                       focused = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       onFocused = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       border = new RectOffset(6, 6, 6, 6)
                   };
        }

        private static GUIStyle Button(State state, TabPosition position, Skin skin)
        {
            string styleName = BUTTON + position + state;
            string filenamePrefix = BUTTON + state + position;
            return new GUIStyle(ButtonLabel(TextAlign.Center, Size.M, skin))
                   {
                       name = styleName,
                       normal = {background = GetTexture(filenamePrefix + StyleState.Normal + skin)},
                       onNormal = {background = GetTexture(filenamePrefix + StyleState.Normal + skin)},
                       hover = {background = GetTexture(filenamePrefix + StyleState.Hover + skin)},
                       onHover = {background = GetTexture(filenamePrefix + StyleState.Hover + skin)},
                       active = {background = GetTexture(filenamePrefix + StyleState.Active + skin)},
                       onActive = {background = GetTexture(filenamePrefix + StyleState.Active + skin)},
                       focused = {background = GetTexture(filenamePrefix + StyleState.Hover + skin)},
                       onFocused = {background = GetTexture(filenamePrefix + StyleState.Hover + skin)},
                       border = new RectOffset(6, 6, 6, 6)
                   };
        }

        #endregion

        #region Bars

        private static List<GUIStyle> Bars(Skin skin)
        {
            var styles = new List<GUIStyle>();

            foreach (Size size in Enum.GetValues(typeof(Size)))
            foreach (State state in Enum.GetValues(typeof(State)))
                styles.AddRange(Bar(size, state, skin));

            return styles;
        }

        private static List<GUIStyle> Bar(Size size, State state, Skin skin)
        {
            GUIStyle label = ButtonLabel(TextAlign.Left, size, skin);
            label.name = BAR + size + state + LABEL;
            label.fixedHeight = DGUI.Sizes.BarHeight(size);

            float caretSize = DGUI.Sizes.BarHeight(size) * DGUI.Bar.CARET_SIZE_RATIO;
            int leftPadding = (int) (caretSize * 1.3f);

            switch (size)
            {
                case Size.S:
                    label.padding = new RectOffset(leftPadding, 6, 2, 2);
                    break;
                case Size.M:
                    label.padding = new RectOffset(leftPadding, 6, 2, 2);
                    break;
                case Size.L:
                    label.padding = new RectOffset(leftPadding, 6, 2, 2);
                    break;
                case Size.XL:
                    label.padding = new RectOffset(leftPadding, 6, 2, 2);
                    break;
                default: throw new ArgumentOutOfRangeException("size", size, null);
            }

            string barStyleName = BAR + size + state;
            string textureFileNamePrefix = BUTTON + state;
            var bar = new GUIStyle(label)
                      {
                          name = barStyleName,
                          normal = {background = GetTexture(textureFileNamePrefix + StyleState.Normal + skin)},
                          onNormal = {background = GetTexture(textureFileNamePrefix + StyleState.Normal + skin)},
                          hover = {background = GetTexture(textureFileNamePrefix + StyleState.Hover + skin)},
                          onHover = {background = GetTexture(textureFileNamePrefix + StyleState.Hover + skin)},
                          active = {background = GetTexture(textureFileNamePrefix + StyleState.Active + skin)},
                          onActive = {background = GetTexture(textureFileNamePrefix + StyleState.Active + skin)},
                          focused = {background = GetTexture(textureFileNamePrefix + StyleState.Hover + skin)},
                          onFocused = {background = GetTexture(textureFileNamePrefix + StyleState.Hover + skin)},
                          border = new RectOffset(6, 6, 6, 6)
                      };

            return new List<GUIStyle> {label, bar};
        }

        #endregion

        #region CheckBoxes

        private static List<GUIStyle> CheckBoxes(Skin skin)
        {
            var styles = new List<GUIStyle>();
            foreach (ToggleState toggleState in Enum.GetValues(typeof(ToggleState)))
                styles.Add(CheckBox(toggleState, skin));
            return styles;
        }

        private static GUIStyle CheckBox(ToggleState toggleState, Skin skin)
        {
            string styleName = CHECKBOX + toggleState;
            return new GUIStyle
                   {
                       name = styleName,

                       normal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                       onNormal = {background = GetTexture(styleName + StyleState.Normal + skin)},

                       hover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       onHover = {background = GetTexture(styleName + StyleState.Hover + skin)},

                       active = {background = GetTexture(styleName + StyleState.Active + skin)},
                       onActive = {background = GetTexture(styleName + StyleState.Active + skin)},

                       focused = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       onFocused = {background = GetTexture(styleName + StyleState.Hover + skin)},

                       imagePosition = ImagePosition.ImageOnly,
                       fixedWidth = DGUI.Sizes.CHECKBOX_WIDTH,
                       fixedHeight = DGUI.Sizes.CHECKBOX_HEIGHT
                   };
        }

        #endregion

        #region ColorButton

        private static GUIStyle ColorButton(bool selected)
        {
            string buttonName = COLOR + BUTTON + (selected ? SELECTED : "");
            float buttonSize = 18 + (selected ? 6 : 0);
            return new GUIStyle
                   {
                       name = buttonName,
                       normal = {background = GetTexture(buttonName + StyleState.Normal)},
                       hover = {background = GetTexture(buttonName + StyleState.Hover)},
                       active = {background = GetTexture(buttonName + StyleState.Active)},
                       focused = {background = GetTexture(buttonName + StyleState.Hover)},
                       onNormal = {background = GetTexture(buttonName + StyleState.Normal)},
                       onHover = {background = GetTexture(buttonName + StyleState.Hover)},
                       onActive = {background = GetTexture(buttonName + StyleState.Active)},
                       onFocused = {background = GetTexture(buttonName + StyleState.Hover)},
                       imagePosition = ImagePosition.ImageOnly,
                       fixedHeight = buttonSize,
                       fixedWidth = buttonSize
                   };
        }

        private static List<GUIStyle> ColorButtons()
        {
            return new List<GUIStyle>
                   {
                       ColorButton(false),
                       ColorButton(true)
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
                             ComponentHeader("ActivateLoadedScenesNode", skin),
                             ComponentHeader("ApplicationQuitNode", skin),
                             ComponentHeader("BackButton", skin),
                             ComponentHeader("BackButtonNode", skin),
                             ComponentHeader("ColorTarget", skin),
                             ComponentHeader("ColorTargetImage", skin),
                             ComponentHeader("ColorTargetParticleSystem", skin),
                             ComponentHeader("ColorTargetRawImage", skin),
                             ComponentHeader("ColorTargetSelectable", skin),
                             ComponentHeader("ColorTargetSpriteRenderer", skin),
                             ComponentHeader("ColorTargetText", skin),
                             ComponentHeader("ColorTargetTextMeshPro", skin),
                             ComponentHeader("ColorTargetUnityEvent", skin),
                             ComponentHeader("DebugNode", skin),
                             ComponentHeader("EditorColors", skin),
                             ComponentHeader("FontTargetText", skin),
                             ComponentHeader("FontTargetTextMeshPro", skin),
                             ComponentHeader("GameEventListener", skin),
                             ComponentHeader("GameEventManager", skin),
                             ComponentHeader("GameEventNode", skin),
                             ComponentHeader("GestureListener", skin),
                             ComponentHeader("KeyToAction", skin),
                             ComponentHeader("KeyToGameEvent", skin),
                             ComponentHeader("LoadSceneNode", skin),
                             ComponentHeader("NodeTemplate", skin),
                             ComponentHeader("OrientationDetector", skin),
                             ComponentHeader("PlaymakerEventDispatcher", skin),
                             ComponentHeader("PortalNode", skin),
                             ComponentHeader("ProgressTarget", skin),
                             ComponentHeader("ProgressTargetAction", skin),
                             ComponentHeader("ProgressTargetAnimator", skin),
                             ComponentHeader("ProgressTargetAudioMixer", skin),
                             ComponentHeader("ProgressTargetImage", skin),
                             ComponentHeader("ProgressTargetText", skin),
                             ComponentHeader("ProgressTargetTextMeshPro", skin),
                             ComponentHeader("Progressor", skin),
                             ComponentHeader("ProgressorGroup", skin),
                             ComponentHeader("RadialLayout", skin),
                             ComponentHeader("RandomNode", skin),
                             ComponentHeader("SceneDirector", skin),
                             ComponentHeader("SceneLoader", skin),
                             ComponentHeader("SoundGroupData", skin),
                             ComponentHeader("SoundNode", skin),
                             ComponentHeader("SoundyController", skin),
                             ComponentHeader("SoundyDatabase", skin),
                             ComponentHeader("SoundyManager", skin),
                             ComponentHeader("SoundyPooler", skin),
                             ComponentHeader("SpriteTargetImage", skin),
                             ComponentHeader("SpriteTargetSelectable", skin),
                             ComponentHeader("SpriteTargetSpriteRenderer", skin),
                             ComponentHeader("SpriteTargetUnityEvent", skin),
                             ComponentHeader("StylesBuilder", skin),
                             ComponentHeader("StylesUtility", skin),
                             ComponentHeader("TextureTargetRawImage", skin),
                             ComponentHeader("TextureTargetUnityEvent", skin),
                             ComponentHeader("ThemeManager", skin),
                             ComponentHeader("ThemeNode", skin),
                             ComponentHeader("TimeScaleNode", skin),
                             ComponentHeader("TouchDetector", skin),
                             ComponentHeader("UIAnimationData", skin),
                             ComponentHeader("UIAnimationDatabase", skin),
                             ComponentHeader("UIAnimations", skin),
                             ComponentHeader("UIButton", skin),
                             ComponentHeader("UIButtonListener", skin),
                             ComponentHeader("UICanvas", skin),
                             ComponentHeader("UIDrawer", skin),
                             ComponentHeader("UIDrawerListener", skin),
                             ComponentHeader("UIDrawerNode", skin),
                             ComponentHeader("UIImage", skin),
                             ComponentHeader("UINode", skin),
                             ComponentHeader("UIPopup", skin),
                             ComponentHeader("UIPopupManager", skin),
                             ComponentHeader("UIToggle", skin),
                             ComponentHeader("UIView", skin),
                             ComponentHeader("UIViewListener", skin),
                             ComponentHeader("UnloadSceneNode", skin),
                             ComponentHeader("WaitNode", skin)
                         };

            return styles;
        }

        #endregion

        #region Dividers

        private static List<GUIStyle> Dividers(Skin skin)
        {
            var styles = new List<GUIStyle>();
            foreach (DividerType dividerType in Enum.GetValues(typeof(DividerType)))
                styles.Add(Divider(dividerType, skin));
            return styles;
        }

        private static GUIStyle Divider(DividerType dividerType, Skin skin)
        {
            string styleName = DIVIDER + dividerType;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName + skin)},
                       border = new RectOffset(1, 1, 0, 0),
                       imagePosition = ImagePosition.ImageOnly,
                       fixedHeight = DGUI.Sizes.DIVIDER_HEIGHT,
                       stretchWidth = true
                   };
        }

        #endregion

        #region DoozyWindow

        private static GUIStyle DoozyWindowIconBackground(Skin skin)
        {
            string styleName = WINDOW + ICON + BACKGROUND;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName + skin)},
                       imagePosition = ImagePosition.ImageOnly,
                       fixedWidth = 78,
                       fixedHeight = 60
                   };
        }

        private static List<GUIStyle> DoozyWindowToolbarButtons(Skin skin)
        {
            var styles = new List<GUIStyle>();
            styles.AddRange(DoozyWindowToolbarButton("General", skin));

            styles.AddRange(DoozyWindowToolbarButton("Canvases", skin));
            styles.AddRange(DoozyWindowToolbarButton("Views", skin));
            styles.AddRange(DoozyWindowToolbarButton("Buttons", skin));
            styles.AddRange(DoozyWindowToolbarButton("Drawers", skin));
            styles.AddRange(DoozyWindowToolbarButton("Popups", skin));
            styles.AddRange(DoozyWindowToolbarButton("GameEvents", skin));

            styles.AddRange(DoozyWindowToolbarButton("Soundy", skin));
            styles.AddRange(DoozyWindowToolbarButton("Touchy", skin));
            styles.AddRange(DoozyWindowToolbarButton("Nody", skin));

            styles.AddRange(DoozyWindowToolbarButton("Animations", skin));
            styles.AddRange(DoozyWindowToolbarButton("Templates", skin));
            styles.AddRange(DoozyWindowToolbarButton("Themes", skin));

            styles.AddRange(DoozyWindowToolbarButton("Settings", skin));
            styles.AddRange(DoozyWindowToolbarButton("Debug", skin));
            styles.AddRange(DoozyWindowToolbarButton("Keys", skin));

            styles.AddRange(DoozyWindowToolbarButton("Help", skin));
            styles.AddRange(DoozyWindowToolbarButton("About", skin));

            styles.AddRange(DoozyWindowToolbarSecondaryButton("", skin)); //Doozy Window Toolbar Secondary Button
            return styles;
        }

        private static List<GUIStyle> DoozyWindowToolbarButton(string buttonName, Skin skin)
        {
            const TextAnchor alignment = TextAnchor.MiddleLeft;
            var border = new RectOffset(30, 1, 2, 2);
            var padding = new RectOffset(34, 4, 2, 2);
            const int fontSize = 14;
            Font font = DGUI.Fonts.SansationRegular;

            string styleName = DOOZY + WINDOW + TOOLBAR + BUTTON + buttonName;
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

        private static List<GUIStyle> DoozyWindowToolbarSecondaryButton(string buttonName, Skin skin)
        {
            const TextAnchor alignment = TextAnchor.MiddleLeft;
            var border = new RectOffset(2, 2, 2, 2);
            var padding = new RectOffset(2, 4, 2, 2);
            const int fontSize = 12;
            Font font = DGUI.Fonts.SansationRegular;

            string styleName = DOOZY + WINDOW + TOOLBAR + SECONDARY + BUTTON + buttonName;
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

        private static List<GUIStyle> DoozyWindowViewHeader(Skin skin)
        {
            var styles = new List<GUIStyle>
                         {
                             DoozyWindowViewHeader("About", skin),
                             DoozyWindowViewHeader("Animations", skin),
                             DoozyWindowViewHeader("Buttons", skin),
                             DoozyWindowViewHeader("Canvases", skin),
                             DoozyWindowViewHeader("Debug", skin),
                             DoozyWindowViewHeader("Drawers", skin),
                             DoozyWindowViewHeader("GameEvents", skin),
                             DoozyWindowViewHeader("General", skin),
                             DoozyWindowViewHeader("Help", skin),
                             DoozyWindowViewHeader("Keys", skin),
                             DoozyWindowViewHeader("Nody", skin),
                             DoozyWindowViewHeader("Popups", skin),
                             DoozyWindowViewHeader("Settings", skin),
                             DoozyWindowViewHeader("Soundy", skin),
                             DoozyWindowViewHeader("Templates", skin),
                             DoozyWindowViewHeader("Themes", skin),
                             DoozyWindowViewHeader("Touchy", skin),
                             DoozyWindowViewHeader("Views", skin)
                         };
            return styles;
        }

        private static GUIStyle DoozyWindowViewHeader(string viewName, Skin skin)
        {
            string styleName = DOOZY + WINDOW + VIEW + HEADER + viewName;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName + skin)},
                       border = new RectOffset(382, 1, 0, 0),
                       fixedHeight = 96
                   };
        }

        #endregion

        #region IconButtons

        private static GUIStyle IconButton(string iconName)
        {
            return new GUIStyle
                   {
                       name = ICON + BUTTON + iconName,
                       normal = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Normal)},
                       hover = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Hover)},
                       active = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Active)},
                       focused = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Hover)},
                       onNormal = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Normal)},
                       onHover = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Hover)},
                       onActive = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Active)},
                       onFocused = {background = GetTexture(ICON + BUTTON + iconName + StyleState.Hover)},
                       margin = new RectOffset(4, 4, 2, 2),
                       imagePosition = ImagePosition.ImageOnly,
                       alignment = TextAnchor.MiddleCenter,
                       fixedWidth = DGUI.Sizes.ICON_BUTTON_SIZE,
                       fixedHeight = DGUI.Sizes.ICON_BUTTON_SIZE
                   };
        }

        private static List<GUIStyle> IconButtons()
        {
            return new List<GUIStyle>
                   {
                       IconButton("Cancel"),
                       IconButton("Close"),
                       IconButton("Link"),
                       IconButton("Load"),
                       IconButton("Minus"),
                       IconButton("More"),
                       IconButton("MuteSound"),
                       IconButton("New"),
                       IconButton("Ok"),
                       IconButton("PlaySound"),
                       IconButton("Plus"),
                       IconButton("Recent"),
                       IconButton("Reset"),
                       IconButton("Save"),
                       IconButton("SaveAs"),
                       IconButton("Settings"),
                       IconButton("Sort"),
                       IconButton("Stop"),
                       IconButton("Unlink")
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

        private static GUIStyle Icon(string iconName, Skin skin)
        {
            return new GUIStyle
                   {
                       name = ICON + iconName,
                       normal =
                       {
                           background = GetTexture(ICON + iconName + skin)
                       },
                       imagePosition = ImagePosition.ImageOnly
                   };
        }

        private static List<GUIStyle> Icons(Skin skin)
        {
            var styles = new List<GUIStyle>
                         {
                             Icon("About"),
                             Icon("Action"),
                             Icon("ActionEnd"),
                             Icon("ActionStart"),
                             Icon("Animations"),
                             Icon("Animator"),
                             Icon("ApplicationQuit"),
                             Icon("AudioMixer"),
                             Icon("AudioMixerGroup"),
                             Icon("BackButton"),
                             Icon("Behavior"),
                             Icon("BehaviorSettings"),
                             Icon("Border"),
                             Icon("ButtonClick"),
                             Icon("ButtonDoubleClick"),
                             Icon("ButtonLongClick"),
                             Icon("ButtonRightClick"),
                             Icon("ChangeVolume"),
                             Icon("Close"),
                             Icon("ColorTarget"),
                             Icon("ColorTargetImage"),
                             Icon("ColorTargetParticleSystem"),
                             Icon("ColorTargetRawImage"),
                             Icon("ColorTargetSelectable"),
                             Icon("ColorTargetSpriteRenderer"),
                             Icon("ColorTargetText"),
                             Icon("ColorTargetTextMeshPro"),
                             Icon("ColorTargetUnityEvent"),
                             Icon("CloseFilledCircle"),
                             Icon("Debug"),
                             Icon("DisableBackButton"),
                             Icon("DisableButton"),
                             Icon("DisableTouch"),
                             Icon("Doozy"),
                             Icon("DoozyUI"),
                             Icon("DoozyUILogoCompact", skin),
                             Icon("EditorSettings"),
                             Icon("EndProgress"),
                             Icon("Error"),
                             Icon("EventDispatcher"),
                             Icon("FaAdobe"),
                             Icon("FaAlarmClock"),
                             Icon("FaAnalytics"),
                             Icon("FaAnchor"),
                             Icon("FaAndroid"),
                             Icon("FaAngleDoubleDown"),
                             Icon("FaAngleDoubleLeft"),
                             Icon("FaAngleDoubleRight"),
                             Icon("FaAngleDoubleUp"),
                             Icon("FaAngleDown"),
                             Icon("FaAngleLeft"),
                             Icon("FaAngleRight"),
                             Icon("FaAngleUp"),
                             Icon("FaApple"),
                             Icon("FaArchive"),
                             Icon("FaArrowAltDown"),
                             Icon("FaArrowAltFromBottom"),
                             Icon("FaArrowAltFromLeft"),
                             Icon("FaArrowAltFromRight"),
                             Icon("FaArrowAltFromTop"),
                             Icon("FaArrowAltLeft"),
                             Icon("FaArrowAltRight"),
                             Icon("FaArrowAltToBottom"),
                             Icon("FaArrowAltToLeft"),
                             Icon("FaArrowAltToRight"),
                             Icon("FaArrowAltToTop"),
                             Icon("FaArrowAltUp"),
                             Icon("FaArrowDown"),
                             Icon("FaArrowFromBottom"),
                             Icon("FaArrowFromLeft"),
                             Icon("FaArrowFromRight"),
                             Icon("FaArrowFromTop"),
                             Icon("FaArrowLeft"),
                             Icon("FaArrowRight"),
                             Icon("FaArrows"),
                             Icon("FaArrowsAlt"),
                             Icon("FaArrowsAltH"),
                             Icon("FaArrowsAltV"),
                             Icon("FaArrowsH"),
                             Icon("FaArrowsV"),
                             Icon("FaArrowToBottom"),
                             Icon("FaArrowToLeft"),
                             Icon("FaArrowToRight"),
                             Icon("FaArrowToTop"),
                             Icon("FaArrowUp"),
                             Icon("FaAsterisk"),
                             Icon("FaAt"),
                             Icon("FaAtlassian"),
                             Icon("FaAtom"),
                             Icon("FaAtomAlt"),
                             Icon("FaBackpack"),
                             Icon("FaBackspace"),
                             Icon("FaBackward"),
                             Icon("FaBalanceScale"),
                             Icon("FaBallot"),
                             Icon("FaBan"),
                             Icon("FaBarcode"),
                             Icon("FaBars"),
                             Icon("FaBell"),
                             Icon("FaBellSlash"),
                             Icon("FaBinoculars"),
                             Icon("FaBitbucket"),
                             Icon("FaBluetooth"),
                             Icon("FaBolt"),
                             Icon("FaBook"),
                             Icon("FaBookmark"),
                             Icon("FaBookOpen"),
                             Icon("FaBox"),
                             Icon("FaBoxOpen"),
                             Icon("FaBrackets"),
                             Icon("FaBracketsCurly"),
                             Icon("FaBrain"),
                             Icon("FaBroadcastTower"),
                             Icon("FaBroom"),
                             Icon("FaBug"),
                             Icon("FaBullhorn"),
                             Icon("FaBullseye"),
                             Icon("FaCalculator"),
                             Icon("FaCalculatorAlt"),
                             Icon("FaCalendar"),
                             Icon("FaCalendarAlt"),
                             Icon("FaCalendarDay"),
                             Icon("FaCalendarWeek"),
                             Icon("FaCaretDown"),
                             Icon("FaCaretLeft"),
                             Icon("FaCaretRight"),
                             Icon("FaCaretUp"),
                             Icon("FaChartNetwork"),
                             Icon("FaCheck"),
                             Icon("FaCheckCircle"),
                             Icon("FaCheckSquare"),
                             Icon("FaChevronDown"),
                             Icon("FaChevronLeft"),
                             Icon("FaChevronRight"),
                             Icon("FaChevronUp"),
                             Icon("FaCircle"),
                             Icon("FaCircleNotch"),
                             Icon("FaClock"),
                             Icon("FaClone"),
                             Icon("FaCode"),
                             Icon("FaCodeBranch"),
                             Icon("FaCodeCommit"),
                             Icon("FaCodeMerge"),
                             Icon("FaCog"),
                             Icon("FaCompress"),
                             Icon("FaCompressAlt"),
                             Icon("FaCompressArrowsAlt"),
                             Icon("FaCopy"),
                             Icon("FaCopyright"),
                             Icon("FaCrosshairs"),
                             Icon("FaCut"),
                             Icon("FaDatabase"),
                             Icon("Fade"),
                             Icon("FaDesktop"),
                             Icon("FaDiscord"),
                             Icon("FaDoNotEnter"),
                             Icon("FaDoorClosed"),
                             Icon("FaDoorOpen"),
                             Icon("FaEar"),
                             Icon("FaEdit"),
                             Icon("FaEllipsisH"),
                             Icon("FaEllipsisV"),
                             Icon("FaEnvelope"),
                             Icon("FaEraser"),
                             Icon("FaExchange"),
                             Icon("FaExchangeAlt"),
                             Icon("FaExclamation"),
                             Icon("FaExclamationCircle"),
                             Icon("FaExclamationTriangle"),
                             Icon("FaExpand"),
                             Icon("FaExpandAlt"),
                             Icon("FaExpandArrows"),
                             Icon("FaExpandArrowsAlt"),
                             Icon("FaExternalLink"),
                             Icon("FaEye"),
                             Icon("FaEyeDropper"),
                             Icon("FaEyeSlash"),
                             Icon("FaFacebook"),
                             Icon("FaFacebookF"),
                             Icon("FaFacebookSquare"),
                             Icon("FaFastBackward"),
                             Icon("FaFastForward"),
                             Icon("FaFile"),
                             Icon("FaFileAudio"),
                             Icon("FaFileCode"),
                             Icon("FaFileEdit"),
                             Icon("FaFileMinus"),
                             Icon("FaFilePdf"),
                             Icon("FaFilePlus"),
                             Icon("FaFileTimes"),
                             Icon("FaFilm"),
                             Icon("FaFilmAlt"),
                             Icon("FaFilter"),
                             Icon("FaFingerprint"),
                             Icon("FaFolder"),
                             Icon("FaFolderMinus"),
                             Icon("FaFolderOpen"),
                             Icon("FaFolderPlus"),
                             Icon("FaFolders"),
                             Icon("FaFolderTimes"),
                             Icon("FaForward"),
                             Icon("FaGamepad"),
                             Icon("FaGlasses"),
                             Icon("FaGraduationCap"),
                             Icon("FaGripHorizontal"),
                             Icon("FaGripLines"),
                             Icon("FaGripLinesVertical"),
                             Icon("FaGripVertical"),
                             Icon("FaHashtag"),
                             Icon("FaHeadphones"),
                             Icon("FaHeart"),
                             Icon("FaHistory"),
                             Icon("FaHome"),
                             Icon("FaHourglass"),
                             Icon("FaHourglassEnd"),
                             Icon("FaHourglassHalf"),
                             Icon("FaHourglassStart"),
                             Icon("FaImage"),
                             Icon("FaImages"),
                             Icon("FaInfinity"),
                             Icon("FaInfo"),
                             Icon("FaInfoCircle"),
                             Icon("FaKey"),
                             Icon("FaKeyboard"),
                             Icon("FaLanguage"),
                             Icon("FaLaptop"),
                             Icon("FaLaptopCode"),
                             Icon("FaLevelDown"),
                             Icon("FaLevelDownAlt"),
                             Icon("FaLevelUp"),
                             Icon("FaLevelUpAlt"),
                             Icon("FaLink"),
                             Icon("FaList"),
                             Icon("FaListUl"),
                             Icon("FaLock"),
                             Icon("FaLockOpen"),
                             Icon("FaMagic"),
                             Icon("FaMagnet"),
                             Icon("FaMicrophone"),
                             Icon("FaMicrophoneSlash"),
                             Icon("FaMicrosoft"),
                             Icon("FaMinus"),
                             Icon("FaMobileAlt"),
                             Icon("FaMousePointer"),
                             Icon("FaMusic"),
                             Icon("FaNewspaper"),
                             Icon("FaObjectGroup"),
                             Icon("FaObjectUngroup"),
                             Icon("FaPaintBrush"),
                             Icon("FaPaintRoller"),
                             Icon("FaPalette"),
                             Icon("FaPaperclip"),
                             Icon("FaPaperPlane"),
                             Icon("FaPaste"),
                             Icon("FaPause"),
                             Icon("FaPen"),
                             Icon("FaPencil"),
                             Icon("FaPercentage"),
                             Icon("FaPlay"),
                             Icon("FaPlug"),
                             Icon("FaPlus"),
                             Icon("FaPodcast"),
                             Icon("FaProjectDiagram"),
                             Icon("FaPuzzlePiece"),
                             Icon("FaQuestion"),
                             Icon("FaQuestionCircle"),
                             Icon("FaRandom"),
                             Icon("FaRecycle"),
                             Icon("FaRedo"),
                             Icon("FaRepeat"),
                             Icon("FaRepeatAlt"),
                             Icon("FaReply"),
                             Icon("FaSatelliteDish"),
                             Icon("FaSearch"),
                             Icon("FaSearchMinus"),
                             Icon("FaSearchPlus"),
                             Icon("FaServer"),
                             Icon("FaShare"),
                             Icon("FaShareAlt"),
                             Icon("FaShield"),
                             Icon("FaShieldAlt"),
                             Icon("FaShieldCheck"),
                             Icon("FaSignature"),
                             Icon("FaSignIn"),
                             Icon("FaSignInAlt"),
                             Icon("FaSignOut"),
                             Icon("FaSignOutAlt"),
                             Icon("FaSitemap"),
                             Icon("FaSlidersH"),
                             Icon("FaSlidersHSquare"),
                             Icon("FaSlidersV"),
                             Icon("FaSlidersVSquare"),
                             Icon("FaSort"),
                             Icon("FaSortAlphaDown"),
                             Icon("FaSortAlphaUp"),
                             Icon("FaSortAmountDown"),
                             Icon("FaSortAmountUp"),
                             Icon("FaSortDown"),
                             Icon("FaSortNumericDown"),
                             Icon("FaSortNumericUp"),
                             Icon("FaSortUp"),
                             Icon("FaSquare"),
                             Icon("FaStar"),
                             Icon("FaStepBackward"),
                             Icon("FaStepForward"),
                             Icon("FaStickyNote"),
                             Icon("FaStop"),
                             Icon("FaStopwatch"),
                             Icon("FaStore"),
                             Icon("FaSync"),
                             Icon("FaSyncAlt"),
                             Icon("FaTabletAlt"),
                             Icon("FaTag"),
                             Icon("FaTags"),
                             Icon("FaTerminal"),
                             Icon("FaTh"),
                             Icon("FaThLarge"),
                             Icon("FaThList"),
                             Icon("FaThumbtack"),
                             Icon("FaTimes"),
                             Icon("FaTimesCircle"),
                             Icon("FaTools"),
                             Icon("FaTrash"),
                             Icon("FaTrello"),
                             Icon("FaTv"),
                             Icon("FaTwitter"),
                             Icon("FaTwitterSquare"),
                             Icon("FaUmbrella"),
                             Icon("FaUndo"),
                             Icon("FaUndoAlt"),
                             Icon("FaUnlink"),
                             Icon("FaUnlock"),
                             Icon("FaUser"),
                             Icon("FaUserAlt"),
                             Icon("FaUserCircle"),
                             Icon("FaUserEdit"),
                             Icon("FaUserMinus"),
                             Icon("FaUserPlus"),
                             Icon("FaUserSecret"),
                             Icon("FaVideo"),
                             Icon("FaVolume"),
                             Icon("FaVolumeDown"),
                             Icon("FaVolumeMute"),
                             Icon("FaVolumeOff"),
                             Icon("FaVolumeSlash"),
                             Icon("FaVolumeUp"),
                             Icon("FaVrCardboard"),
                             Icon("FaWifi"),
                             Icon("FaWifiSlash"),
                             Icon("FaWindows"),
                             Icon("FaYoutube"),
                             Icon("FaYoutubeSquare"),
                             Icon("Feather"),
                             Icon("FinishAnimation"),
                             Icon("FixedUpdate"),
                             Icon("Font"),
                             Icon("FontAsset"),
                             Icon("FontTargetText"),
                             Icon("FontTargetTextMeshPro"),
                             Icon("GameEvent"),
                             Icon("GameEventListener"),
                             Icon("GameEventManager"),
                             Icon("GestureListener"),
                             Icon("Help"),
                             Icon("Hide"),
                             Icon("HideAnimation"),
                             Icon("Info"),
                             Icon("KeyMapper"),
                             Icon("KeyToAction"),
                             Icon("KeyToGameEvent"),
                             Icon("Label"),
                             Icon("Landscape"),
                             Icon("LateUpdate"),
                             Icon("Load"),
                             Icon("Loop"),
                             Icon("LoopAnimation"),
                             Icon("MasterAudio"),
                             Icon("MidProgress"),
                             Icon("Minus"),
                             Icon("More"),
                             Icon("Move"),
                             Icon("Music"),
                             Icon("MuteSound"),
                             Icon("New"),
                             Icon("Nody"),
                             Icon("NotificationManager"),
                             Icon("Ok"),
                             Icon("OnButtonDeselect"),
                             Icon("OnButtonSelect"),
                             Icon("OnPointerDown"),
                             Icon("OnPointerEnter"),
                             Icon("OnPointerExit"),
                             Icon("OnPointerUp"),
                             Icon("OrientationDetector"),
                             Icon("OrientationManager"),
                             Icon("OutAnimation"),
                             Icon("Playmaker"),
                             Icon("PlaymakerEventDispatcher"),
                             Icon("PlaySound"),
                             Icon("Plus"),
                             Icon("Portrait"),
                             Icon("Progressor"),
                             Icon("ProgressorGroup"),
                             Icon("ProgressTarget"),
                             Icon("ProgressTargetAction"),
                             Icon("ProgressTargetAnimator"),
                             Icon("ProgressTargetImage"),
                             Icon("ProgressTargetText"),
                             Icon("ProgressTargetTextMeshPro"),
                             Icon("PunchAnimation"),
                             Icon("RadialLayout"),
                             Icon("RawImage"),
                             Icon("Recent"),
                             Icon("Reset"),
                             Icon("Rotate"),
                             Icon("RoundedCornerBottomLeft"),
                             Icon("RoundedCornerBottomRight"),
                             Icon("RoundedCornerTopLeft"),
                             Icon("RoundedCornerTopRight"),
                             Icon("Save"),
                             Icon("SaveAs"),
                             Icon("Scale"),
                             Icon("SceneDirector"),
                             Icon("SceneLoader"),
                             Icon("Search"),
                             Icon("Selectable"),
                             Icon("Settings"),
                             Icon("Show"),
                             Icon("ShowAnimation"),
                             Icon("Sound"),
                             Icon("SoundGroupData"),
                             Icon("SoundGroupDatabase"),
                             Icon("Soundy"),
                             Icon("SoundyController"),
                             Icon("SoundyDatabase"),
                             Icon("SoundyPooler"),
                             Icon("Sprite"),
                             Icon("SpriteRenderer"),
                             Icon("SpriteTargetImage"),
                             Icon("SpriteTargetSelectable"),
                             Icon("SpriteTargetSpriteRenderer"),
                             Icon("SpriteTargetUnityEvent"),
                             Icon("StartAnimation"),
                             Icon("StartNode"),
                             Icon("StartProgress"),
                             Icon("StateAnimation"),
                             Icon("StopSound"),
                             Icon("TextMeshPro"),
                             Icon("Texture"),
                             Icon("TextureTargetRawImage"),
                             Icon("TextureTargetUnityEvent"),
                             Icon("ThemeManager"),
                             Icon("Time"),
                             Icon("TimeScale"),
                             Icon("TogglePause"),
                             Icon("TouchManager"),
                             Icon("Touchy"),
                             Icon("UIAnimation"),
                             Icon("UIAnimationData"),
                             Icon("UIAnimationDatabase"),
                             Icon("UIAnimations"),
                             Icon("UIAnimationsDatabase"),
                             Icon("UIButton"),
                             Icon("UIButtonListener"),
                             Icon("UICanvas"),
                             Icon("UIDrawer"),
                             Icon("UIDrawerListener"),
                             Icon("UIEffect"),
                             Icon("UIImage"),
                             Icon("UIListView"),
                             Icon("UIManager"),
                             Icon("UINode"),
                             Icon("UINotification"),
                             Icon("UIPopup"),
                             Icon("UIPopupManager"),
                             Icon("UITemplate"),
                             Icon("UIToggle"),
                             Icon("UITrigger"),
                             Icon("UIView"),
                             Icon("UIViewListener"),
                             Icon("UnityEvent"),
                             Icon("Update"),
                             Icon("Wait"),
                             Icon("Warning")
                         };

            return styles;
        }

        #endregion

        #region InfoMessage

        private static List<GUIStyle> InfoMessage()
        {
            return new List<GUIStyle>
                   {
                       new GUIStyle
                       {
                           name = INFO_MESSAGE + HEADER,
                           normal = {background = GetTexture(INFO_MESSAGE + HEADER)},
                           border = new RectOffset(8, 8, 8, 8)
                       },

                       new GUIStyle
                       {
                           name = INFO_MESSAGE + MESSAGE,
                           normal = {background = GetTexture(INFO_MESSAGE + MESSAGE)},
                           border = new RectOffset(8, 8, 8, 8)
                       },

                       new GUIStyle
                       {
                           name = INFO_MESSAGE + ROUNDED,
                           normal = {background = GetTexture(INFO_MESSAGE + ROUNDED)},
                           border = new RectOffset(8, 8, 8, 8)
                       }
                   };
        }

        #endregion

        #region NodeIcons

        private static List<GUIStyle> NodeIcons()
        {
            return new List<GUIStyle>
                   {
                       NodeIcon("ActivateLoadedScenesNode"),
                       NodeIcon("BackButtonNode"),
                       NodeIcon("DebugNode"),
                       NodeIcon("GameEventNode"),
                       NodeIcon("LoadSceneNode"),
                       NodeIcon("PortalNode"),
                       NodeIcon("RandomNode"),
                       NodeIcon("SoundNode"),
                       NodeIcon("ThemeNode"),
                       NodeIcon("TimeScaleNode"),
                       NodeIcon("UIDrawerNode"),
                       NodeIcon("UINode"),
                       NodeIcon("UIPopupNode"),
                       NodeIcon("UnloadSceneNode"),
                       NodeIcon("WaitNode")
                   };
        }

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

        #endregion

        #region NodyLabels

        private static List<GUIStyle> Labels(Skin skin)
        {
            var styles = new List<GUIStyle>();
            foreach (Size size in Enum.GetValues(typeof(Size)))
            foreach (TextAlign textAlign in Enum.GetValues(typeof(TextAlign)))
                styles.Add(Label(size, textAlign, skin));

            return styles;
        }

        private static GUIStyle Label(Size size, TextAlign textAlign, Skin skin)
        {
            var style = new GUIStyle
                        {
                            name = LABEL + size + textAlign,

                            normal = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},
                            onNormal = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},

                            hover = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                            onHover = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},

                            active = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Active, skin)},
                            onActive = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Active, skin)},

                            focused = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                            onFocused = {textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},

                            fontSize = DGUI.Sizes.LabelFontSize(size),
                            clipping = TextClipping.Clip,
                            stretchWidth = true
                        };

            switch (textAlign)
            {
                case TextAlign.Left:
                    style.alignment = TextAnchor.MiddleLeft;
                    break;
                case TextAlign.Center:
                    style.alignment = TextAnchor.MiddleCenter;
                    break;
                case TextAlign.Right:
                    style.alignment = TextAnchor.MiddleRight;
                    break;
                default: throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
            }

            return style;
        }

        #endregion

        #region Tabs

        private static List<GUIStyle> Tabs(Skin skin)
        {
            var list = new List<GUIStyle>();
            list.AddRange(Tab(ShortDirection.ToTop, skin));
            return list;
        }

        private static List<GUIStyle> Tab(ShortDirection direction, Skin skin)
        {
            const int fontSize = 12;
            const TextAnchor alignment = TextAnchor.UpperLeft;
            const TextClipping clipping = TextClipping.Clip;

            string styleName = TAB + direction;
            string styleNameSelected = styleName + SELECTED;
            return new List<GUIStyle>
                   {
                       new GUIStyle
                       {
                           name = styleName,
                           normal = {background = GetTexture(styleName + StyleState.Normal + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},
                           onNormal = {background = GetTexture(styleName + StyleState.Normal + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},
                           hover = {background = GetTexture(styleName + StyleState.Hover + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                           onHover = {background = GetTexture(styleName + StyleState.Hover + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                           active = {background = GetTexture(styleName + StyleState.Active + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Active, skin)},
                           onActive = {background = GetTexture(styleName + StyleState.Active + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Active, skin)},
                           focused = {background = GetTexture(styleName + StyleState.Hover + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                           onFocused = {background = GetTexture(styleName + StyleState.Hover + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Hover, skin)},
                           fontSize = fontSize,
                           alignment = alignment,
                           clipping = clipping,
                           border = new RectOffset(6, 6, 12, 6),
                           padding = new RectOffset(8, 8, 7, 0)
                       },

                       new GUIStyle
                       {
                           name = styleNameSelected,
                           normal = {background = GetTexture(styleNameSelected + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},
                           onNormal = {background = GetTexture(styleNameSelected + skin), textColor = DGUI.Colors.ButtonBaseColor(StyleState.Normal, skin)},
                           fontSize = fontSize,
                           alignment = alignment,
                           clipping = clipping,
                           border = new RectOffset(6, 6, 12, 6),
                           padding = new RectOffset(8, 8, 5, 0)
                       }
                   };
        }

        #endregion

        private static GUIStyle RadioButton(ToggleState toggleState, Skin skin)
        {
            string styleName = RADIO + BUTTON + toggleState;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                       hover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       active = {background = GetTexture(styleName + StyleState.Active + skin)},
                       focused = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       onNormal = {background = GetTexture(styleName + StyleState.Normal + skin)},
                       onHover = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       onActive = {background = GetTexture(styleName + StyleState.Active + skin)},
                       onFocused = {background = GetTexture(styleName + StyleState.Hover + skin)},
                       imagePosition = ImagePosition.ImageOnly,
                       fixedWidth = 14,
                       fixedHeight = 14
                   };
        }

        private static List<GUIStyle> Switches(Skin skin)
        {
            var styles = new List<GUIStyle>();
            foreach (ToggleState state in Enum.GetValues(typeof(ToggleState)))
                styles.Add(Switch(state, skin));
            return styles;
        }

        private static GUIStyle Switch(ToggleState toggleState, Skin skin)
        {
            return new GUIStyle
                   {
                       name = SWITCH + toggleState,
                       normal = {background = GetTexture(SWITCH + toggleState + StyleState.Normal + skin)},
                       onNormal = {background = GetTexture(SWITCH + toggleState + StyleState.Normal + skin)},
                       hover = {background = GetTexture(SWITCH + toggleState + StyleState.Hover + skin)},
                       onHover = {background = GetTexture(SWITCH + toggleState + StyleState.Hover + skin)},
                       active = {background = GetTexture(SWITCH + toggleState + StyleState.Active + skin)},
                       onActive = {background = GetTexture(SWITCH + toggleState + StyleState.Active + skin)},
                       focused = {background = GetTexture(SWITCH + toggleState + StyleState.Hover + skin)},
                       onFocused = {background = GetTexture(SWITCH + toggleState + StyleState.Hover + skin)},
                       imagePosition = ImagePosition.ImageOnly,
                       fixedWidth = DGUI.Sizes.SWITCH_WIDTH,
                       fixedHeight = DGUI.Sizes.SWITCH_HEIGHT
                   };
        }

        private static GUIStyle TitleBackground(Skin skin)
        {
            string styleName = TITLE + BACKGROUND;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName + skin)},
                       border = new RectOffset(8, 8, 8, 8)
                   };
        }

        private static List<GUIStyle> WhiteGradients()
        {
            return new List<GUIStyle>
                   {
                       WhiteGradient(Direction.LeftToRight),
                       WhiteGradient(Direction.TopToBottom),
                       WhiteGradient(Direction.RightToLeft),
                       WhiteGradient(Direction.BottomToTop)
                   };
        }

        private static GUIStyle WhiteGradient(Direction direction)
        {
            return new GUIStyle
                   {
                       name = WHITE_GRADIENT + direction,
                       normal =
                       {
                           background = GetTexture(WHITE_GRADIENT + direction)
                       },
                       imagePosition = ImagePosition.ImageOnly,
                       stretchWidth = true,
                       stretchHeight = true
                   };
        }

        private static List<GUIStyle> WindowToolbarElements()
        {
            return new List<GUIStyle>
                   {
                       WindowToolbarElement(BACKGROUND, Direction.LeftToRight, new RectOffset(4, 4, 4, 4)),
                       WindowToolbarElement(BACKGROUND, Direction.TopToBottom, new RectOffset(4, 4, 4, 4)),
                       WindowToolbarElement(BACKGROUND, Direction.RightToLeft, new RectOffset(4, 4, 4, 4)),
                       WindowToolbarElement(BACKGROUND, Direction.BottomToTop, new RectOffset(4, 4, 4, 4)),
                       WindowToolbarElement(TAB, Direction.LeftToRight, new RectOffset(0, 0, 8, 8)),
                       WindowToolbarElement(TAB, Direction.TopToBottom, new RectOffset(8, 8, 0, 0)),
                       WindowToolbarElement(TAB, Direction.RightToLeft, new RectOffset(0, 0, 8, 8)),
                       WindowToolbarElement(TAB, Direction.BottomToTop, new RectOffset(8, 8, 0, 0))
                   };
        }

        private static GUIStyle WindowToolbarElement(string elementName, Direction direction, RectOffset border)
        {
            string styleName = WINDOW + TOOLBAR + elementName + direction;
            return new GUIStyle
                   {
                       name = styleName,
                       normal = {background = GetTexture(styleName)},
                       border = border,
                       imagePosition = ImagePosition.ImageOnly,
                       stretchWidth = true,
                       stretchHeight = true
                   };
        }
    }
}