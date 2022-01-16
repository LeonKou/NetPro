using Localization.SqlLocalizer;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NetPro.Globalization
{
    public interface IHybridStringLocalizerFactory : IStringLocalizerFactory
    {
    }

    public class HybridResourceManagerStringLocalizerFactory : ResourceManagerStringLocalizerFactory, IHybridStringLocalizerFactory
    {
        public HybridResourceManagerStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions, ILoggerFactory loggerFactory) : base(localizationOptions, loggerFactory)
        {
        }
    }

    public class HybridSqlStringLocalizerFactory : SqlStringLocalizerFactory, IStringExtendedLocalizerFactory, IHybridStringLocalizerFactory
    {
        public HybridSqlStringLocalizerFactory(LocalizationModelContext context, DevelopmentSetup developmentSetup, IOptions<SqlLocalizationOptions> localizationOptions) : base(context, developmentSetup, localizationOptions)
        {
        }
    }

    public class CustomStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IEnumerable<IHybridStringLocalizerFactory> _localizerFactories;
        private readonly ILogger<CustomStringLocalizerFactory> _logger;

        public CustomStringLocalizerFactory(IEnumerable<IHybridStringLocalizerFactory> localizerFactories, ILogger<CustomStringLocalizerFactory> logger)
        {
            _localizerFactories = localizerFactories;
            _logger = logger;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new CustomStringLocalizer(_localizerFactories.Select(x =>
            {
                try
                {
                    return x.Create(baseName, location);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return null;
                }
            }));
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new CustomStringLocalizer(_localizerFactories.Select(x =>
            {
                try
                {
                    return x.Create(resourceSource);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return null;
                }
            }));
        }
    }

    public class CustomStringLocalizer : IStringLocalizer
    {
        private readonly IEnumerable<IStringLocalizer> _stringLocalizers;

        public CustomStringLocalizer(IEnumerable<IStringLocalizer> stringLocalizers)
        {
            _stringLocalizers = stringLocalizers;
        }

        public virtual LocalizedString this[string name]
        {
            get
            {
                var localizer = _stringLocalizers.SingleOrDefault(x => x is ResourceManagerStringLocalizer);
                var result = localizer?[name];
                if (!(result?.ResourceNotFound ?? true)) return result;

                localizer = _stringLocalizers.SingleOrDefault(x => x is SqlStringLocalizer) ?? throw new InvalidOperationException($"没有找到可用的 {nameof(IStringLocalizer)}");
                result = localizer[name];
                return result;
            }
        }

        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var localizer = _stringLocalizers.SingleOrDefault(x => x is ResourceManagerStringLocalizer);
                var result = localizer?[name, arguments];
                if (!(result?.ResourceNotFound ?? true)) return result;

                localizer = _stringLocalizers.SingleOrDefault(x => x is SqlStringLocalizer) ?? throw new InvalidOperationException($"没有找到可用的 {nameof(IStringLocalizer)}");
                result = localizer[name, arguments];
                return result;
            }
        }

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var localizer = _stringLocalizers.SingleOrDefault(x => x is ResourceManagerStringLocalizer);
            var result = localizer?.GetAllStrings(includeParentCultures);
            if (!(result?.Any(x => x.ResourceNotFound) ?? true)) return result;

            localizer = _stringLocalizers.SingleOrDefault(x => x is SqlStringLocalizer) ?? throw new InvalidOperationException($"没有找到可用的 {nameof(IStringLocalizer)}");
            result = localizer?.GetAllStrings(includeParentCultures);
            return result;
        }

        [Obsolete]
        public virtual IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomStringLocalizer<T> : CustomStringLocalizer, IStringLocalizer<T>
    {
        public CustomStringLocalizer(IEnumerable<IStringLocalizer> stringLocalizers) : base(stringLocalizers)
        {
        }

        public override LocalizedString this[string name] => base[name];

        public override LocalizedString this[string name, params object[] arguments] => base[name, arguments];

        public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return base.GetAllStrings(includeParentCultures);
        }

        [Obsolete]
        public override IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
