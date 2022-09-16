// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        /// <summary>
        ///     Helper class used to hold all the calculated connection point data.
        ///     <para />
        ///     A socket can have one or many connection points. We select (and calculate) the best available connection point in order to draw a connection taking into account distances and a few other factors.
        /// </summary>
        [Serializable]
        public class VirtualPoint
        {
            /// <summary> The Node this connection point belongs to </summary>
            public Node Node;

            /// <summary> The Socket this connection point belongs top </summary>
            public Socket Socket;

            /// <summary> The Point position in Node Space (not World Space) </summary>
            public Vector2 PointPosition;

            /// <summary>  The Point position in Socket Space (not Node or World Space). This is the value found in the Socket.ConnectionPoints list </summary>
            public Vector2 LocalPointPosition;

            /// <summary>
            ///     The Rect of this connection point in World Space.
            ///     <para />
            ///     Note that CalculateRect() needs to be called before using this rect.
            ///     <para />
            ///     CalculateRect() is done automatically when building the Database or when the Database is updated.
            ///     <para />
            ///     The Database update is executed automatically when (Event.current.type == EventType.MouseDrag) is performed on the parent Node.
            /// </summary>
            public Rect Rect;

            /// <summary>
            ///     Remembers if this point is occupied or not.
            ///     <para />
            ///     A point is deemed occupied when in the ConnectionsDatabase there is at least one VirtualConnection that uses this PointPosition (for the parent Socket, for the parent Node)
            /// </summary>
            public bool IsConnected;


            /// <summary> Construct a Virtual Point </summary>
            /// <param name="node"> The node this virtual point belongs to </param>
            /// <param name="socket"> The socket this virtual point is attached to </param>
            /// <param name="pointPosition"> The position - in node space - this point is located at </param>
            /// <param name="localPointPosition"> The position - in socket space - this point is located at </param>
            public VirtualPoint(Node node, Socket socket, Vector2 pointPosition, Vector2 localPointPosition)
            {
                Node = node;
                Socket = socket;
                PointPosition = pointPosition;
                LocalPointPosition = localPointPosition;
                IsConnected = false;
                CalculateRect(); //calculate the Rect by converting the position from NodeSpace to WorldSpace and setting up correct values
            }

            /// <summary>
            ///     Calculate this virtual point's rect by converting it's point position node space position into a world position.
            ///     <para />
            ///     It uses the default connector width and height (found in Settings.GUI) in order to create the rect.
            /// </summary>
            public void CalculateRect()
            {
                Rect = new Rect();
                if (Node == null)
                {
                    Debug.Log("Cannot calculate the connection point Rect because the parent Node is null.");
                    return;
                }

                if (Socket == null)
                {
                    Debug.Log("Cannot calculate the connection point Rect because the parent Socket is null.");
                    return;
                }

                var socketWorldRect = new Rect(Node.GetX(),
                                               Node.GetY() + Socket.GetY(),
                                               Socket.GetWidth(),
                                               Socket.GetHeight());

                Rect = new Rect(socketWorldRect.x + PointPosition.x + NodySettings.Instance.ConnectionPointWidth / 2,
                                socketWorldRect.y + PointPosition.y + NodySettings.Instance.ConnectionPointHeight / 2,
                                NodySettings.Instance.ConnectionPointWidth,
                                NodySettings.Instance.ConnectionPointHeight);
            }
        }
    }
}