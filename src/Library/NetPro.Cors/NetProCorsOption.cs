using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Cors
{
    internal class NetProCorsOption
    {
        /// <summary>
        /// 跨域允许的站点
        /// </summary>
        public string CorsOrigins { get; set; } = "*";
    }
}
