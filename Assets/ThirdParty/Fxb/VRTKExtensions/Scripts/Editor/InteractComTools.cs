using Framework;
using Fxb.CMSVR;
using System;
using UnityEditor;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.Highlighters;

namespace VRTKExtensions
{
    public static class InteractComTools
    {
        private static readonly string DEFAULT_HIGHLIGHTPLUS_PROFILE = "Assets/Resources/Configs/Highlight Plus Profile Interactable Obj.asset";

        #region 拆装部件脚本

        [MenuItem("Tools/VRTK Interact component tools/更新物体身上的交互脚本")]
        static void UpdateSelectObjInteractComponents()
        {
            if (Selection.transforms.Length == 0)
            {
                Debug.LogWarning("未选择物体");
                return;
            }

            foreach (var t in Selection.transforms)
            {
                Debug.Log($"更新:{t.name}");

                AddInteractableComponents(t.gameObject);
            }
        }

        [MenuItem("Tools/VRTK Interact component tools/删除指定物体身上的交互脚本")]
        public static void ClearSelectGoInteractComponents()
        {
            if (Selection.transforms.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个或者多个物体", "确定", "取消");
                return;
            }

            foreach (var t in Selection.transforms)
            {
                t.DestoryComponents(
                    typeof(VRTK_InteractableObject),
                    typeof(VRTK_InteractableListener),
                    typeof(VRTK_BaseHighlighter),
                    typeof(VRTK_InteractObjectHighlighter),
                    typeof(VRTK_BaseGrabAttach),
                    typeof(InteractObjTooltipTriggerBase),
                    typeof(HighlightPlus.HighlightEffect),
                    typeof(Rigidbody),
                    typeof(PanelSpawnTooltipTrigger)
                    );

                Debug.Log($"清理:{t.name}");
            }
        }

        [MenuItem("Tools/VRTK Interact component tools/删除指定物体及子物体的交互脚本")]
        public static void ClearSelectGOInteractComponentsWithChildren()
        {
            if (Selection.transforms.Length != 1)
            {
                Debug.LogWarning("未选择物体或选择了多个物体");
                return;
            }

            var rootGO = Selection.transforms[0];

            var allObjs = rootGO.GetComponentsInChildren<VRTK_InteractableObject>();

            if (allObjs.Length == 0)
            {
                Debug.LogWarning("无可交互物体需要清理");

                return;
            }

            if (EditorUtility.DisplayDialog("提示", $"一共清理 {allObjs.Length} 个物体，是否继续？", "确定", "取消"))
            {
                foreach (var obj in allObjs)
                {
                    var t = obj.transform;

                    t.DestoryComponents(
                        typeof(VRTK_InteractableObject),
                        typeof(VRTK_InteractableListener),
                        typeof(VRTK_BaseHighlighter),
                        typeof(VRTK_InteractObjectHighlighter),
                        typeof(VRTK_BaseGrabAttach),
                        typeof(InteractObjTooltipTriggerBase),
                        typeof(HighlightPlus.HighlightEffect),
                        typeof(Rigidbody),
                        typeof(PanelSpawnTooltipTrigger)
                        );

                    Debug.Log($"清理:{t.name}");
                }
            }
        }

