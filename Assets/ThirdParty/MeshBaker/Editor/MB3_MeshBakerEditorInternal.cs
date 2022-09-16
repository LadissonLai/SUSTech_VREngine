//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

using UnityEditor;

namespace DigitalOpus.MB.MBEditor
{

    public interface MB3_MeshBakerEditorWindowInterface
    {
        MonoBehaviour target
        {
            get;
            set;
        }
    }

    public class MB_EditorStyles
    {

        public GUIStyle multipleMaterialBackgroundStyle = new GUIStyle();
        public GUIStyle multipleMaterialBackgroundStyleDarker = new GUIStyle();
        public GUIStyle editorBoxBackgroundStyle = new GUIStyle();

        Texture2D multipleMaterialBackgroundColor;
        Texture2D multipleMaterialBackgroundColorDarker;
        Texture2D editorBoxBackgroundColor;

        public void Init()
        {
            bool isPro = EditorGUIUtility.isProSkin;
            Color backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);
            if (multipleMaterialBackgroundColor == null)
            {
                multipleMaterialBackgroundColor = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            }

            backgroundColor = isPro
                ? new Color32(50, 50, 50, 255)
                : new Color32(160, 160, 160, 255);
            if (multipleMaterialBackgroundColorDarker == null)
            {
                multipleMaterialBackgroundColorDarker = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            }

            backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);

            multipleMaterialBackgroundStyle.normal.background = multipleMaterialBackgroundColor;
            multipleMaterialBackgroundStyleDarker.normal.background = multipleMaterialBackgroundColorDarker;

            if (editorBoxBackgroundColor == null)
            {
                editorBoxBackgroundColor = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            }

