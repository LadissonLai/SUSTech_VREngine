// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Base;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Connections
{
    /// <summary>
    ///     Data container used by Nody to store the connection data between two nodes.
    /// </summary>
    [Serializable]
    public class UIConnection
    {
        #region Constants

        public const float DEFAULT_TIME_DELAY = 3f;

        #endregion

        #region Public Variables

        /// <summary> Button category value used only if Trigger is set to UIConnectionTrigger.ButtonClick or UIConnectionTrigger.ButtonDoubleClick or UIConnectionTrigger.ButtonLongClick </summary>
        public string ButtonCategory = NamesDatabase.GENERAL;

        /// <summary> Button name value used only if Trigger is set to UIConnectionTrigger.ButtonClick or UIConnectionTrigger.ButtonDoubleClick or UIConnectionTrigger.ButtonLongClick </summary>
        public string ButtonName = NamesDatabase.UNNAMED;

        /// <summary> Game event string value used only if Trigger is set to UIConnectionTrigger.GameEvent </summary>
        public string GameEvent = string.Empty;

        /// <summary> Delay value used if Trigger is set to UIConnectionTrigger.TimeDelay </summary>
        public float TimeDelay = DEFAULT_TIME_DELAY;

        /// <summary> Trigger that determines a UIConnection input node to become active </summary>
        public UIConnectionTrigger Trigger = UIConnectionTrigger.ButtonClick;

        #endregion

        #region Private Methods

        /// <summary> Resets this instance to the default values </summary>
        // ReSharper disable once UnusedMember.Local
        private void Reset()
        {
            Trigger = UIConnectionTrigger.ButtonClick;
            ButtonCategory = NamesDatabase.GENERAL;
            ButtonName = NamesDatabase.UNNAMED;
            GameEvent = string.Empty;
            TimeDelay = DEFAULT_TIME_DELAY;
        }

        #endregion

        #region Static Methods

        /// <summary> Returns an UIConnection instance from a socket by using JsonUtility.FromJson(socket.Value, socket.ValueType) </summary>
        /// <param name="socket"> Socket that has an UIConnection type Value </param>
        public static UIConnection GetValue(Socket socket)
        {
//            return (UIConnection) JsonUtility.FromJson(socket.Value, socket.ValueType);
            return (UIConnection) JsonUtility.FromJson(socket.Value, typeof(UIConnection));
        }

        /// <summary> Sets a socket.Value by using JsonUtility.ToJson(value) </summary>
        /// <param name="socket"> Socket that has an UIConnection type Value </param>
        /// <param name="value"> UIConnection instance that will get converted to Json format and set as the socket.Value value </param>
        public static void SetValue(Socket socket, UIConnection value) { socket.Value = JsonUtility.ToJson(value); }

        #endregion
    }
}