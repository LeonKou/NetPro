using NetPro.Core.Consts;
using NetPro.Utility;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace NetPro.Web.Api.Filters
{
    /// <summary>
    /// model绑定元数据有效性验证异常
    /// </summary>
    public class ViewModelStateActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                string errorMsg = context.ModelState.ToModelStateError();
                if (string.IsNullOrEmpty(errorMsg))
                {
                    errorMsg = "数据验证失败";
                }
                string bindModelText = context.ModelState.ToBindModelText();
                throw new ViewModelStateValidException(errorMsg, bindModelText, AppErrorCode.ModelInValid.Value());
            }
            await next();
        }
    }
}