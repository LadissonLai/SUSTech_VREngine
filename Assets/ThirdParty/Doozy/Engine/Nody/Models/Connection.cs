// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable PublicConstructorInAbstractClass

namespace Doozy.Engine.Nody.Models
{
    /// <summary> Base class for any connection between two Sockets </summary>
    [Serializable]
    public class Connection
    {
        #region Properties

        /// <summary> [Editor Only] Pings this connection </summary>
        public bool Ping { get; set; }

        /// <summary> Returns the id of this connection </summary>
        public string Id { get { return m_id; } set { m_id = value; } }

        /// <summary> Returns the id of this connection's input Node </summary>
        public string InputNodeId { get { return m_inputNodeId; } set { m_inputNodeId = value; } }

        /// <summary> Returns the id of this connection's input Socket </summary>
        public string InputSocketId { get { return m_inputSocketId; } set { m_inputSocketId = value; } }

        /// <summary> Returns the id of this connection's output Node </summary>
        public string OutputNodeId { get { return m_outputNodeId; } set { m_outputNodeId = value; } }

        /// <summary> Returns the id of this connection's output Socket </summary>
        public string OutputSocketId { get { return m_outputSocketId; } set { m_outputSocketId = value; } }

        /// <summary> [Editor Only] Returns the position of this connection's input point </summary>
        public Vector2 InputConnectionPoint { get { return m_inputConnectionPoint; } set { m_inputConnectionPoint = value; } }

        /// <summary> [Editor Only] Returns the position of this connection's output point </summary>
        public Vector2 OutputConnectionPoint { get { return m_outputConnectionPoint; } set { m_outputConnectionPoint = value; } }

        #endregion

        #region Private Variables

        [SerializeField] private Vector2 m_inputConnectionPoint;
        [SerializeField] private Vector2 m_outputConnectionPoint;
        [SerializeField] private string m_id;
        [SerializeField] private string m_inputNodeId;
        [SerializeField] private string m_inputSocketId;
        [SerializeField] private string m_outputNodeId;
        [SerializeField] private string m_outputSocketId;

        #endregion

        #region Constructors

        /// <summary> Creates a new instance for this class between two sockets (Input - Output or Output - Input) </summary>
        /// <param name="socket1"> Socket One </param>
        /// <param name="socket2"> Socket Two </param>
        public Connection(Socket socket1, Socket socket2)
        {
            GenerateNewId();
            if (socket1.IsOutput && socket2.IsInput)
            {
                m_outputNodeId = socket1.NodeId;
                m_outputSocketId = socket1.Id;
                m_outputConnectionPoint = socket1.GetClosestConnectionPointToSocket(socket2);

                m_inputNodeId = socket2.NodeId;
                m_inputSocketId = socket2.Id;
                m_inputConnectionPoint = socket2.GetClosestConnectionPointToSocket(socket1);
            }

            if (socket1.IsInput && socket2.IsOutput)
            {
                m_outputNodeId = socket2.NodeId;
                m_outputSocketId = socket2.Id;
                m_outputConnectionPoint = socket2.GetClosestConnectionPointToSocket(socket1);

                m_inputNodeId = socket1.NodeId;
                m_inputSocketId = socket1.Id;
                m_inputConnectionPoint = socket1.GetClosestConnectionPointToSocket(socket2);
            }
        }

        /// <summary> Creates a new instance for this class with the settings of the passed connection </summary>
        /// <param name="other"> Source connection </param>
        public Connection(Connection other)
        {
            m_id = other.Id;

            m_outputNodeId = other.m_outputNodeId;
            m_outputSocketId = other.m_outputSocketId;
            m_outputConnectionPoint = other.m_outputConnectionPoint;

            m_inputNodeId = other.m_inputNodeId;
            m_inputSocketId = other.m_inputSocketId;
            m_inputConnectionPoint = other.m_inputConnectionPoint;
        }

        #endregion

        #region Public Methods

        /// <summary> Generates a new id for this connections and returns it </summary>
        public string GenerateNewId()
        {
            m_id = Guid.NewGuid().ToString();
            return m_id;
        }

        #endregion
    }
}