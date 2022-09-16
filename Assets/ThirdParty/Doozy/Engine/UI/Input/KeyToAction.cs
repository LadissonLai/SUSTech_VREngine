// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Settings;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.UI.Input
{
    /// <inheritdoc />
    /// <summary> Listens for the set input key or virtual button and, when triggered, executes an UIAction </summary>
    [AddComponentMenu(MenuUtils.KeyToAction_AddComponentMenu_MenuName, MenuUtils.KeyToAction_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.KEY_TO_ACTION)]
    public class KeyToAction : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.KeyToAction_MenuItem_ItemName, false, MenuUtils.KeyToAction_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Properties

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugKeyToAction; } }

        #endregion

        #region Public Variables

        /// <summary> UIAction executed when this Key To Action is triggered </summary>
        public UIAction Actions = new UIAction();

        /// <summary> Enable relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> Key code and virtual button input settings </summary>
        public InputData InputData = new InputData();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (InputData == null) enabled = false;
        }

        private void Update()
        {
            if (InputData.InputMode == InputMode.None) return;
            bool execute = false;
            switch (InputData.InputMode)
            {
                case InputMode.KeyCode:
                    if (UnityEngine.Input.GetKeyDown(InputData.KeyCode))
                    {
                        if (Actions != null)
                        {
                            if (DebugComponent) DDebug.Log("Executed UIAction via KeyCode: " + InputData.KeyCode, gameObject);
                            execute = true;
                        }
                    }
                    else if (InputData.EnableAlternateInputs && UnityEngine.Input.GetKeyDown(InputData.KeyCodeAlt))
                    {
                        if (Actions != null)
                        {
                            if (DebugComponent) DDebug.Log("Executed UIAction via KeyCode: " + InputData.KeyCodeAlt, gameObject);
                            execute = true;
                        }
                    }

                    break;
                case InputMode.VirtualButton:
                    if (UnityEngine.Input.GetButtonDown(InputData.VirtualButtonName))
                    {
                        if (Actions != null)
                        {
                            if (DebugComponent) DDebug.Log("Executed UIAction via Virtual Button: " + InputData.VirtualButtonName, gameObject);
                            execute = true;
                        }
                    }
                    else if (InputData.EnableAlternateInputs && UnityEngine.Input.GetButtonDown(InputData.VirtualButtonNameAlt))
                    {
                        if (Actions != null)
                        {
                            if (DebugComponent) DDebug.Log("Executed UIAction via Virtual Button: " + InputData.VirtualButtonNameAlt, gameObject);
                            execute = true;
                        }
                    }

                    break;
            }

            if (!execute) return;
            Execute();
        }

        #endregion

        #region Public Methods

        /// <summary> Execute the UIAction </summary>
        public void Execute()
        {
            if (Actions == null)
            {
                if (DebugComponent)
                    DDebug.Log("UIAction is null!", gameObject);
                return;
            }

            Actions.Invoke(gameObject);
        }

        #endregion

        #region Static Methods

        /// <summary> Add a KeyToAction to scene and get a reference to it </summary>
        private static KeyToAction AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<KeyToAction>(MenuUtils.KeyToAction_GameObject_Name, false, selectGameObjectAfterCreation); }

        #endregion
    }
}