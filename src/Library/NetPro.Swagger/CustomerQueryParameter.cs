using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

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
                //本地localhost会导致自定义query或header无法显示，出现Could not render this component, see the console.错误，用网卡地址显示即可正常
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
