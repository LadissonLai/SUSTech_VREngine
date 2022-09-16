// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace Doozy.Engine.Utils
{
    public static class AssetUtils
    {
        /// <summary>
        /// Returns a reference to a scriptable object of type T with the given fileName at the relative resourcesPath.
        /// <para/> If the asset is not found, one will get created automatically (in the Editor only) 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="resourcesPath"></param>
        /// <param name="saveAssetDatabase"></param>
        /// <param name="refreshAssetDatabase"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetScriptableObject<T>(string fileName,
                                               string resourcesPath,
                                               bool saveAssetDatabase,
                                               bool refreshAssetDatabase)
            where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(resourcesPath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            // ReSharper disable once SuspiciousTypeConversion.Global
//            if (!resourcesPath[resourcesPath.Length - 1].Equals(@"\")) resourcesPath += @"\";
//            resourcesPath = resourcesPath.Replace(@"\", "/");
            resourcesPath = CleanPath(resourcesPath);

            var obj = (T) Resources.Load(fileName, typeof(T));

            if (obj == null)
            {
                string simpleResourcesPath = resourcesPath.Replace(resourcesPath.Substring(0, resourcesPath.LastIndexOf("Resources", StringComparison.Ordinal)), "");
                simpleResourcesPath = simpleResourcesPath.Replace("Resources", "").Remove(0, 1);
                obj = (T) Resources.Load(Path.Combine(simpleResourcesPath, fileName), typeof(T));
            }

#if UNITY_EDITOR
            if (obj != null) return obj;
            obj = CreateAsset<T>(resourcesPath, fileName, ".asset", saveAssetDatabase, refreshAssetDatabase);
#endif
            return obj;
        }

        public static T GetResource<T>(string resourcesPath, string fileName) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(resourcesPath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            resourcesPath = CleanPath(resourcesPath);
            // ReSharper disable once SuspiciousTypeConversion.Global
//            if (!resourcesPath[resourcesPath.Length - 1].Equals(@"\")) resourcesPath += @"\";
//            resourcesPath = resourcesPath.Replace(@"\", "/");

            return (T) Resources.Load(resourcesPath + fileName, typeof(T));
        }

        public static string CleanPath(string path)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!path[path.Length - 1].Equals(@"\")) path += @"\";
            path = path.Replace(@"\\", @"\");
            path = path.Replace(@"\", "/");
            return path;
        }

#if UNITY_EDITOR
        public static T CreateAsset<T>(string relativePath,
                                       string fileName,
                                       string extension = ".asset",
                                       bool saveAssetDatabase = true,
                                       bool refreshAssetDatabase = true)
            where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            relativePath = CleanPath(relativePath);
            // ReSharper disable once SuspiciousTypeConversion.Global
//            if (!relativePath[relativePath.Length - 1].Equals(@"\")) relativePath += @"\";
//            relativePath = relativePath.Replace(@"\\", @"\");
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, relativePath + fileName + extension);
            EditorUtility.SetDirty(asset);
            if (saveAssetDatabase) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return asset;
        }

        public static List<T> GetAssets<T>() where T : ScriptableObject
        {
            var list = new List<T>();
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            foreach (string guid in guids)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset == null) continue;
                list.Add(asset);
            }

            return list;
        }
        
        public static void MoveAssetToTrash(string relativePath, string fileName, bool saveAssetDatabase = true,
                                            bool refreshAssetDatabase = true, bool printDebugMessage = true)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            if (string.IsNullOrEmpty(fileName)) return;
            // ReSharper disable once SuspiciousTypeConversion.Global
//            if (!relativePath[relativePath.Length - 1].Equals(@"\")) relativePath += @"\";
            relativePath = CleanPath(relativePath);
            if (!AssetDatabase.MoveAssetToTrash(relativePath + fileName + ".asset")) return;
            if (printDebugMessage) DDebug.Log("The " + fileName + ".asset file has been moved to trash.");
            if (saveAssetDatabase) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
        }

        public static Texture GetTexture(string filePath, string fileName, string fileExtension = ".png")
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            // ReSharper disable once SuspiciousTypeConversion.Global
//            if (!filePath[filePath.Length - 1].Equals(@"\")) filePath += @"\";
            filePath = CleanPath(filePath);
            return AssetDatabase.LoadAssetAtPath<Texture>(filePath + fileName + fileExtension);
        }

        public static Texture2D GetTexture2D(string filePath, string fileName, string fileExtension = ".png")
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            // ReSharper disable once SuspiciousTypeConversion.Global
//            if (!filePath[filePath.Length - 1].Equals(@"\")) filePath += @"\";
            filePath = CleanPath(filePath);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(filePath + fileName + fileExtension);
        }
#endif
    }
}