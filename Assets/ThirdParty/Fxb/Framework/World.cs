
namespace Framework
{
    using System;
    using System.Threading;
    using System.Collections.Generic;

    using UnityEngine;
    using Framework.Tools;

    /// <summary>
    /// World. 全局一直存在的一个MonoBehaviour，也是程序入口。
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class World : MonoBehaviour
    {
        protected int currentThreadId;

        protected Action UpdateHandle;
 
        public Injecter Injecter {get;private set;}

        public bool IsAppQuitting { get; private set; }

        /// <summary>
        /// 当前world示例
        /// </summary>
        public static World current
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取注入的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T Get<T>(string customKey = null) where T : class
        {
            if(current == null)
                return null;

            return current.Injecter.Get<T>(customKey);
        }
 
        protected virtual void DebugInfo()
        {
            DebugEx.Log(LogTag.Info
                , "--deviceModel:{0}\n" +
                "--deviceName:{1}\n" +
                "--graphicsDeviceType:{2}\n" +
                "--screen:{3}x{4}"
                , SystemInfo.deviceModel
                , SystemInfo.deviceName
                , SystemInfo.graphicsDeviceType
                , Screen.width
                , Screen.height
                );

            DebugEx.Log(LogTag.Info
                , "--platform:{0}\n--STREAMINGASSETS_WWWPATH_:{1}\n--PERSISTENTDATA_WWWPATH_:{2}\n--PERSISTENTDATA_IOPATH_:{3}"
                , Application.platform
                , Config.STREAMINGASSETS_WWWPATH_
                , Config.PERSISTENTDATA_WWWPATH_
                , Config.PERSISTENTDATA_IOPATH_
                );
        }

        protected virtual void InitPathConfig()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    Config.STREAMINGASSETS_WWWPATH_ = string.Concat("file:///", Application.streamingAssetsPath, "/");

                    Config.PERSISTENTDATA_WWWPATH_ = string.Concat("file:///", Application.persistentDataPath, "/");

                    Config.PERSISTENTDATA_IOPATH_ = string.Concat(Application.persistentDataPath, "/");

                    break;
                case RuntimePlatform.IPhonePlayer:
                    Config.STREAMINGASSETS_WWWPATH_ = string.Concat("file://", Application.streamingAssetsPath, "/");

                    Config.PERSISTENTDATA_IOPATH_ = string.Concat(Application.persistentDataPath, "/");

                    Config.PERSISTENTDATA_WWWPATH_ = string.Concat("file://", Application.persistentDataPath, "/");

                    break;
                case RuntimePlatform.Android:
                    /*   jar:file:///data/app/#PROJECT_NAME.apk!/assets/   */
                    Config.STREAMINGASSETS_WWWPATH_ = string.Concat(Application.streamingAssetsPath, "/");

                    /*  /storage/emulated/0/Android/data/#PROJECT_NAME/files/    */
                    Config.PERSISTENTDATA_IOPATH_ = string.Concat(Application.persistentDataPath, "/");

                    /* file:///storage/emulated/0/Android/data/#PROJECT_NAME/files/  */
                    Config.PERSISTENTDATA_WWWPATH_ = string.Concat("file://", Application.persistentDataPath, "/");

                    break;
                default:
                    throw new Exception("No suport platform");
            }
        }

        /// <summary>
        /// Inits the config.
        /// </summary>
        protected virtual void InitConfig()
        {
            InitPathConfig();

            Config.TARGET_FRAMERATE = 60;

            Config.LOG_ENABLE = true;

            //Config.LOG_OUTPUT_TYPE = typeof(LoggerWriter);

            Config.LOGWRITE_ENABLE = false;

            Config.FILTER_LOG_TAG = LogTag.All;

            Config.FILTER_LOG_TYPE = LogType.Exception;

            Config.DEBUG_FLAG.isDebug = Debug.isDebugBuild;
        }

        /// <summary>
        /// Inits the application.
        /// </summary>
        protected virtual void InitApplication()
        {
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Low;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if(Config.TARGET_FRAMERATE != 0)
                Application.targetFrameRate = Config.TARGET_FRAMERATE;
        }

        /// <summary>
        /// Inits the framework.
        /// </summary>
        protected virtual void InitFramework()
        {

        }

        protected virtual void DisplayFps()
        {

        }

        /// <summary>
        /// Regists the update call back.
        /// </summary>
        /// <param name="callBack">Call back.</param>
        public void RegistUpdateCallBack(Action callBack)
        {
            UpdateHandle -= callBack;

            UpdateHandle += callBack;
        }

        /// <summary>
        /// Uns the regist update call back.
        /// </summary>
        /// <param name="callBack">Call back.</param>
        public void UnRegistUpdateCallBack(Action callBack)
        {
            UpdateHandle -= callBack;
        }
  
        public bool IsMainThread()
        {
            return Thread.CurrentThread.ManagedThreadId == currentThreadId;
        }

        #region Monobehaviour 生命周期

        protected virtual void OnDestroy()
        {
            
        }

        protected virtual void Awake()
        {
            if (current != null)
            {
                Destroy(this);

                return;
            }

            current = this;

            DontDestroyOnLoad(gameObject);

            currentThreadId = Thread.CurrentThread.ManagedThreadId;

            Injecter = new Injecter();

            InitConfig();
             
            InitApplication();

            InitFramework();
        }

        // Use this for initialization
        protected virtual void Start()
        {
            if (Config.DEBUG_FLAG.fpsDisplay)
                DisplayFps();

            DebugInfo();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            UpdateHandle?.Invoke();
        }

        protected virtual void OnApplicationFocus(bool focus)
        {

        }

        protected virtual void OnApplicationPause(bool pause)
        {

        }

        protected virtual void OnApplicationQuit()
        {
            DebugEx.Dispose();

            IsAppQuitting = true;
        }

        #endregion
    }
}

