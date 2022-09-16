using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleModelImportHelper : AssetPostprocessor
{
    /// <summary>
    /// 文件名E结尾跳过转换
    /// </summary>
    void OnPreprocessModel()
    {
        if (!IsFirstImport())
            return;

        ModelImporter importer = (ModelImporter)assetImporter;
   
        string assetPath = importer.assetPath;

        int speratorIndex = assetPath.LastIndexOf('.');

        string assetMark = assetPath.Substring(speratorIndex - 1, 1);

        string assetType = assetPath.Substring(speratorIndex + 1).ToLower();

        Debug.Log($"assetMark:{assetMark} assetType:{assetType}");

        switch (assetType)
        {
            case "fbx":
                if (assetMark == "E") //Embedded
                    return;

                importer.materialLocation = ModelImporterMaterialLocation.External;

                importer.materialName = ModelImporterMaterialName.BasedOnMaterialName;

                break;
            default:
                break;
        }

    }

    void OnPostprocessModel(GameObject g)
    {
        if (!IsFirstImport())
            return;

        ModelImporter importer = (ModelImporter)assetImporter;

        importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
    }

    /// <summary>
    /// 临时逻辑 缺少meta意味着首次导入此模型
    /// </summary>
    /// <returns></returns>
    bool IsFirstImport()
    {
        ModelImporter importer = (ModelImporter)assetImporter;

        return importer.importSettingsMissing;
    }

    //void OnPostprocessMaterial(Material material)
    //{
    //    Debug.Log($"mtr:{material.name}");

    //    material.shader = Shader.Find("Unlit/Color");
    //}

    //void OnPostprocessGameObjectWithUserProperties(
    //GameObject go,
    //string[] propNames,
    //System.Object[] values)
    //{
    //    for (int i = 0; i < propNames.Length; i++)
    //    {
    //        string propName = propNames[i];
    //        System.Object value = (System.Object)values[i];

    //        Debug.Log("Propname: " + propName + " value: " + values[i]);

    //        if (value.GetType().ToString() == "System.Int32")
    //        {
    //            int myInt = (int)value;
    //            // do something useful
    //        }

    //        // etc...
    //    }
    //}

    //Material OnAssignMaterialModel(Material mtr, Renderer rt)
    //{
    //    //ModelImporter importer = (ModelImporter)assetImporter;
    //    //mtr = Resources.Load<Material>("Test");

    //    mtr.shader = Shader.Find("Unlit/Color");

    //    Debug.Log($"mtr:{mtr.name}");

    //    Debug.Log($"mtr:{rt.name}");

    //    return mtr;
    //}


}
