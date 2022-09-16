using System;
using System.Collections.Generic;

using UnityEngine;

using UDebug = UnityEngine.Debug;
 
 
namespace Framework
{
    public interface ILogOutput
    {
        void Init(string path, string file);

        void Release();

        void WriteLog(string msg);
    }

    public static class DebugEx
    {
        private static ILogOutput lW;

        public static void Init()
        {
            UDebug.unityLogger.logEnabled = Config.LOG_ENABLE;

            UDebug.unityLogger.filterLogType = Config.FILTER_LOG_TYPE;

            if (Config.LOG_ENABLE && Config.LOGWRITE_ENABLE)
            {
                lW = System.Activator.CreateInstance(Config.LOG_OUTPUT_TYPE) as ILogOutput;

                lW.Init(Config.PERSISTENTDATA_IOPATH_, "LOG");

                Application.logMessageReceived += HandleLog;

                lW.WriteLog("\n------------------------LoggerWriter Start-----------------------------\n");
            }
        }

        private static void HandleLog(string message, string stackTrace, LogType type)
        {
            string info = null;

            if (type == LogType.Log)
            {
                info = string.Format("[{0}]\t[{1}]\n[{2}]\n", DateTime.Now.ToString("T"), type.ToString(), message);
            }
            else
            {
                info = string.Format("[{0}]\t[{1}]\n[{2}]\n[{3}]\n", DateTime.Now.ToString("T"), type.ToString(), message, stackTrace);
            }

            lW.WriteLog(info);
        }

        public static void AssertArrayLength(Array arr, int minLength, string errorMsg = null, params object[] args)
        {
            AssertIsTrue(arr.Length >= minLength, errorMsg, args);
        }

        public static void AssertNotNull(System.Object objCheck, string errorMsg = null, params object[] args)
        {
            bool condition = objCheck != null && !objCheck.Equals(null);

            AssertIsTrue(condition, errorMsg, args);
        }
        
        public static void AssertIsTrue(bool condition, string errorMsg = null, params object[] args)
        {
            if (!condition)
            {
                string msg = errorMsg == null ? null : string.Format(errorMsg, args);

                UDebug.LogError(msg);
                //throw new Exception("Assert exception. " + msg);
            }
        }
 
        public static void Error(object msg, params object[] args)
        {
            UDebug.LogErrorFormat(msg.ToString(), args);
        }

        public static void Log(object msg)
        {
            Log(LogTag.Debug, msg, null);
        }

        public static void Log(LogTag tag, object msg, params object[] args)
        {
            if ((tag & Config.FILTER_LOG_TAG) == LogTag.None)
            {
                return;
            }

            UDebug.unityLogger.Log(tag.ToString(), args != null ? string.Format(msg.ToString(), args) : msg);
        }

        public static void Warning(object msg, params object[] args)
        {
            UDebug.LogWarningFormat(msg.ToString(), args);
        }

        public static void AssertArrayLength<T>(List<T> list, int minLength, string errorMsg = null, params object[] args)
        {
            AssertIsTrue(list != null && list.Count >= minLength, errorMsg, args);
        }

        public static void Dispose()
        {
            if (lW != null)
            {
                Application.logMessageReceived -= HandleLog;

                lW.Release();
            }
        }
    }
}
