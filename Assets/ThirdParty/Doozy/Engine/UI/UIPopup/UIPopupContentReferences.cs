// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if dUI_TextMeshPro
using TMPro;
#endif

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Contains references to UIPopup elements that can be customized (Labels, Images and Buttons)
    /// </summary>
    [Serializable]
    public class UIPopupContentReferences
    {
        #region Properties

        /// <summary> Returns the number of entries in the Buttons list </summary>
        public int ButtonsCount { get { return Buttons.Count; } }

        /// <summary> Returns TRUE if the Buttons list has at least one entry </summary>
        public bool HasButtons { get { return ButtonsCount > 0; } }

        /// <summary> Returns TRUE if the Images list has at least one entry </summary>
        public bool HasImages { get { return ImagesCount > 0; } }

        /// <summary> Returns TRUE if the Labels list has at least one entry </summary>
        public bool HasLabels { get { return LabelsCount > 0; } }

        /// <summary> Returns the number of entries in the Images list </summary>
        public int ImagesCount { get { return Images.Count; } }

        /// <summary> Returns the number of entries in the Labels list </summary>
        public int LabelsCount { get { return Labels.Count; } }

        #endregion

        #region Public Variables

        /// <summary> List of UIButtons </summary>
        public List<UIButton> Buttons = new List<UIButton>();

        /// <summary> List of Images </summary>
        public List<Image> Images = new List<Image>();

        /// <summary> List of GameObjects used as labels. They should have either a Text or TextMeshPro component attached </summary>
        public List<GameObject> Labels = new List<GameObject>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets the passed callback as Buttons OnClick listener.
        ///     <para />
        ///     <para />
        ///     Buttons[0].OnClick.OnTrigger.Event.AddListener(callbacks[0])
        ///     <para />
        ///     Buttons[1].OnClick.OnTrigger.Event.AddListener(callbacks[1])
        ///     <para />
        ///     ...
        ///     <para />
        ///     Buttons[n].OnClick.OnTrigger.Event.AddListener(callbacks[n])
        /// </summary>
        /// <param name="callbacks"> List of UnityActions </param>
        public void SetButtonsCallbacks(params UnityAction[] callbacks)
        {
            if (callbacks == null || callbacks.Length == 0 || !HasButtons) return;
            for (int i = 0; i < Buttons.Count; i++)
            {
                UIButton button = Buttons[i];
                if (button == null) continue;
                if (callbacks[i] == null) continue;
                button.OnClick.OnTrigger.Event.AddListener(callbacks[i]);
            }
        }

        /// <summary>
        ///     Sets the passed strings as the referenced Buttons label text.
        ///     <para />
        ///     <para />
        ///     Buttons[0].SetLabelText(buttonLabels[0])
        ///     <para />
        ///     Buttons[1].SetLabelText(buttonLabels[1])
        ///     <para />
        ///     ...
        ///     <para />
        ///     Buttons[n].SetLabelText(buttonLabels[n])
        /// </summary>
        /// <param name="buttonLabels"> List of strings </param>
        public void SetButtonsLabels(params string[] buttonLabels)
        {
            if (buttonLabels == null || buttonLabels.Length == 0 || !HasButtons) return;
            for (int i = 0; i < Buttons.Count; i++)
            {
                UIButton button = Buttons[i];
                if (button == null) continue;
                button.SetLabelText(buttonLabels[i]);
            }
        }

        /// <summary>
        ///     Sets the passed strings as the referenced Buttons button name.
        ///     <para />
        ///     <para />
        ///     Buttons[0].ButtonName = buttonNames[0]
        ///     <para />
        ///     Buttons[1].ButtonName = buttonNames[1]
        ///     <para />
        ///     ...
        ///     <para />
        ///     Buttons[n].ButtonName = buttonNames[n]
        /// </summary>
        /// <param name="buttonNames"> List of strings </param>
        public void SetButtonsNames(params string[] buttonNames)
        {
            if (buttonNames == null || buttonNames.Length == 0 || !HasButtons) return;
            for (int i = 0; i < Buttons.Count; i++)
            {
                UIButton button = Buttons[i];
                if (button == null) continue;
                button.ButtonCategory = UIButton.CustomButtonCategory;
                button.ButtonName = buttonNames[i];
            }
        }

        /// <summary> Sets the given content data to all of the referenced components in the lists </summary>
        /// <param name="data"> Content data </param>
        public void SetContentData(UIPopupContentData data)
        {
            if (data == null) return;
            SetLabelsTexts(data.Labels.ToArray());
            SetImagesSprites(data.Sprites.ToArray());
            SetButtonsNames(data.ButtonNames.ToArray());
            SetButtonsLabels(data.ButtonLabels.ToArray());
            SetButtonsCallbacks(data.ButtonCallbacks.ToArray());
        }

        /// <summary>
        ///     Sets the passed sprites as Images sprite.
        ///     <para />
        ///     <para />
        ///     Images[0].sprite = sprites[0]
        ///     <para />
        ///     Images[1].sprite = sprites[1]
        ///     <para />
        ///     ...
        ///     <para />
        ///     Images[n].sprite = sprites[n]
        /// </summary>
        /// <param name="sprites"> List of Sprites </param>
        public void SetImagesSprites(params Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0 || !HasImages) return;
            for (int i = 0; i < Images.Count; i++)
            {
                Image image = Images[i];
                if (image == null) continue;
                image.sprite = sprites[i];
            }
        }

        /// <summary>
        ///     Sets the passed strings as Labels text.
        ///     <para />
        ///     <para />
        ///     Labels[0].text = labels[0]
        ///     <para />
        ///     Labels[1].text = labels[1]
        ///     <para />
        ///     ...
        ///     <para />
        ///     Labels[n].text = labels[n]
        /// </summary>
        /// <param name="labels"> List of strings </param>
        public void SetLabelsTexts(params string[] labels)
        {
            if (labels == null || labels.Length == 0 || !HasLabels) return;
            for (int i = 0; i < Labels.Count; i++)
            {
#if dUI_TextMeshPro
                var textMeshProUguiComponent = Labels[i].GetComponent<TextMeshProUGUI>();
                if (textMeshProUguiComponent != null) textMeshProUguiComponent.text = labels[i];
#else
                var textComponent = Labels[i].GetComponent<Text>();
                if (textComponent != null) textComponent.text = labels[i];
#endif
            }
        }

        #endregion
    }
}