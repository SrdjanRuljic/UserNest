using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebAPI.Filters
{
    public class ExampleOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Add examples for specific operations
            if (context.MethodInfo.Name == "Login")
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(Application.Auth.Commands.Login.LoginCommand), context.SchemaRepository),
                            Example = new OpenApiObject
                            {
                                ["username"] = new OpenApiString("johndoe"),
                                ["password"] = new OpenApiString("SecurePassword123!")
                            }
                        }
                    }
                };
            }
            else if (context.MethodInfo.Name == "Logout")
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(Application.Auth.Commands.Logout.LogoutCommand), context.SchemaRepository),
                            Example = new OpenApiObject
                            {
                                ["refreshToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
                            }
                        }
                    }
                };
            }
            else if (context.MethodInfo.Name == "RefreshToken")
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(Application.Auth.Commands.Refresh.RefreshTokenCommand), context.SchemaRepository),
                            Example = new OpenApiObject
                            {
                                ["refreshToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
                            }
                        }
                    }
                };
            }
            else if (context.MethodInfo.Name == "Insert")
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(Application.Users.Commands.Insert.InsertUserCommand), context.SchemaRepository),
                            Example = new OpenApiObject
                            {
                                ["userName"] = new OpenApiString("johndoe"),
                                ["email"] = new OpenApiString("john.doe@example.com"),
                                ["password"] = new OpenApiString("SecurePassword123!"),
                                ["firstName"] = new OpenApiString("John"),
                                ["lastName"] = new OpenApiString("Doe"),
                                ["languageId"] = new OpenApiInteger(1),
                                ["role"] = new OpenApiString("Admin"),
                                ["phoneNumber"] = new OpenApiString("+1234567890")
                            }
                        }
                    }
                };
            }
            else if (context.MethodInfo.Name == "Update")
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(Application.Users.Commands.Update.UpdateUserCommand), context.SchemaRepository),
                            Example = new OpenApiObject
                            {
                                ["firstName"] = new OpenApiString("John"),
                                ["lastName"] = new OpenApiString("Smith"),
                                ["userName"] = new OpenApiString("johnsmith"),
                                ["email"] = new OpenApiString("john.smith@example.com"),
                                ["password"] = new OpenApiString("NewPassword123!"),
                                ["languageId"] = new OpenApiInteger(2)
                            }
                        }
                    }
                };
            }
        }
    }
}
