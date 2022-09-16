// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Settings;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private static Texture[] s_nodyToolbarHeader;
        private static Texture[] NodyToolbarHeader { get { return s_nodyToolbarHeader ?? (s_nodyToolbarHeader = DGUI.AnimatedTexture.GetAnimatedTextureArray("NodyToolbarHeader", DoozyPath.EDITOR_NODY_IMAGES_PATH)); } }

        private void DrawToolbar()
        {
            GUILayout.BeginVertical(GUILayout.Height(position.height));
            {
                DGUI.AnimatedTexture.Draw(ToolbarAnimBool, NodyToolbarHeader, DoozyWindowSettings.Instance.ToolbarExpandedWidth, DoozyWindowSettings.Instance.ToolbarHeaderHeight); //Nody Toolbar Header

                DrawToolbarExpandCollapseButton();

                GUILayout.Space(DGUI.Properties.Space(4));

                DrawToolbarButton(UILabels.Graph,
                                  Styles.GetStyle(Styles.StyleName.WindowToolbarButtonNewGraph),
                                  Styles.GetStyle(Styles.StyleName.WindowToolbarButtonNewGraphSelected),
                                  false,
                                  DGUI.Colors.DisabledTextColorName,
                                  () => { CreateAndOpenGraphWidthDialog(false); });

                DrawToolbarButton(UILabels.SubGraph,
                                  Styles.GetStyle(Styles.StyleName.WindowToolbarButtonNewSubGraph),
                                  Styles.GetStyle(Styles.StyleName.WindowToolbarButtonNewSubGraphSelected),
                                  false,
                                  DGUI.Colors.DisabledTextColorName,
                                  () => { CreateAndOpenGraphWidthDialog(true); });

                GUILayout.Space(DGUI.Properties.Space(4));

                DrawToolbarButton(UILabels.Load,
                                  Styles.GetStyle(Styles.StyleName.WindowToolbarButtonLoad),
                                  Styles.GetStyle(Styles.StyleName.WindowToolbarButtonLoadSelected),
                                  false,
                                  DGUI.Colors.DisabledTextColorName,
                                  LoadGraphWidthDialog);

                if (CurrentGraph != null)
                {
                    GUILayout.Space(DGUI.Properties.Space(4));

                    DrawToolbarButton(UILabels.Save,
                                      Styles.GetStyle(Styles.StyleName.WindowToolbarButtonSave),
                                      Styles.GetStyle(Styles.StyleName.WindowToolbarButtonSaveSelected),
                                      false,
                                      DGUI.Colors.DisabledTextColorName,
                                      SaveGraph);

                    DrawToolbarButton(UILabels.SaveAs,
                                      Styles.GetStyle(Styles.StyleName.WindowToolbarButtonSaveAs),
                                      Styles.GetStyle(Styles.StyleName.WindowToolbarButtonSaveAsSelected),
                                      false,
                                      DGUI.Colors.DisabledTextColorName,
                                      SaveGraphAs);

                    GUILayout.Space(DGUI.Properties.Space(4));

                    DrawToolbarButton(UILabels.Close,
                                      Styles.GetStyle(Styles.StyleName.WindowToolbarButtonClose),
                                      Styles.GetStyle(Styles.StyleName.WindowToolbarButtonCloseSelected),
                                      false,
                                      DGUI.Colors.DisabledTextColorName,
                                      () => { CloseCurrentGraph(); });
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }

        private void DrawToolbarExpandCollapseButton(ColorName colorName = ColorName.White)
        {
            Color initialColor = GUI.color;
            GUI.color = DGUI.Colors.GetDColor(colorName).Normal.WithAlpha(GUI.color.a);
            GUILayout.BeginHorizontal(GUILayout.Width(ToolbarWidth));
            GUILayout.Space(6 * (1 - ToolbarAnimBool.faded));
            bool result = GUILayout.Button(GUIContent.none,
                                           Editor.Styles.GetStyle(ToolbarAnimBool.target
                                                                      ? Editor.Styles.StyleName.ArrowButtonToLeft
                                                                      : Editor.Styles.StyleName.ArrowButtonToRight),
                                           GUILayout.Height(DoozyWindowSettings.Instance.ToolbarExpandCollapseButtonHeight));
            GUILayout.Space(2 * ToolbarAnimBool.faded);
            GUILayout.EndHorizontal();
            GUI.color = initialColor;

            ToolbarAnimBool.target = DoozyWindowSettings.Instance.DynamicToolbarExpanded;
            if (!result) return;
            ToggleToolbarMenuExpandOrCollapse();
        }

        private void ToggleToolbarMenuExpandOrCollapse()
        {
            DoozyWindowSettings.Instance.DynamicToolbarExpanded = !ToolbarAnimBool.target;
            DoozyWindowSettings.Instance.SetDirty(false);
        }

        private static void DrawToolbarButton(string label, GUIStyle normalStyle, GUIStyle selectedStyle, bool selected, ColorName colorName, Action callback = null)
        {
            if (!DGUI.Button.DrawToolbarButton(label, normalStyle, selectedStyle, selected,
                                               selected ? colorName : DGUI.Utility.IsProSkin ? ColorName.UnityLight : ColorName.Gray,
                                               selected ? colorName : DGUI.Utility.IsProSkin ? ColorName.UnityLight : ColorName.UnityDark,
                                               GUILayout.Height(DoozyWindowSettings.Instance.ToolbarButtonHeight))) return;
            if (callback != null) callback.Invoke();
            Event.current.Use();
        }
    }
}