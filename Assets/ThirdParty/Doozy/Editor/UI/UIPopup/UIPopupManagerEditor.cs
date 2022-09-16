// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.UI;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIPopupManager))]
    public class UIPopupManagerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIPopupManagerColorName; } }
        private UIPopupManager m_target;

        private UIPopupManager Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIPopupManager) target;
                return m_target;
            }
        }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIPopupManager), MenuUtils.UIPopupManager_Manual, MenuUtils.UIPopupManager_YouTube);
            DrawUIPopupDatabaseButton();
//            DrawPopupTesterOptions();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUIPopupDatabaseButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconUIPopup),
                                                   UILabels.UIPopupDatabase,
                                                   Size.M, TextAlign.Left,
                                                   DGUI.Colors.DisabledBackgroundColorName,
                                                   DGUI.Colors.DisabledTextColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4),
                                                   false))
                DoozyWindow.Open(DoozyWindow.View.Popups);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #region Popup Tester Options

        private string m_targetPopupName = "miau";
        private bool m_addToQueue = true;
        private bool m_hideOnClickAnywhere;
        private bool m_hideOnClickOverlay;
        private bool m_hideOnClickContainer;
        private bool m_hideOnBackButton;
        private Sprite m_exampleSprite;
        
        private void DrawPopupTesterOptions()
        {
            GUILayout.Space(DGUI.Properties.Space(4));

            m_exampleSprite = (Sprite) EditorGUILayout.ObjectField(m_exampleSprite, typeof(Sprite), false);
            GUILayout.Space(DGUI.Properties.Space(4));

            GUILayout.BeginHorizontal();
            {
                m_targetPopupName = EditorGUILayout.TextField(m_targetPopupName);
                GUILayout.Space(DGUI.Properties.Space(2));
                m_addToQueue = DGUI.Toggle.Switch.Draw(m_addToQueue, "Add to Queue", ComponentColorName, false, true, false);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.BeginHorizontal();
            {
                m_hideOnClickAnywhere = DGUI.Toggle.Switch.Draw(m_hideOnClickAnywhere, "Hide OnClick Anywhere", ComponentColorName, false, true, true);
                GUILayout.Space(DGUI.Properties.Space(2));
                m_hideOnClickOverlay = DGUI.Toggle.Switch.Draw(m_hideOnClickOverlay, "Hide OnClick Overlay", ComponentColorName, false, true, true);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.BeginHorizontal();
            {
                m_hideOnClickContainer = DGUI.Toggle.Switch.Draw(m_hideOnClickContainer, "Hide OnClick Container", ComponentColorName, false, true, true);
                GUILayout.Space(DGUI.Properties.Space(2));
                m_hideOnBackButton = DGUI.Toggle.Switch.Draw(m_hideOnBackButton, "Hide On Back Button", ComponentColorName, false, true, true);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(4));

            var textSize = Size.XL;
            float buttonHeight = DGUI.Sizes.BarHeight(textSize);

            GUILayout.BeginHorizontal();
            {
                if (DGUI.Button.Dynamic.DrawIconButton(DGUI.Icon.Show, UILabels.Show, textSize, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight))
                {
                    UIPopup popup = UIPopupManager.ShowPopup(m_targetPopupName, m_addToQueue, false);
                    popup.DestroyAfterHide = true;
                    popup.HideOnClickAnywhere = m_hideOnClickAnywhere;
                    popup.HideOnClickOverlay = m_hideOnClickOverlay;
                    popup.HideOnClickContainer = m_hideOnClickContainer;
                    popup.HideOnBackButton = m_hideOnBackButton;

                    if (m_exampleSprite != null) popup.Data.SetImagesSprites(m_exampleSprite, m_exampleSprite, m_exampleSprite);
                }

                GUILayout.Space(DGUI.Properties.Space());

                if (DGUI.Button.Dynamic.DrawIconButton(DGUI.Icon.Hide, UILabels.Hide, textSize, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight)) UIPopup.HidePopup(m_targetPopupName);
            }
            GUILayout.EndHorizontal();
        }

        #endregion
        
    }
}