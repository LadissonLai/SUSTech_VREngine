// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Nody.Models
{
    /// <summary> Describes the types of nodes available in the Nody node graph engine </summary>
    public enum NodeType
    {
        /// <summary>
        ///     StartNode - the first activated Node of a Graph
        /// </summary>
        Start = 0,

        /// <summary>
        ///     EnterNode - the first activated node if a SubGraph
        /// </summary>
        Enter = 1,

        /// <summary>
        ///     ExitNode - the exit node of a SubGraph
        /// </summary>
        Exit = 2,

        /// <summary>
        ///     SubGraphNode - reference to a SubGraph
        /// </summary>
        SubGraph = 3,

        /// <summary>
        ///     General node type - normal node
        /// </summary>
        General = 4,

        /// <summary>
        ///     Global node type - a node that is always active
        /// </summary>
        Global = 5
    }
}