            editorBoxBackgroundStyle.normal.background = editorBoxBackgroundColor;
            editorBoxBackgroundStyle.border = new RectOffset(0, 0, 0, 0);
            editorBoxBackgroundStyle.margin = new RectOffset(5, 5, 5, 5);
            editorBoxBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);
        }

        public void DestroyTextures()
        {
            if (multipleMaterialBackgroundColor != null) GameObject.DestroyImmediate(multipleMaterialBackgroundColor);
            if (multipleMaterialBackgroundColorDarker != null) GameObject.DestroyImmediate(multipleMaterialBackgroundColorDarker);
            if (editorBoxBackgroundColor != null) GameObject.DestroyImmediate(editorBoxBackgroundColor);
        }
    }

    public class MB3_MeshBakerEditorInternal
    {
        //add option to exclude skinned mesh renderer and mesh renderer in filter
        //example scenes for multi material
        private static GUIContent
            gc_outputOptoinsGUIContent = new GUIContent("Output"),
            gc_logLevelContent = new GUIContent("Log Level"),
            gc_openToolsWindowLabelContent = new GUIContent("Open Tools For Adding Objects", "Use these tools to find out what can be combined, discover problems with meshes, and quickly add objects."),


            gc_objectsToCombineGUIContent = new GUIContent("Custom List Of Objects To Be Combined", "You can add objects here that were not on the list in the MB3_TextureBaker as long as they use a material that is in the Texture Bake Results"),
            gc_textureBakeResultsGUIContent = new GUIContent("Texture Bake Result", "When materials are combined a MB2_TextureBakeResult Asset is generated. Drag that Asset to this field to use the combined material."),
            gc_useTextureBakerObjsGUIContent = new GUIContent("Same As Texture Baker", "Build a combined mesh using using the same list of objects that generated the Combined Material"),
            gc_combinedMeshPrefabGUIContent = new GUIContent("Combined Mesh Prefab", "Create a new prefab asset an drag an empty game object to it. Drag the prefab asset to here."),
            gc_SortAlongAxis = new GUIContent("SortAlongAxis", "Transparent materials often require that triangles be rendered in a certain order. This will sort Game Objects along the specified axis. Triangles will be added to the combined mesh in this order."),
            gc_Settings = new GUIContent("Use Shared Settings", "Different bakers can share the same settings. If this field is None, then the settings below will be used. " +
                                                                "Assign one of the following:\n" +
                                                                "   - Mesh Baker Settings project asset \n" +
                                                                "   - Mesh Baker Grouper scene instance \n");



        private SerializedObject meshBaker;
        private SerializedProperty logLevel, combiner, outputOptions, textureBakeResults, useObjsToMeshFromTexBaker, objsToMesh, mesh, sortOrderAxis;

        private SerializedProperty settingsHolder;

        private MB_MeshBakerSettingsEditor meshBakerSettingsThis;
        private MB_MeshBakerSettingsEditor meshBakerSettingsExternal;

        bool showInstructions = false;
        bool showContainsReport = true;

        MB_EditorStyles editorStyles = new MB_EditorStyles();

        Color buttonColor = new Color(.8f, .8f, 1f, 1f);
        void _init(SerializedObject mb)
        {
            this.meshBaker = mb;

            objsToMesh = meshBaker.FindProperty("objsToMesh");
            combiner = meshBaker.FindProperty("_meshCombiner");
            logLevel = combiner.FindPropertyRelative("_LOG_LEVEL");
            outputOptions = combiner.FindPropertyRelative("_outputOption");
            useObjsToMeshFromTexBaker = meshBaker.FindProperty("useObjsToMeshFromTexBaker");
            textureBakeResults = combiner.FindPropertyRelative("_textureBakeResults");
            mesh = combiner.FindPropertyRelative("_mesh");
            sortOrderAxis = meshBaker.FindProperty("sortAxis");
            settingsHolder = combiner.FindPropertyRelative("_settingsHolder");
            meshBakerSettingsThis = new MB_MeshBakerSettingsEditor();
            meshBakerSettingsThis.OnEnable(combiner, meshBaker);
            editorStyles.Init();
        }

        public void OnEnable(SerializedObject meshBaker)
        {
            _init(meshBaker);
        }

        public void OnDisable()
        {
            editorStyles.DestroyTextures();
            if (meshBakerSettingsThis != null) meshBakerSettingsThis.OnDisable();
            if (meshBakerSettingsExternal != null) meshBakerSettingsExternal.OnDisable();
        }

        public void OnInspectorGUI(SerializedObject meshBaker, MB3_MeshBakerCommon target, System.Type editorWindowType)
        {
            DrawGUI(meshBaker, target, editorWindowType);
        }

        public void DrawGUI(SerializedObject meshBaker, MB3_MeshBakerCommon target, System.Type editorWindowType)
        {
            if (meshBaker == null)
            {
                return;
            }

            meshBaker.Update();

            showInstructions = EditorGUILayout.Foldout(showInstructions, "Instructions:");
            if (showInstructions)
            {
                EditorGUILayout.HelpBox("1. Bake combined material(s).\n\n" +
                                        "2. If necessary set the 'Texture Bake Results' field.\n\n" +
                                        "3. Add scene objects or prefabs to combine or check 'Same As Texture Baker'. For best results these should use the same shader as result material.\n\n" +
                                        "4. Select options and 'Bake'.\n\n" +
                                        "6. Look at warnings/errors in console. Decide if action needs to be taken.\n\n" +
                                        "7. (optional) Disable renderers in source objects.", UnityEditor.MessageType.None);

                EditorGUILayout.Separator();
            }

            MB3_MeshBakerCommon momm = (MB3_MeshBakerCommon)target;
            EditorGUILayout.PropertyField(logLevel, gc_logLevelContent);
            EditorGUILayout.PropertyField(textureBakeResults, gc_textureBakeResultsGUIContent);
            bool doingTextureArray = false;
            if (textureBakeResults.objectReferenceValue != null)
            {
                doingTextureArray = ((MB2_TextureBakeResults)textureBakeResults.objectReferenceValue).resultType == MB2_TextureBakeResults.ResultType.textureArray;
                showContainsReport = EditorGUILayout.Foldout(showContainsReport, "Shaders & Materials Contained");
                if (showContainsReport)
                {
                    EditorGUILayout.HelpBox(((MB2_TextureBakeResults)textureBakeResults.objectReferenceValue).GetDescription(), MessageType.Info);
                }
            }

            EditorGUILayout.BeginVertical(editorStyles.editorBoxBackgroundStyle);
            EditorGUILayout.LabelField("Objects To Be Combined", EditorStyles.boldLabel);
            if (momm.GetTextureBaker() != null)
            {
                EditorGUILayout.PropertyField(useObjsToMeshFromTexBaker, gc_useTextureBakerObjsGUIContent);
            }
            else
            {
                useObjsToMeshFromTexBaker.boolValue = false;
                momm.useObjsToMeshFromTexBaker = false;
                GUI.enabled = false;
                EditorGUILayout.PropertyField(useObjsToMeshFromTexBaker, gc_useTextureBakerObjsGUIContent);
                GUI.enabled = true;
            }

            if (!momm.useObjsToMeshFromTexBaker)
            {
                if (GUILayout.Button(gc_openToolsWindowLabelContent))
                {
                    MB3_MeshBakerEditorWindowInterface mmWin = (MB3_MeshBakerEditorWindowInterface)EditorWindow.GetWindow(editorWindowType);
                    mmWin.target = (MB3_MeshBakerRoot)target;
                }

                object[] objs = MB3_EditorMethods.DropZone("Drag & Drop Renderers Or Parents Here To Add Objects To Be Combined", 300, 50);
                MB3_EditorMethods.AddDroppedObjects(objs, momm);

                EditorGUILayout.PropertyField(objsToMesh, gc_objectsToCombineGUIContent, true);
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Objects In Scene"))
                {
                    Selection.objects = momm.GetObjectsToCombine().ToArray();
                    if (momm.GetObjectsToCombine().Count > 0)
                    {
                        SceneView.lastActiveSceneView.pivot = momm.GetObjectsToCombine()[0].transform.position;
                    }
                }
                if (GUILayout.Button(gc_SortAlongAxis))
                {
                    MB3_MeshBakerRoot.ZSortObjects sorter = new MB3_MeshBakerRoot.ZSortObjects();
                    sorter.sortAxis = sortOrderAxis.vector3Value;
                    sorter.SortByDistanceAlongAxis(momm.GetObjectsToCombine());
                }
                EditorGUILayout.PropertyField(sortOrderAxis, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(objsToMesh, gc_objectsToCombineGUIContent, true);
                GUI.enabled = true;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(outputOptions, gc_outputOptoinsGUIContent);
            if (momm.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoSceneObject)
            {
                //todo switch to renderer
                momm.meshCombiner.resultSceneObject = (GameObject)EditorGUILayout.ObjectField("Combined Mesh Object", momm.meshCombiner.resultSceneObject, typeof(GameObject), true);
                if (momm is MB3_MeshBaker)
                {
                    string l = "Mesh";
                    Mesh m = (Mesh)mesh.objectReferenceValue;
                    if (m != null)
                    {
                        l += " (" + m.GetInstanceID() + ")";
                    }
                    Mesh nm = (Mesh)EditorGUILayout.ObjectField(new GUIContent(l), m, typeof(Mesh), true);
                    if (nm != m)
                    {
                        Undo.RecordObject(momm, "Assign Mesh");
                        ((MB3_MeshCombinerSingle)momm.meshCombiner).SetMesh(nm);
                        mesh.objectReferenceValue = nm;
                    }
                }
            }
            else if (momm.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoPrefab)
            {
                momm.resultPrefab = (GameObject)EditorGUILayout.ObjectField(gc_combinedMeshPrefabGUIContent, momm.resultPrefab, typeof(GameObject), true);
                if (momm is MB3_MeshBaker)
                {
                    string l = "Mesh";
                    Mesh m = (Mesh)mesh.objectReferenceValue;
                    if (m != null)
                    {
                        l += " (" + m.GetInstanceID() + ")";
                    }
                    Mesh nm = (Mesh)EditorGUILayout.ObjectField(new GUIContent(l), m, typeof(Mesh), true);
                    if (nm != m)
                    {
                        Undo.RecordObject(momm, "Assign Mesh");
                        ((MB3_MeshCombinerSingle)momm.meshCombiner).SetMesh(nm);
                        mesh.objectReferenceValue = nm;
                    }
                }
            }
            else if (momm.meshCombiner.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
            {
                EditorGUILayout.HelpBox("NEW! Try the BatchPrefabBaker component. It makes preparing a batch of prefabs for static/ dynamic batching much easier.", MessageType.Info);
                if (GUILayout.Button("Choose Folder For Bake In Place Meshes"))
                {
                    string newFolder = EditorUtility.SaveFolderPanel("Folder For Bake In Place Meshes", Application.dataPath, "");
                    if (!newFolder.Contains(Application.dataPath)) Debug.LogWarning("The chosen folder must be in your assets folder.");
                    momm.bakeAssetsInPlaceFolderPath = "Assets" + newFolder.Replace(Application.dataPath, "");
                }
                EditorGUILayout.LabelField("Folder For Meshes: " + momm.bakeAssetsInPlaceFolderPath);
            }

            if (momm is MB3_MultiMeshBaker)
            {
                MB3_MultiMeshCombiner mmc = (MB3_MultiMeshCombiner)momm.meshCombiner;
                mmc.maxVertsInMesh = EditorGUILayout.IntField("Max Verts In Mesh", mmc.maxVertsInMesh);
            }

            //-----------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            bool settingsEnabled = true;

            //------------- Mesh Baker Settings is a bit tricky because it is an interface.



            EditorGUILayout.Space();
            UnityEngine.Object obj = settingsHolder.objectReferenceValue;

            // Don't use a PropertyField because we may not be able to use the assigned object. It may not implement requried interface.
            obj = EditorGUILayout.ObjectField(gc_Settings, obj, typeof(UnityEngine.Object), true);

            if (obj == null)
            {
                settingsEnabled = true;
                settingsHolder.objectReferenceValue = null;
                if (meshBakerSettingsExternal != null)
                {
                    meshBakerSettingsExternal.OnDisable();
                    meshBakerSettingsExternal = null;
                }
            }

            else if (obj is GameObject)
            {
                // Check to see if there is a component on this game object that implements MB_IMeshBakerSettingsHolder
                MB_IMeshBakerSettingsHolder itf = (MB_IMeshBakerSettingsHolder)((GameObject)obj).GetComponent(typeof(MB_IMeshBakerSettingsHolder));
                if (itf != null)
                {
                    settingsEnabled = false;
                    Component settingsHolderComponent = (Component)itf;
                    if (settingsHolder.objectReferenceValue != settingsHolderComponent)
                    {
                        settingsHolder.objectReferenceValue = settingsHolderComponent;
                        meshBakerSettingsExternal = new MB_MeshBakerSettingsEditor();
                        UnityEngine.Object targetObj;
                        string propertyName;
                        itf.GetMeshBakerSettingsAsSerializedProperty(out propertyName, out targetObj);
                        SerializedProperty meshBakerSettings = new SerializedObject(targetObj).FindProperty(propertyName);
                        meshBakerSettingsExternal.OnEnable(meshBakerSettings);
                    }
                }
                else
                {
                    settingsEnabled = true;
                    settingsHolder = null;
                    if (meshBakerSettingsExternal != null)
                    {
                        meshBakerSettingsExternal.OnDisable();
                        meshBakerSettingsExternal = null;
                    }
                }
            }
            else if (obj is MB_IMeshBakerSettingsHolder)
            {
                settingsEnabled = false;
                if (settingsHolder.objectReferenceValue != obj)
                {
                    settingsHolder.objectReferenceValue = obj;
                    meshBakerSettingsExternal = new MB_MeshBakerSettingsEditor();
                    UnityEngine.Object targetObj;
                    string propertyName;
                    ((MB_IMeshBakerSettingsHolder)obj).GetMeshBakerSettingsAsSerializedProperty(out propertyName, out targetObj);
                    SerializedProperty meshBakerSettings = new SerializedObject(targetObj).FindProperty(propertyName);
                    meshBakerSettingsExternal.OnEnable(meshBakerSettings);
                }
            }
            else
            {
                Debug.LogError("Object was not a Mesh Baker Settings object.");
            }
            EditorGUILayout.Space();

            if (settingsHolder.objectReferenceValue == null)
            {
                // Use the meshCombiner settings
                meshBakerSettingsThis.DrawGUI(momm.meshCombiner, settingsEnabled, doingTextureArray);
            }
            else
            {
                if (meshBakerSettingsExternal == null)
                {
                    meshBakerSettingsExternal = new MB_MeshBakerSettingsEditor();
                    UnityEngine.Object targetObj;
                    string propertyName;
                    ((MB_IMeshBakerSettingsHolder)obj).GetMeshBakerSettingsAsSerializedProperty(out propertyName, out targetObj);
                    SerializedProperty meshBakerSettings = new SerializedObject(targetObj).FindProperty(propertyName);
                    meshBakerSettingsExternal.OnEnable(meshBakerSettings);
                }
                meshBakerSettingsExternal.DrawGUI(((MB_IMeshBakerSettingsHolder)settingsHolder.objectReferenceValue).GetMeshBakerSettings(), settingsEnabled, doingTextureArray);
            }

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;
            if (GUILayout.Button("Bake"))
            {
                bake(momm, ref meshBaker);
            }
            GUI.backgroundColor = oldColor;

            string enableRenderersLabel;
            bool disableRendererInSource = false;
            if (momm.GetObjectsToCombine().Count > 0)
            {
                Renderer r = MB_Utility.GetRenderer(momm.GetObjectsToCombine()[0]);
                if (r != null && r.enabled) disableRendererInSource = true;
            }
            if (disableRendererInSource)
            {
                enableRenderersLabel = "Disable Renderers On Source Objects";
            }
            else
            {
                enableRenderersLabel = "Enable Renderers On Source Objects";
            }
            if (GUILayout.Button(enableRenderersLabel))
            {
                momm.EnableDisableSourceObjectRenderers(!disableRendererInSource);
            }

            meshBaker.ApplyModifiedProperties();
            meshBaker.SetIsDifferentCacheDirty();
        }

        public static void updateProgressBar(string msg, float progress)
        {
            EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
        }

        public static bool bake(MB3_MeshBakerCommon mom)
        {
            SerializedObject so = null;
            return bake(mom, ref so);
        }

        /// <summary>
        /// Bakes a combined mesh.
        /// </summary>
        /// <param name="mom"></param>
        /// <param name="so">This is needed to work around a Unity bug where UnpackPrefabInstance corrupts 
        /// a SerializedObject. Only needed for bake into prefab.</param>
        public static bool bake(MB3_MeshBakerCommon mom, ref SerializedObject so)
        {
            bool createdDummyTextureBakeResults = false;
            bool success = false;
            try
            {
                if (mom.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoSceneObject ||
                    mom.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoPrefab)
                {
                    success = MB3_MeshBakerEditorFunctions.BakeIntoCombined(mom, out createdDummyTextureBakeResults, ref so);
                }
                else
                {
                    //bake meshes in place
                    if (mom is MB3_MeshBaker)
                    {
                        if (MB3_MeshCombiner.EVAL_VERSION)
                        {
                            Debug.LogError("Bake Meshes In Place is disabled in the evaluation version.");
                        }
                        else
                        {
                            MB2_ValidationLevel vl = Application.isPlaying ? MB2_ValidationLevel.quick : MB2_ValidationLevel.robust;
                            if (!MB3_MeshBakerRoot.DoCombinedValidate(mom, MB_ObjsToCombineTypes.prefabOnly, new MB3_EditorMethods(), vl)) return false;

                            List<GameObject> objsToMesh = mom.GetObjectsToCombine();
                            success = MB3_BakeInPlace.BakeMeshesInPlace((MB3_MeshCombinerSingle)((MB3_MeshBaker)mom).meshCombiner, objsToMesh, mom.bakeAssetsInPlaceFolderPath, mom.clearBuffersAfterBake, updateProgressBar);
                        }
                    }
                    else
                    {
                        Debug.LogError("Multi-mesh Baker components cannot be used for Bake In Place. Use an ordinary Mesh Baker object instead.");
                    }
                }
                mom.meshCombiner.CheckIntegrity();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (createdDummyTextureBakeResults && mom.textureBakeResults != null)
                {
                    MB_Utility.Destroy(mom.textureBakeResults);
                    mom.textureBakeResults = null;
                }
                EditorUtility.ClearProgressBar();
            }
            return success;
        }
    }
}
