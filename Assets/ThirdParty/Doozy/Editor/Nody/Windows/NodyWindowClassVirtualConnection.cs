// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        /// <summary> Helper class used to hold all the connections info </summary>
        [Serializable]
        public class VirtualConnection
        {
            /// <summary> The connection id. Every connection will have a record on two sockets (the output socket and the input socket) </summary>
            public string ConnectionId;

            /// <summary> Reference to the OutputSocket Node of this connections </summary>
            public Node OutputNode;

            /// <summary> Reference to the OutputSocket Socket of the OutputSocket Node of this connection </summary>
            public Socket OutputSocket;

            /// <summary> Reference to the InputSocket Node of this connection </summary>
            public Node InputNode;

            /// <summary> Reference to the InputSocket Socket of the InputSocket Node of this connection </summary>
            public Socket InputSocket;

            /// <summary> Reference to the OutputSocket Virtual Point of this connection </summary>
            public VirtualPoint OutputVirtualPoint;

            /// <summary> Reference to the InputSocket Virtual Point of this connection </summary>
            public VirtualPoint InputVirtualPoint;

            /// <summary> Holds the last calculated value of the OutputSocket Tangent in order to draw the connection curve (huge performance boost as we won't need to recalculate it on every frame) </summary>
            public Vector2 OutputTangent = Vector2.zero;

            /// <summary> Holds the last calculated value of the InputSocket Tangent in order to draw the connection curve (huge performance boost as we won't need to recalculate it on every frame) </summary>
            public Vector2 InputTangent = Vector2.zero;

            public AnimBool Ping = new AnimBool {speed = NodySettings.Instance.PingColorChangeSpeed};
        }

        /// <summary> Lightweight handles material </summary>
        private static Material s_handleMaterial;

        /// <summary> Returns a lightweight handles material </summary>
        private static Material HandleMaterial
        {
            get
            {
                if (s_handleMaterial != null) return s_handleMaterial;
                Shader shader = Shader.Find("Hidden/Nody/LineDraw");
                var m = new Material(shader) {hideFlags = HideFlags.HideAndDontSave};
                s_handleMaterial = m;
                return s_handleMaterial;
            }
        }
    }
}