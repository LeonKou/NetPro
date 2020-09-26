using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetPro.Swagger
{
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        private static readonly string[] FileParameters = new[] { "ContentType", "ContentDisposition", "Headers", "Length", "Name", "FileName" };
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!((context.ApiDescription.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                context.ApiDescription.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase)) && context.ApiDescription.ParameterDescriptions.Any(s => s.Type == typeof(IFormFile) || s.Type == typeof(IFormFileCollection))))
            {
                return;
            }
            RemoveExistingFileParameters(operation.Parameters);

            #region 留用
            //IDictionary<string, OpenApiSchema> pro = new Dictionary<string, OpenApiSchema>();
            //foreach (var item in context.ApiDescription.ParameterDescriptions)
            //{
            //	if (item.Type == typeof(IFormFileCollection) || item.Type == typeof(IFormFile))
            //	{
            //		pro.Add($"{item.Name}", new OpenApiSchema()
            //		{
            //			Description = "Select file",
            //			Type = "string",
            //			Format = "binary"
            //		});
            //	}
            //}
            //operation.RequestBody = new OpenApiRequestBody()
            //{
            //	Content =
            //		{
            //			["multipart/form-data"] = new OpenApiMediaType()
            //			{
            //				Schema = new OpenApiSchema()
            //				{
            //					Type = "object",
            //					Properties = pro
            //				}
            //			} ,
            //		}
            //};
            #endregion

            operation.Responses.Clear();
            operation.Responses.Add("200", new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    "multipart/form-data", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    }
                }
            }
            });
        }

        private void RemoveExistingFileParameters(IList<OpenApiParameter> operationParameters)
        {
            foreach (var parameter in operationParameters.Where(p => p.In == 0 && FileParameters.Contains(p.Name)).ToList())
            {
                operationParameters.Remove(parameter);
            }
        }
    }
}