        private static void AddInteractableComponents(GameObject go)
        {
            if (go == null)
                return;

            if (go.AddMissingComponent<AdvancedInteractableObj>(out var interactObj))
            {

            }

            if (go.GetComponent<VRTK_BaseHighlighter>() == null)
            {
                var hl = go.AddMissingComponent<HLPlusSuportHighlighter>();

                go.AddMissingComponent<HighlightPlus.HighlightEffect>(out var hlPlusEffect);

                if (hlPlusEffect.profile == null)
                {
                    hlPlusEffect.profile = AssetDatabase.LoadAssetAtPath<HighlightPlus.HighlightProfile>(DEFAULT_HIGHLIGHTPLUS_PROFILE);
                    hlPlusEffect.ProfileLoad(hlPlusEffect.profile);

                    if (hlPlusEffect.profile == null)
                    {
                        EditorUtility.DisplayDialog("提示", "缺少Highlight plus profile:" + DEFAULT_HIGHLIGHTPLUS_PROFILE, "确定", null);
                    }
                    else
                    {
                        hlPlusEffect.profile.Load(hlPlusEffect);
                    }
                }

                hl.highlightPlusEffect = hlPlusEffect;
            }

            if (go.AddMissingComponent<AdvancedInteractObjectHighlighter>(out var objHL))
            {
                objHL.objectHighlighter = go.GetComponent<VRTK_BaseHighlighter>();

                objHL.objectToMonitor = interactObj;
            }

            objHL.touchHighlight = objHL.focusHighlight = Color.yellow;

            objHL.tipHighlight = Color.green;

            if (go.AddMissingComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.isKinematic = true;

                rigidbody.useGravity = false;
            }

            if (go.AddMissingComponent<PanelSpawnTooltipTrigger>(out var tooltipTrigger))
            {
                tooltipTrigger.enableTooltipOnFocus = true;

                tooltipTrigger.enableTooltipOnTouch = true;

                tooltipTrigger.followMarkerPoint = false;
            }
        }

        #endregion

        #region Vr工具

        [MenuItem("Tools/VRTK Interact component tools/更新Vr工具身上的交互脚本")]
        static void UpdateSelectObjInteractComponentsVrTool()
        {
            if (Selection.transforms.Length == 0)
            {
                Debug.LogWarning("未选择物体");
                return;
            }

            foreach (var t in Selection.transforms)
            {
                Debug.Log($"更新:{t.name}");

                AddInteractableComponentsVrTool(t.gameObject);
            }
        }

        static void AddInteractableComponentsVrTool(GameObject go)
        {
            if (go == null)
                return;

            if (go.GetComponent<VRTK_InteractableObject>() == null)
            {
                if (go.AddMissingComponent<AdvancedInteractableObj>(out var interactObj))
                {
                    interactObj.isGrabbable = true;

                    interactObj.holdButtonToGrab = false;

                    interactObj.isUsable = true;

                    interactObj.useOnlyIfGrabbed = true;
                }
            }

            if (go.GetComponent<VRTK_BaseHighlighter>() == null)
                go.AddMissingComponent<VRTK_MaterialPropertyBlockColorSwapHighlighter>();

            if (go.GetComponent<VRTK_InteractableListener>() == null)
            {
                go.AddMissingComponent<AdvancedInteractObjectHighlighter>(out var objHL);

                objHL.touchHighlight = objHL.focusHighlight = Color.yellow;

                objHL.tipHighlight = Color.green;
            }

            if (go.AddMissingComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.isKinematic = true;

                rigidbody.useGravity = false;
            }


            if (go.GetComponent<VRTK_BaseGrabAttach>() == null)
            {
                go.AddMissingComponent<VRTK_ChildOfControllerGrabAttach>();


            }

            if (go.AddMissingComponent<VRTK_PolicyList>(out var plist))
            {
                plist.operation = VRTK_PolicyList.OperationTypes.Include;
                plist.checkType = VRTK_PolicyList.CheckTypes.Script;
            }

            if (go.AddMissingComponent<HandToolCollisionTracker>(out var tracker))
            {
                tracker.policyList = plist;
            }

            if (go.AddMissingComponent<PanelSpawnTooltipTrigger>(out var tooltipTrigger))
            {
                tooltipTrigger.enableTooltipOnFocus = true;

                tooltipTrigger.enableTooltipOnTouch = true;

                tooltipTrigger.followMarkerPoint = false;
            }
        }

        [MenuItem("Tools/VRTK Interact component tools/删除指定Vr工具的交互脚本")]
        static void ClearSelectGoInteractComponentsVrTool()
        {
            if (Selection.transforms.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个或者多个物体", "确定");
                return;
            }

            foreach (var t in Selection.transforms)
            {
                t.DestoryComponents(
                    typeof(VRTK_InteractableObject),
                    typeof(VRTK_InteractableListener),
                    typeof(VRTK_BaseHighlighter),
                    typeof(VRTK_BaseGrabAttach),
                    typeof(HandToolCollisionTracker),
                    typeof(VRTK_PolicyList),
                    typeof(Rigidbody),
                    typeof(PanelSpawnTooltipTrigger)
                    );

                Debug.Log($"清理:{t.name}");
            }
        }

