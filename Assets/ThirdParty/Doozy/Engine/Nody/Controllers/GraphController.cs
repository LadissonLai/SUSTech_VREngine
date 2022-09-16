// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Nody
{
    /// <summary>
    ///     Core component of Nody, the node graph engine module of DoozyUI.
    ///     It contains all the logic needed to manage a Nody Graph model.
    /// </summary>
    [AddComponentMenu(MenuUtils.GraphController_AddComponentMenu_MenuName, MenuUtils.GraphController_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.GRAPH_CONTROLLER)]
    public class GraphController : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.GraphController_MenuItem_ItemName, false, MenuUtils.GraphController_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Constants

        private const string DEFAULT_CONTROLLER_NAME = "";
        private const bool DEFAULT_DONT_DESTROY_CONTROLLER_ON_LOAD = true;

        #endregion

        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        /// <summary> Database used to keep track of all the GraphControllers </summary>
        public static readonly List<GraphController> Database = new List<GraphController>();

        #endregion

        #region Properties

        /// <summary> Reference to the Graph managed by this controller </summary>
        public Graph Graph
        {
            get
            {
                return m_graphModel;
//                return GraphModel;
//                if (m_graph != null) return m_graph;
//                m_graph = m_graphModel.Clone();
//                return m_graph;
            }
        }

        /// <summary> Reference to the Graph model managed by this controller </summary>
        public Graph GraphModel { get { return m_graphModel; } }


        /// <summary> Returns TRUE if this controller has been initialized </summary>
        public bool Initialized { get; private set; }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugGraphController; } }

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> The name of this GraphController (useful if you need to find it in the Database) </summary>
        public string ControllerName;

        /// <summary> Makes the GameObject, that this GraphController component is attached to, not to get destroyed on load (when the scene changes) </summary>
        public bool DontDestroyControllerOnLoad;

        #endregion

        #region Private Variables

#pragma warning disable 0649
        // ReSharper disable once InconsistentNaming
        [SerializeField] private Graph m_graphModel;
#pragma warning restore 0649

        private Graph m_graph;

        #endregion

        #region Constructors

        /// <summary> Creates a new instance for this class </summary>
        public GraphController() { Initialized = false; }

        #endregion

        #region Unity Methods

        private void Reset()
        {
            ControllerName = DEFAULT_CONTROLLER_NAME;
            DontDestroyControllerOnLoad = DEFAULT_DONT_DESTROY_CONTROLLER_ON_LOAD;
        }

        private void Awake()
        {
            if (Graph == null)
            {
                DDebug.LogError(UILabels.NoGraphReferenced + ". " + UILabels.ComponentDisabled + ".", gameObject);
                enabled = false;
                return;
            }

            if (Graph.Nodes.Count == 0)
            {
                DDebug.LogError("'" + Graph.name + "' " + UILabels.GraphHasNoNodes + ". " + UILabels.ComponentDisabled + ".", gameObject);
                enabled = false;
                return;
            }

            if (DebugComponent) DDebug.Log(UILabels.LoadedGraph + ": " + Graph.name);

            Database.Add(this);
            InitializeGraph();

            if (DontDestroyControllerOnLoad) DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (Graph == null) return;
            Graph.Enabled = true;
        }

        private void OnDisable()
        {
            if (Graph == null) return;
            Graph.Enabled = false;
        }

        public virtual void OnDestroy() { Database.Remove(this); }

        private void Update()
        {
            if (Graph != null) Graph.Update();
        }

        private void FixedUpdate()
        {
            if (Graph != null) Graph.FixedUpdate();
        }

        private void LateUpdate()
        {
            if (Graph != null) Graph.LateUpdate();
        }

        #endregion

        #region Public Methods

        /// <summary> Activate the passed Node if it belongs to the Graph </summary>
        /// <param name="node"> Node to search for </param>
        public void GoToNode(Node node)
        {
            if (Graph == null) return;
            if (node == null) return;
            if (!Graph.ContainsNode(node)) return;
            if (DebugComponent) DDebug.Log("GoTo Node: " + node.Name, this);
            Graph.SetActiveNode(node);
        }

        /// <summary> Activate the first Node, found inside the Graph, with the given node name (if it exists) </summary>
        /// <param name="nodeName"> Node name to search for </param>
        public void GoToNodeByName(string nodeName)
        {
            if (Graph == null) return;
            Node node = Graph.GetNodeByName(nodeName);
            if (node == null) return;
            if (DebugComponent) DDebug.Log("GoTo Node by Name: " + nodeName, this);
            Graph.SetActiveNode(node);
        }

        /// <summary> Activate the Node, found inside the Graph, with the given node id (if it exists) </summary>
        /// <param name="nodeId"> Node id to search for </param>
        public void GoToNodeById(string nodeId)
        {
            if (Graph == null) return;
            Node node = Graph.GetNodeById(nodeId);
            if (node == null) return;
            if (DebugComponent) DDebug.Log("GoTo Node by Id: " + nodeId, this);
            Graph.SetActiveNode(node);
        }

        #endregion

        #region Private Methods

        private void InitializeGraph(bool reset = true)
        {
            if (reset) ResetController();
            if (Initialized) return;

            if (Graph == null)
            {
                DDebug.LogError("Missing Graph reference...", this);
                return;
            }

            if (Graph.Nodes.Count == 0)
            {
                DDebug.LogError("No nodes have been added to the '" + Graph.name + "' Graph.", this);
                return;
            }

            if (Graph.GetStartOrEnterNode() == null)
            {
                DDebug.LogError("No start node has been set for the '" + Graph.name + "' Graph.", this);
                return;
            }


            StartCoroutine(ActivateStartOrEnterNodeEnumerator());
//            Graph.ActivateStartOrEnterNode();
//            Initialized = true;
        }

        private void ResetController()
        {
            Initialized = false;
            Graph.SetActiveNode(null);
            Graph.DeactivateGlobalNodes();
        }

        #endregion

        #region IEnumerators

        private IEnumerator ActivateStartOrEnterNodeEnumerator()
        {
            yield return null;
            Initialized = true;
            if (Graph.ActiveNode == null) Graph.ActivateStartOrEnterNode();
        }

        #endregion

        #region Static Mehtods

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedMethodReturnValue.Global
        /// <summary> Adds GraphController to scene and returns a reference to it </summary>
        public static GraphController AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<GraphController>(MenuUtils.GraphController_GameObject_Name, false, selectGameObjectAfterCreation); }

        /// <summary> Returns the first GraphController with the given controllerName. Returns null if no registered GraphController has the given controllerName </summary>
        /// <param name="controllerName"> Controller name to search for </param>
        public static GraphController Get(string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName)) return null;
            if (Database.Count == 0) return null;
            foreach (GraphController controller in Database)
                if (controller.ControllerName.Equals(controllerName))
                    return controller;

            return null;
        }

        #endregion
    }
}