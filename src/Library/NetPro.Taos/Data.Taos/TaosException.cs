// Copyright (c)  maikebing All rights reserved.
//// Licensed under the MIT License, See License.txt in the project root for license information.

using System;
using System.Data.Common;
using TDengineDriver;

namespace Maikebing.Data.Taos
{
    /// <summary>
    ///     Represents a Taos error.
    /// </summary>
    public class TaosException : DbException
    {
        TaosErrorResult _taosError;

        public TaosException(TaosErrorResult taosError) : base(taosError.Error, null)
        {
            _taosError = taosError;
            base.HResult = _taosError.Code;
        }

        public TaosException(TaosErrorResult taosError, Exception ex) : base(taosError.Error, ex)
        {
            _taosError = taosError;
            base.HResult = _taosError.Code;
        }


   

      
        public override string Message => _taosError?.Error;
        public override int ErrorCode =>   (int) _taosError?.Code;
        /// <summary>
        ///     Throws an exception with a specific Taos error code value.
        /// </summary>
        /// <param name="rc">The Taos error code corresponding to the desired exception.</param>
        /// <param name="db">A handle to database connection.</param>
        /// <remarks>
        ///     No exception is thrown for non-error result codes.
        /// </remarks>
        public static void ThrowExceptionForRC(string _commandText, TaosErrorResult taosError)
        {
            var te = new TaosException(taosError);
            te.Data.Add("commandText", _commandText);
            throw te;
        }
        public static void ThrowExceptionForRC(IntPtr _taos)
        {
            var te = new TaosException(new TaosErrorResult() { Code = TDengine.ErrorNo(_taos), Error = TDengine.Error(_taos) });
            throw te;
        }
        public static void ThrowExceptionForRC(int code, string message, Exception ex)
        {
            var te = new TaosException(new TaosErrorResult() { Code = code, Error = message }, ex);
            throw te;
        }
    }
}
