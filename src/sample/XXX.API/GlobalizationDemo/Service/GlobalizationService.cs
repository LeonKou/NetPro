using Microsoft.Extensions.Localization;

namespace XXX.API.Service
{
    public interface IGlobalizationDemoService
    {
        string GetLanguage();
    }

    public class GlobalizationDemoService : IGlobalizationDemoService,IScopedDependency
    {
        private readonly IStringLocalizer<NetPro.Globalization.Globalization> _localizer;
        public GlobalizationDemoService(IStringLocalizer<NetPro.Globalization.Globalization> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetLanguage()
        {
            var message = _localizer["当前时间为"] + $"：{DateTime.Now}";
            return message;
        }
    }
}