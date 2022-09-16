using System;

namespace Doozy.Engine.Utils
{
    public static class GuidUtils
    {
        /// <summary>
        /// Creates the SerializedGuid byte array
        /// <para/> Used OnBeforeSerialize
        /// </summary>
        public static byte[] GuidToSerializedGuid(Guid guid) { return guid != Guid.Empty ? guid.ToByteArray() : null; }

        /// <summary>
        /// Restores the Guid from the SerializedGuid byte array
        /// <para/> Used OnAfterDeserialize 
        /// </summary>
        public static Guid SerializedGuidToGuid(byte[] serializedGuid) { return serializedGuid != null && serializedGuid.Length == 16 ? new Guid(serializedGuid) : Guid.Empty; }
    }
}