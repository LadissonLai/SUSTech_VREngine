

namespace Framework
{
    using System;

    using UnityEngine;

    [Flags]
    public enum LogTag : byte
    {
        None = 0,
        Debug = 1,
        Cmd = 2,
        Info = 4,
        Net = 8,
        Loader = 16,
        All = 255
    }

    public struct DebugFlag
    {
        public bool fpsDisplay;

        public bool isDebug;
    }

    public static class Config
    {
        public static DebugFlag DEBUG_FLAG;
        
        #region render

        public static int TARGET_FRAMERATE;

        #endregion
 
        #region Log
 
        public static bool LOG_ENABLE;

        public static bool LOGWRITE_ENABLE;

        public static Type LOG_OUTPUT_TYPE;

        public static LogTag FILTER_LOG_TAG;

        public static LogType FILTER_LOG_TYPE;

        #endregion
  
        #region Path
         
        public static string STREAMINGASSETS_WWWPATH_;

        public static string PERSISTENTDATA_WWWPATH_;

        public static string PERSISTENTDATA_IOPATH_;

        #endregion

    }
}
