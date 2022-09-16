// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Linq;
using Doozy.Engine.Settings;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Core component in the DoozyUI system.
    ///     It is used to identify GameObjects with root Canvas components attached to instantiate UIPopups, at runtime, and to parent other UI components under them, in the Editor.
    /// </summary>
    [AddComponentMenu(MenuUtils.UICanvas_AddComponentMenu_MenuName, MenuUtils.UICanvas_AddComponentMenu_Order)]
    [RequireComponent(typeof(Canvas))]
    [DefaultExecutionOrder(DoozyExecutionOrder.UICANVAS)]
    public class UICanvas : UIComponentBase<UICanvas>
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UICanvas_MenuItem_ItemName, false, MenuUtils.UICanvas_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            string canvasName = MasterCanvasName;                     // create a new canvas with the name 'MasterCanvas'
            UICanvas[] searchResults = FindObjectsOfType<UICanvas>(); //search the entire scene for any Source that may have an UICanvas component attached
            if (searchResults != null && searchResults.Length > 0)    //check if there isn't another UICanvas with the proposed canvasName
            {
                bool renameRequired = true;
                int canvasCount = 0;
                while (renameRequired) //do this until we have an unique canvasName
                {
                    renameRequired = false;
                    if (!searchResults.Any(uiCanvas => canvasName.Equals(uiCanvas.CanvasName))) continue;
                    canvasCount++;
                    canvasName = MenuUtils.UICanvas_GameObject_Name + " " + canvasCount;
                    renameRequired = true;
                }
            }

            UICanvas canvas = CreateUICanvas(canvasName);
            Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Create UICanvas '" + canvasName + "'"); //undo option
            Selection.activeObject = canvas.gameObject;                                                //select the newly created UICanvas
        }