        #endregion


        #region Clone预制体

        [MenuItem("Tools/VRTK Interact component tools/更新VrClone物体身上的交互脚本")]
        static void UpdateSelectObjInteractComponentsVrClone()
        {
            if (Selection.transforms.Length == 0)
            {
                Debug.LogWarning("未选择物体");
                return;
            }

            foreach (var t in Selection.transforms)
            {
                Debug.Log($"更新:{t.name}");

                AddInteractableComponentsVrClone(t.gameObject);
            }
        }


        static void AddInteractableComponentsVrClone(GameObject go)
        {
            if (go == null)
                return;

            AdvancedInteractableObj interactObj = null;

            if (go.GetComponent<VRTK_InteractableObject>() == null)
            {
                if (go.AddMissingComponent(out interactObj))
                {
                    interactObj.isGrabbable = true;

                    interactObj.holdButtonToGrab = false;

                    interactObj.stayGrabbedOnTeleport = true;

                    interactObj.isUsable = true;
                }
            }

            if (go.GetComponent<VRTK_BaseHighlighter>() == null)
            {
                var hl = go.AddMissingComponent<HLPlusSuportHighlighter>();

                go.AddMissingComponent<HighlightPlus.HighlightEffect>(out var hlPlusEffect);

                if (hlPlusEffect.profile == null)
                {
                    hlPlusEffect.profile = AssetDatabase.LoadAssetAtPath<HighlightPlus.HighlightProfile>(DEFAULT_HIGHLIGHTPLUS_PROFILE);

                    if (hlPlusEffect.profile == null)
                    {
                        EditorUtility.DisplayDialog("提示", "缺少Highlight plus profile:" + DEFAULT_HIGHLIGHTPLUS_PROFILE, "确定", null);
                    }
                    else
                    {
                        hlPlusEffect.profile.Load(hlPlusEffect);
                    }
                }

                hl.highlightPlusEffect = hlPlusEffect;
            }

            if (go.GetComponent<VRTK_InteractableListener>() == null)
            {
                go.AddMissingComponent<AdvancedInteractObjectHighlighter>(out var objHL);

                objHL.touchHighlight = objHL.focusHighlight = Color.yellow;

                objHL.tipHighlight = Color.green;

                objHL.objectHighlighter = go.GetComponent<VRTK_BaseHighlighter>();
            }

            if (go.AddMissingComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.isKinematic = true;

                rigidbody.useGravity = false;
            }


            if (go.GetComponent<VRTK_BaseGrabAttach>() == null)
            {
                go.AddMissingComponent<VRTK_ChildOfControllerGrabAttach>();


            }

            go.AddMissingComponent<HandToolCollisionTracker>(out var tracker);

            go.AddMissingComponent<DACloneObjCtr>(out var cloneObj);
            cloneObj.interactObj = interactObj;
            cloneObj.collisionTracker = tracker;

            if (go.AddMissingComponent<PanelSpawnTooltipTrigger>(out var tooltipTrigger))
            {
                tooltipTrigger.enableTooltipOnFocus = true;

                tooltipTrigger.enableTooltipOnTouch = true;

                tooltipTrigger.followMarkerPoint = false;
            }
        }

        [MenuItem("Tools/VRTK Interact component tools/删除指定VrClone物体的交互脚本")]
        static void ClearSelectGoInteractComponentsVrClone()
        {
            if (Selection.transforms.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个或者多个物体", "确定");
                return;
            }

            foreach (var t in Selection.transforms)
            {
                t.DestoryComponents(
                    typeof(VRTK_InteractableObject),
                    typeof(VRTK_InteractableListener),
                    typeof(VRTK_BaseHighlighter),
                    typeof(HighlightPlus.HighlightEffect),
                    typeof(VRTK_BaseGrabAttach),
                    typeof(HandToolCollisionTracker),
                    typeof(Rigidbody),
                    typeof(DACloneObjCtr),
                    typeof(PanelSpawnTooltipTrigger)
                    );

                Debug.Log($"清理:{t.name}");
            }
        }

        #endregion

    }
}