using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetPro.TypeFinder
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
            , INetProFileProvider fileProvider = null) : base(fileProvider)
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
            if (EnsureBinFolderAssembliesLoaded && !_binFolderAssembliesLoaded)
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
