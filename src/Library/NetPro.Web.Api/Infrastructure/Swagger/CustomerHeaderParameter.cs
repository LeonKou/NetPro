using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

namespace NetPro.Web.Api.Infrastructure.Swagger
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

            var headers = _configuration.GetSection("SwaggerOption:Headers").Get<string[]>();
            if (headers == null) return;
            foreach (var header in headers)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = header,
                    In = ParameterLocation.Header,
                    Required = false,
                });
            }
        }
    }
}
