// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;

namespace Doozy.Engine.Nody.Nodes
{
    /// <summary>
    /// Remembers the source (input) node when going to a target (output) node. After that, when returning from the target (output) node, it goes back to the source node from where it came from.
    /// <para/>
    /// <para/>
    /// Example:
    /// <para/>
    /// The 'Settings Menu' node needs to be accessed from different parts of the UI and you need to go back from the area of the UI where you came from.
    /// <para/>
    /// To do this, set up one or more sources ('Main Menu' and 'Game Menu' for example) and set the 'Settings Menu' as the target. 
    /// <para/>
    /// <para/>
    /// When you access the 'Settings Menu' from the 'Main Menu', the Switch Back Node will remember 'Main Menu' as the source.
    /// <para/>
    /// And, when returning from the 'Settings Menu' (via a 'Back' button for example), the UI flow will be redirected to the 'Main Menu'
    /// <para/>
    /// <para/>
    /// When you access the 'Settings Menu' from the 'Game Menu', the Switch Back Node will remember 'Game Menu' as the source.
    /// <para/>
    /// And, when returning from the 'Settings Menu', the UI flow will be redirected to the 'Game Menu' as this was the last recorded source.
    /// </summary>
    [NodeMenu(MenuUtils.SwitchBackNode_CreateNodeMenu_Name, MenuUtils.SwitchBackNode_CreateNodeMenu_Order)]
    public class SwitchBackNode : Node
    {
        [Serializable]
        public class SourceInfo
        {
            public string SourceName;
            public string InputSocketId;
            public string OutputSocketId;

            public bool InputSocketIsConnected;
            public bool OutputSocketIsConnected;
            public bool IsConnected { get { return InputSocketIsConnected && OutputSocketIsConnected; } }

            public SourceInfo(string sourceName, string inputSocketId, string outputSocketId)
            {
                SourceName = sourceName;
                InputSocketId = inputSocketId;
                OutputSocketId = outputSocketId;
            }
        }

        [NonSerialized] private Graph m_targetGraph;
        [NonSerialized] private string m_returnSourceOutputSocketId;

        public List<SourceInfo> Sources = new List<SourceInfo>();

        public Socket TargetInputSocket { get { return InputSockets[0]; } }
        public Socket TargetOutputSocket { get { return OutputSockets[0]; } }
        public string ReturnSourceOutputSocketId { get { return m_returnSourceOutputSocketId; } }

#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNoTargetConnected || ErrorNoSourceConnected; } }

