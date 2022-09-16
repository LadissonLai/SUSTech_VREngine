
namespace Framework
{
    /// <summary>
    /// 非monobehaviour 单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : IDispose, new()
    {
        protected static T _inst = default;

        private static object _objLock = new object();

         protected Singleton()
        {
            
        }

        public static T Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_objLock)
                    {
                        if (_inst == null)
                        {
                            _inst = new T();
                        }
                    }
                }

                return _inst;
            }
        }

        public static bool DestroyInstance()
        {
            bool result = false;

            if (_inst != null)
            {
                _inst.Dispose();

                result = true;
            }

            _inst = default;

            return result;
        }
    }
     
    public interface IDispose
    {
        void Dispose();
    }
}
