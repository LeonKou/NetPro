using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace NetPro.Web.Api.Infrastructure.Swagger
{
	public class CustomerHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null )
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Description= "this is user identity information,formate : {'Id':'1','Name':'netcore'}",
                Name = "User",
                In =  ParameterLocation.Header,
                Required = false ,
            });
        }
    }
}
