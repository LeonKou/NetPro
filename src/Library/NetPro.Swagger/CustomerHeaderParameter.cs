using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace NetPro.Swagger
{
    public class CustomerHeaderParameter : IOperationFilter
    {
        private readonly IConfiguration _configuration;
        public CustomerHeaderParameter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var headers = _configuration.GetSection("SwaggerOption:Headers").Get<List<OpenApiParameter>>();
            if (headers == null) return;
            foreach (var header in headers)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = header.Name,
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = header.Description,
                });
            }
        }
    }
}
