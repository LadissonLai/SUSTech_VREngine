using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework;


public static class CombineToolWithBox
{
    [MenuItem("Tools/MountToolWithTarget")]
    public static void Mount()
    {
        string toolName = "CombineWrenchParts";

        var tool = GameObject.Find(toolName);

        if (tool == null)
        {
            Debug.LogError($"未找到{toolName}");

            return;
        }

        string targetName = "GJX_ALL_V1 Variant";

        var target = GameObject.Find(targetName);

        if (target == null)
        {
            Debug.LogError($"未找到{targetName}");

            return;
        }

        for (int i = 0; i < tool.transform.childCount; i++)
        {
            var toolChild = tool.transform.GetChild(i);

            if (toolChild.gameObject.activeInHierarchy)
            {
                var result = FindChild(target.transform, toolChild.name);

                if (result != null)
                {
                    toolChild.ResetLocalMatrix();

                    toolChild.transform.position = result.position;

                    toolChild.transform.rotation = result.rotation;

                    Debug.Log($"{toolChild.name}Done");
                }
                else
                    Debug.Log($"{toolChild.name}Not Found");
            }
        }
    }


    static Transform FindChild(Transform parent, string name)
    {
        var child = parent.Find(name);

        if (child)
            return child;

        for (int i = 0; i < parent.childCount; i++)
        {
            child = FindChild(parent.GetChild(i), name);

            if (child)
                return child;
        }

        return null;
    }
}