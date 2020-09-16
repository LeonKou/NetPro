using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NetPro.Swagger
{
    /// <summary>
    /// swagger默认值，只对swagger生效，避免直接使用[DefaultValue]影响正常业务
    /// </summary>
    public class SwaggerDefaultValueFilter : IOperationFilter
    {
        private readonly dynamic _mvcJsonOptions;

        public SwaggerDefaultValueFilter(IOptions<dynamic> mvcJsonOptions)
        {
            _mvcJsonOptions = mvcJsonOptions.Value;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return;
            }

            var parameterValuePairs = context.ApiDescription.ParameterDescriptions
                .Where(parameter => GetDefaultValueAttribute(parameter) != null || (GetParameterInfo(parameter)?.HasDefaultValue ?? false))
                .ToDictionary(parameter => parameter.Name, GetDefaultValue);
            if (parameterValuePairs.Count == 0) return;
            foreach (var parameter in operation.Parameters)
            {
                if (parameterValuePairs.TryGetValue(parameter.Name, out var defaultValue))
                {
                    parameter.Extensions.Add("default", new OpenApiString(defaultValue?.ToString()));

                }
            }
        }

        private SwaggerDefaultValueAttribute GetDefaultValueAttribute(ApiParameterDescription parameter)
        {
            if (!(parameter.ModelMetadata is DefaultModelMetadata metadata) || metadata.Attributes.PropertyAttributes == null)
            {
                return null;
            }

            return metadata.Attributes.PropertyAttributes
                .OfType<SwaggerDefaultValueAttribute>()
                .FirstOrDefault();
        }

        public ParameterInfo GetParameterInfo(ApiParameterDescription parameter)
        {
            if (parameter.ParameterDescriptor != null)
                return ((ControllerParameterDescriptor)parameter.ParameterDescriptor).ParameterInfo;
            return null;
        }

        private object GetDefaultValue(ApiParameterDescription parameter)
        {
            var parameterInfo = GetParameterInfo(parameter);

            if (parameterInfo.HasDefaultValue)
            {
                if (parameter.Type.IsEnum)
                {
                    var stringEnumConverter = _mvcJsonOptions.SerializerSettings.Converters
                        .OfType<StringEnumConverter>()
                        .FirstOrDefault();

                    if (stringEnumConverter != null)
                    {
                        if (parameterInfo.DefaultValue != null)
                        {
                            var defaultValue = parameterInfo.DefaultValue.ToString();
                            return stringEnumConverter.CamelCaseText ? ToCamelCase(defaultValue) : defaultValue;
                        }
                    }
                }

                return parameterInfo.DefaultValue;
            }

            var defaultValueAttribute = GetDefaultValueAttribute(parameter);

            return defaultValueAttribute.Value;
        }

        private string ToCamelCase(string name)
        {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }

    public class SwaggerJsonDefaultValueFilter : IOperationFilter
    {
        //private JsonSerializer _jsonSerializer;

        public SwaggerJsonDefaultValueFilter(IOptions<dynamic> mvcJsonOptions)
        {
            //_jsonSerializer = JsonSerializer.CreateDefault(mvcJsonOptions.Value.SerializerSettings);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;
            var parameterValuePairs = context.ApiDescription.ParameterDescriptions
                .Where(parameter => GetDefaultValueAttribute(parameter) != null || GetParameterInfo(parameter).HasDefaultValue)
                .ToDictionary(parameter => parameter.Name, GetDefaultValue);

            foreach (var parameter in operation.Parameters)
            {
                if (parameterValuePairs.TryGetValue(parameter.Name, out var defaultValue))
                {
                    parameter.Required = false;
                    if (defaultValue != null)
                    {
                        //var jValue = (JValue)JValue.FromObject(defaultValue, _jsonSerializer);
                        //parameter.Extensions.Add("default", jValue.Value);
                        parameter.Extensions.Add("default", new OpenApiString(defaultValue.ToString()));
                    }
                }
            }
        }

        private DefaultValueAttribute GetDefaultValueAttribute(ApiParameterDescription parameter)
        {
            return (parameter.ModelMetadata as DefaultModelMetadata)?
                .Attributes.PropertyAttributes?
                .OfType<DefaultValueAttribute>()
                .FirstOrDefault();
        }

        public ParameterInfo GetParameterInfo(ApiParameterDescription parameter)
        {
            return ((ControllerParameterDescriptor)parameter.ParameterDescriptor).ParameterInfo;
        }

        private object GetDefaultValue(ApiParameterDescription parameter)
        {
            var parameterInfo = GetParameterInfo(parameter);
            if (parameterInfo.HasDefaultValue)
            {
                return parameterInfo.DefaultValue;
            }
            else
            {
                return GetDefaultValueAttribute(parameter)?.Value;
            }
        }
    }
}
