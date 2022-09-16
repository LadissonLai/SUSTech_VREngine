using Fxb.CMSVR;
using UnityEditor;
using UnityEngine;

namespace Fxb.CMSVREditor
{
    [CustomEditor(typeof(WrenchCombineCtr), true)]
    public class WrenchCombineEditor : Editor
    {
        //CsvSerializer.Serialize<TConfig, TItem>(Resources.Load<TextAsset>(csvPath).text, 1)
        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();
            
        //    var wrenchCombineCtr = (WrenchCombineCtr)target;

        //    var wrenchCtr = wrenchCombineCtr.GetComponent<VRAnimWrenchCtr>();

        //    //Debug.Log(serializedObject);
 
        //}
    }
}
