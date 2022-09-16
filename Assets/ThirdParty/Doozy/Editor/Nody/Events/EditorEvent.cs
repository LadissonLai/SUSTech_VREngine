// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Editor.Nody.Events
{
    public class EditorEvent<T>
    {
        public static Action<EditorEvent<T>> OnEditorEvent = delegate { };

        public readonly Event Event;
        public readonly T Source;

        public EditorEvent(T source)
        {
            Event = Event.current;
            Source = source;
        }

        public static void Send(T s) { OnEditorEvent.Invoke(new EditorEvent<T>(s)); }
    }
}