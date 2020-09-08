using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetPro.Swagger
{
    public class CustomerQueryParameter : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public CustomerQueryParameter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var queryArray = _configuration.GetSection("SwaggerOption:Query").Get<List<OpenApiParameter>>();
            if (queryArray == null) return;
            foreach (var query in queryArray)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = query.Name,
                    In = ParameterLocation.Query,
                    Required = false,
                    Description = query.Description,
                });
            }
        }
    }
}
