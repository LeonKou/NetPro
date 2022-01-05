using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXX.Plugin.MediatR.Model
{
    /// <summary>
    /// 传输的消息
    /// </summary>
    public class SendRequestModel :
        IRequest<string>
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Message { get; set; }
    }
}
