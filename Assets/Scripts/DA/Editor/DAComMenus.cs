using Fxb.CsvConfig;
using UnityEditor;
using UnityEngine;

namespace Fxb.CMSVR
{
    //好像没有什么鸟用
    public static class DAComMenus
    {
        //public static DACsvConfig daConfig;

        //private static void UpdateConfig()
        //{
        //    if (daConfig == null)
        //    {
        //        daConfig = CsvSerializer.Serialize<DACsvConfig, DACsvConfig.Item>(Resources.Load<TextAsset>(PathConfig.CONFIG_DA).text, 1);
        //    }
        //}

        //[MenuItem("GameObject/DATools/更新ModelGroup", priority = 0)]
        //public static void UpdateObjDAModelGroupComponents()
        //{
        //    UpdateSelectObjDAComponents<VRDaModelGroupCtr>();
        //}

        //[MenuItem("GameObject/DATools/更新parts", priority = 0)]
        //public static void UpdateObjDAPartsComponents()
        //{
        //    UpdateSelectObjDAComponents<VRDaPartsCtr>();
        //}

        //[MenuItem("GameObject/DATools/更新Screws", priority = 0)]
        //public static void UpdateObjDAScrewsComponents()
        //{
        //    UpdateSelectObjDAComponents<VRDAScrew>();
        //}

        //[MenuItem("GameObject/DATools/更新Clasp", priority = 0)]
        //public static void UpdateObjDAClaspsComponents()
        //{
        //    UpdateSelectObjDAComponents<VRDAClaspCtr>();
        //}

        //[MenuItem("GameObject/DATools/刷新配置信息", priority = 0)]
        //public static void ClearConfigs()
        //{
        //    daConfig = null;

        //    Debug.Log("清空配置信息");
        //}

        //public static void UpdateSelectObjDAComponents<T>() where T : DAObjCtr
        //{
        //    if (Selection.transforms.Length == 0)
        //    {
        //        Debug.LogWarning("未选择物体");
        //        return;
        //    }

        //    UpdateConfig();

        //    foreach (var t in Selection.transforms)
        //    {
        //        //FocusInteractComHelpMenus.AddInteractableComponents(t.gameObject);

        //        UpdateDAComponents<T>(t.gameObject);
        //    }
        //}

        //public static void UpdateDAComponents<T>(GameObject go) where T : DAObjCtr
        //{
        //    if (!go.TryGetComponent<DAObjCtr>(out var daObjCtr))
        //    {
        //        daObjCtr = go.AddComponent<T>();
        //    }

        //    var rd = daConfig.FindRDByModelName(go.name);

        //    if (rd == null)
        //        return;

        //    //daObjCtr.id = rd.Id;

        //    EditorUtility.SetDirty(daObjCtr);
        //}
    }
}


