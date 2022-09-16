// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using UnityEngine;

namespace Doozy.Engine.UI.Connections
{
    [Serializable]
    public class WeightedConnection : PassthroughConnection
    {
        private const int DEFAULT_WEIGHT = 100;
        
        public int Weight = DEFAULT_WEIGHT;
        
        #region Static Methods

        /// <summary> Returns an WeightedConnection instance from a socket by using JsonUtility.FromJson(socket.Value, socket.ValueType) </summary>
        /// <param name="socket"> Socket that has an WeightedConnection type Value </param>
        public static WeightedConnection GetValue(Socket socket) { return (WeightedConnection) JsonUtility.FromJson(socket.Value, socket.ValueType); }

        /// <summary> Sets a socket.Value by using JsonUtility.ToJson(value) </summary>
        /// <param name="socket"> Socket that has an WeightedConnection type Value </param>
        /// <param name="value"> WeightedConnection instance that will get converted to Json format and set as the socket.Value value </param>
        public static void SetValue(Socket socket, WeightedConnection value) { socket.Value = JsonUtility.ToJson(value); }

        #endregion
    }
}