using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Helpers
{
    public class SwaggerLanguageHeader : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
                operation.Parameters?.Add(new OpenApiParameter
                {
                    Name = "Accept-Language",
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Enum =
                        [
                            new OpenApiString("en"),
                            new OpenApiString("sr-Cyrl-RS"),
                            new OpenApiString("sr-Latn-RS"),
                            new OpenApiString("sl")
                        ]
                    }
                });
        }
    }
}