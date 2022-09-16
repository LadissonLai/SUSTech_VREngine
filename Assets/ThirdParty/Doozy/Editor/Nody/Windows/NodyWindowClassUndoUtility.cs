// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable SuggestBaseTypeForParameter

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private UndoUtility m_undo = new UndoUtility();
        private List<Object> m_undoObjectList;

        /// <summary> Records undo action with the given message </summary>
        /// <param name="message"> Undo message </param>
        private void RecordUndo(string message)
        {
            if (m_undo == null) m_undo = new UndoUtility();           //init the UndoUtility if found null
            m_undoObjectList = new List<Object> {this, CurrentGraph}; //add the graph to the list
            if (CurrentGraph != null && m_nodesDatabase != null)
                foreach (Node node in NodesDatabase.Values)
                    m_undoObjectList.Add(node);

            m_undo.RecordUndo(this, new List<Object>(m_undoObjectList), message); //add the data to the UndoUtility for referencing and management
        }

        /// <summary> Triggered when an UndoRedo action has been performed </summary>
        private void UndoRedoPerformed()
        {
//            if (CurrentGraph != null)
//            {
//                //if any node that has been soft deleted has been bought back via the undo / redo operation -> we need to remove it from the soft delete database
//                foreach (Node node in CurrentGraph.Nodes) //look through the node references list that the graph has
//                    if (EditorSettings.DeletedNodes.Contains(node)) //check if any node id, that is currently referenced, if it's also in the soft delete list (the one recovered from editor prefs)
//                    {
//                        EditorSettings.DeletedNodes.Remove(node); //if a nose was found -> remove it from the soft delete nodes list in order to prevent the node deletion when entering play mode, or closing the graph
//                        EditorSettings.SetDirty(false);
//                    }
//
//                CurrentGraph.SetGraphDirty(); //mark graph dirty
//            }

            ValidateDatabases();                                    //makes sure there are no invalid entries in the databases
            ConstructGraphGUI();                                    //recalculates the GUI
            m_initialDragNodePositions.Clear();                     //clears any currently dragged nodes
            UpdateNodesSelectedState(WindowSettings.SelectedNodes); //updates all the nodes selected state
            GraphEvent.Send(GraphEvent.EventType.EVENT_UNDO_REDO_PERFORMED);
            Repaint(); //does a quick repaint
        }

        /// <summary>  Helper class used to save graph and nodes states for undo/redo procedures </summary>
        private class UndoUtility
        {
            /// <summary> All the cached objects (undo/redo states) </summary>
            private Object[] m_cachedUndoObjects;

            /// <summary> All the objects that were modified </summary>
            private readonly List<Object> m_objects;

            /// <summary> Number of recorded undo steps </summary>
            private int m_undoObjectsCount;

            public UndoUtility()
            {
                m_objects = new List<Object>();
                m_undoObjectsCount = 0;
            }

            /// <summary> Clear all the undo steps and the undo cache </summary>
            public void Clear()
            {
                m_objects.Clear();
                m_undoObjectsCount = 0;
                m_cachedUndoObjects = null;
            }

            /// <summary> Record an action that can be reversed </summary>
            /// <param name="window"> Source window </param>
            /// <param name="objects"> What objects need to be saved for undo purposes </param>
            /// <param name="message"> Undo message describing the action that can be reversed if performing this undo </param>
            public void RecordUndo(NodyWindow window, List<Object> objects, string message)
            {
                if (m_cachedUndoObjects == null ||                                                 //check that the cache is not null
                    m_undoObjectsCount != (objects == null ? 0 : objects.Count) ||                 //check the undo steps
                    ArrayUtility.Contains(m_cachedUndoObjects, null))                              //make sure this step is not null
                    UpdateUndoCacheObject(window, objects);                                        //add this step to the undo cache
                if (m_cachedUndoObjects != null) Undo.RecordObjects(m_cachedUndoObjects, message); //add this undo to the UnityEditor
            }

            /// <summary> Add and undo step to the undo cache </summary>
            /// <param name="window"> Source window </param>
            /// <param name="objects"> What objects need to be saved for undo purposes </param>
            private void UpdateUndoCacheObject(NodyWindow window, List<Object> objects)
            {
                m_objects.Clear();                         //clear any previous entries in the objects list
                if (window != null) m_objects.Add(window); //sanity check and add this window to the cache
                m_objects.Add(WindowSettings);             //add the editor settings to the undo cache
                if (objects != null)                       //check that at lease one object was sent with this undo step
                    foreach (Object obj in objects)        //add all the sent objects (that need to be saved) to the objects list
                        if (obj != null)
                            m_objects.Add(obj);

                m_cachedUndoObjects = m_objects.ToArray();                //update the undo cache
                m_undoObjectsCount = objects == null ? 0 : objects.Count; //update the undo steps count
            }
        }
    }
}