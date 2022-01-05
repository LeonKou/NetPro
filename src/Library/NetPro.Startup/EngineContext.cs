using System.Runtime.CompilerServices;

namespace System.NetPro
{
    /// <summary>
    /// NetPro Engine 单例实现
    /// </summary>
    public class EngineContext
    {
        #region Methods

        private EngineContext()
        {

        }
        /// <summary>
        /// 返回NetProEngine 单例对象
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            //create NetProEngine as engine
            return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new NetProEngine());
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        ///当前IEngine对象
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Create();
                }

                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
