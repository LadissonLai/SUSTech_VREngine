// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using Doozy.Engine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Utils
        {
            /// <summary> C#'s Script Icon [The one MonoBehaviour Scripts have] </summary>
            public static readonly Texture2D ScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            /// <summary> Returns an Array of all Objects of type T for all Assets in a folder, using the .NET File APIs </summary>
            public static T[] GetAtPath<T>(string path)
            {
                path = path.Replace("Assets/", "");
                var al = new ArrayList();
                string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
                foreach (string fileName in fileEntries)
                {
                    string localPath = "Assets/" + path + "/" + fileName.Replace(Application.dataPath + "/" + path, "").Replace(@"\", "");
                    Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
                    if (t != null)
                        al.Add(t);
                }

                var result = new T[al.Count];
                for (int i = 0; i < al.Count; i++)
                    result[i] = (T) al[i];

                return result;
            }

            public static bool GetAttrib<T>(Type classType, out T attribOut) where T : Attribute
            {
                object[] attribs = classType.GetCustomAttributes(typeof(T), false);
                return GetAttrib(attribs, out attribOut);
            }

            public static bool GetAttrib<T>(object[] attribs, out T attribOut) where T : Attribute
            {
                foreach (object attribute in attribs)
                    if (attribute.GetType() == typeof(T))
                    {
                        attribOut = attribute as T;
                        return true;
                    }

                attribOut = null;
                return false;
            }

            public static bool GetAttrib<T>(Type classType, string fieldName, out T attribOut) where T : Attribute
            {
                object[] attribs = classType.GetField(fieldName).GetCustomAttributes(typeof(T), false);
                return GetAttrib(attribs, out attribOut);
            }

            public static bool HasAttrib<T>(object[] attribs) where T : Attribute { return attribs.Any(t => t.GetType() == typeof(T)); }

            public static void CreateFromTemplate(string initialName, string templatePath)
            {
                ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                    0,
                    ScriptableObject.CreateInstance<DoCreateCodeFile>(),
                    initialName,
                    ScriptIcon,
                    templatePath
                );
            }

            /// <summary> Creates Script from Template's path </summary>
            public static Object CreateScript(string pathName, string templatePath)
            {
                if (pathName != null)
                {
                    string className = Path.GetFileNameWithoutExtension(pathName).Replace(" ", string.Empty);

                    var encoding = new UTF8Encoding(true, false);

                    if (File.Exists(templatePath))
                    {
                        // Read procedures.
                        var reader = new StreamReader(templatePath);
                        string templateText = reader.ReadToEnd();
                        reader.Close();

                        templateText = templateText.Replace("#SCRIPTNAME#", className);
                        templateText = templateText.Replace("#NOTRIM#", string.Empty);
                        // You can replace as many tags you make on your templates, just repeat Replace function
                        // e.g.:
                        // templateText = templateText.Replace("#NEWTAG#", "MyText");

                        // Write procedures.
                        var writer = new StreamWriter(Path.GetFullPath(pathName), false, encoding);
                        writer.Write(templateText);
                        writer.Close();

                        AssetDatabase.ImportAsset(pathName);
                        return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
                    }
                }

                DDebug.LogError(string.Format("The template file was not found: {0}", templatePath));
                return null;
            }

            /// <summary> Generates an enum by writing a new file at the specified path </summary>
            /// <param name="enumName"> SocketName of the enum </param>
            /// <param name="enumEntries"> The enum entries </param>
            /// <param name="filePath"> The file path (without the file name). The path is expected to exist. MyProject/MyEnums/ </param>
            /// <param name="enumNamespace"> The enum namespace </param>
            /// <param name="addCopyright"></param>
            /// <param name="autoSelectAfterCreation"></param>
            public static void GenerateEnum(string enumName, string[] enumEntries, string filePath, string enumNamespace,
                                            bool addCopyright = true, bool autoSelectAfterCreation = false)
            {
                if (string.IsNullOrEmpty(enumName))
                {
                    DDebug.Log("Cannot generate and enum. Enum name is empty.");
                    return;
                }

                if (enumEntries == null)
                {
                    DDebug.Log("Cannot generate and enum. Enum entries array is null.");
                    return;
                }

                if (enumEntries.Length == 0)
                {
                    DDebug.Log("Cannot generate and enum. Enum entries array is empty.");
                    return;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    DDebug.Log("Cannot generate and enum. File path is empty.");
                    return;
                }

                if (string.IsNullOrEmpty(enumName))
                {
                    DDebug.Log("Cannot generate and enum. Enum namespace is empty.");
                    return;
                }

                string filePathAndName = filePath + enumName + ".cs";
                using (var streamWriter = new StreamWriter(filePathAndName))
                {
                    if (addCopyright)
                    {
                        streamWriter.WriteLine(Copyright.LINE_ONE);
                        streamWriter.WriteLine(Copyright.LINE_TWO);
                        streamWriter.WriteLine(Copyright.LINE_THREE);
                        streamWriter.WriteLine("");
                    }

                    streamWriter.WriteLine("// --- Generated Enum ---");
                    streamWriter.WriteLine("");
                    if (!string.IsNullOrEmpty(enumNamespace))
                    {
                        streamWriter.WriteLine("namespace " + enumNamespace);
                        streamWriter.WriteLine("{");
                    }

                    streamWriter.WriteLine("    public enum " + enumName);
                    streamWriter.WriteLine("    {");
                    foreach (string entry in enumEntries)
                    {
                        if (string.IsNullOrEmpty(entry))
                        {
                            DDebug.Log("While generating the " + enumName + " enum, an empty entry was found.");
                            continue;
                        }

                        streamWriter.WriteLine("        " + entry + ",");
                    }

                    streamWriter.WriteLine("    }");
                    if (!string.IsNullOrEmpty(enumNamespace)) streamWriter.WriteLine("}");
                }

                if (autoSelectAfterCreation)
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filePathAndName);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }

                DDebug.Log("Finished - generating the '" + enumName + "' enum in the '" + enumNamespace +
                          "' namespace, at '" + filePathAndName + "'");
            }

            /// <summary> Generates an enum by writing a new file at the specified path </summary>
            /// <param name="enumName"> SocketName of the enum </param>
            /// <param name="enumEntries"> The enum entries </param>
            /// <param name="filePath"> The file path (without the file name). The path is expected to exist. MyProject/MyEnums/ </param>
            /// <param name="enumNamespace"> The enum namespace </param>
            /// <param name="resourcesClassName"></param>
            /// <param name="additionalMethods"></param>
            /// <param name="addCopyright"></param>
            /// <param name="autoSelectAfterCreation"></param>
            public static void GenerateEnumAndAddToResources(string enumName, string[] enumEntries, string filePath,
                                                             string enumNamespace, string resourcesClassName, string[] additionalMethods = null,
                                                             bool addCopyright = true, bool autoSelectAfterCreation = false)
            {
                if (string.IsNullOrEmpty(enumName))
                {
                    DDebug.Log("Cannot generate and enum. Enum name is empty.");
                    return;
                }

                if (enumEntries == null)
                {
                    DDebug.Log("Cannot generate and enum. Enum entries array is null.");
                    return;
                }

                if (enumEntries.Length == 0)
                {
                    DDebug.Log("Cannot generate and enum. Enum entries array is empty.");
                    return;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    DDebug.Log("Cannot generate and enum. File path is empty.");
                    return;
                }

                if (string.IsNullOrEmpty(enumName))
                {
                    DDebug.Log("Cannot generate and enum. Enum namespace is empty.");
                    return;
                }

                string filePathAndName = filePath + enumName + ".cs";

                using (var streamWriter = new StreamWriter(filePathAndName))
                {
                    if (addCopyright)
                    {
                        streamWriter.WriteLine(Copyright.LINE_ONE);
                        streamWriter.WriteLine(Copyright.LINE_TWO);
                        streamWriter.WriteLine(Copyright.LINE_THREE);
                        streamWriter.WriteLine("");
                    }

                    streamWriter.WriteLine("// --- Generated Enum ---");
                    streamWriter.WriteLine("");
                    if (!string.IsNullOrEmpty(enumNamespace))
                    {
                        streamWriter.WriteLine("namespace " + enumNamespace);
                        streamWriter.WriteLine("{");
                    }

                    streamWriter.WriteLine("    public enum " + enumName);
                    streamWriter.WriteLine("    {");
                    foreach (string entry in enumEntries)
                    {
                        if (string.IsNullOrEmpty(entry))
                        {
                            DDebug.Log("While generating the " + enumName + " enum, an empty entry was found.");
                            continue;
                        }

                        streamWriter.WriteLine("        " + entry + ",");
                    }

                    streamWriter.WriteLine("    }");
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("    public partial class " + resourcesClassName);
                    streamWriter.WriteLine("    {");
                    streamWriter.WriteLine("        public static UnityEngine.GUIStyle GetStyle(" + enumName +
                                           " styleName) { return Skin.GetStyle(styleName.ToString()); }");
                    if (additionalMethods != null)
                    {
                        streamWriter.WriteLine("");
                        for (int i = 0; i < additionalMethods.Length; i++)
                            streamWriter.WriteLine("        " + additionalMethods[i]);
                    }

                    streamWriter.WriteLine("    }");
                    if (!string.IsNullOrEmpty(enumNamespace)) streamWriter.WriteLine("}");
                }

                if (autoSelectAfterCreation)
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filePathAndName);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }

                DDebug.Log("Created the '" + enumName + "' enum in the '" + enumNamespace + "' namespace, at '" +
                          filePathAndName + "'");
            }

            /// Inherits from EndNameAction, must override EndNameAction.Action
            public class DoCreateCodeFile : EndNameEditAction
            {
                public override void Action(int instanceId, string pathName, string resourceFile)
                {
                    Object o = CreateScript(pathName, resourceFile);
                    ProjectWindowUtil.ShowCreatedAsset(o);
                }
            }
        }
    }
}