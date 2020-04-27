using System;
using System.Collections.Generic;

namespace MQMiddleware
{
    /// <summary>
    /// Message handler router model.
    /// </summary>
    internal class MessageHandlerRouter
    {
        /// <summary>
        /// Message Handler Type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Collection of routing keys which that handler will be "listening".
        /// </summary>
        public List<string> RoutingKeys { get; set; } = new List<string>();
    }
}