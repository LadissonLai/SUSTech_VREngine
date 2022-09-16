// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        [Serializable]
        public class SavedNodesIds
        {
            // ReSharper disable once InconsistentNaming
            /// <summary> Key used to save and restore values to and from EditorPrefs </summary>
            [SerializeField] private string m_editorPrefsKey;

            // ReSharper disable once InconsistentNaming
            /// <summary> List of node ids </summary>
            [SerializeField] private List<string> m_ids;

            /// <summary> Returns a list of all the node ids saved by this instance </summary>
            public List<string> Ids { get { return m_ids ?? (m_ids = new List<string>()); } }

            /// <summary>  The path for the loaded graph </summary>
            public string GraphAssetPath = string.Empty;

            /// <summary>  Returns true if at least one node has been added to the Ids list </summary>
            public bool IsEmpty { get { return Ids.Count == 0; } }

            /// <summary> Returns true if the given nodeId can be found in the Ids list </summary>
            public bool Contains(string nodeId) { return Ids.Contains(nodeId); }

            /// <summary> Initialize an instance of SavedNodeIds </summary>
            /// <param name="editorPrefsKey"> Key used to save and restore values to and from EditorPrefs </param>
            public SavedNodesIds(string editorPrefsKey)
            {
                m_editorPrefsKey = editorPrefsKey;
                RestoreFromEditorPrefs();
            }

            /// <summary> Add a node (only the node id) to the Ids list </summary>
            /// <param name="parentGraph"> The graph this node belongs to (needed to save the Asset path) </param>
            /// <param name="node"> The node being saved (needs only the node id) </param>
            public void Add(Graph parentGraph, Node node) { Add(parentGraph, node.Id, true); }

            /// <summary> Add a node id to the Ids list </summary>
            /// <param name="parentGraph"> The graph this node belongs to (needed to save the Asset path) </param>
            /// <param name="nodeId"> The node is being saved </param>
            /// <param name="saveToEditorPrefs"> If true, it will also save the Ids list to EditorPrefs. This bool is useful when adding a range if Ids at once so that we don't save after every node. </param>
            public void Add(Graph parentGraph, string nodeId, bool saveToEditorPrefs)
            {
                Ids.Add(nodeId); //add the node id to the ids list
                if (!saveToEditorPrefs) return;
                SaveToEditorPrefs(parentGraph);
            }

            private void SaveToEditorPrefs(Graph parentGraph)
            {
                string stringValue = AssetDatabase.GetAssetPath(parentGraph) + ScriptUtils.STRING_SEPARATOR; //add the graph path to the saved info
                foreach (string id in Ids) stringValue += id + ScriptUtils.STRING_SEPARATOR;                 //create the string that will be saved in editor prefs
                stringValue = stringValue.TrimEnd(ScriptUtils.STRING_SEPARATOR);                             //remove the last separator
                EditorPrefs.SetString(m_editorPrefsKey, stringValue);                                        //save the string
            }

            /// <summary> Add a range of node ids to the Ids list </summary>
            /// <param name="parentGraph"> The graph the nodes belong to (needed to save the asset path) </param>
            /// <param name="nodeIds"> The range of node ids being saved </param>
            public void AddRange(Graph parentGraph, List<string> nodeIds)
            {
                for (int i = 0; i < nodeIds.Count; i++) Add(parentGraph, nodeIds[i], i == nodeIds.Count - 1); //if it's the last entry, also save the Ids list to EditorPrefs
            }

            /// <summary> Remove the passed node id from the Ids list </summary>
            /// <param name="parentGraph"> The graph this node belongs to (needed to save the Asset path) </param>
            /// <param name="nodeId"> The node id being removed </param>
            /// <param name="saveToEditorPrefs"> If true, it will also save the Ids list to EditorPrefs. This bool is useful when adding a range if Ids at once so that we don't save after every node. </param>
            public void Remove(Graph parentGraph, string nodeId, bool saveToEditorPrefs)
            {
                Ids.Remove(nodeId);
                if (!saveToEditorPrefs) return;
                SaveToEditorPrefs(parentGraph);
            }

            /// <summary> Clear the Ids list and also the EditorPrefs entry </summary>
            public void Clear()
            {
                Ids.Clear();                                 //clear the current ids list
                EditorPrefs.SetString(m_editorPrefsKey, ""); //clear the saved entry in editor prefs
            }

            /// <summary> Restore any previously saved values (Ids list) from the EditorPrefs </summary>
            public void RestoreFromEditorPrefs()
            {
                string ids = EditorPrefs.GetString(m_editorPrefsKey, "");    //get all the ids (the first entry is the parent graph asset path)
                if (string.IsNullOrEmpty(ids)) return;                       //if nothing is there -> return
                string[] idsArray = ids.Split(ScriptUtils.STRING_SEPARATOR); //split the ids
                Ids.Clear();                                                 //clear the current ids list
                GraphAssetPath = idsArray[0];                                //set the graph asset path -> this is the first entry in the string value and it is the parent graph of the copied nodes
                ArrayUtility.RemoveAt(ref idsArray, 0);                      //remove the parent graph as now we want the node ids
                Ids.AddRange(idsArray);                                      //add the saved node ids to the ids list
            }
        }
    }
}