/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.NetPro
{
    /// <summary>
    ///  Web程序域内 循环类型查找(在bin目录中)
    /// </summary>
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        #region Fields

        private bool _ensureBinFolderAssembliesLoaded = true;
        private static bool _binFolderAssembliesLoaded;
        private readonly IConfiguration _configuration;
        private readonly TypeFinderOption _typeFinderOption;

        #endregion

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFinderOption"></param>
        /// <param name="configuration"></param>
        /// <param name="fileProvider"></param>
        public WebAppTypeFinder(
            TypeFinderOption typeFinderOption,
            IConfiguration configuration
            , INetProFileProvider fileProvider = null) : base(fileProvider, typeFinderOption)
        {
            _typeFinderOption = typeFinderOption;
            _configuration = configuration;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether assemblies in the bin folder of the web application should be specifically checked for being loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return _ensureBinFolderAssembliesLoaded; }
            set { _ensureBinFolderAssembliesLoaded = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a physical disk path of \Bin directory
        /// </summary>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string GetBinDirectory()
        {
            return AppContext.BaseDirectory;
        }

        /// <summary>
        /// Get assemblies.Default the bin directory first, then traverse the PluginPath directory
        /// </summary>
        /// <returns>Result</returns>
        public override IList<Assembly> GetAssemblies()
        {
            //load bin folder
            if (!_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                var binPath = GetBinDirectory();
                //binPath = _webHelper.MapPath("~/bin");
                if (string.IsNullOrWhiteSpace(_typeFinderOption.MountePath))
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        _typeFinderOption.MountePath = "C:/opt/netpro";
                    }
                    else
                    {
                        _typeFinderOption.MountePath = "/opt/netpro";
                    }
                }

                LoadMatchingAssemblies(_typeFinderOption.MountePath, binPath);//MountePath:plugin dll path
            }

            return base.GetAssemblies();
        }

        #endregion
    }
}
