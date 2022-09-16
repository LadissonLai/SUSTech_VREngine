// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine
{
    /// <summary> Extension methods for the Message Class </summary>
    public static class MessageExtensions
    {
        /// <summary> Quick method to send a Message </summary>
        /// <param name="self"> Message </param>
        /// <typeparam name="T"> Type of Message </typeparam>
        public static void Send<T>(this T self) where T : Message
        {
            Message.Send(self);
        }
    }
}