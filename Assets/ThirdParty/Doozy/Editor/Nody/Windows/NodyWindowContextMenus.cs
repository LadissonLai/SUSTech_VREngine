// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms
// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Doozy.Editor.Nody.Utils;
using Doozy.Editor.Windows;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        /// <summary> Show a context menu over a node. Shown when the developer right clicks over a node's header, footer or any parts of its body, except the sockets, the connection points and any other buttons </summary>
        /// <param name="node"> Target node </param>
        private void ShowNodeContextMenu(Node node)
        {
            var menu = new GenericMenu(); //create a generic menu

            if (EditorApplication.isPlaying)
            {
                menu.AddItem(new GUIContent(UILabels.SetActiveNode), false, () => { CurrentGraph.SetActiveNode(node); }); //Set Active Node
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent(UILabels.Center), false, () => { CenterSelectedNodesInWindow(); }); //Center Node
            menu.AddSeparator("");
            menu.AddItem(new GUIContent(UILabels.Disconnect), false, () =>
                                                                     {
                                                                         if (WindowSettings.SelectedNodes.Count > 0)
//                if (ActiveSelection.Nodes.Count > 0)
                                                                             ClearNodesConnections(WindowSettings.SelectedNodes, true); //Disconnect all selected nodes
                                                                         else
                                                                             ClearNodeConnections(node, true); //Disconnect Node
                                                                     });
            menu.AddSeparator("");
            var copyNode = new GUIContent(UILabels.Copy);
            var deleteNode = new GUIContent(UILabels.Delete);
            if (node.CanBeDeleted)
            {
                menu.AddItem(copyNode, false, () => { ExecuteGraphAction(GraphAction.Copy); });          //Copy Node
                menu.AddItem(deleteNode, false, () => { ExecuteGraphAction(GraphAction.DeleteNodes); }); //Delete Node
            }
            else
            {
                menu.AddDisabledItem(copyNode);   //Copy Node label
                menu.AddDisabledItem(deleteNode); //Delete Node label
            }

            menu.ShowAsContext(); //show menu at mouse position
        }

        /// <summary> Show a context menu over a socket. Shown when the developer right clicks over a socket </summary>
        /// <param name="socket"> Target socket that the virtual point belongs to </param>
        private void ShowSocketContextMenu(Socket socket)
        {
            var menu = new GenericMenu(); //create a generic menu
            if (socket.CanBeReordered)
            {
                menu.AddItem(new GUIContent(UILabels.MoveUp), false, () => { ReorderSocketMove(socket, true, true); });    //Reorder Socket - Move Up
                menu.AddItem(new GUIContent(UILabels.MoveDown), false, () => { ReorderSocketMove(socket, false, true); }); //Reorder Socket - Move Up
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent(UILabels.Disconnect), false, () => { DisconnectSocket(socket, true); }); //Disconnect Socket
            if (NodesDatabase[socket.NodeId].CanDeleteSocket(socket))
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent(UILabels.Delete), false, () => { RemoveSocket(socket, true); }); //Delete Socket
            }

            menu.ShowAsContext(); //show menu at mouse position
        }

        /// <summary> Show a context menu over a virtual point (connection point). Shown when the developer right clicks over a connection point </summary>
        /// <param name="virtualPoint"> Target virtual point </param>
        private void ShowConnectionPointContextMenu(VirtualPoint virtualPoint)
        {
            var menu = new GenericMenu();                                                                                    //create a generic menu
            menu.AddItem(new GUIContent(UILabels.Disconnect), false, () => { DisconnectVirtualPoint(virtualPoint, true); }); //Disconnect Connection Point
            menu.ShowAsContext();                                                                                            //show menu at mouse position
        }

        private class NodeMenuItem
        {
            public readonly NodeMenu Menu;
            public readonly Type NodeType;

            public NodeMenuItem(NodeMenu menu, Type nodeType)
            {
                Menu = menu;
                NodeType = nodeType;
            }
        }

        /// <summary> Show a context menu for the graph. Shown when the developer right clicks over the grid area </summary>
        private void ShowGraphContextMenu()
        {
            var menu = new GenericMenu(); //create a generic menu

            var nodeMenus = new List<NodeMenuItem>();

            foreach (Type type in NodeTypes)
            {
                NodeMenu nodeMenu;
                string path = GetNodeMenuName(type, out nodeMenu); //Get node context menu path
                if (string.IsNullOrEmpty(path)) continue;          //empty entry means that the node will be hidden from the menu
                nodeMenus.Add(new NodeMenuItem(nodeMenu, type));
            }

            nodeMenus = nodeMenus.OrderBy(n => n.Menu.Order).ThenBy(o => o.Menu.MenuName).ToList();

            string nodeRootPath = UILabels.CreateNode + "/";
            foreach (NodeMenuItem nodeMenuItem in nodeMenus)
            {
                NodeMenuItem menuItem = nodeMenuItem;
                if (menuItem.Menu.AddSeparatorBefore) menu.AddSeparator(nodeRootPath);
                menu.AddItem(new GUIContent(nodeRootPath + menuItem.Menu.MenuName), false, () => ExecuteGraphAction(GraphAction.CreateNode, menuItem.NodeType.AssemblyQualifiedName)); //Create Node
                if (menuItem.Menu.AddSeparatorAfter) menu.AddSeparator(nodeRootPath);
            }


//            foreach (Type type in NodeTypes)
//            {
//                NodeMenu nodeMenu;
//                string path = GetNodeMenuName(type, out nodeMenu); //Get node context menu path
//                if (string.IsNullOrEmpty(path)) continue; //empty entry means that the node will be hidden from the menu
//                Type nodeType = type;
//                menu.AddItem(new GUIContent(UILabels.Create + "/" + path), false, () => ExecuteGraphAction(GraphAction.CreateNode, nodeType.AssemblyQualifiedName)); //Create Node
//            }

            menu.AddSeparator("");
            var pasteNode = new GUIContent(UILabels.Paste);
            if (WindowSettings.CanPasteNodes)
                menu.AddItem(pasteNode, false, () => { ExecuteGraphAction(GraphAction.Paste); }); //Paste Node
            else
                menu.AddDisabledItem(pasteNode); //Paste Node label

            menu.AddSeparator("");
            if (WindowSettings.SelectedNodes.Count > 0)
            {
                menu.AddItem(new GUIContent(UILabels.CenterSelectedNodes), false, () => { CenterSelectedNodesInWindow(); }); //Center Selected Nodes
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent(UILabels.Overview), false, () => { CenterAllNodesInWindow(); }); //Overview
            menu.AddItem(new GUIContent(UILabels.GoToStartNode), false, GoToStartOrEnterNode);           //Go to Start Node
            menu.AddSeparator("");
            AddCustomContextMenuItems(menu, CurrentGraph);                                                              //Add custom menu options from graph
            menu.AddItem(new GUIContent(UILabels.Settings), false, () => { DoozyWindow.Open(DoozyWindow.View.Nody); }); //Settings
            menu.ShowAsContext();                                                                                       //show menu at mouse position
        }

        /// <summary> Add the custom menu items from a given object to the target menu </summary>
        /// <param name="targetMenu">The generic menu where the items will get added</param>
        /// <param name="obj">The target object that adds the menu items</param>
        private static void AddCustomContextMenuItems(GenericMenu targetMenu, object obj)
        {
            KeyValuePair<ContextMenu, MethodInfo>[] items = GetContextMenuMethods(obj);
            if (items.Length == 0) return;
            foreach (KeyValuePair<ContextMenu, MethodInfo> kvp in items)
            {
                KeyValuePair<ContextMenu, MethodInfo> kvp1 = kvp;
                targetMenu.AddItem(new GUIContent(kvp.Key.menuItem), false, () => kvp1.Value.Invoke(obj, null));
            }

            targetMenu.AddSeparator("");
        }

        /// <summary> Returns context node menu path. Null or empty strings for hidden nodes </summary>
        private static string GetNodeMenuName(Type type, out NodeMenu nodeMenu)
        {
            //check if type has the NodeMenu attribute
            NodeMenu attribute;

            //if it has the attribute -> return the custom MenuName -> otherwise return the type as a namespace path
            string namespacePath = Doozy.Editor.Nody.Utils.ReflectionUtils.GetAttribute(type, out attribute) ? attribute.MenuName : ObjectNames.NicifyVariableName(type.ToString().Replace('.', '/'));
            nodeMenu = attribute;

            return namespacePath;
        }
    }
}