        public bool ErrorNoTargetConnected;
        public bool ErrorNoSourceConnected;
#endif

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.SwitchBackNodeName);
            SetWidth(NodySettings.Instance.SwitchBackNodeWidth);
            MinimumInputSocketsCount = 2;
            MinimumOutputSocketsCount = 2;
        }

        public override float GetDefaultNodeWidth() { return NodySettings.Instance.SwitchBackNodeWidth; }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();

            //INDEX 0 - add Switch Target input and output sockets
            AddInputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);

            //INDEX 1 - add Switch Sources input and output sockets 
            AddSourceSocketPair();

            //INDEX 2 - add Switch Sources input and output sockets 
            AddSourceSocketPair();
        }

        public void AddSourceSocketPair()
        {
            Socket inputSocket = AddInputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
            Socket outputSocket = AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
            Sources.Add(new SourceInfo(UILabels.SourceName + " " + (Sources.Count + 1), inputSocket.Id, outputSocket.Id));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_returnSourceOutputSocketId = string.Empty;
        }

        private void OnDisable() { m_returnSourceOutputSocketId = string.Empty; }

        private SourceInfo GetSource(Connection connection)
        {
            foreach (SourceInfo source in Sources)
                if (connection.InputSocketId == source.InputSocketId)
                    return source;

            return null;
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);

            var switchBackNode = (SwitchBackNode) original;
            Sources = new List<SourceInfo>();
            for (int i = 0; i < switchBackNode.Sources.Count; i++)
                Sources.Add(new SourceInfo(switchBackNode.Sources[i].SourceName, InputSockets[i + 1].Id, OutputSockets[i + 1].Id));
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;

            SourceInfo source = GetSource(connection);
            if (source == null) //entered the Switch from the Target Node
            {
                if (!string.IsNullOrEmpty(m_returnSourceOutputSocketId)) //we have a return Source OutputSocketId registered -> we need to check that it's valid
                {
                    Socket socket = GetSocketFromId(m_returnSourceOutputSocketId); //try to get a reference to the socket
                    if (socket != null)                                            //the the output socket was found
                        if (socket.IsConnected)                                    //the found socket is indeed connected -> go to the input node of this socket
                        {
                            ActiveGraph.SetActiveNodeByConnection(socket.FirstConnection); //go to the input node of this output socket
                            m_returnSourceOutputSocketId = string.Empty;
                            return; //stop here
                        }
                }

                //there is no return to Source OutputSocketId registered
                //this is a strange case where we come from the Target Node without getting there via the Switch
                //if we are in this particular situation, then the developer, that created this graph, made some 'uninspired' connections
                //look for the first available Source OutputSocketId
                foreach (SourceInfo s in Sources)
                {
                    if (string.IsNullOrEmpty(s.OutputSocketId)) continue;          //this output socket id is null or empty -> skip this entry
                    Socket socket = GetSocketFromId(s.OutputSocketId);             //try to get a reference to the socket
                    if (socket == null) continue;                                  //the id is not valid as no socket was found
                    if (!socket.IsConnected) continue;                             //the id was good, the socket was found, but it is not connected -> try to get the next source output socket id
                    ActiveGraph.SetActiveNodeByConnection(socket.FirstConnection); //go to the input node of this output socket
                    m_returnSourceOutputSocketId = string.Empty;
                    return; //stop here
                }

                //if we got to this point then none of the source output sockets are connected
                //this is very bad and we need to avoid having the UI flow blocked in this Switch
                //let's return to the node we came from as a last resort
                ActiveGraph.SetActiveNodeById(connection.OutputNodeId); //go back to the node we came from
                m_returnSourceOutputSocketId = string.Empty;
                return; //stop here
            }

            //the source is not null -> we entered the Switch from a Source Node
            //now we need to see if we have a Target Node output connected -> otherwise we need to go back in order to avoid having the UI flow blocked in this Switch
            if (!TargetOutputSocket.IsConnected) //there is no Target Node connected to the Target Output Socket -> return to the Node we came from
            {
                //---> this is not normal as the graph should not work like this, BUT we want to avoid having the UI flow blocked in this node
                ActiveGraph.SetActiveNodeById(connection.OutputNodeId); //go back
                m_returnSourceOutputSocketId = string.Empty;
                return; //stop here
            }

            //there is a Target Node connected to the Target Output Socket

            m_returnSourceOutputSocketId = !string.IsNullOrEmpty(source.OutputSocketId) ? source.OutputSocketId : string.Empty; //save the source output socket id in order to go back to it when we return from the target node
            ActiveGraph.SetActiveNodeByConnection(TargetOutputSocket.FirstConnection);                                          //go to the Target Node via the Target Output Socket connection
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR

            ErrorNoTargetConnected = !TargetInputSocket.IsConnected || !TargetOutputSocket.IsConnected;
            ErrorNoSourceConnected = true;
            foreach (SourceInfo source in Sources)
            {
                Socket inputSocket = GetSocketFromId(source.InputSocketId);
                source.InputSocketIsConnected = inputSocket != null && inputSocket.IsConnected;
                Socket outputSocket = GetSocketFromId(source.OutputSocketId);
                source.OutputSocketIsConnected = outputSocket != null && outputSocket.IsConnected;
                if (!source.IsConnected) continue;
                ErrorNoSourceConnected = false;
                break;
            }
#endif
        }

        public void RegenerateSourcesSocketIds()
        {
            for (int i = 1; i < InputSockets.Count; i++)
            {
                Sources[i - 1].InputSocketId = InputSockets[i].Id;
                Sources[i - 1].OutputSocketId = OutputSockets[i].Id;
            }

            if (Sources.Count <= InputSockets.Count - 1) return;
            while (Sources.Count > InputSockets.Count - 1)
                Sources.RemoveAt(Sources.Count - 1);
        }
    }
}