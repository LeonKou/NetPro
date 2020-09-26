using System;

namespace NetPro.Logging
{
    public interface ILog
    {
        void Verbose(string msg);
        /// <summary>
        /// 调试信息日志
        /// </summary>
        /// <param name="msg"></param>
        void Debug(string msg);
        /// <summary>
        /// 普通信息 日志
        /// </summary>
        /// <param name="msg"></param>
        void Information(string msg);
        /// <summary>
        ///警告
        /// </summary>
        /// <param name="msg"></param>
        void Warning(string msg);

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="msg"></param>
        void Error(string msg);
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="error"></param>
        /// <param name="msg"></param>
        void Error(Exception error, string msg);
        /// <summary>
        /// 致命的错误
        /// </summary>
        /// <param name="msg"></param>
        void Fatal(string msg);

    }
}