#endif

        #endregion

        #region Static Properties        

        /// <summary> Default UICanvas canvas category name  </summary>
        public static string DefaultCanvasCategory { get { return NamesDatabase.GENERAL; } }

        /// <summary> Default UICanvas canvas name </summary>
        public static string DefaultCanvasName { get { return NamesDatabase.UNNAMED; } }

        /// <summary> Returns a reference to the UICanvas named 'MasterCanvas'. There can be only one! </summary>
        public static UICanvas MasterCanvas { get; private set; }

        /// <summary> Default UICanvas canvas name for the MasterCanvas </summary>
        public static string MasterCanvasName { get { return NamesDatabase.MASTER_CANVAS; } }

        #endregion

        #region Properties

        /// <summary> Reference to the Canvas component </summary>
        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null) m_canvas = GetComponent<Canvas>();
                return m_canvas;
            }
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary> Returns true it this UICanvas has the name 'MasterCanvas' and if it has been registered as the MasterCanvas </summary>
        public bool IsMasterCanvas { get { return CanvasName.Equals(MasterCanvasName) && GetMasterCanvas() == this; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugUICanvas; } }
        
        #endregion

        #region Public Variables

        /// <summary> UICanvas canvas name </summary>
        public string CanvasName = DefaultCanvasName;

        // ReSharper disable once NotAccessedField.Global
        /// <summary> [Editor Only] Internal variable used by the custom inspector to allow you to type a custom canvas name instead of selecting it from the database </summary>
        public bool CustomCanvasName = true;

        /// <summary> Makes the GameObject, that this UICanvas component is attached to, not to get destroyed on load (when the scene changes) </summary>
        public bool DontDestroyCanvasOnLoad;

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the Canvas attached to this Source </summary>
        private Canvas m_canvas;

        #endregion

        #region Unity Methods

        protected override void Reset()
        {
            base.Reset();
            UICanvasSettings.Instance.ResetComponent(this);
            CanvasName = DefaultCanvasName;
        }

        public override void Awake()
        {
            if (DatabaseContains(CanvasName))
            {
                DDebug.LogError(
                               "Error duplicate UICanvas found. " +
                               "You cannot have multiple UICanvases with the same canvas name. " +
                               "This error originated from the UICanvas component attached to the " + name + " gameObject. " +
                               "The duplicate canvas name is '" + CanvasName + "'.",
                               this);

                gameObject.SetActive(false);
                return;
            }

            base.Awake();

            if (Canvas == null)
            {
                DDebug.LogError(
                               "The UICanvas, attached to the " + name + " gameObject, does not have a Canvas component attached. Fix this by adding a Canvas component.",
                               this);

                gameObject.SetActive(false);
                return;
            }

            if (!Canvas.isRootCanvas)
            {
                DDebug.LogError(
                               "The Canvas, attached to the " + name + " gameObject, is to a root canvas. The UICanvas component must be attached to a top (root) canvas in the Hierarchy.",
                               this);

                gameObject.SetActive(false);
                return;
            }

            if (DontDestroyCanvasOnLoad) DontDestroyOnLoad(gameObject);

            if (CanvasName.Equals(MasterCanvasName) && MasterCanvas == null) MasterCanvas = this;
        }

        #endregion

        #region Static Methods

        /// <summary> Create a new UICanvas with the given canvasName and return a reference to it </summary>
        /// <param name="canvasName"> Name of the canvas </param>
        public static UICanvas CreateUICanvas(string canvasName)
        {
            UnityEventSystem.transform.SetParent(null);
            canvasName = canvasName.Trim();
            if (string.IsNullOrEmpty(canvasName))
            {
                DDebug.Log("You cannot create a new UICanvas without entering a 'canvasName'. The 'canvasName' you passed was an empty string. No UICanvas was created and returned null.");
                return null;
            }

            if (DatabaseContains(canvasName))
            {
                DDebug.Log("Cannot create a new UICanvas with the '" + canvasName + "' canvasName because another UICanvas with the same name already exists in the UICanvas.Database. Returned the existing UICanvas instead.");
                return GetUICanvas(canvasName);
            }

            var go = new GameObject(canvasName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var canvas = go.AddComponent<UICanvas>();
            canvas.CanvasName = canvasName;
            canvas.CustomCanvasName = !canvasName.Equals(MasterCanvasName);
            return canvas;
        }

        /// <summary> Returns true if the UICanvas Database contains an UICanvas with the given canvasName </summary>
        /// <param name="canvasName"> Name of the canvas </param>
        public static bool DatabaseContains(string canvasName) { return Database.Any(t => t.CanvasName.Equals(canvasName)); }

        /// <summary> Returns a reference to the UICanvas that is considered and used as the 'MasterCanvas'. If no such UICanvas exists, one will be automatically created by default </summary>
        /// <param name="createMasterCanvasIfNotFound">if set to <c>true</c> [create master canvas if not found].</param>
        public static UICanvas GetMasterCanvas(bool createMasterCanvasIfNotFound = true)
        {
            if (MasterCanvas != null) return MasterCanvas;
            if (DatabaseContains(MasterCanvasName))
            {
                MasterCanvas = GetUICanvas(MasterCanvasName);
                return MasterCanvas;
            } //there is a MasterCanvas in the UICanvas.Database
#if UNITY_EDITOR
            UICanvas[] searchResults = FindObjectsOfType<UICanvas>();
            if (searchResults != null && searchResults.Length > 0)
                foreach (UICanvas uiCanvas in searchResults)
                    if (uiCanvas.CanvasName.Equals(MasterCanvasName))
                        return uiCanvas;
#endif
            if (!createMasterCanvasIfNotFound) return null;
            MasterCanvas = CreateUICanvas(MasterCanvasName); //no UICanvas that has been registered to the UICanvas.Database has the name 'MasterCanvas' -> create a new UICanvas as the MasterCanvas and return it
            MasterCanvas.CustomCanvasName = false;
            return MasterCanvas;
        }

        /// <summary> Returns an UICanvas that has been registered in the UICanvas Database, that has the given canvasName. If no UICanvas is found, it will return null </summary>
        /// <param name="canvasName"> Name of the canvas </param>
        public static UICanvas GetUICanvas(string canvasName) { return Database.FirstOrDefault(t => t.CanvasName.Equals(canvasName)); }

        /// <summary> Returns an UICanvas that has been registered in the UICanvas Database, that has the given canvasName. If no UICanvas is found, it will either create a new UICanvas with the given canvasName or it will return the MasterCanvas </summary>
        /// <param name="canvasName"> Name of the canvas </param>
        /// <param name="createUICanvasIfNotFound"> If set to <c>true</c> it will create a new UICanvas if no UICanvas with the 'canvasName' was found </param>
        /// <param name="returnMasterCanvasIfUICanvasNotFound"> If set to <c>true</c> it will return the MasterCanvas if no UICanvas with the 'canvasName' was found </param>
        public static UICanvas GetUICanvas(string canvasName, bool createUICanvasIfNotFound, bool returnMasterCanvasIfUICanvasNotFound = true)
        {
            canvasName = canvasName.Trim();
            if (string.IsNullOrEmpty(canvasName))
            {
                DDebug.Log("You cannot search for an UICanvas without entering a 'canvasName'. The 'canvasName' you passed was an empty string. Returned null.");
                return null;
            }

            if (DatabaseContains(canvasName)) return GetUICanvas(canvasName);
            if (createUICanvasIfNotFound) return CreateUICanvas(canvasName);
            return returnMasterCanvasIfUICanvasNotFound ? GetMasterCanvas() : null;
        }

        #endregion
    }
}