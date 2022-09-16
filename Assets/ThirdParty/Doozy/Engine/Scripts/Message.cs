// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// ReSharper disable UnusedMember.Global

namespace Doozy.Engine
{
    /// <summary> Global messaging system </summary>
    public class Message
    {
        protected Message() { }
        
        #region Constants
        
        /// <summary> Prefix added to typeless message names internally to distinct them from the typed messages </summary>
        private const string TYPELESS_MESSAGE_PREFIX = "typeless ";
        
        #endregion

        #region Static Properties

        /// <summary> The handlers database </summary>
        private static readonly Dictionary<string, List<Delegate>> Handlers = new Dictionary<string, List<Delegate>>();

        // ReSharper disable once UnassignedField.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary> [Editor Only] Called when a message is sent and handled (works only in the Unity Editor) </summary>
        public static OnMessageHandleDelegate OnMessageHandle;
        
        /// <summary> [Editor Only] A delegation type used to callback when messages are sent and received </summary>
        /// <param name="callerType"> The caller type of the message </param>
        /// <param name="handlerType"> The handler type of the message </param>
        /// <param name="messageType"> The type of the sent message </param>
        /// <param name="messageName"> The name of the sent message </param>
        /// <param name="handlerMethodName"> The name of the method handling the sent message </param>
        public delegate void OnMessageHandleDelegate(Type callerType, Type handlerType, Type messageType, string messageName, string handlerMethodName);
        
        #endregion

        #region Static Methods

        /// <summary> Adds a listener that triggers the given callback when the message with the given name is received </summary>
        /// <param name="messageName"> The message name that will be listened to </param>
        /// <param name="callback"> The callback that will be triggered when the message is received </param>
        public static void AddListener(string messageName, Action callback) { RegisterListener(TYPELESS_MESSAGE_PREFIX + messageName, callback); }

        /// <summary> Adds a listener that triggers the given callback when a message of the given type is received </summary>
        /// <typeparam name="T"> The message type that will be listened to </typeparam>
        /// <param name="callback"> The callback that will be triggered when the message is received </param>
        public static void AddListener<T>(Action<T> callback) where T : Message { RegisterListener(typeof(T).ToString(), callback); }

        /// <summary> Adds a listener that triggers the given callback when a message of the given type and name is received </summary>
        /// <typeparam name="T"> The message type that will be listened to </typeparam>
        /// <param name="messageName"> The message name that will be listened to </param>
        /// <param name="callback"> The callback that is triggered when the message is received </param>
        public static void AddListener<T>(string messageName, Action<T> callback) where T : Message { RegisterListener(typeof(T) + messageName, callback); }

        /// <summary> Removes a listener that would trigger the given callback when a message with the given name is received </summary>
        /// <param name="messageName"> The message name that is being listened to </param>
        /// <param name="callback"> The callback that is triggered when the message is received </param>
        public static void RemoveListener(string messageName, Action callback) { UnregisterListener(TYPELESS_MESSAGE_PREFIX + messageName, callback); }

        /// <summary> Removes a listener that would trigger the given callback when a message of the given type is received </summary>
        /// <typeparam name="T"> The message type that is being listened to </typeparam>
        /// <param name="callback"> The callback that is triggered when the message is received </param>
        public static void RemoveListener<T>(Action<T> callback) where T : Message { UnregisterListener(typeof(T).ToString(), callback); }

        /// <summary> Removes a listener that would trigger the given callback when a message of the given type is received </summary>
        /// <typeparam name="T"> The message type that is being listened to </typeparam>
        /// <param name="messageName"> The message name that is being listened to </param>
        /// <param name="callback"> The callback that is triggered when the message is received </param>
        public static void RemoveListener<T>(string messageName, Action<T> callback) where T : Message { UnregisterListener(typeof(T) + messageName, callback); }

        /// <summary> Sends a message of the given name </summary>
        /// <param name="messageName"> The name of the message </param>
        public static void Send(string messageName) { SendMessage<Message>(TYPELESS_MESSAGE_PREFIX + messageName, null); }

        /// <summary> Sends a message of the given type </summary>
        /// <typeparam name="T"> The type of the message </typeparam>
        /// <param name="message"> The instance of the message </param>
        public static void Send<T>(T message) where T : Message { SendMessage(typeof(T).ToString(), message); }

        /// <summary> Sends a message of the given name and type </summary>
        /// <typeparam name="T"> The type of the message </typeparam>
        /// <param name="messageName"> The name of the message </param>
        /// <param name="message"> The instance of the message </param>
        public static void Send<T>(string messageName, T message) where T : Message { SendMessage(typeof(T) + messageName, message); }

        private static void RegisterListener(string messageName, Delegate callback)
        {
            if (callback == null) //check that the passed callback is not null
            {
                DDebug.LogError("Failed to add listener because the given callback is null!"); //print the relevant debug.error message
                return; //stop here
            }

            if (!Handlers.ContainsKey(messageName)) //check that this messageName has not been added to the handlers database
                Handlers.Add(messageName, new List<Delegate>()); //create a new entry in the handlers database with the given messageName
            List<Delegate> messageHandlers = Handlers[messageName]; //create a new list of Delegates so that we can add the callback
            messageHandlers.Add(callback); //add the callback
        }

        private static void UnregisterListener(string messageName, Delegate callback)
        {
            if (!Handlers.ContainsKey(messageName)) return;
            List<Delegate> messageHandlers = Handlers[messageName]; //create a list of delegates in order to be able to search through it
            Delegate messageHandler = messageHandlers.Find(x => x.Method == callback.Method && x.Target == callback.Target); //look for the callback
            if (messageHandler == null) return;
            messageHandlers.Remove(messageHandler); //remove the callback
        }

        private static void SendMessage<T>(string messageName, T e) where T : Message
        {
            if (!Handlers.ContainsKey(messageName)) return;
            Type callerType = null;
            if (Application.isEditor)
            {
                var stackTrace = new StackTrace();
                callerType = stackTrace.GetFrame(2).GetMethod().DeclaringType;
            }

            List<Delegate> messageHandlers = Handlers[messageName];
            Delegate[] handlers = messageHandlers.ToArray();
            foreach (Delegate messageHandler in handlers)
            {
                if (messageHandler.GetType() != typeof(Action<T>) && messageHandler.GetType() != typeof(Action)) continue;
                if (Application.isEditor && OnMessageHandle != null)
                {
                    string methodName = messageHandler.Method.Name;
                    messageName = messageName.Replace(TYPELESS_MESSAGE_PREFIX, "");
                    if (typeof(T) != typeof(Message)) messageName = messageName.Replace(typeof(T).ToString(), "");
                    OnMessageHandle(callerType, messageHandler.Target.GetType(), typeof(T), messageName, methodName);
                }

                if (typeof(T) == typeof(Message))
                {
                    var action = (Action) messageHandler;
                    action();
                }
                else
                {
                    var action = (Action<T>) messageHandler;
                    action(e);
                }
            }
        }

        #endregion
    }
}