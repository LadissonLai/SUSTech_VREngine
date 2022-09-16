using UnityEditor;
using UnityEngine;
using Framework;
using Fxb.CMSVR;

namespace Fxb.CMSVREditor
{
    [CustomEditor(typeof(DAObjCtr), true)]
    public class DAObjEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var daObj = (DAObjCtr)target;

            if(daObj.autoDisappear && daObj.dropGridPlane == null)
            {
                if (GUILayout.Button("创建网格平面"))
                {
                    var plane = new GameObject("GridPlane").AddComponent<DAGridPlane>();

                    plane.transform.SetParent(daObj.transform);

                    plane.transform.ResetLocalMatrix();

                    plane.gridSize = 0.02f;

                    daObj.dropGridPlane = plane;

                    EditorUtility.SetDirty(daObj);
                }
            }
        }
    }
}

 