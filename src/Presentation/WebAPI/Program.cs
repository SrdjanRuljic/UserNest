using Application;
using Application.Common.Interfaces;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using WebAPI.Filters;
using WebAPI.Helpers;
using WebAPI.Middlewares;
using WebAPI.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "UserNest API", 
        Version = "v1",
        Description = "A comprehensive user management API with authentication and authorization features.",
        Contact = new OpenApiContact
        {
            Name = "UserNest Team",
            Email = "support@usernest.com"
        }
    });
    
    // Security definition for JWT Bearer tokens
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            }, new List<string>()
        }
    });
    
    // Custom type mappings
    options.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("00:00")
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.OperationFilter<SwaggerLanguageHeader>();
    options.OperationFilter<ExampleOperationFilter>();
    
    // Enable annotations
    options.EnableAnnotations();
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();

    // Initialise and seed database
    using (IServiceScope scope = app.Services.CreateScope())
    {
        ApplicationDbContextInitializer initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initialiser.InitializeAsync();
        await initialiser.SeedAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();