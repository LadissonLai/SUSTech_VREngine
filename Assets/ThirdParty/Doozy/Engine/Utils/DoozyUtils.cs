// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using Doozy.Engine.Settings;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Utils
{
    public static class DoozyUtils
    {
        public const string BACKGROUND = "Background";
        public const string OVERLAY = "Overlay";

        public static Color BackgroundColor = new Color(31f / 255f, 136f / 255f, 201f / 255f);
        public static Color CheckmarkColor = new Color(11f / 255f, 50f / 255f, 74f / 255f);
        public static Color OverlayColor = new Color(0, 0, 0, 205f / 255f);
        public static Color TextColor = new Color(2f / 255f, 10f / 255f, 15f / 255f);
        public const int TEXT_FONT_SIZE = 14;

        /// <summary>
        ///     Adds an Image component to the target GameObject and returns a reference to it.
        ///     <para />
        ///     If an Image component is already attached to the target GameObject, a reference to it will be returned instead.
        /// </summary>
        /// <param name="target"> Target GameObject that will have the Image component attached </param>
        public static Image AddImageToGameObject(GameObject target)
        {
            Image image = target.GetComponent<Image>() ?? target.AddComponent<Image>();
            image.color = BackgroundColor;
            return image;
        }

        /// <summary> Returns a reference to a newly created GameObject that will have an Image component attached </summary>
        /// <param name="objectName"> The name of the newly created GameObject </param>
        /// <param name="color"> The color applied to the attached Image component </param>
        /// <param name="parent"> Optional parent for the newly created GameObject </param>
        public static GameObject CreateGameObjectWithAnImageComponent(string objectName, Color color, GameObject parent = null)
        {
            var go = new GameObject(objectName, typeof(RectTransform));
            if (parent != null)
            {
#if UNITY_EDITOR

                UnityEditor.GameObjectUtility.SetParentAndAlign(go, parent);
#else
                go.transform.SetParent(parent.transform);
#endif
            }

            go.GetComponent<RectTransform>().FullScreen(true);
            AddImageToGameObject(go).color = color;
            return go;
        }

        public static GameObject CreateBackgroundImage(GameObject parent) { return CreateGameObjectWithAnImageComponent(BACKGROUND, BackgroundColor, parent); }
        public static GameObject CreateOverlayImage(GameObject parent) { return CreateGameObjectWithAnImageComponent(OVERLAY, OverlayColor, parent); }

        /// <summary> Adds a new GameObject with the attached MonoBehavior of type T </summary>
        /// <param name="gameObjectName"> The name of the newly created GameObject </param>
        /// <param name="isSingleton"> If TRUE, it will check if there isn't another GameObject with the MonoBehavior attached. If there is, it will select it (Editor only)</param>
        /// <param name="selectGameObjectAfterCreation"> If TRUE, after creating a new GameObject, it will get automatically selected (Editor only)</param>
        /// <typeparam name="T"> MonoBehaviour </typeparam>
        public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation = false) where T : MonoBehaviour
        {
            var component = Object.FindObjectOfType<T>();
            if (component != null && isSingleton)
            {
                DDebug.Log("Cannot add another " + typeof(T).Name + " to this Scene because you don't need more than one.");
#if UNITY_EDITOR
                UnityEditor.Selection.activeObject = component;
#endif
                return component;
            }

            component = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(component.gameObject, "Created " + gameObjectName);
            if (selectGameObjectAfterCreation) UnityEditor.Selection.activeObject = component.gameObject;
#endif
            return component;
        }

        /// <summary> Adds objectToAdd to an existing asset identified by assetObject </summary>
        /// <param name="objectToAdd"> Object that will get added under the asset</param>
        /// <param name="assetObject"> Asset that will become the parent of the object</param>
        public static void AddObjectToAsset(Object objectToAdd, Object assetObject)
        {
#if UNITY_EDITOR
//            Debug.Log("objectToAdd is null? " + (objectToAdd == null));
//            Debug.Log("assetObject is null? " + (assetObject == null));

            if (objectToAdd == null || assetObject == null) return;

            if (!UnityEditor.EditorUtility.IsPersistent(objectToAdd)) UnityEditor.AssetDatabase.AddObjectToAsset(objectToAdd, assetObject);
#endif
        }

        /// <summary> Displays a modal dialog </summary>
        /// <param name="title"> The title of the message box </param>
        /// <param name="message"> The text of the message </param>
        /// <param name="ok"> Label displayed on the OK dialog button </param>
        public static bool DisplayDialog(string title, string message, string ok)
        {
#if UNITY_EDITOR
            return UnityEditor.EditorUtility.DisplayDialog(title, message, ok);
#else
            return false;
#endif
        }

        /// <summary> Displays a modal dialog </summary>
        /// <param name="title"> The title of the message box </param>
        /// <param name="message"> The text of the message </param>
        /// <param name="ok"> Label displayed on the OK dialog button </param>
        /// <param name="cancel"> Label displayed on the Cancel dialog button </param>
        public static bool DisplayDialog(string title, string message, string ok, string cancel)
        {
#if UNITY_EDITOR
            return UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel);
#else
            return false;
#endif
        }

        /// <summary> Displays or updates a progress bar </summary>
        /// <param name="title"> Bar title </param>
        /// <param name="info"> Bar info </param>
        /// <param name="progress"> Bar progress from 0 to 1 </param>
        public static void DisplayProgressBar(string title, string info, float progress)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar(title, info, progress);
