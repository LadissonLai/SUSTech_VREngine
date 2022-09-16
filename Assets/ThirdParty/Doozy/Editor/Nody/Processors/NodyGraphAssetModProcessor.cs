// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Nody.Processors
{
    /// <summary>
    ///     Deals with modified assets
    /// </summary>
    public class NodyGraphAssetModProcessor : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        ///     Automatically delete Node sub-assets before deleting their script.
        ///     <para />
        ///     This is important to do, because you can't delete null sub assets.
        /// </summary>
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            //get the object that is requested for deletion
            var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            // If we aren't deleting a script, return
            if (!(loadAssetAtPath is MonoScript)) return AssetDeleteResult.DidNotDelete;

            // Check script type. Return if deleting a non-node script
            var script = loadAssetAtPath as MonoScript;
            Type scriptType = script.GetClass();
            if (scriptType == null || scriptType != typeof(Node) && !scriptType.IsSubclassOf(typeof(Node))) return AssetDeleteResult.DidNotDelete;

            // Find all Graph ScriptableObjects
            string[] graphGUIDs = AssetDatabase.FindAssets("t:" + typeof(Graph));
            List<Graph> graphs = graphGUIDs.Select(t => AssetDatabase.LoadAssetAtPath<Graph>(AssetDatabase.GUIDToAssetPath(t))).Where(graph => graph != null).ToList();

            // Find all ScriptableObjects using this script
            string[] nodeGUIDs = AssetDatabase.FindAssets("t:" + scriptType);
            foreach (string nodeGUID in nodeGUIDs)
            {
                string assetpath = AssetDatabase.GUIDToAssetPath(nodeGUID);
                Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetpath);
                foreach (Object obj in objs)
                {
                    var node = obj as Node;
                    if (node != null && (node.GetType() != scriptType || node == null)) continue;
                    // Delete the node and notify the user
                    if (node == null) continue;
                    //find the graph this node belongs to
                    Graph parentGraph = null;
                    foreach (Graph graph in graphs)
                    {
                        if (graph.Nodes == null) continue;
                        if (!graph.Nodes.Contains(node)) continue;
                        parentGraph = graph;
                        break;
                    }

                    if (parentGraph == null) continue;
                    UnityEngine.Debug.LogWarning(node.name + " of " + parentGraph + " depended on deleted script and has been removed automatically.", parentGraph);
                    parentGraph.Nodes.Remove(node);
                }
            }

            // We didn't actually delete the script. Tell the internal system to carry on with normal deletion procedure
            return AssetDeleteResult.DidNotDelete;
        }

        /// <summary> Automatically re-add loose node assets to the Graph node list </summary>
        [InitializeOnLoadMethod]
        private static void OnReloadEditor()
        {
            // Find all Graph assets
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(Graph));
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var graph = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Graph)) as Graph;
                if (graph == null) continue;
                graph.Nodes.RemoveAll(x => x == null); //Remove null items
                Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                // Ensure that all sub node assets are present in the graph node list
                foreach (Object obj in objects)
                    if (!graph.Nodes.Contains(obj as Node))
                        graph.Nodes.Add(obj as Node);
            }
        }
    }
}