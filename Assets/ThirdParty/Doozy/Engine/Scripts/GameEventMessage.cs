// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEngine;

// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine
{
    /// <inheritdoc />
    /// <summary>
    /// Game Event global message
    /// </summary>
    public class GameEventMessage : Message
    {
        #region Constants

        private const string NO_GAME_EVENT = "None";

        #endregion

        #region Properties

        /// <summary>
        /// Returns TRUE if this message contains a reference to a custom Object
        /// </summary>
        public bool HasCustomObject { get { return CustomObject != null; } }
        
        /// <summary>
        /// Returns TRUE if this message contains a game event string.
        /// If FALSE, then this message was used only to send a GameObject reference.
        /// </summary>
        public bool HasGameEvent { get { return !EventName.Equals(NO_GAME_EVENT); } }

        /// <summary>
        /// Returns TRUE if this message contains a reference to the source GameObject that sent it
        /// </summary>
        public bool HasSource {get {return Source != null; }}
        
        /// <summary>
        /// Returns TRUE if this message contains a game event string that is also a system event.
        /// A game event string is considered to be a system event if it is contained in the SystemGameEvent enum values.
        /// </summary>
        public bool IsSystemEvent { get; private set; }

        #endregion

        #region Public Variables

        /// <summary>
        /// The game event string sent with this message.
        /// If "None", then no game event is considered to have been passed with this message.
        /// </summary>
        public readonly string EventName;

        /// <summary>
        /// The source GameObject reference that sent this message.
        /// If null, then no GameObject reference was passed with this message.
        /// </summary>
        public GameObject Source;

        /// <summary>
        /// A custom Object reference passed with this message
        /// </summary>
        public Object CustomObject;

        #endregion

        #region Constuctors

        /// <summary> Initializes a new instance of the class with the passed SystemGameEvent </summary>
        /// <param name="systemGameEvent"> The game event string that will get sent with this message </param>
        public GameEventMessage(SystemGameEvent systemGameEvent)
        {
            EventName = systemGameEvent.ToString();
            Source = null;
            CustomObject = null;
            IsSystemEvent = true;
        }

        /// <summary> Initializes a new instance of the class with the passed game event string value </summary>
        /// <param name="gameEvent"> The game event string that will get sent with this message </param>
        public GameEventMessage(string gameEvent)
        {
            EventName = gameEvent;
            Source = null;
            CustomObject = null;
            IsSystemEvent = false;
        }

        /// <summary>
        /// Initializes a new instance of the class with a GameObject reference and no game event string.
        /// <para/> This overload can be used to send only a GameObject reference.
        /// </summary>
        /// <param name="source"> The GameObject reference that will get sent with this message </param>
        public GameEventMessage(GameObject source)
        {
            EventName = NO_GAME_EVENT;
            Source = source;
            CustomObject = null;
            IsSystemEvent = false;
        }

        /// <summary> Initializes a new instance of the class with a SystemGameEvent and a source GameObject reference </summary>
        /// <param name="systemGameEvent"> The game event string that will get sent with this message </param>
        /// <param name="source"> The GameObject reference that will get sent with this message </param>
        /// <param name="customObject"> A custom Object reference that will get sent with this message </param>
        public GameEventMessage(SystemGameEvent systemGameEvent, GameObject source, Object customObject = null)
        {
            EventName = systemGameEvent.ToString();
            Source = source;
            CustomObject = customObject;
            IsSystemEvent = true;
        }

        /// <summary> Initializes a new instance of the class with a game event string and a source GameObject reference </summary>
        /// <param name="gameEvent"> The game event string that will get sent with this message </param>
        /// <param name="source"> The game object reference that will get sent with this message </param>
        public GameEventMessage(string gameEvent, GameObject source)
        {
            EventName = gameEvent;
            Source = source;
            CustomObject = null;
            IsSystemEvent = false;
        }
        
        /// <summary> Initializes a new instance of the class with a source GameObject reference and a custom Object reference </summary>
        /// <param name="source"> The GameObject reference that will get sent with this message </param>
        /// <param name="customObject"> A custom Object reference that will get sent with this message </param>
        public GameEventMessage(GameObject source, Object customObject)
        {
            EventName = NO_GAME_EVENT;
            Source = source;
            CustomObject = customObject;
            IsSystemEvent = false;
        }
        
        /// <summary> Initializes a new instance of the class with a game event string and a custom Object reference </summary>
        /// <param name="gameEvent"> The game event string that will get sent with this message </param>
        /// <param name="customObject"> A custom Object reference that will get sent with this message </param>
        public GameEventMessage(string gameEvent, Object customObject)
        {
            EventName = gameEvent;
            Source = null;
            CustomObject = customObject;
            IsSystemEvent = false;
        }
        
        /// <summary> Initializes a new instance of the class with a game event string, a source GameObject reference and a custom Object reference </summary>
        /// <param name="gameEvent"> The game event string that will get sent with this message </param>
        /// <param name="source"> The GameObject reference that will get sent with this message </param>
        /// <param name="customObject"> A custom Object reference that will get sent with this message </param>
        public GameEventMessage(string gameEvent, GameObject source, Object customObject)
        {
            EventName = gameEvent;
            Source = source;
            CustomObject = customObject;
            IsSystemEvent = false;
        }

        #endregion

        #region Static Methods

        /// <summary> Send a message with the passed SystemGameEvent </summary>
        /// <param name="systemGameEvent"> The game event string sent with this message </param>
        public static void SendEvent(SystemGameEvent systemGameEvent) { SendEvent(new GameEventMessage(systemGameEvent)); }

        /// <summary> Send a message with the passed game event string </summary>
        /// <param name="gameEvent"> The game event string sent with this message </param>
        public static void SendEvent(string gameEvent) { SendEvent(new GameEventMessage(gameEvent)); }

        /// <summary> Send a message with the passed source GameObject reference </summary>
        /// <param name="source"> The source game object reference that sent this message </param>
        public static void SendEvent(GameObject source) { SendEvent(new GameEventMessage(source)); }

        /// <summary> Send a message with a passed SystemGameEvent and a source GameObject reference </summary>
        /// <param name="systemGameEvent"> The game event string sent with this message </param>
        /// <param name="source"> The source game object reference that sent this message </param>
        public static void SendEvent(SystemGameEvent systemGameEvent, GameObject source) { SendEvent(new GameEventMessage(systemGameEvent, source)); }

        /// <summary> Send a message with a given game event string and a source GameObject reference </summary>
        /// <param name="gameEvent"> The game event string sent with this message </param>
        /// <param name="source"> The source game object reference that sent this message </param>
        public static void SendEvent(string gameEvent, GameObject source) { SendEvent(new GameEventMessage(gameEvent, source)); }

        /// <summary> Send a message with a given game event string and a custom Object reference </summary>
        /// <param name="gameEvent"> The game event string sent with this message </param>
        /// <param name="customObject"> A custom Object reference sent with this message </param>
        public static void SendEvent(string gameEvent, Object customObject) { SendEvent(new GameEventMessage(gameEvent, customObject)); }
        
        /// <summary> Send a message with a given source GameObject reference and a custom Object reference </summary>
        /// <param name="source"> The source game object reference that sent this message </param>
        /// <param name="customObject"> A custom Object reference sent with this message </param>
        public static void SendEvent(GameObject source, Object customObject) { SendEvent(new GameEventMessage(source, customObject)); }
        
        /// <summary> Send a message with a given game event string, a source GameObject reference and a custom Object reference </summary>
        /// <param name="gameEvent"> The game event string sent with this message </param>
        /// <param name="source"> The source game object reference that sent this message </param>
        /// <param name="customObject"> A custom Object reference sent with this message </param>
        public static void SendEvent(string gameEvent, GameObject source, Object customObject) { SendEvent(new GameEventMessage(gameEvent, source, customObject)); }
        
        /// <summary> Send a list of messages, each message with the given event string, a source GameObject reference and a custom Object reference </summary>
        /// <param name="gameEvents"> The game event strings sent with these messages </param>
        /// <param name="source"> The source game object reference that sent these messages </param>
        /// <param name="customObject"> A custom Object reference sent with these messages </param>
        public static void SendEvents(List<string> gameEvents, GameObject source = null, Object customObject = null)
        {
            if (gameEvents == null || gameEvents.Count == 0) return;
            foreach (string gameEvent in gameEvents)
                SendEvent(gameEvent, source, customObject);
        }

        private static void SendEvent(GameEventMessage gameEventMessage)
        {
            GameEventManager.ProcessGameEvent(gameEventMessage);
            Send(gameEventMessage.EventName, gameEventMessage);
            Send(gameEventMessage);
        }

        #endregion
    }
}