#endif
        }

        /// <summary> Displays or updates a progress bar that has a cancel button </summary>
        /// <param name="title"> Bar title </param>
        /// <param name="info"> Bar info </param>
        /// <param name="progress"> Bar progress from 0 to 1 </param>
        public static bool DisplayCancelableProgressBar(string title, string info, float progress)
        {
#if UNITY_EDITOR
            return UnityEditor.EditorUtility.DisplayCancelableProgressBar(title, info, progress);
#else
            return false;
#endif
        }

        /// <summary> Removes progress bar </summary>
        public static void ClearProgressBar()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
        }

        /// <summary> Moves the asset at path to the trash </summary>
        public static bool MoveAssetToTrash(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.MoveAssetToTrash(path);
#else
            return false;
#endif
        }

        /// <summary> [Editor Only] Writes all unsaved asset changes to disk </summary>
        public static void SaveAssets()
        {
//            DDebug.Log("SaveAssets");
#if UNITY_EDITOR
            DoozySettings.Instance.AssetDatabaseSaveAssetsNeeded = false;
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="target"> The object to mark as dirty </param>
        public static void SetDirty(Object target)
        {
            if (target == null) return;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
            DoozySettings.Instance.AssetDatabaseSaveAssetsNeeded = true;
#endif
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="target"> The object to mark as dirty </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public static void SetDirty(Object target, bool saveAssets)
        {
            if (target == null) return;
            SetDirty(target);
            if (saveAssets) SaveAssets();
            else DoozySettings.Instance.AssetDatabaseSaveAssetsNeeded = true;
        }

        /// <summary> [Editor Only] Records any changes done on the object after the RecordObject function </summary>
        /// <param name="objectToUndo"> The reference to the object that you will be modifying </param>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public static void UndoRecordObject(Object objectToUndo, string undoMessage)
        {
            if (objectToUndo == null) return;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(objectToUndo, undoMessage);
#endif
        }

        /// <summary> [Editor Only] Records any changes done on the object after the RecordObject function </summary>
        /// <param name="objectToUndo"> The reference to the object that you will be modifying </param>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public static void UndoRecordObject(Object objectToUndo, string undoMessage, bool saveAssets)
        {
            if (objectToUndo == null) return;
            UndoRecordObject(objectToUndo, undoMessage);
            if (saveAssets) SaveAssets();
        }

        /// <summary> [Editor Only] Records multiple undoable objects in a single call. This is the same as calling Undo.RecordObject multiple times </summary>
        /// <param name="objectsToUndo"> The references to the objects that you will be modifying </param>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public static void UndoRecordObjects(Object[] objectsToUndo, string undoMessage)
        {
            if (objectsToUndo == null) return;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObjects(objectsToUndo, undoMessage);
#endif
        }

        /// <summary> [Editor Only] Records multiple undoable objects in a single call. This is the same as calling Undo.RecordObject multiple times </summary>
        /// <param name="objectsToUndo"> The references to the objects that you will be modifying </param>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public static void UndoRecordObjects(Object[] objectsToUndo, string undoMessage, bool saveAssets)
        {
            if (objectsToUndo == null) return;
            UndoRecordObjects(objectsToUndo, undoMessage);
            if (saveAssets) SaveAssets();
        }
    }
}