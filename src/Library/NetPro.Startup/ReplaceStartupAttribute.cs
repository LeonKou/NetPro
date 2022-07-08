using System;

namespace System.NetPro
{
    /// <summary>
    /// Attribute to replace existing startup
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ReplaceStartupAttribute : Attribute
    {
        /// <summary>
        /// The name of startup to be replaced
        /// </summary>
        public string StartupClassName { get; }

        /// <summary>
        /// Attribute to replace existing startup
        /// </summary>
        /// <param name="startupClassName">The name of startup to be replaced</param>
        public ReplaceStartupAttribute(string startupClassName)
        {
            StartupClassName = startupClassName;
        }
    }
}
